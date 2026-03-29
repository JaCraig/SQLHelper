using System;

namespace SQLHelper.Tests.Utils
{
    internal static class TestConnectionStrings
    {
        private const string SqlUser = "sa";

        public static string Default => Build("TestDatabase");

        public static string Master => Build("master");

        public static string NormalizeLineEndings(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Replace("\r", "\n", StringComparison.Ordinal)
                .Replace("\n", Environment.NewLine, StringComparison.Ordinal);
        }

        private static string Build(string databaseName)
        {
            var sqlPassword = Environment.GetEnvironmentVariable("SQLHELPER_SQL_PASSWORD");
            if (string.IsNullOrWhiteSpace(sqlPassword))
            {
                return $"Data Source=localhost;Initial Catalog={databaseName};Integrated Security=SSPI;Pooling=false;TrustServerCertificate=True";
            }

            var sqlServer = Environment.GetEnvironmentVariable("SQLHELPER_SQL_SERVER");
            if (string.IsNullOrWhiteSpace(sqlServer))
            {
                sqlServer = "127.0.0.1,1433";
            }

            return $"Server={sqlServer};Database={databaseName};User ID={SqlUser};Password={sqlPassword};TrustServerCertificate=True";
        }
    }
}