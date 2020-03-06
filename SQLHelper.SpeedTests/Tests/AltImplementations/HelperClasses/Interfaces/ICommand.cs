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

using System.Collections.Generic;
using System.Data;

namespace SQLHelperDBTests.HelperClasses.Interfaces
{
    /// <summary>
    /// Command interface
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Command type
        /// </summary>
        CommandType CommandType { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ICommand"/> is finalizable.
        /// </summary>
        /// <value><c>true</c> if finalizable; otherwise, <c>false</c>.</value>
        bool Finalizable { get; }

        /// <summary>
        /// Parameters associated with the command
        /// </summary>
        IParameter[] Parameters { get; }

        /// <summary>
        /// Actual SQL command
        /// </summary>
        string SQLCommand { get; }

        /// <summary>
        /// Gets a value indicating whether [transaction needed].
        /// </summary>
        /// <value><c>true</c> if [transaction needed]; otherwise, <c>false</c>.</value>
        bool TransactionNeeded { get; }

        /// <summary>
        /// Called after the command is run
        /// </summary>
        /// <param name="result">Result of the command</param>
        void Finalize(List<dynamic> result);
    }
}