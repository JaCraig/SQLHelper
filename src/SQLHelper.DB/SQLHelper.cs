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
using System.Linq;
using System.Threading.Tasks;

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
            : this(new Connection(configuration, factory, database))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLHelper"/> class.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        public SQLHelper(IConnection connection)
        {
            DatabaseConnection = connection;
            Batch = new Batch(DatabaseConnection);
        }

        /// <summary>
        /// Gets the number of commands currently in the batch.
        /// </summary>
        /// <value>The number of commands currently in the batch</value>
        public int Count { get { return Batch.CommandCount; } }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public IConnection DatabaseConnection { get; }

        /// <summary>
        /// Gets the batch.
        /// </summary>
        /// <value>The batch.</value>
        protected IBatch Batch { get; private set; }

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
        /// <typeparam name="TCallbackData">The type of the callback data.</typeparam>
        /// <param name="callback">The callback.</param>
        /// <param name="callbackObject">The callback object.</param>
        /// <param name="command">The command.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>This</returns>
        public SQLHelper AddQuery<TCallbackData>(Action<ICommand, List<dynamic>, TCallbackData> callback, TCallbackData callbackObject,
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
        public SQLHelper AddQuery<TCallbackData>(Action<ICommand, List<dynamic>, TCallbackData> callback, TCallbackData callbackObject,
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
            Batch = new Batch(DatabaseConnection);
            return this;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns>The results of the batched queries.</returns>
        public List<List<dynamic>> Execute()
        {
            return Batch.Execute();
        }

        /// <summary>
        /// Executes the queries asynchronously.
        /// </summary>
        /// <returns>The result of the queries</returns>
        public Task<List<List<dynamic>>> ExecuteAsync()
        {
            return Batch.ExecuteAsync();
        }

        /// <summary>
        /// Executes the batched commands and returns the first value, ignoring the rest.
        /// </summary>
        /// <typeparam name="TData">The type of the data to return.</typeparam>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The first value of the batch</returns>
        public TData ExecuteScalar<TData>(TData defaultValue = default(TData))
        {
            var BatchResults = Batch.Execute();
            if (BatchResults.Count == 0 || BatchResults[0].Count == 0)
                return defaultValue;
            return !(BatchResults[0][0] is IDictionary<string, object> Value) ?
                ((object)BatchResults[0][0]).To(defaultValue) :
                Value[Value.Keys.First()].To(defaultValue);
        }

        /// <summary>
        /// Executes the batched commands and returns the first value, ignoring the rest (async).
        /// </summary>
        /// <typeparam name="TData">The type of the data to return.</typeparam>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The first value of the batch</returns>
        public async Task<TData> ExecuteScalarAsync<TData>(TData defaultValue = default(TData))
        {
            var BatchResults = await Batch.ExecuteAsync().ConfigureAwait(false);
            if (BatchResults.Count == 0 || BatchResults[0].Count == 0)
                return defaultValue;
            return !(BatchResults[0][0] is IDictionary<string, object> Value) ?
                ((object)BatchResults[0][0]).To(defaultValue) :
                Value[Value.Keys.First()].To(defaultValue);
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