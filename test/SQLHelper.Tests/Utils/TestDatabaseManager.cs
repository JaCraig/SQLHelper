using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace SQLHelper.Tests.Utils
{
    internal static class TestDatabaseManager
    {
        private static readonly string[] KnownDatabases =
        [
            "TestDatabase"
        ];

        public static async Task ResetKnownDatabasesAsync()
        {
            await using var connection = new SqlConnection(TestConnectionStrings.Master);
            await connection.OpenAsync().ConfigureAwait(false);

            await using var command = connection.CreateCommand();
            command.CommandTimeout = 120;
            foreach (var databaseName in KnownDatabases)
            {
                command.CommandText = BuildResetDatabaseScript(databaseName);
                _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        private static string BuildResetDatabaseScript(string databaseName)
        {
            var variableSuffix = databaseName.Replace("-", string.Empty, StringComparison.Ordinal);
            return TestConnectionStrings.NormalizeLineEndings($@"IF DB_ID(N'{databaseName}') IS NULL
BEGIN
    CREATE DATABASE [{databaseName}]
END

DECLARE @dropConstraints_{variableSuffix} NVARCHAR(MAX) = N'';
SELECT @dropConstraints_{variableSuffix} = @dropConstraints_{variableSuffix} +
    N'ALTER TABLE [{databaseName}].' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name) +
    N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';'
FROM [{databaseName}].sys.foreign_keys fk
INNER JOIN [{databaseName}].sys.tables t ON fk.parent_object_id = t.object_id;

IF LEN(@dropConstraints_{variableSuffix}) > 0
BEGIN
    EXEC(@dropConstraints_{variableSuffix})
END

DECLARE @dropTables_{variableSuffix} NVARCHAR(MAX) = N'';
SELECT @dropTables_{variableSuffix} = @dropTables_{variableSuffix} +
    N'DROP TABLE [{databaseName}].' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name) + N';'
FROM [{databaseName}].sys.tables t;

IF LEN(@dropTables_{variableSuffix}) > 0
BEGIN
    EXEC(@dropTables_{variableSuffix})
END");
        }
    }
}