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
using SQLHelperDB.HelperClasses.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace SQLHelperDB.HelperClasses
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
        public Command(Action<ICommand, List<dynamic>, TCallbackData> callBack, TCallbackData callbackObject, string sqlCommand, CommandType commandType, IParameter[] parameters)
        {
            SQLCommand = (sqlCommand ?? "");
            CommandType = commandType;
            CallBack = callBack ?? ((x, y, z) => { });
            Object = callbackObject;
            Parameters = parameters ?? new IParameter[0];
            var ComparisonString = SQLCommand.ToUpperInvariant();
            DetermineFinalizable(Parameters.FirstOrDefault()?.ParameterStarter ?? "@", ComparisonString);
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
        public Command(Action<ICommand, List<dynamic>, TCallbackData> callBack, TCallbackData callbackObject, string sqlCommand, CommandType commandType, string parameterStarter, object[] parameters)
        {
            SQLCommand = (sqlCommand ?? "");
            CommandType = commandType;
            parameters = parameters ?? new object[0];
            Parameters = new IParameter[parameters.Length];
            CallBack = callBack ?? ((x, y, z) => { });
            Object = callbackObject;
            var ComparisonString = SQLCommand.ToUpperInvariant();
            DetermineFinalizable(parameterStarter, ComparisonString);
            if (parameters != null)
            {
                for (int x = 0, parametersLength = parameters.Length; x < parametersLength; ++x)
                {
                    object CurrentParameter = parameters[x];
                    var TempParameter = CurrentParameter as string;
                    if (CurrentParameter == null)
                        Parameters[x] = new Parameter<object>(x.ToString(CultureInfo.InvariantCulture), default(DbType), null, ParameterDirection.Input, parameterStarter);
                    else if (TempParameter != null)
                        Parameters[x] = new StringParameter(x.ToString(CultureInfo.InvariantCulture), TempParameter, ParameterDirection.Input, parameterStarter);
                    else
                        Parameters[x] = new Parameter<object>(x.ToString(CultureInfo.InvariantCulture), CurrentParameter, ParameterDirection.Input, parameterStarter);
                }
            }
        }

        /// <summary>
        /// The simple select regex
        /// </summary>
        private static readonly Regex SimpleSelectRegex = new Regex(@"^SELECT\s|\sSELECT\s", RegexOptions.IgnoreCase);

        /// <summary>
        /// Call back
        /// </summary>
        public Action<ICommand, List<dynamic>, TCallbackData> CallBack { get; }

        /// <summary>
        /// Command type
        /// </summary>
        public CommandType CommandType { get; set; }

        /// <summary>
        /// Used to determine if Finalize should be called.
        /// </summary>
        /// <value><c>true</c> if finalizable; otherwise, <c>false</c>.</value>
        public bool Finalizable { get; private set; }

        /// <summary>
        /// Object
        /// </summary>
        /// <value>The object.</value>
        public TCallbackData Object { get; }

        /// <summary>
        /// Parameters
        /// </summary>
        public IParameter[] Parameters { get; }

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
            if (!(obj is Command<TCallbackData> OtherCommand))
                return false;

            if (OtherCommand.SQLCommand != SQLCommand
                || OtherCommand.CommandType != CommandType
                || Parameters.Length != OtherCommand.Parameters.Length)
            {
                return false;
            }

            for (int x = 0, ParametersLength = Parameters.Length; x < ParametersLength; ++x)
            {
                if (!OtherCommand.Parameters.Contains(Parameters[x]))
                    return false;
            }

            for (int x = 0, OtherCommandParametersLength = OtherCommand.Parameters.Length; x < OtherCommandParametersLength; ++x)
            {
                if (!Parameters.Contains(OtherCommand.Parameters[x]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Called after the command is run
        /// </summary>
        /// <param name="result">Result of the command</param>
        public void Finalize(List<dynamic> result)
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
                for (int x = 0, ParametersLength = Parameters.Length; x < ParametersLength; ++x)
                {
                    ParameterTotal += Parameters[x].GetHashCode();
                }

                if (ParameterTotal > 0)
                    return (((SQLCommand.GetHashCode() * 23) + CommandType.GetHashCode()) * 23) + ParameterTotal;
                return (SQLCommand.GetHashCode() * 23) + CommandType.GetHashCode();
            }
        }

        /// <summary>
        /// Returns the string representation of the command
        /// </summary>
        /// <returns>The string representation of the command</returns>
        public override string ToString()
        {
            var TempCommand = SQLCommand.Check("");
            for (int x = 0, ParametersLength = Parameters.Length; x < ParametersLength; ++x)
            {
                TempCommand = Parameters[x].AddParameter(TempCommand);
            }

            return TempCommand;
        }

        private void DetermineFinalizable(string parameterStarter, string ComparisonString)
        {
            if (parameterStarter == "@" && ComparisonString.Contains("SELECT ") && ComparisonString.Contains("IF "))
            {
                var TempParser = new SelectFinder();
                SQLParser.Parser.Parse(SQLCommand, TempParser, SQLParser.Enums.SQLType.TSql);
                Finalizable = TempParser.StatementFound;
            }
            else
            {
                Finalizable = SimpleSelectRegex.IsMatch(ComparisonString);
            }
        }
    }
}