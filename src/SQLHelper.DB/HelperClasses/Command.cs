﻿/*
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
using Microsoft.SqlServer.Management.SqlParser.Parser;
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
        /// <param name="callBack">Called when command has been executed</param>
        /// <param name="callbackObject">Object</param>
        /// <param name="header">
        /// Determines if this command is a "header" and should be carried across batches.
        /// </param>
        /// <param name="sqlCommand">SQL Command</param>
        /// <param name="commandType">Command type</param>
        /// <param name="parameters">Parameters</param>
        public Command(Action<ICommand, List<dynamic>, TCallbackData> callBack, TCallbackData callbackObject, bool header, string sqlCommand, CommandType commandType, IParameter[]? parameters)
        {
            SQLCommand = sqlCommand ?? string.Empty;
            CommandType = commandType;
            CallBack = callBack ?? DefaultAction;
            CallbackData = callbackObject;
            Parameters = parameters ?? Array.Empty<IParameter>();
            DetermineFinalizable(Parameters.FirstOrDefault()?.ParameterStarter ?? "@", SQLCommand.ToUpperInvariant());
            Header = header;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callBack">Called when command has been executed</param>
        /// <param name="callbackObject">Object</param>
        /// <param name="header">
        /// Determines if this command is a "header" and should be carried across batches.
        /// </param>
        /// <param name="sqlCommand">SQL Command</param>
        /// <param name="commandType">Command type</param>
        /// <param name="parameterStarter">Parameter starter</param>
        /// <param name="parameters">Parameters</param>
        public Command(Action<ICommand, List<dynamic>, TCallbackData> callBack, TCallbackData callbackObject, bool header, string sqlCommand, CommandType commandType, string parameterStarter, object[]? parameters)
        {
            SQLCommand = sqlCommand ?? string.Empty;
            CommandType = commandType;
            parameters ??= Array.Empty<object>();
            Parameters = new IParameter[parameters.Length];
            CallBack = callBack ?? DefaultAction;
            CallbackData = callbackObject;
            DetermineFinalizable(parameterStarter, SQLCommand.ToUpperInvariant());
            Header = header;

            for (int x = 0, parametersLength = parameters.Length; x < parametersLength; ++x)
            {
                var CurrentParameter = parameters[x];
                if (CurrentParameter is IParameter parameter)
                    Parameters[x] = parameter;
                else if (CurrentParameter is null)
                    Parameters[x] = new Parameter<object>(x.ToString(CultureInfo.InvariantCulture), default(DbType), null, ParameterDirection.Input, parameterStarter);
                else if (CurrentParameter is string TempParameter)
                    Parameters[x] = new StringParameter(x.ToString(CultureInfo.InvariantCulture), TempParameter, ParameterDirection.Input, parameterStarter);
                else
                    Parameters[x] = new Parameter<object>(x.ToString(CultureInfo.InvariantCulture), CurrentParameter, ParameterDirection.Input, parameterStarter);
            }
        }

        /// <summary>
        /// The simple select regex
        /// </summary>
        private static readonly Regex SimpleSelectRegex = new Regex(@"^SELECT\s|\sSELECT\s", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The internal hash code value.
        /// </summary>
        private int InternalHashCode;

        /// <summary>
        /// The internal to string value.
        /// </summary>
        private string InternalToString = string.Empty;

        /// <summary>
        /// Call back
        /// </summary>
        public Action<ICommand, List<dynamic>, TCallbackData> CallBack { get; }

        /// <summary>
        /// Object
        /// </summary>
        /// <value>The object.</value>
        public TCallbackData CallbackData { get; }

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
        /// Determines if this command is a "header" and should be carried across batches.
        /// </summary>
        /// <value><c>true</c> if header; otherwise, <c>false</c>.</value>
        public bool Header { get; }

        /// <summary>
        /// Parameters
        /// </summary>
        public IParameter[] Parameters { get; }

        /// <summary>
        /// SQL command
        /// </summary>
        public string SQLCommand { get; set; }

        /// <summary>
        /// Gets a value indicating whether [transaction needed].
        /// </summary>
        /// <value><c>true</c> if [transaction needed]; otherwise, <c>false</c>.</value>
        public bool TransactionNeeded { get; set; }

        /// <summary>
        /// Determines if the objects are equal
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>Determines if the commands are equal</returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

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
            if (CallBack is null)
                return;
            CallBack(this, result, CallbackData);
        }

        /// <summary>
        /// Returns the hash code for the command
        /// </summary>
        /// <returns>The hash code for the object</returns>
        public override int GetHashCode()
        {
            if (InternalHashCode == 0)
            {
                unchecked
                {
                    var ParameterTotal = 0;
                    for (int x = 0, ParametersLength = Parameters.Length; x < ParametersLength; ++x)
                    {
                        ParameterTotal += Parameters[x].GetHashCode();
                    }

                    if (ParameterTotal > 0)
                        InternalHashCode = (((SQLCommand.GetHashCode(StringComparison.InvariantCultureIgnoreCase) * 23) + CommandType.GetHashCode()) * 23) + ParameterTotal;
                    InternalHashCode = (SQLCommand.GetHashCode(StringComparison.InvariantCultureIgnoreCase) * 23) + CommandType.GetHashCode();
                }
            }
            return InternalHashCode;
        }

        /// <summary>
        /// Returns the string representation of the command
        /// </summary>
        /// <returns>The string representation of the command</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(InternalToString))
            {
                InternalToString = SQLCommand ?? string.Empty;
                for (int x = 0, ParametersLength = Parameters.Length; x < ParametersLength; ++x)
                {
                    InternalToString = Parameters[x].AddParameter(InternalToString);
                }
            }
            return InternalToString;
        }

        /// <summary>
        /// Default action.
        /// </summary>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        private void DefaultAction(ICommand arg1, List<dynamic> arg2, TCallbackData arg3)
        {
        }

        /// <summary>
        /// Determines if the query is finalizable.
        /// </summary>
        /// <param name="parameterStarter">The parameter starter.</param>
        /// <param name="ComparisonString">The comparison string.</param>
        private void DetermineFinalizable(string parameterStarter, string ComparisonString)
        {
            if (parameterStarter == "@" && ComparisonString.Contains("SELECT ", StringComparison.Ordinal) && ComparisonString.Contains("IF ", StringComparison.Ordinal))
            {
                var TempParser = new SimpleSelectFinder();//new SelectFinder();
                var Results = Parser.Parse(SQLCommand);
                Results.Script.Accept(TempParser);
                //Finalizable = Results
                //SQLParser.Parser.Parse(SQLCommand, TempParser, SQLParser.Enums.SQLType.TSql);
                Finalizable = TempParser.StatementFound;
            }
            else if (CommandType == CommandType.Text)
            {
                Finalizable = SimpleSelectRegex.IsMatch(ComparisonString);
            }
            else
            {
                Finalizable = true;
            }

            TransactionNeeded = (ComparisonString.Contains("INSERT", StringComparison.Ordinal)
                || ComparisonString.Contains("UPDATE", StringComparison.Ordinal)
                || ComparisonString.Contains("DELETE", StringComparison.Ordinal)
                || ComparisonString.Contains("INTO", StringComparison.Ordinal)
                || ComparisonString.Contains("DROP", StringComparison.Ordinal)
                || ComparisonString.Contains("CREATE", StringComparison.Ordinal)
                || ComparisonString.Contains("ALTER", StringComparison.Ordinal))
                && !ComparisonString.Contains("CREATE DATABASE", StringComparison.Ordinal)
                && !ComparisonString.Contains("ALTER DATABASE", StringComparison.Ordinal);
        }
    }
}