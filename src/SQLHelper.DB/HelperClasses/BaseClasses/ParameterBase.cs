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

using BigBook.Comparison;
using ObjectCartographer;
using SQLHelperDB.HelperClasses.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace SQLHelperDB.HelperClasses.BaseClasses
{
    /// <summary>
    /// Parameter base class
    /// </summary>
    /// <typeparam name="TDataType">Data type of the parameter</typeparam>
    /// <remarks>Constructor</remarks>
    /// <param name="id">ID of the parameter</param>
    /// <param name="type">Database type</param>
    /// <param name="value">Value of the parameter</param>
    /// <param name="direction">Direction of the parameter</param>
    /// <param name="parameterStarter">
    /// What the database expects as the parameter starting string ("@" for SQL Server, ":" for
    /// Oracle, etc.)
    /// </param>
    public abstract class ParameterBase<TDataType>(string id, DbType type, object? value = null, ParameterDirection direction = ParameterDirection.Input, string parameterStarter = "@") : IParameter<TDataType>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">ID of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <param name="direction">Direction of the parameter</param>
        /// <param name="parameterStarter">
        /// What the database expects as the parameter starting string ("@" for SQL Server, ":" for
        /// Oracle, etc.)
        /// </param>
        protected ParameterBase(string id, TDataType value, ParameterDirection direction = ParameterDirection.Input, string parameterStarter = "@")
            : this(id, value is null ? typeof(TDataType).To<DbType>() : value.GetType().To<DbType>(), value, direction, parameterStarter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterBase{TDataType}"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected ParameterBase(ParameterBase<TDataType> parameter)
            : this(parameter.ID, parameter.DatabaseType, parameter.Value, parameter.Direction, parameter.ParameterStarter)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">ID of the parameter</param>
        /// <param name="type">Database type</param>
        /// <param name="value">Value of the parameter</param>
        /// <param name="direction">Direction of the parameter</param>
        /// <param name="parameterStarter">
        /// What the database expects as the parameter starting string ("@" for SQL Server, ":" for
        /// Oracle, etc.)
        /// </param>
        protected ParameterBase(string id, SqlDbType type, object? value = null, ParameterDirection direction = ParameterDirection.Input, string parameterStarter = "@")
            : this(id, type.To<DbType>(), value, direction, parameterStarter)
        {
        }

        /// <summary>
        /// Database type
        /// </summary>
        public DbType DatabaseType { get; set; } = type;

        /// <summary>
        /// Direction of the parameter
        /// </summary>
        public ParameterDirection Direction { get; set; } = direction;

        /// <summary>
        /// The Name that the parameter goes by
        /// </summary>
        public string ID { get; set; } = id;

        /// <summary>
        /// Gets the internal value.
        /// </summary>
        /// <value>The internal value.</value>
        public object? InternalValue => Value;

        /// <summary>
        /// Starting string of the parameter
        /// </summary>
        public string ParameterStarter { get; set; } = parameterStarter;

        /// <summary>
        /// Parameter value
        /// </summary>
        public TDataType Value { get; set; } = (TDataType)value!;

        /// <summary>
        /// Batch ID
        /// </summary>
        protected string BatchID { get; set; } = id;

        /// <summary>
        /// != operator
        /// </summary>
        /// <param name="first">First item</param>
        /// <param name="second">Second item</param>
        /// <returns>returns true if they are not equal, false otherwise</returns>
        public static bool operator !=(ParameterBase<TDataType> first, ParameterBase<TDataType> second) => !(first == second);

        /// <summary>
        /// The == operator
        /// </summary>
        /// <param name="first">First item</param>
        /// <param name="second">Second item</param>
        /// <returns>true if the first and second item are the same, false otherwise</returns>
        public static bool operator ==(ParameterBase<TDataType> first, ParameterBase<TDataType> second)
        {
            return ReferenceEquals(first, second)
                || (first is not null
                    && second is not null
                    && first.GetHashCode() == second.GetHashCode());
        }

        /// <summary>
        /// Adds this parameter to the SQLHelper
        /// </summary>
        /// <param name="helper">SQLHelper</param>
        public abstract void AddParameter(DbCommand helper);

        /// <summary>
        /// Finds itself in the string command and adds the value
        /// </summary>
        /// <param name="command">Command to add to</param>
        /// <returns>The resulting string</returns>
        public string AddParameter(string command)
        {
            if (string.IsNullOrEmpty(command))
                return string.Empty;
            var StringValue = Value is null ? "NULL" : Value.ToString();
            return command.Replace(ParameterStarter + ID, typeof(TDataType) == typeof(string) ? "'" + StringValue + "'" : StringValue, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates a copy of the parameter
        /// </summary>
        /// <param name="suffix">Suffix to add to the parameter (for batching purposes)</param>
        /// <returns>A copy of the parameter</returns>
        public abstract IParameter CreateCopy(string suffix);

        /// <summary>
        /// Determines if the objects are equal
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public override bool Equals(object? obj)
        {
            return (obj is ParameterBase<TDataType> OtherParameter)
                && OtherParameter.DatabaseType == DatabaseType
                && OtherParameter.Direction == Direction
                && OtherParameter.ID == ID
                && GenericEqualityComparer<TDataType>.Comparer.Equals(OtherParameter.Value, Value);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            var HashCode = 2030399226;
            HashCode = (HashCode * -1521134295) + DatabaseType.GetHashCode();
            HashCode = (HashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(ID);
            return (HashCode * -1521134295) + EqualityComparer<TDataType>.Default.GetHashCode(Value!);
        }

        /// <summary>
        /// Returns the string version of the parameter
        /// </summary>
        /// <returns>The string representation of the parameter</returns>
        public override string ToString() => ParameterStarter + ID;
    }
}