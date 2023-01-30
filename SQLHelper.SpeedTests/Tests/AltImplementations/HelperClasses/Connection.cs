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
using SQLHelperDBTests.HelperClasses.Interfaces;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace SQLHelperDBTests.HelperClasses
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
            Retries = retries;
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Name = string.IsNullOrEmpty(name) ? "Default" : name;
            Factory = factory ?? Microsoft.Data.SqlClient.SqlClientFactory.Instance;
            SourceType = Factory.GetType().FullName ?? string.Empty;
            var TempConfig = configuration.GetConnectionString(Name);
            ConnectionString = !string.IsNullOrEmpty(connection) ? connection : (TempConfig ?? Name);
            if (string.IsNullOrEmpty(parameterPrefix))
            {
                if (Factory is Microsoft.Data.SqlClient.SqlClientFactory || Factory is SqlClientFactory)
                {
                    DatabaseName = DatabaseNameRegex.Match(ConnectionString).Groups[1].Value;
                    ParameterPrefix = "@";
                }
                else if (SourceType.Contains("Oracle", StringComparison.OrdinalIgnoreCase))
                {
                    ParameterPrefix = ":";
                }
            }
            else
            {
                ParameterPrefix = parameterPrefix;
                if (Factory is Microsoft.Data.SqlClient.SqlClientFactory || Factory is SqlClientFactory)
                {
                    DatabaseName = DatabaseNameRegex.Match(ConnectionString).Groups[1].Value;
                }
            }
            if (ConnectionTimeoutRegex.IsMatch(ConnectionString))
            {
                var TimeoutValue = ConnectionTimeoutRegex.Match(ConnectionString).Groups[2].Value;
                CommandTimeout = int.TryParse(TimeoutValue, out var TempCommandTimeout) ? TempCommandTimeout : 30;
            }
            CommandTimeout = CommandTimeout <= 0 ? 30 : CommandTimeout;
        }

        /// <summary>
        /// Gets the command timeout.
        /// </summary>
        /// <value>The command timeout.</value>
        public int CommandTimeout { get; }

        /// <summary>
        /// Gets the configuration information.
        /// </summary>
        /// <value>Gets the configuration information.</value>
        public IConfiguration Configuration { get; }

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
        /// Source type, based on ADO.Net provider name or identifier used by CUL
        /// </summary>
        /// <value>The type of the source.</value>
        public string SourceType { get; protected set; }

        /// <summary>
        /// Gets the connection timeout regex.
        /// </summary>
        /// <value>The connection timeout regex.</value>
        private static Regex ConnectionTimeoutRegex { get; } = new Regex("Connect(ion)? Timeout=([^;]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        private static Regex DatabaseNameRegex { get; } = new Regex("Initial Catalog=([^;]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}