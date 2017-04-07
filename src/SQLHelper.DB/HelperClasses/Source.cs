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
using SQLHelper.HelperClasses.Interfaces;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace SQLHelper.HelperClasses
{
    /// <summary>
    /// Data source class
    /// </summary>
    /// <seealso cref="ISource"/>
    public class Source : ISource
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="name">The name.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="parameterPrefix">The parameter prefix.</param>
        public Source(IConfiguration configuration, DbProviderFactory factory, string connection, string name, string sourceType = "System.Data.SqlClient",
                        string parameterPrefix = "@")
        {
            Name = string.IsNullOrEmpty(name) ? "Default" : name;
            SourceType = string.IsNullOrEmpty(sourceType) ? "System.Data.SqlClient" : sourceType;
            Factory = factory;
            var TempConfig = configuration.GetConnectionString(Name);
            if (string.IsNullOrEmpty(connection) && TempConfig != null)
            {
                Connection = TempConfig;
            }
            else
            {
                Connection = string.IsNullOrEmpty(connection) ? name : connection;
            }
            if (string.IsNullOrEmpty(parameterPrefix))
            {
                if (sourceType.Contains("MySql"))
                    ParameterPrefix = "?";
                else if (sourceType.Contains("Oracle"))
                    ParameterPrefix = ":";
                else
                {
                    DatabaseName = Regex.Match(Connection, @"Initial Catalog=([^;]*)").Groups[1].Value;
                    ParameterPrefix = "@";
                }
            }
            else
            {
                ParameterPrefix = parameterPrefix;
                if (sourceType.Contains("SqlClient"))
                {
                    DatabaseName = Regex.Match(Connection, @"Initial Catalog=([^;]*)").Groups[1].Value;
                }
            }
        }

        /// <summary>
        /// Connection string
        /// </summary>
        public string Connection { get; protected set; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        public string DatabaseName { get; protected set; }

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
        /// Source type, based on ADO.Net provider name or identifier used by CUL
        /// </summary>
        /// <value>The type of the source.</value>
        public string SourceType { get; protected set; }
    }
}