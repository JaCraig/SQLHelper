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
using SQLHelper.ExtensionMethods;
using SQLHelper.HelperClasses.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SQLHelper.HelperClasses
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
        public Batch(IConnection source)
        {
            Commands = new List<ICommand>();
            Source = source;
        }

        /// <summary>
        /// Used to parse SQL commands to find parameters (when batching)
        /// </summary>
        private static readonly Regex ParameterRegex = new Regex(@"[^@](?<ParamStart>[:@?])(?<ParamName>\w+)", RegexOptions.Compiled);

        /// <summary>
        /// Command count
        /// </summary>
        public int CommandCount { get { return Commands.Count; } }

        /// <summary>
        /// Commands to batch
        /// </summary>
        protected IList<ICommand> Commands { get; private set; }

        /// <summary>
        /// Connection string
        /// </summary>
        protected IConnection Source { get; set; }

        /// <summary>
        /// Adds a command to be batched
        /// </summary>
        /// <typeparam name="TCallbackData">The type of the callback data.</typeparam>
        /// <param name="callBack">Callback action</param>
        /// <param name="callbackObject">Object used in the callback action</param>
        /// <param name="commandType">Command type</param>
        /// <param name="command">Command (SQL or stored procedure) to run</param>
        /// <returns>This</returns>
        public IBatch AddQuery<TCallbackData>(Action<ICommand, List<dynamic>, TCallbackData> callBack, TCallbackData callbackObject, CommandType commandType, string command)
        {
            Commands.Add(new Command<TCallbackData>(callBack, callbackObject, command, commandType, null));
            return this;
        }

        /// <summary>
        /// Adds a command to be batched
        /// </summary>
        /// <typeparam name="TCallbackData">The type of the callback data.</typeparam>
        /// <param name="callBack">Callback action</param>
        /// <param name="callbackObject">Object used in the callback action</param>
        /// <param name="command">Command (SQL or stored procedure) to run</param>
        /// <param name="commandType">Command type</param>
        /// <param name="parameters">Parameters to add</param>
        /// <returns>This</returns>
        public IBatch AddQuery<TCallbackData>(Action<ICommand, List<dynamic>, TCallbackData> callBack, TCallbackData callbackObject, string command, CommandType commandType, params object[] parameters)
        {
            Commands.Add(new Command<TCallbackData>(callBack, callbackObject, command, commandType, Source.ParameterPrefix, parameters));
            return this;
        }

        /// <summary>
        /// Adds a command to be batched
        /// </summary>
        /// <typeparam name="TCallbackData">The type of the callback data.</typeparam>
        /// <param name="callBack">Callback action</param>
        /// <param name="callbackObject">Object used in the callback action</param>
        /// <param name="command">Command (SQL or stored procedure) to run</param>
        /// <param name="commandType">Command type</param>
        /// <param name="parameters">Parameters to add</param>
        /// <returns>This</returns>
        public IBatch AddQuery<TCallbackData>(Action<ICommand, List<dynamic>, TCallbackData> callBack, TCallbackData callbackObject, string command, CommandType commandType, params IParameter[] parameters)
        {
            Commands.Add(new Command<TCallbackData>(callBack, callbackObject, command, commandType, parameters));
            return this;
        }

        /// <summary>
        /// Adds a batch's commands to the current batch
        /// </summary>
        /// <param name="batch">Batch to add</param>
        /// <returns>This</returns>
        public IBatch AddQuery(IBatch batch)
        {
            var TempValue = batch as Batch;
            if (TempValue == null)
                return this;
            Commands.Add(TempValue.Commands);
            return this;
        }

        /// <summary>
        /// Executes the commands and returns the results
        /// </summary>
        /// <returns>The results of the batched commands</returns>
        public List<List<dynamic>> Execute()
        {
            return ExecuteCommands();
        }

        /// <summary>
        /// Executes the commands and returns the results (async)
        /// </summary>
        /// <returns>The results of the batched commands</returns>
        public async Task<List<List<dynamic>>> ExecuteAsync()
        {
            return await ExecuteCommandsAsync();
        }

        /// <summary>
        /// Removes duplicate commands from the batch
        /// </summary>
        /// <returns>This</returns>
        public IBatch RemoveDuplicateCommands()
        {
            Commands = Commands.Distinct().ToList();
            return this;
        }

        /// <summary>
        /// Converts the batch to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Commands.ToString(x => x.ToString(), Environment.NewLine);
        }

        /// <summary>
        /// Checks whether a transaction is needed.
        /// </summary>
        /// <returns>True if it is, false otherwise</returns>
        protected bool CheckTransaction()
        {
            return Commands.Count > 1
                              && Commands.Any(x =>
                              {
                                  var TempCommand = x.SQLCommand.ToUpper();
                                  return TempCommand.Contains("INSERT")
                                            || TempCommand.Contains("UPDATE")
                                            || TempCommand.Contains("DELETE")
                                            || TempCommand.Contains("CREATE")
                                            || TempCommand.Contains("ALTER")
                                            || TempCommand.Contains("INTO")
                                            || TempCommand.Contains("DROP");
                              })
                              && !Commands.Any(x =>
                              {
                                  var TempCommand = x.SQLCommand.ToUpper();
                                  return TempCommand.Contains("ALTER DATABASE") || TempCommand.Contains("CREATE DATABASE");
                              });
        }

        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <param name="ReturnValue">The return value.</param>
        /// <param name="ExecutableCommand">The executable command.</param>
        /// <param name="FinalParameters">The final parameters.</param>
        /// <param name="Finalizable">if set to <c>true</c> [finalizable].</param>
        /// <param name="FinalSQLCommand">The final SQL command.</param>
        private static void GetResults(List<List<dynamic>> ReturnValue, DbCommand ExecutableCommand, List<IParameter> FinalParameters, bool Finalizable, string FinalSQLCommand)
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
                using (DbDataReader TempReader = ExecutableCommand.ExecuteReader())
                {
                    ReturnValue.Add(GetValues(TempReader));
                    while (TempReader.NextResult())
                    {
                        ReturnValue.Add(GetValues(TempReader));
                    }
                }
            }
            else
            {
                var TempValue = new List<dynamic>
                                {
                                    ExecutableCommand.ExecuteNonQuery()
                                };
                ReturnValue.Add(TempValue);
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
        private static async Task GetResultsAsync(List<List<dynamic>> ReturnValue, DbCommand ExecutableCommand, List<IParameter> FinalParameters, bool Finalizable, string FinalSQLCommand)
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
                using (DbDataReader TempReader = await ExecutableCommand.ExecuteReaderAsync())
                {
                    ReturnValue.Add(GetValues(TempReader));
                    while (TempReader.NextResult())
                    {
                        ReturnValue.Add(GetValues(TempReader));
                    }
                }
            }
            else
            {
                var TempValue = new List<dynamic>
                                {
                                    await ExecutableCommand.ExecuteNonQueryAsync()
                                };
                ReturnValue.Add(TempValue);
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="tempReader">The temporary reader.</param>
        /// <returns>The resulting values</returns>
        private static List<dynamic> GetValues(DbDataReader tempReader)
        {
            if (tempReader == null)
                return new List<dynamic>();
            var ReturnValue = new List<dynamic>();
            string[] FieldNames = new string[tempReader.FieldCount];
            for (int x = 0; x < tempReader.FieldCount; ++x)
            {
                FieldNames[x] = tempReader.GetName(x);
            }
            while (tempReader.Read())
            {
                var Value = new Dynamo();
                for (int x = 0; x < tempReader.FieldCount; ++x)
                {
                    Value.Add(FieldNames[x], tempReader[x]);
                }
                ReturnValue.Add(Value);
            }
            return ReturnValue;
        }

        /// <summary>
        /// Executes the commands.
        /// </summary>
        /// <returns>The list of results</returns>
        private List<List<dynamic>> ExecuteCommands()
        {
            if (Source == null)
                return new List<List<dynamic>>();
            if (Commands == null)
                return new List<List<dynamic>>();
            var ReturnValue = new List<List<dynamic>>();
            if (Commands.Count == 0)
            {
                ReturnValue.Add(new List<dynamic>());
                return ReturnValue;
            }
            var Factory = Source.Factory;
            using (DbConnection Connection = Factory.CreateConnection())
            {
                Connection.ConnectionString = Source.ConnectionString;
                using (DbCommand ExecutableCommand = Factory.CreateCommand())
                {
                    SetupCommand(Connection, ExecutableCommand);
                    try
                    {
                        int Count = 0;
                        while (true)
                        {
                            var FinalParameters = new List<IParameter>();
                            bool Finalizable = false;
                            string FinalSQLCommand = "";
                            int ParameterTotal = 0;
                            ExecutableCommand.Parameters.Clear();
                            SetupParameters(ref Count, FinalParameters, ref Finalizable, ref FinalSQLCommand, ref ParameterTotal);
                            GetResults(ReturnValue, ExecutableCommand, FinalParameters, Finalizable, FinalSQLCommand);
                            if (Count >= CommandCount)
                                break;
                        }
                        ExecutableCommand.Commit();
                    }
                    catch { ExecutableCommand.Rollback(); throw; }
                    finally { ExecutableCommand.Close(); }
                }
            }
            FinalizeCommands(ReturnValue);
            return ReturnValue;
        }

        /// <summary>
        /// Executes the commands asynchronously.
        /// </summary>
        /// <returns>The list of results</returns>
        private async Task<List<List<dynamic>>> ExecuteCommandsAsync()
        {
            if (Source == null)
                return new List<List<dynamic>>();
            if (Commands == null)
                return new List<List<dynamic>>();
            var ReturnValue = new List<List<dynamic>>();
            if (Commands.Count == 0)
            {
                ReturnValue.Add(new List<dynamic>());
                return ReturnValue;
            }
            var Factory = Source.Factory;
            using (DbConnection Connection = Factory.CreateConnection())
            {
                Connection.ConnectionString = Source.ConnectionString;
                using (DbCommand ExecutableCommand = Factory.CreateCommand())
                {
                    SetupCommand(Connection, ExecutableCommand);

                    try
                    {
                        int Count = 0;
                        while (true)
                        {
                            var FinalParameters = new List<IParameter>();
                            bool Finalizable = false;
                            string FinalSQLCommand = "";
                            int ParameterTotal = 0;
                            ExecutableCommand.Parameters.Clear();
                            SetupParameters(ref Count, FinalParameters, ref Finalizable, ref FinalSQLCommand, ref ParameterTotal);
                            await GetResultsAsync(ReturnValue, ExecutableCommand, FinalParameters, Finalizable, FinalSQLCommand);
                            if (Count >= CommandCount)
                                break;
                        }
                        ExecutableCommand.Commit();
                    }
                    catch { ExecutableCommand.Rollback(); throw; }
                    finally { ExecutableCommand.Close(); }
                }
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
                    Commands[x].Finalize(new List<dynamic>());
            }
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
            for (int y = Count; y < Commands.Count; ++y)
            {
                ICommand Command = Commands[y];
                if (ParameterTotal + Command.Parameters.Length >= 2000)
                    break;
                ParameterTotal += Command.Parameters.Length;
                Finalizable |= Commands[y].Finalizable;
                if (Command.CommandType == CommandType.Text)
                {
                    var TempCommandText = Command.SQLCommand ?? "";
                    string Suffix = "Command" + Count.ToString(CultureInfo.InvariantCulture);
                    FinalSQLCommand += string.IsNullOrEmpty(Command.SQLCommand) ?
                                        "" :
                                        ParameterRegex.Replace(Command.SQLCommand, x =>
                                        {
                                            var Param = Command.Parameters.FirstOrDefault(z => z.ID == x.Groups["ParamName"].Value);
                                            if (Param != null)
                                                return x.Value + Suffix;
                                            return x.Value;
                                        }) + Environment.NewLine;

                    foreach (IParameter TempParameter in Command.Parameters)
                    {
                        FinalParameters.Add(TempParameter.CreateCopy(Suffix));
                    }
                }
                else
                {
                    FinalSQLCommand += Command.SQLCommand + Environment.NewLine;
                    foreach (IParameter TempParameter in Command.Parameters)
                    {
                        FinalParameters.Add(TempParameter.CreateCopy(""));
                    }
                }
                ++Count;
            }
        }
    }
}