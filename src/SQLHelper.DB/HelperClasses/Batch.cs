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
        public Batch(ISource source)
        {
            Commands = new List<ICommand>();
            Source = source;
        }

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
        protected ISource Source { get; set; }

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
        /// <param name="commandType">Command type</param>
        /// <param name="command">Command (SQL or stored procedure) to run</param>
        /// <returns>This</returns>
        public IBatch AddQuery<TCallbackData>(Action<ICommand, IList<dynamic>, TCallbackData> callBack, TCallbackData callbackObject, CommandType commandType, string command)
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
        public IBatch AddQuery<TCallbackData>(Action<ICommand, IList<dynamic>, TCallbackData> callBack, TCallbackData callbackObject, string command, CommandType commandType, params object[] parameters)
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
        public IBatch AddQuery<TCallbackData>(Action<ICommand, IList<dynamic>, TCallbackData> callBack, TCallbackData callbackObject, string command, CommandType commandType, params IParameter[] parameters)
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
        public IList<IList<dynamic>> Execute()
        {
            return ExecuteCommands();
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

        private static IList<dynamic> GetValues(DbDataReader tempReader)
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

        private IList<IList<dynamic>> ExecuteCommands()
        {
            if (Source == null)
                return new List<IList<dynamic>>();
            if (Commands == null)
                return new List<IList<dynamic>>();
            var ReturnValue = new List<IList<dynamic>>();
            if (Commands.Count == 0)
            {
                ReturnValue.Add(new List<dynamic>());
                return ReturnValue;
            }
            var Factory = Source.Factory;
            using (DbConnection Connection = Factory.CreateConnection())
            {
                Connection.ConnectionString = Source.Connection;
                using (DbCommand ExecutableCommand = Factory.CreateCommand())
                {
                    ExecutableCommand.Connection = Connection;
                    ExecutableCommand.CommandType = CommandType.Text;
                    if (CheckTransaction())
                        ExecutableCommand.BeginTransaction();
                    ExecutableCommand.Open();

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
                            for (int y = Count; y < Commands.Count; ++y)
                            {
                                ICommand Command = Commands[y];
                                if (ParameterTotal + Command.Parameters.Count > 2100)
                                    break;
                                ParameterTotal += Command.Parameters.Count;
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

                            ExecutableCommand.CommandText = FinalSQLCommand;
                            FinalParameters.ForEach(x => x.AddParameter(ExecutableCommand));
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
                                var TempValue = new List<dynamic>();
                                TempValue.Add(ExecutableCommand.ExecuteNonQuery());
                                ReturnValue.Add(TempValue);
                            }
                            if (Count >= CommandCount)
                                break;
                        }
                        ExecutableCommand.Commit();
                    }
                    catch { ExecutableCommand.Rollback(); throw; }
                    finally { ExecutableCommand.Close(); }
                }
            }
            for (int x = 0, y = 0; x < Commands.Count(); ++x)
            {
                if (Commands[x].Finalizable)
                {
                    Commands[x].Finalize(ReturnValue[y]);
                    ++y;
                }
                else
                    Commands[x].Finalize(new List<dynamic>());
            }
            return ReturnValue;
        }
    }
}