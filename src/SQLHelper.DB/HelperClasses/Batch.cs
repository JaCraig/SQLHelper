/*
Copyright 2016 James Craig

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using BigBook;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using SQLHelperDB.ExtensionMethods;
using SQLHelperDB.HelperClasses.Interfaces;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SQLHelperDB.HelperClasses
{
    /// <summary>
    /// Holds information for a set of commands
    /// </summary>
    /// <seealso cref="IBatch"/>
    /// <remarks>Constructor</remarks>
    /// <param name="source">Source info</param>
    /// <param name="stringBuilderPool">The string builder pool.</param>
    /// <param name="logger">The logger.</param>
    public partial class Batch(IConnection source, ObjectPool<StringBuilder> stringBuilderPool, ILogger? logger = null) : IBatch
    {
        /// <summary>
        /// Command count
        /// </summary>
        public int CommandCount => Commands.Count;

        /// <summary>
        /// Gets the string builder pool.
        /// </summary>
        /// <value>The string builder pool.</value>
        public ObjectPool<StringBuilder> StringBuilderPool { get; } = stringBuilderPool;

        /// <summary>
        /// Commands to batch
        /// </summary>
        protected List<ICommand> Commands { get; private set; } = [];

        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>The headers.</value>
        protected List<ICommand> Headers { get; private set; } = [];

        /// <summary>
        /// Connection string
        /// </summary>
        protected IConnection Source { get; private set; } = source;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private ILogger? Logger { get; } = logger;

        /// <summary>
        /// Used to parse SQL commands to find parameters (when batching)
        /// </summary>
        private static readonly Regex _ParameterRegex = BuildParameterRegex();

        /// <summary>
        /// Adds a command to be batched
        /// </summary>
        /// <typeparam name="TCallbackData">The type of the callback data.</typeparam>
        /// <param name="callBack">Callback action</param>
        /// <param name="callbackObject">Object used in the callback action</param>
        /// <param name="header">
        /// Determines if this command is a "header" and should be carried across batches.
        /// </param>
        /// <param name="command">Command (SQL or stored procedure) to run</param>
        /// <param name="commandType">Command type</param>
        /// <param name="parameters">Parameters to add</param>
        /// <returns>This</returns>
        public IBatch AddQuery<TCallbackData>(Action<ICommand, List<dynamic>, TCallbackData> callBack, TCallbackData callbackObject, bool header, string command, CommandType commandType, params object[]? parameters)
        {
            if (header)
                Headers.Add(new Command<TCallbackData>(callBack, callbackObject, header, command, commandType, Source.ParameterPrefix, parameters));
            else
                Commands.Add(new Command<TCallbackData>(callBack, callbackObject, header, command, commandType, Source.ParameterPrefix, parameters));
            return this;
        }

        /// <summary>
        /// Adds a batch's commands to the current batch
        /// </summary>
        /// <param name="batch">Batch to add</param>
        /// <returns>This</returns>
        public IBatch AddQuery(IBatch batch)
        {
            if (batch is not Batch TempValue)
                return this;
            _ = Commands.Add(TempValue.Commands);
            _ = Headers.Add(TempValue.Headers);
            return this;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <returns>This.</returns>
        public IBatch Clear()
        {
            Commands.Clear();
            Headers.Clear();
            return this;
        }

        /// <summary>
        /// Executes the commands and returns the results (async)
        /// </summary>
        /// <returns>The results of the batched commands</returns>
        public Task<List<List<dynamic>>> ExecuteAsync() => ExecuteCommandsAsync();

        /// <summary>
        /// Removes duplicate commands from the batch
        /// </summary>
        /// <returns>This</returns>
        public IBatch RemoveDuplicateCommands()
        {
            Commands = [.. Commands.Distinct()];
            Headers = [.. Headers.Distinct()];
            return this;
        }

        /// <summary>
        /// Sets the connection.
        /// </summary>
        /// <param name="databaseConnection">The database connection.</param>
        public void SetConnection(IConnection databaseConnection) => Source = databaseConnection;

        /// <summary>
        /// Converts the batch to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Headers.ToString(x => x.ToString() ?? "", Environment.NewLine)
                + Commands.ToString(x => x.ToString() ?? "", Environment.NewLine);
        }

        /// <summary>
        /// Checks whether a transaction is needed.
        /// </summary>
        /// <returns>True if it is, false otherwise</returns>
        protected bool CheckTransaction() => Commands.Count > 1 && Commands.Any(command => command.TransactionNeeded);

        /// <summary>
        /// Builds the parameter regex.
        /// </summary>
        /// <returns>The parameter regex</returns>
        [GeneratedRegex(@"[^@](?<ParamStart>[:@?])(?<ParamName>\w+)", RegexOptions.Compiled)]
        private static partial Regex BuildParameterRegex();

        /// <summary>
        /// Gets the results asynchronous.
        /// </summary>
        /// <param name="returnValue">The return value.</param>
        /// <param name="executableCommand">The executable command.</param>
        /// <param name="finalParameters">The final parameters.</param>
        /// <param name="finalizable">if set to <c>true</c> [finalizable].</param>
        /// <param name="finalSQLCommand">The final SQL command.</param>
        /// <returns>The async task.</returns>
        private static async Task GetResultsAsync(List<List<dynamic>> returnValue, DbCommand executableCommand, List<IParameter> finalParameters, bool finalizable, string finalSQLCommand)
        {
            if (string.IsNullOrEmpty(finalSQLCommand))
                return;
            executableCommand.CommandText = finalSQLCommand;
            for (int X = 0, FinalParametersCount = finalParameters.Count; X < FinalParametersCount; ++X)
            {
                finalParameters[X].AddParameter(executableCommand);
            }
            if (finalizable)
            {
                await using DbDataReader TempReader = await executableCommand.ExecuteReaderAsync().ConfigureAwait(false);
                do
                {
                    returnValue.Add(GetValues(TempReader));
                }
                while (TempReader.NextResult());
            }
            else
            {
                returnValue.Add([new Dynamo(await executableCommand.ExecuteNonQueryAsync().ConfigureAwait(false), false)]);
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="tempReader">The temporary reader.</param>
        /// <returns>The resulting values</returns>
        private static List<dynamic> GetValues(DbDataReader tempReader)
        {
            if (tempReader is null)
                return [];
            var ReturnValue = new List<dynamic>();
            var FieldNames = ArrayPool<string>.Shared.Rent(tempReader.FieldCount);
            for (var X = 0; X < tempReader.FieldCount; ++X)
            {
                var FieldName = tempReader.GetName(X);
                FieldNames[X] = !string.IsNullOrWhiteSpace(FieldName) ? FieldName : $"(No column name #{X})";
            }
            while (tempReader.Read())
            {
                var Value = new Dynamo(false);
                for (var X = 0; X < tempReader.FieldCount; ++X)
                {
                    Value.Add(FieldNames[X], tempReader[X]);
                }
                ReturnValue.Add(Value);
            }
            ArrayPool<string>.Shared.Return(FieldNames);
            return ReturnValue;
        }

        /// <summary>
        /// Executes the commands asynchronously.
        /// </summary>
        /// <returns>The list of results</returns>
        private async Task<List<List<dynamic>>> ExecuteCommandsAsync()
        {
            if (Source is null || Commands is null)
                return [];
            var ReturnValue = new List<List<dynamic>>();
            if (Commands.Count == 0)
            {
                ReturnValue.Add([]);
                return ReturnValue;
            }
            DbProviderFactory Factory = Source.Factory;
            await using (DbConnection? Connection = Factory.CreateConnection())
            {
                if (Connection is null)
                    return ReturnValue;
                Connection.ConnectionString = Source.ConnectionString;
                await using DbCommand? ExecutableCommand = Factory.CreateCommand();
                if (ExecutableCommand is null)
                    return ReturnValue;
                SetupCommand(Connection, ExecutableCommand);

                try
                {
                    var Count = 0;
                    do
                    {
                        var FinalParameters = new List<IParameter>();
                        var Finalizable = false;
                        var FinalSQLCommand = string.Empty;
                        ExecutableCommand.Parameters.Clear();
                        var EndCount = SetupParameters(Count, FinalParameters, ref Finalizable, ref FinalSQLCommand);
                        await GetResultsAsync(ReturnValue, ExecutableCommand, FinalParameters, Finalizable, FinalSQLCommand).ConfigureAwait(false);
                        Count = EndCount;
                    }
                    while (Count < CommandCount);
                    _ = ExecutableCommand.Commit();
                }
                catch (Exception Ex)
                {
                    Logger?.LogError(Ex, "{commandText}", ExecutableCommand.CommandText);
                    _ = ExecutableCommand.Rollback();
                    throw;
                }
                finally { _ = ExecutableCommand.Close(); }
            }
            FinalizeCommands(ReturnValue);
            return ReturnValue;
        }

        /// <summary>
        /// Finalizes the commands.
        /// </summary>
        /// <param name="returnValue">The return value.</param>
        private void FinalizeCommands(List<List<dynamic>> returnValue)
        {
            for (int X = 0, Y = 0; X < Commands.Count; ++X)
            {
                if (Commands[X].Finalizable)
                {
                    Commands[X].Finalize(returnValue[Y]);
                    ++Y;
                }
                else
                {
                    Commands[X].Finalize([]);
                }
            }
        }

        /// <summary>
        /// Setups the command.
        /// </summary>
        /// <param name="databaseConnection">The database connection.</param>
        /// <param name="executableCommand">The executable command.</param>
        private void SetupCommand(DbConnection databaseConnection, DbCommand executableCommand)
        {
            executableCommand.Connection = databaseConnection;
            executableCommand.CommandType = CommandType.Text;
            executableCommand.CommandTimeout = databaseConnection.ConnectionTimeout;
            if (CheckTransaction())
                _ = executableCommand.BeginTransaction(Source.Retries);
            _ = executableCommand.Open(Source.Retries);
        }

        /// <summary>
        /// Setups the parameters.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="finalParameters">The final parameters.</param>
        /// <param name="finalizable">if set to <c>true</c> [finalizable].</param>
        /// <param name="finalSQLCommand">The final SQL command.</param>
        private int SetupParameters(int count, List<IParameter> finalParameters, ref bool finalizable, ref string finalSQLCommand)
        {
            var ParameterTotal = 0;
            StringBuilder Builder = StringBuilderPool?.Get() ?? new StringBuilder();
            _ = Builder.Append(finalSQLCommand);
            for (var Y = 0; Y < Headers.Count; ++Y)
            {
                ICommand Command = Headers[Y];
                if (ParameterTotal + Command.Parameters.Length >= 2000)
                    break;
                ParameterTotal += Command.Parameters.Length;
                finalizable |= Command.Finalizable;
                if (Command.CommandType == CommandType.Text)
                {
                    var TempCommandText = Command.SQLCommand ?? string.Empty;
                    _ = Builder.Append(Command.SQLCommand ?? string.Empty).Append(Environment.NewLine);

                    for (int I = 0, CommandParametersLength = Command.Parameters.Length; I < CommandParametersLength; I++)
                    {
                        IParameter TempParameter = Command.Parameters[I];
                        finalParameters.Add(TempParameter.CreateCopy(string.Empty));
                    }
                }
                else
                {
                    _ = Builder.Append(Command.SQLCommand).Append(Environment.NewLine);
                    for (int I = 0, CommandParametersLength = Command.Parameters.Length; I < CommandParametersLength; I++)
                    {
                        IParameter TempParameter = Command.Parameters[I];
                        finalParameters.Add(TempParameter.CreateCopy(string.Empty));
                    }
                }
            }
            for (var Y = count; Y < Commands.Count; ++Y)
            {
                ICommand Command = Commands[Y];
                if (ParameterTotal + Command.Parameters.Length >= 2000)
                    break;
                ParameterTotal += Command.Parameters.Length;
                finalizable |= Command.Finalizable;
                if (Command.CommandType == CommandType.Text)
                {
                    var TempCommandText = Command.SQLCommand ?? string.Empty;
                    var Suffix = "Command" + count.ToString(CultureInfo.InvariantCulture);
                    _ = Builder.Append(string.IsNullOrEmpty(Command.SQLCommand) ?
                                        string.Empty :
                                        _ParameterRegex.Replace(Command.SQLCommand, x =>
                                        {
                                            IParameter? Param = Array.Find(Command.Parameters, z => z.ID == x.Groups["ParamName"].Value);
                                            return Param is not null ? x.Value + Suffix : x.Value;
                                        })).Append(Environment.NewLine);

                    for (int I = 0, CommandParametersLength = Command.Parameters.Length; I < CommandParametersLength; I++)
                    {
                        IParameter TempParameter = Command.Parameters[I];
                        finalParameters.Add(TempParameter.CreateCopy(Suffix));
                    }
                }
                else
                {
                    _ = Builder.Append(Command.SQLCommand).Append(Environment.NewLine);
                    for (int I = 0, CommandParametersLength = Command.Parameters.Length; I < CommandParametersLength; I++)
                    {
                        IParameter TempParameter = Command.Parameters[I];
                        finalParameters.Add(TempParameter.CreateCopy(string.Empty));
                    }
                }
                ++count;
            }
            finalSQLCommand = Builder.ToString();
            StringBuilderPool?.Return(Builder);
            return count;
        }
    }
}