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

using Microsoft.Extensions.Configuration;
using SQLHelperDB.HelperClasses.Interfaces;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace SQLHelperDB.HelperClasses
{
    /// <summary>
    /// Data source class
    /// </summary>
    /// <seealso cref="IConnection"/>
    public class Connection : IConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="name">The name.</param>
        public Connection(IConfiguration configuration, DbProviderFactory factory, string name)
            : this(configuration, factory, string.Empty, name)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="name">The name.</param>
        /// <param name="parameterPrefix">The parameter prefix.</param>
        /// <param name="retries">The retries.</param>
        /// <exception cref="System.ArgumentNullException">configuration</exception>
        public Connection(IConfiguration configuration, DbProviderFactory factory, string connection, string name, string parameterPrefix = "@", int retries = 0)
        {
            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration));
            Retries = retries;
            Name = string.IsNullOrEmpty(name) ? "Default" : name;
            Factory = factory ?? SqlClientFactory.Instance;
            ConnectionString = !string.IsNullOrEmpty(connection) ? connection : (configuration.GetConnectionString(Name) ?? Name);
            var DatabaseRegexResult = DatabaseNameRegex.Match(ConnectionString);
            if (DatabaseRegexResult.Success)
                DatabaseName = DatabaseRegexResult.Groups["name"].Value;
            ParameterPrefix = !string.IsNullOrEmpty(parameterPrefix) ? parameterPrefix : GetParameterPrefix(Factory);
            CommandTimeout = GetCommandTimeout(ConnectionString);
        }

        /// <summary>
        /// Gets the command timeout.
        /// </summary>
        /// <value>The command timeout.</value>
        public int CommandTimeout { get; }

        /// <summary>
        /// Connection string
        /// </summary>
        public string ConnectionString { get; protected set; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        public string? DatabaseName { get; protected set; }

        /// <summary>
        /// Gets the factory that the system uses to actually do the connection.
        /// </summary>
        /// <value>The factory that the system needs to actually do the connection.</value>
        public DbProviderFactory Factory { get; protected set; }

        /// <summary>
        /// Name of the source
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; protected set; }

        /// <summary>
        /// Parameter prefix that the source uses
        /// </summary>
        /// <value>The parameter prefix.</value>
        public string ParameterPrefix { get; protected set; }

        /// <summary>
        /// Gets the number of retries if unable to connect.
        /// </summary>
        /// <value>The number of retries if unable to connect.</value>
        public int Retries { get; protected set; }

        /// <summary>
        /// Gets the connection timeout regex.
        /// </summary>
        /// <value>The connection timeout regex.</value>
        private static Regex ConnectionTimeoutRegex { get; } = new Regex("Connect(ion)? Timeout=([^;]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        private static Regex DatabaseNameRegex { get; } = new Regex("(Initial Catalog|Database)=(?<name>[^;]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Gets the command timeout.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The command timeout.</returns>
        private static int GetCommandTimeout(string connectionString)
        {
            var TimeoutMatch = ConnectionTimeoutRegex.Match(connectionString);
            return TimeoutMatch.Success
                && int.TryParse(TimeoutMatch.Groups[2].Value, out var TempCommandTimeout)
                && TempCommandTimeout > 0
                ? TempCommandTimeout
                : 30;
        }

        /// <summary>
        /// Gets the parameter prefix.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns>The parameter prefix.</returns>
        private static string GetParameterPrefix(DbProviderFactory factory)
        {
            var SourceType = factory.GetType().FullName ?? string.Empty;
            return SourceType.Contains("Oracle", StringComparison.OrdinalIgnoreCase) ? ":" : "@";
        }
    }
}