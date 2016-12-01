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
using Microsoft.Extensions.Configuration;
using SQLHelper.HelperClasses;
using SQLHelper.HelperClasses.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace SQLHelper
{
    /// <summary>
    /// SQL helper class
    /// </summary>
    public class SQLHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLHelper"/> class.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="database">The database.</param>
        public SQLHelper(IConfiguration configuration, DbProviderFactory factory, string database = "Default")
        {
            DatabaseSource = new Source(configuration, factory, "", database);
            Batch = new Batch(DatabaseSource);
        }

        /// <summary>
        /// Gets the batch.
        /// </summary>
        /// <value>The batch.</value>
        protected IBatch Batch { get; private set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        protected ISource DatabaseSource { get; private set; }

        /// <summary>
        /// Adds a command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>This</returns>
        public SQLHelper AddQuery(string command, CommandType commandType, params IParameter[] parameters)
        {
            return AddQuery<object>((x, y, z) => { }, null, command, commandType, parameters);
        }

        /// <summary>
        /// Adds a command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>This</returns>
        public SQLHelper AddQuery(CommandType commandType, string command, params object[] parameters)
        {
            return AddQuery<object>((x, y, z) => { }, null, commandType, command, parameters);
        }

        /// <summary>
        /// Adds a command.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="callbackObject">The callback object.</param>
        /// <param name="command">The command.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>This</returns>
        public SQLHelper AddQuery<TCallbackData>(Action<ICommand, IList<dynamic>, TCallbackData> callback, TCallbackData callbackObject,
            string command, CommandType commandType, params IParameter[] parameters)
        {
            Batch.AddQuery(callback, callbackObject, command, commandType, parameters);
            return this;
        }

        /// <summary>
        /// Adds a command which will call the callback function with the object specified when it
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="callbackObject">The callback object.</param>
        /// <param name="command">The command.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>This</returns>
        public SQLHelper AddQuery<TCallbackData>(Action<ICommand, IList<dynamic>, TCallbackData> callback, TCallbackData callbackObject,
            CommandType commandType, string command, params object[] parameters)
        {
            Batch.AddQuery(callback, callbackObject, command, commandType, parameters);
            return this;
        }

        /// <summary>
        /// Adds an SQLHelper's commands to this instance
        /// </summary>
        /// <param name="helper">The helper to copy the commands from</param>
        /// <returns>This</returns>
        public SQLHelper AddQuery(SQLHelper helper)
        {
            Batch.AddQuery(helper.Batch);
            return this;
        }

        /// <summary>
        /// Clears the system and creates a new batch.
        /// </summary>
        /// <returns>This</returns>
        public SQLHelper CreateBatch()
        {
            Batch = new Batch(DatabaseSource);
            return this;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        public IList<IList<dynamic>> Execute()
        {
            return Batch.Execute();
        }

        /// <summary>
        /// Executes the batched commands and returns the first value, ignoring the rest.
        /// </summary>
        /// <typeparam name="TData">The type of the data to return.</typeparam>
        /// <returns>The first value of the batch</returns>
        public TData ExecuteScalar<TData>()
        {
            return ((object)Batch.Execute()[0][0]).To<object, TData>();
        }

        /// <summary>
        /// Removes duplicate queries from the batch.
        /// </summary>
        /// <returns>This</returns>
        public SQLHelper RemoveDuplicateCommands()
        {
            Batch.RemoveDuplicateCommands();
            return this;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return Batch.ToString();
        }
    }
}