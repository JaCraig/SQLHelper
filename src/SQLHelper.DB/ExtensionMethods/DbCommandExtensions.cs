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
using BigBook.Comparison;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SQLHelper.ExtensionMethods
{
    /// <summary>
    /// Extension methods for DbCommand
    /// </summary>
    public static class DbCommandExtensions
    {
        private static DbType[] BadDbTypes = {
            DbType.Time,
            DbType.SByte,
            DbType.UInt16,
            DbType.UInt32,
            DbType.UInt64
        };

        /// <summary>
        /// Adds a parameter to the call (for strings only)
        /// </summary>
        /// <param name="command">Command object</param>
        /// <param name="id">Name of the parameter</param>
        /// <param name="value">Value to add</param>
        /// <param name="direction">Direction that the parameter goes (in or out)</param>
        /// <returns>The DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand command, string id, string value = "",
            ParameterDirection direction = ParameterDirection.Input)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));
            int Length = string.IsNullOrEmpty(value) ? 1 : value.Length;
            if (direction == ParameterDirection.Output
                || direction == ParameterDirection.InputOutput
                || Length > 4000
                || Length < -1)
                Length = -1;
            DbParameter Parameter = command.GetOrCreateParameter(id);
            Parameter.Value = string.IsNullOrEmpty(value) ? DBNull.Value : (object)value;
            Parameter.IsNullable = string.IsNullOrEmpty(value);
            Parameter.DbType = DbType.String;
            Parameter.Direction = direction;
            Parameter.Size = Length;
            return command;
        }

        /// <summary>
        /// Adds a parameter to the call (for all types other than strings)
        /// </summary>
        /// <param name="id">Name of the parameter</param>
        /// <param name="value">Value to add</param>
        /// <param name="direction">Direction that the parameter goes (in or out)</param>
        /// <param name="command">Command object</param>
        /// <param name="type">SQL type of the parameter</param>
        /// <returns>The DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand command, string id, SqlDbType type,
            object value = null, ParameterDirection direction = ParameterDirection.Input)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));
            return command.AddParameter(id, type.To(DbType.Int32), value, direction);
        }

        /// <summary>
        /// Adds a parameter to the call (for all types other than strings)
        /// </summary>
        /// <typeparam name="DataType">Data type of the parameter</typeparam>
        /// <param name="id">Name of the parameter</param>
        /// <param name="direction">Direction that the parameter goes (in or out)</param>
        /// <param name="command">Command object</param>
        /// <param name="value">Value to add</param>
        /// <returns>The DbCommand object</returns>
        public static DbCommand AddParameter<DataType>(this DbCommand command, string id, DataType value = default(DataType),
            ParameterDirection direction = ParameterDirection.Input)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));
            return command.AddParameter(id,
                new GenericEqualityComparer<DataType>().Equals(value, default(DataType)) ? typeof(DataType).To(DbType.Int32) : value.GetType().To(DbType.Int32),
                value, direction);
        }

        /// <summary>
        /// Adds a parameter to the call (for all types other than strings)
        /// </summary>
        /// <param name="id">Name of the parameter</param>
        /// <param name="direction">Direction that the parameter goes (in or out)</param>
        /// <param name="command">Command object</param>
        /// <param name="value">Value to add</param>
        /// <param name="type">SQL type of the parameter</param>
        /// <returns>The DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand command, string id, DbType type, object value = null,
            ParameterDirection direction = ParameterDirection.Input)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));
            DbParameter Parameter = command.GetOrCreateParameter(id);
            Parameter.IsNullable = value == null || DBNull.Value == value;
            Parameter.Value = Parameter.IsNullable ? DBNull.Value : value;
            if (type != default(DbType) && !BadDbTypes.Contains(type))
                Parameter.DbType = type;
            Parameter.Direction = direction;
            return command;
        }

        /// <summary>
        /// Begins a transaction
        /// </summary>
        /// <param name="command">Command object</param>
        /// <returns>A transaction object</returns>
        public static DbTransaction BeginTransaction(this DbCommand command)
        {
            if (command == null || command.Connection == null)
                return null;
            command.Open();
            command.Transaction = command.Connection.BeginTransaction();
            return command.Transaction;
        }

        /// <summary>
        /// Clears the parameters
        /// </summary>
        /// <param name="command">Command object</param>
        /// <returns>The DBCommand object</returns>
        public static DbCommand ClearParameters(this DbCommand command)
        {
            if (command != null && command.Parameters != null)
                command.Parameters.Clear();
            return command;
        }

        /// <summary>
        /// Closes the connection
        /// </summary>
        /// <param name="command">Command object</param>
        /// <returns>The DBCommand object</returns>
        public static DbCommand Close(this DbCommand command)
        {
            if (command != null
                && command.Connection != null
                && command.Connection.State != ConnectionState.Closed)
                command.Connection.Close();
            return command;
        }

        /// <summary>
        /// Commits a transaction
        /// </summary>
        /// <param name="command">Command object</param>
        /// <returns>The DBCommand object</returns>
        public static DbCommand Commit(this DbCommand command)
        {
            if (command != null && command.Transaction != null)
                command.Transaction.Commit();
            return command;
        }

        /// <summary>
        /// Executes the stored procedure as a scalar query
        /// </summary>
        /// <param name="command">Command object</param>
        /// <param name="defaultValue">Default value if there is an issue</param>
        /// <returns>The object of the first row and first column</returns>
        public static DataType ExecuteScalar<DataType>(this DbCommand command, DataType defaultValue = default(DataType))
        {
            if (command == null)
                return defaultValue;
            command.Open();
            return command.ExecuteScalar().To(defaultValue);
        }

        /// <summary>
        /// Executes the stored procedure as a scalar query async
        /// </summary>
        /// <param name="command">Command object</param>
        /// <param name="defaultValue">Default value if there is an issue</param>
        /// <returns>The object of the first row and first column</returns>
        public static async Task<DataType> ExecuteScalarAsync<DataType>(this DbCommand command, DataType defaultValue = default(DataType))
        {
            if (command == null)
                return defaultValue;
            command.Open();
            var ReturnValue = await command.ExecuteScalarAsync();
            return ReturnValue.To(defaultValue);
        }

        /// <summary>
        /// Gets a parameter or creates it, if it is not found
        /// </summary>
        /// <param name="id">Name of the parameter</param>
        /// <param name="command">Command object</param>
        /// <returns>The DbParameter associated with the ID</returns>
        public static DbParameter GetOrCreateParameter(this DbCommand command, string id)
        {
            if (command.Parameters.Contains(id))
                return command.Parameters[id];
            else
            {
                DbParameter Parameter = command.CreateParameter();
                Parameter.ParameterName = id;
                command.Parameters.Add(Parameter);
                return Parameter;
            }
        }

        /// <summary>
        /// Returns an output parameter's value
        /// </summary>
        /// <typeparam name="DataType">Data type of the object</typeparam>
        /// <param name="id">Parameter name</param>
        /// <param name="command">Command object</param>
        /// <param name="defaultValue">Default value for the parameter</param>
        /// <returns>
        /// if the parameter exists (and isn't null or empty), it returns the parameter's value.
        /// Otherwise the default value is returned.
        /// </returns>
        public static DataType GetOutputParameter<DataType>(this DbCommand command, string id, DataType defaultValue = default(DataType))
        {
            return command != null && command.Parameters[id] != null ?
                command.Parameters[id].Value.To(defaultValue) :
                defaultValue;
        }

        /// <summary>
        /// Opens the connection
        /// </summary>
        /// <param name="command">Command object</param>
        /// <returns>The DBCommand object</returns>
        public static DbCommand Open(this DbCommand command)
        {
            if (command != null
                && command.Connection != null
                && command.Connection.State != ConnectionState.Open)
                command.Connection.Open();
            return command;
        }

        /// <summary>
        /// Rolls back a transaction
        /// </summary>
        /// <param name="command">Command object</param>
        /// <returns>The DBCommand object</returns>
        public static DbCommand Rollback(this DbCommand command)
        {
            if (command != null && command.Transaction != null)
                command.Transaction.Rollback();
            return command;
        }
    }
}