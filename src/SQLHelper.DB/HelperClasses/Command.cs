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
using SQLHelper.DB.HelperClasses;
using SQLHelper.HelperClasses.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace SQLHelper.HelperClasses
{
    /// <summary>
    /// Command holder class
    /// </summary>
    /// <typeparam name="TCallbackData">The type of the callback data.</typeparam>
    /// <seealso cref="ICommand"/>
    public class Command<TCallbackData> : ICommand
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlCommand">SQL Command</param>
        /// <param name="commandType">Command type</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="callBack">Called when command has been executed</param>
        /// <param name="callbackObject">Object</param>
        public Command(Action<ICommand, IList<dynamic>, TCallbackData> callBack, TCallbackData callbackObject, string sqlCommand, CommandType commandType, IParameter[] parameters)
        {
            SQLCommand = (sqlCommand ?? "");
            CommandType = commandType;
            CallBack = callBack ?? ((x, y, z) => { });
            Object = callbackObject;
            Parameters = parameters ?? new IParameter[0];
            if (Parameters.Any(x => x.ParameterStarter == "@"))
            {
                var TempParser = new SelectFinder();
                SQLParser.Parser.Parse(SQLCommand, TempParser, SQLParser.Enums.SQLType.TSql);
                Finalizable = TempParser.StatementFound;
            }
            else
            {
                Finalizable = SQLCommand.ToUpperInvariant().Contains("SELECT");
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlCommand">SQL Command</param>
        /// <param name="commandType">Command type</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="parameterStarter">Parameter starter</param>
        /// <param name="callBack">Called when command has been executed</param>
        /// <param name="callbackObject">Object</param>
        public Command(Action<ICommand, IList<dynamic>, TCallbackData> callBack, TCallbackData callbackObject, string sqlCommand, CommandType commandType, string parameterStarter, object[] parameters)
        {
            SQLCommand = (sqlCommand ?? "");
            CommandType = commandType;
            Parameters = new List<IParameter>();
            CallBack = callBack ?? ((x, y, z) => { });
            Object = callbackObject;
            if (parameterStarter == "@")
            {
                var TempParser = new SelectFinder();
                SQLParser.Parser.Parse(SQLCommand, TempParser, SQLParser.Enums.SQLType.TSql);
                Finalizable = TempParser.StatementFound;
            }
            else
            {
                Finalizable = SQLCommand.ToUpperInvariant().Contains("SELECT");
            }
            if (parameters != null)
            {
                foreach (object CurrentParameter in parameters)
                {
                    var TempParameter = CurrentParameter as string;
                    if (CurrentParameter == null)
                        Parameters.Add(new Parameter<object>(Parameters.Count().ToString(CultureInfo.InvariantCulture), default(DbType), null, ParameterDirection.Input, parameterStarter));
                    else if (TempParameter != null)
                        Parameters.Add(new StringParameter(Parameters.Count().ToString(CultureInfo.InvariantCulture), TempParameter, ParameterDirection.Input, parameterStarter));
                    else
                        Parameters.Add(new Parameter<object>(Parameters.Count().ToString(CultureInfo.InvariantCulture), CurrentParameter, ParameterDirection.Input, parameterStarter));
                }
            }
        }

        /// <summary>
        /// Call back
        /// </summary>
        public Action<ICommand, IList<dynamic>, TCallbackData> CallBack { get; private set; }

        /// <summary>
        /// Command type
        /// </summary>
        public CommandType CommandType { get; set; }

        /// <summary>
        /// Used to determine if Finalize should be called.
        /// </summary>
        public bool Finalizable { get; private set; }

        /// <summary>
        /// Object
        /// </summary>
        public TCallbackData Object { get; private set; }

        /// <summary>
        /// Parameters
        /// </summary>
        public ICollection<IParameter> Parameters { get; private set; }

        /// <summary>
        /// SQL command
        /// </summary>
        public string SQLCommand { get; set; }

        /// <summary>
        /// Determines if the objects are equal
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>Determines if the commands are equal</returns>
        public override bool Equals(object obj)
        {
            var OtherCommand = obj as Command<TCallbackData>;
            if (OtherCommand == null)
                return false;

            if (OtherCommand.SQLCommand != SQLCommand
                || OtherCommand.CommandType != CommandType
                || Parameters.Count != OtherCommand.Parameters.Count)
                return false;

            foreach (IParameter TempParameter in Parameters)
                if (!OtherCommand.Parameters.Contains(TempParameter))
                    return false;

            foreach (IParameter TempParameter in OtherCommand.Parameters)
                if (!Parameters.Contains(TempParameter))
                    return false;

            return true;
        }

        /// <summary>
        /// Called after the command is run
        /// </summary>
        /// <param name="result">Result of the command</param>
        public void Finalize(IList<dynamic> result)
        {
            if (CallBack == null)
                return;
            CallBack(this, result, Object);
        }

        /// <summary>
        /// Returns the hash code for the command
        /// </summary>
        /// <returns>The hash code for the object</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int ParameterTotal = 0;
                foreach (IParameter TempParameter in Parameters)
                {
                    ParameterTotal += TempParameter.GetHashCode();
                }
                if (ParameterTotal > 0)
                    return (SQLCommand.GetHashCode() * 23 + CommandType.GetHashCode()) * 23 + ParameterTotal;
                return SQLCommand.GetHashCode() * 23 + CommandType.GetHashCode();
            }
        }

        /// <summary>
        /// Returns the string representation of the command
        /// </summary>
        /// <returns>The string representation of the command</returns>
        public override string ToString()
        {
            var TempCommand = SQLCommand.Check("");
            Parameters.ForEach(x => { TempCommand = x.AddParameter(TempCommand); });
            return TempCommand;
        }
    }
}