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
    public class Batch : IBatch
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Source info</param>
        /// <param name="stringBuilderPool">The string builder pool.</param>
        /// <param name="dynamoFactory">The dynamo factory.</param>
        public Batch(IConnection source, ObjectPool<StringBuilder> stringBuilderPool, DynamoFactory dynamoFactory)
        {
            Commands = new List<ICommand>();
            Headers = new List<ICommand>();
            Source = source;
            StringBuilderPool = stringBuilderPool;
            DynamoFactory = dynamoFactory;
        }

        /// <summary>
        /// Command count
        /// </summary>
        public int CommandCount => Commands.Count;

        /// <summary>
        /// Gets the dynamo factory.
        /// </summary>
        /// <value>The dynamo factory.</value>
        public DynamoFactory DynamoFactory { get; }

        /// <summary>
        /// Gets the string builder pool.
        /// </summary>
        /// <value>The string builder pool.</value>
        public ObjectPool<StringBuilder> StringBuilderPool { get; }

        /// <summary>
        /// Commands to batch
        /// </summary>
        protected List<ICommand> Commands { get; private set; }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>The headers.</value>
        protected List<ICommand> Headers { get; private set; }

        /// <summary>
        /// Connection string
        /// </summary>
        protected IConnection Source { get; private set; }

        /// <summary>
        /// Used to parse SQL commands to find parameters (when batching)
        /// </summary>
        private static readonly Regex ParameterRegex = new Regex(@"[^@](?<ParamStart>[:@?])(?<ParamName>\w+)", RegexOptions.Compiled);

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
            if (!(batch is Batch TempValue))
                return this;
            Commands.Add(TempValue.Commands);
            Headers.Add(TempValue.Headers);
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
            Commands = Commands.Distinct().ToList();
            Headers = Headers.Distinct().ToList();
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
            return Headers.ToString(x => x.ToString(), Environment.NewLine)
                + Commands.ToString(x => x.ToString(), Environment.NewLine);
        }

        /// <summary>
        /// Checks whether a transaction is needed.
        /// </summary>
        /// <returns>True if it is, false otherwise</returns>
        protected bool CheckTransaction() => Commands.Count > 1 && Commands.Any(Command => Command.TransactionNeeded);

        /// <summary>
        /// Executes the commands asynchronously.
        /// </summary>
        /// <returns>The list of results</returns>
        private async Task<List<List<dynamic>>> ExecuteCommandsAsync()
        {
            if (Source is null || Commands is null)
                return new List<List<dynamic>>();
            var ReturnValue = new List<List<dynamic>>();
            if (Commands.Count == 0)
            {
                ReturnValue.Add(new List<dynamic>());
                return ReturnValue;
            }
            var Factory = Source.Factory;
            using (var Connection = Factory.CreateConnection())
            {
                Connection.ConnectionString = Source.ConnectionString;
                using var ExecutableCommand = Factory.CreateCommand();
                SetupCommand(Connection, ExecutableCommand);

                try
                {
                    var Count = 0;
                    do
                    {
                        var FinalParameters = new List<IParameter>();
                        var Finalizable = false;
                        var FinalSQLCommand = string.Empty;
                        var ParameterTotal = 0;
                        ExecutableCommand.Parameters.Clear();
                        SetupParameters(ref Count, FinalParameters, ref Finalizable, ref FinalSQLCommand, ref ParameterTotal);
                        await GetResultsAsync(ReturnValue, ExecutableCommand, FinalParameters, Finalizable, FinalSQLCommand).ConfigureAwait(false);
                    }
                    while (Count < CommandCount);
                    ExecutableCommand.Commit();
                }
                catch { ExecutableCommand.Rollback(); throw; }
                finally { ExecutableCommand.Close(); }
            }
            FinalizeCommands(ReturnValue);
            return ReturnValue;
        }

        /// <summary>
        /// Finalizes the commands.
        /// </summary>
        /// <param name="ReturnValue">The return value.</param>
        private void FinalizeCommands(List<List<dynamic>> ReturnValue)
        {
            for (int x = 0, y = 0; x < Commands.Count; ++x)
            {
                if (Commands[x].Finalizable)
                {
                    Commands[x].Finalize(ReturnValue[y]);
                    ++y;
                }
                else
                {
                    Commands[x].Finalize(new List<dynamic>());
                }
            }
        }

        /// <summary>
        /// Gets the results asynchronous.
        /// </summary>
        /// <param name="ReturnValue">The return value.</param>
        /// <param name="ExecutableCommand">The executable command.</param>
        /// <param name="FinalParameters">The final parameters.</param>
        /// <param name="Finalizable">if set to <c>true</c> [finalizable].</param>
        /// <param name="FinalSQLCommand">The final SQL command.</param>
        /// <returns>The async task.</returns>
        private async Task GetResultsAsync(List<List<dynamic>> ReturnValue, DbCommand ExecutableCommand, List<IParameter> FinalParameters, bool Finalizable, string FinalSQLCommand)
        {
            if (string.IsNullOrEmpty(FinalSQLCommand))
                return;
            ExecutableCommand.CommandText = FinalSQLCommand;
            for (int x = 0, FinalParametersCount = FinalParameters.Count; x < FinalParametersCount; ++x)
            {
                FinalParameters[x].AddParameter(ExecutableCommand);
            }
            if (Finalizable)
            {
                using var TempReader = await ExecutableCommand.ExecuteReaderAsync().ConfigureAwait(false);
                do
                {
                    ReturnValue.Add(GetValues(TempReader));
                }
                while (TempReader.NextResult());
            }
            else
            {
                ReturnValue.Add(new List<dynamic> { DynamoFactory.Create(await ExecutableCommand.ExecuteNonQueryAsync().ConfigureAwait(false), false) });
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="tempReader">The temporary reader.</param>
        /// <returns>The resulting values</returns>
        private List<dynamic> GetValues(DbDataReader tempReader)
        {
            if (tempReader is null)
                return new List<dynamic>();
            var ReturnValue = new List<dynamic>();
            var FieldNames = ArrayPool<string>.Shared.Rent(tempReader.FieldCount);
            for (var x = 0; x < tempReader.FieldCount; ++x)
            {
                var FieldName = tempReader.GetName(x);
                FieldNames[x] = !string.IsNullOrWhiteSpace(FieldName) ? FieldName : $"(No column name #{x})";
            }
            while (tempReader.Read())
            {
                var Value = DynamoFactory.Create(false);
                for (var x = 0; x < tempReader.FieldCount; ++x)
                {
                    Value.Add(FieldNames[x], tempReader[x]);
                }
                ReturnValue.Add(Value);
            }
            ArrayPool<string>.Shared.Return(FieldNames);
            return ReturnValue;
        }

        /// <summary>
        /// Setups the command.
        /// </summary>
        /// <param name="DatabaseConnection">The database connection.</param>
        /// <param name="ExecutableCommand">The executable command.</param>
        private void SetupCommand(DbConnection DatabaseConnection, DbCommand ExecutableCommand)
        {
            ExecutableCommand.Connection = DatabaseConnection;
            ExecutableCommand.CommandType = CommandType.Text;
            ExecutableCommand.CommandTimeout = DatabaseConnection.ConnectionTimeout;
            if (CheckTransaction())
                ExecutableCommand.BeginTransaction(Source.Retries);
            ExecutableCommand.Open(Source.Retries);
        }

        /// <summary>
        /// Setups the parameters.
        /// </summary>
        /// <param name="Count">The count.</param>
        /// <param name="FinalParameters">The final parameters.</param>
        /// <param name="Finalizable">if set to <c>true</c> [finalizable].</param>
        /// <param name="FinalSQLCommand">The final SQL command.</param>
        /// <param name="ParameterTotal">The parameter total.</param>
        private void SetupParameters(ref int Count, List<IParameter> FinalParameters, ref bool Finalizable, ref string FinalSQLCommand, ref int ParameterTotal)
        {
            var Builder = StringBuilderPool?.Get() ?? new StringBuilder();
            Builder.Append(FinalSQLCommand);
            for (var y = 0; y < Headers.Count; ++y)
            {
                var Command = Headers[y];
                if (ParameterTotal + Command.Parameters.Length >= 2000)
                    break;
                ParameterTotal += Command.Parameters.Length;
                Finalizable |= Command.Finalizable;
                if (Command.CommandType == CommandType.Text)
                {
                    var TempCommandText = Command.SQLCommand ?? string.Empty;
                    Builder.Append(Command.SQLCommand ?? string.Empty).Append(Environment.NewLine);

                    for (int i = 0, CommandParametersLength = Command.Parameters.Length; i < CommandParametersLength; i++)
                    {
                        var TempParameter = Command.Parameters[i];
                        FinalParameters.Add(TempParameter.CreateCopy(string.Empty));
                    }
                }
                else
                {
                    Builder.Append(Command.SQLCommand).Append(Environment.NewLine);
                    for (int i = 0, CommandParametersLength = Command.Parameters.Length; i < CommandParametersLength; i++)
                    {
                        var TempParameter = Command.Parameters[i];
                        FinalParameters.Add(TempParameter.CreateCopy(string.Empty));
                    }
                }
            }
            for (var y = Count; y < Commands.Count; ++y)
            {
                var Command = Commands[y];
                if (ParameterTotal + Command.Parameters.Length >= 2000)
                    break;
                ParameterTotal += Command.Parameters.Length;
                Finalizable |= Command.Finalizable;
                if (Command.CommandType == CommandType.Text)
                {
                    var TempCommandText = Command.SQLCommand ?? string.Empty;
                    var Suffix = "Command" + Count.ToString(CultureInfo.InvariantCulture);
                    Builder.Append(string.IsNullOrEmpty(Command.SQLCommand) ?
                                        string.Empty :
                                        ParameterRegex.Replace(Command.SQLCommand, x =>
                                        {
                                            var Param = Array.Find(Command.Parameters, z => z.ID == x.Groups["ParamName"].Value);
                                            return !(Param is null) ? x.Value + Suffix : x.Value;
                                        })).Append(Environment.NewLine);

                    for (int i = 0, CommandParametersLength = Command.Parameters.Length; i < CommandParametersLength; i++)
                    {
                        var TempParameter = Command.Parameters[i];
                        FinalParameters.Add(TempParameter.CreateCopy(Suffix));
                    }
                }
                else
                {
                    Builder.Append(Command.SQLCommand).Append(Environment.NewLine);
                    for (int i = 0, CommandParametersLength = Command.Parameters.Length; i < CommandParametersLength; i++)
                    {
                        var TempParameter = Command.Parameters[i];
                        FinalParameters.Add(TempParameter.CreateCopy(string.Empty));
                    }
                }
                ++Count;
            }
            FinalSQLCommand = Builder.ToString();
            StringBuilderPool?.Return(Builder);
        }
    }
}