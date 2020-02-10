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

using System.Data;
using System.Data.Common;

namespace SQLHelperDB.HelperClasses.Interfaces
{
    /// <summary>
    /// Parameter interface
    /// </summary>
    /// <typeparam name="T">Value type</typeparam>
    public interface IParameter<T> : IParameter
    {
        /// <summary>
        /// The value that the parameter is associated with
        /// </summary>
        T Value { get; set; }
    }

    /// <summary>
    /// Parameter interface
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// Database type
        /// </summary>
        DbType DatabaseType { get; set; }

        /// <summary>
        /// Direction of the parameter
        /// </summary>
        ParameterDirection Direction { get; set; }

        /// <summary>
        /// The name that the parameter goes by
        /// </summary>
        string ID { get; set; }

        /// <summary>
        /// Gets the internal value.
        /// </summary>
        /// <value>The internal value.</value>
        object? InternalValue { get; }

        /// <summary>
        /// Gets the parameter starter.
        /// </summary>
        /// <value>The parameter starter.</value>
        string ParameterStarter { get; }

        /// <summary>
        /// Adds this parameter to the SQLHelper
        /// </summary>
        /// <param name="helper">SQLHelper</param>
        void AddParameter(DbCommand helper);

        /// <summary>
        /// Finds itself in the string command and adds the value
        /// </summary>
        /// <param name="command">Command to add to</param>
        /// <returns>The resulting string</returns>
        string AddParameter(string command);

        /// <summary>
        /// Creates a copy of the parameter
        /// </summary>
        /// <param name="suffix">Suffix to add to the parameter (for batching purposes)</param>
        /// <returns>A copy of the parameter</returns>
        IParameter CreateCopy(string suffix);
    }
}