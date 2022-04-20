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
using ObjectCartographer;
using SQLHelperDBTests.HelperClasses;
using SQLHelperDBTests.HelperClasses.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SQLHelperDBTests
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
        public SQLHelper(IConfiguration configuration, DbProviderFactory? factory = null, string database = "Default")
            : this(Connections.ContainsKey(database) ? Connections[database] : new Connection(configuration, factory ?? SqlClientFactory.Instance, database))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLHelper"/> class.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        public SQLHelper(IConnection connection)
        {
            SetConnection(connection);
        }

        /// <summary>
        /// Gets the number of commands currently in the batch.
        /// </summary>
        /// <value>The number of commands currently in the batch</value>
        public int Count => Batch.CommandCount;

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public IConnection DatabaseConnection { get; private set; }

        /// <summary>
        /// Gets the batch.
        /// </summary>
        /// <value>The batch.</value>
        protected IBatch Batch { get; private set; }

        /// <summary>
        /// Gets the connections.
        /// </summary>
        /// <value>The connections.</value>
        private static ConcurrentDictionary<string, IConnection> Connections { get; } = new ConcurrentDictionary<string, IConnection>();

        /// <summary>
        /// Adds a query that gets carried across in internal batches.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>This</returns>
        public SQLHelper AddHeader(CommandType commandType, string command, params object[]? parameters) => AddHeader<object>(DefaultAction, null!, commandType, command, parameters);

        /// <summary>
        /// Adds a query that gets carried across in internal batches.
        /// </summary>
        /// <typeparam name="TCallbackData">The type of the callback data.</typeparam>
        /// <param name="callback">The callback.</param>
        /// <param name="callbackObject">The callback object.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>This</returns>
        public SQLHelper AddHeader<TCallbackData>(Action<ICommand, List<dynamic>, TCallbackData> callback, TCallbackData callbackObject,
                    CommandType commandType, string command, params object[]? parameters)
        {
            Batch.AddQuery(callback, callbackObject, true, command, commandType, parameters);
            return this;
        }

        /// <summary>
        /// Adds a command.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>This</returns>
        public SQLHelper AddQuery(CommandType commandType, string command, params object[]? parameters) => AddQuery<object>(DefaultAction, null!, commandType, command, parameters);

        /// <summary>
        /// Adds a command which will call the callback function with the object specified when it
        /// </summary>
        /// <typeparam name="TCallbackData">The type of the callback data.</typeparam>
        /// <param name="callback">The callback.</param>
        /// <param name="callbackObject">The callback object.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>This</returns>
        public SQLHelper AddQuery<TCallbackData>(Action<ICommand, List<dynamic>, TCallbackData> callback, TCallbackData callbackObject,
            CommandType commandType, string command, params object[]? parameters)
        {
            Batch.AddQuery(callback, callbackObject, false, command, commandType, parameters);
            return this;
        }

        /// <summary>
        /// Adds an SQLHelper's commands to this instance
        /// </summary>
        /// <param name="helper">The helper to copy the commands from</param>
        /// <returns>This</returns>
        public SQLHelper AddQuery(SQLHelper helper)
        {
            if (helper is not null)
                Batch.AddQuery(helper.Batch);
            return this;
        }

        /// <summary>
        /// Clears the system and creates a new batch.
        /// </summary>
        /// <returns>This</returns>
        public SQLHelper CreateBatch()
        {
            Batch.Clear();
            return this;
        }

        /// <summary>
        /// Creates the batch using the connection specified.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>This</returns>
        public SQLHelper CreateBatch(IConnection connection)
        {
            Batch.Clear();
            SetConnection(connection);
            return this;
        }

        /// <summary>
        /// Creates the batch using the connection info specified.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="database">The database.</param>
        /// <returns>This.</returns>
        public SQLHelper CreateBatch(IConfiguration configuration, DbProviderFactory? factory = null, string database = "Default") => CreateBatch(Connections.ContainsKey(database) ? Connections[database] : new Connection(configuration, factory ?? SqlClientFactory.Instance, database));

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns>The results of the batched queries.</returns>
        public List<List<dynamic>> Execute() => Batch.Execute();

        /// <summary>
        /// Executes the queries asynchronously.
        /// </summary>
        /// <returns>The result of the queries</returns>
        public Task<List<List<dynamic>>> ExecuteAsync() => Batch.ExecuteAsync();

        /// <summary>
        /// Executes the batched commands and returns the first value, ignoring the rest.
        /// </summary>
        /// <typeparam name="TData">The type of the data to return.</typeparam>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The first value of the batch</returns>
        public TData ExecuteScalar<TData>(TData defaultValue = default) => AsyncHelper.RunSync(() => ExecuteScalarAsync(defaultValue));

        /// <summary>
        /// Executes the batched commands and returns the first value, ignoring the rest (async).
        /// </summary>
        /// <typeparam name="TData">The type of the data to return.</typeparam>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The first value of the batch</returns>
        public async Task<TData> ExecuteScalarAsync<TData>(TData defaultValue = default)
        {
            var BatchResults = await Batch.ExecuteAsync().ConfigureAwait(false);
            if (BatchResults.Count == 0 || BatchResults[0].Count == 0)
                return defaultValue;
            if (BatchResults[0][0] is not IDictionary<string, object> Value)
                return ((object)BatchResults[0][0]).To(defaultValue);
            return Value[Value.Keys.First()].To(defaultValue);
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
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString() => Batch.ToString() ?? string.Empty;

        /// <summary>
        /// The default action
        /// </summary>
        /// <param name="___">Ignored</param>
        /// <param name="__">Ignored</param>
        /// <param name="_">Ignored</param>
        private static void DefaultAction(ICommand ___, List<dynamic> __, object _) { }

        /// <summary>
        /// Sets the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private void SetConnection(IConnection connection)
        {
            DatabaseConnection = connection ?? throw new ArgumentNullException(nameof(connection));
            if (!Connections.ContainsKey(connection.Name))
                Connections.AddOrUpdate(connection.Name, connection, (_, value) => value);
            Batch ??= new Batch(DatabaseConnection);
            Batch.SetConnection(DatabaseConnection);
        }
    }
}