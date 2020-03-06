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

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SQLHelperDBTests.HelperClasses.Interfaces
{
    /// <summary>
    /// A batch of commands interface
    /// </summary>
    public interface IBatch
    {
        /// <summary>
        /// Number of commands being batched
        /// </summary>
        int CommandCount { get; }

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
        IBatch AddQuery<TCallbackData>(Action<ICommand, List<dynamic>, TCallbackData> callBack, TCallbackData callbackObject, bool header, string command, CommandType commandType, params object[]? parameters);

        /// <summary>
        /// Adds a batch's commands to the current batch
        /// </summary>
        /// <param name="batch">Batch to add</param>
        /// <returns>This</returns>
        IBatch AddQuery(IBatch batch);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <returns>This.</returns>
        IBatch Clear();

        /// <summary>
        /// Executes the commands and returns the results
        /// </summary>
        /// <returns>The results of the batched commands</returns>
        List<List<dynamic>> Execute();

        /// <summary>
        /// Executes the commands and returns the results (async)
        /// </summary>
        /// <returns>The results of the batched commands</returns>
        Task<List<List<dynamic>>> ExecuteAsync();

        /// <summary>
        /// Removes duplicate commands from the batch
        /// </summary>
        /// <returns>This</returns>
        IBatch RemoveDuplicateCommands();

        /// <summary>
        /// Sets the connection.
        /// </summary>
        /// <param name="databaseConnection">The database connection.</param>
        void SetConnection(IConnection databaseConnection);
    }
}