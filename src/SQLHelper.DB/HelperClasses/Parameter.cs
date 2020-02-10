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

using SQLHelperDB.ExtensionMethods;
using SQLHelperDB.HelperClasses.BaseClasses;
using SQLHelperDB.HelperClasses.Interfaces;
using System.Data;
using System.Data.Common;

namespace SQLHelperDB.HelperClasses
{
    /// <summary>
    /// Holds parameter information
    /// </summary>
    /// <typeparam name="TDataType">Data type of the parameter</typeparam>
    public class Parameter<TDataType> : ParameterBase<TDataType>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">ID of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <param name="direction">Direction of the parameter</param>
        /// <param name="parameterStarter">Parameter starter</param>
        public Parameter(string id, TDataType value, ParameterDirection direction = ParameterDirection.Input, string parameterStarter = "@")
            : base(id, value, direction, parameterStarter)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">ID of the parameter</param>
        /// <param name="type">Database type</param>
        /// <param name="value">Value of the parameter</param>
        /// <param name="direction">Direction of the parameter</param>
        /// <param name="parameterStarter">Parameter starter</param>
        public Parameter(string id, SqlDbType type, object? value = null, ParameterDirection direction = ParameterDirection.Input, string parameterStarter = "@")
            : base(id, type, value, direction, parameterStarter)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">ID of the parameter</param>
        /// <param name="type">Database type</param>
        /// <param name="value">Value of the parameter</param>
        /// <param name="direction">Direction of the parameter</param>
        /// <param name="parameterStarter">Parameter starter</param>
        public Parameter(string id, DbType type, object? value = null, ParameterDirection direction = ParameterDirection.Input, string parameterStarter = "@")
            : base(id, type, value, direction, parameterStarter)
        {
        }

        /// <summary>
        /// Adds this parameter to the SQLHelper
        /// </summary>
        /// <param name="helper">SQLHelper</param>
        public override void AddParameter(DbCommand helper)
        {
            helper.AddParameter(ID, DatabaseType, Value, Direction);
        }

        /// <summary>
        /// Creates a copy of the parameter
        /// </summary>
        /// <param name="suffix">Suffix to add to the parameter (for batching purposes)</param>
        /// <returns>A copy of the parameter</returns>
        public override IParameter CreateCopy(string suffix)
        {
            return new Parameter<TDataType>(ID + suffix, DatabaseType, Value, Direction, ParameterStarter);
        }
    }
}