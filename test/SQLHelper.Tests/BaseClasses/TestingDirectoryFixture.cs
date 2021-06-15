using Microsoft.Extensions.DependencyInjection;
using SQLHelperDB.ExtensionMethods;
using System;
using System.Data.SqlClient;
using Xunit;

namespace SQLHelperDB.Tests.BaseClasses
{
    [Collection("DirectoryCollection")]
    public class TestingDirectoryFixture : IDisposable
    {
        public TestingDirectoryFixture()
        {
            if (Canister.Builder.Bootstrapper is null)
            {
                new ServiceCollection().AddCanisterModules();
            }

            using (var TempConnection = SqlClientFactory.Instance.CreateConnection())
            {
                TempConnection.ConnectionString = "Data Source=localhost;Initial Catalog=master;Integrated Security=SSPI;Pooling=false";
                using (var TempCommand = TempConnection.CreateCommand())
                {
                    try
                    {
                        TempCommand.CommandText = "Create Database TestDatabase";
                        TempCommand.Open(3);
                        TempCommand.ExecuteNonQuery();
                    }
                    catch { }
                    finally { TempCommand.Close(); }
                }
            }
            using (var TempConnection = SqlClientFactory.Instance.CreateConnection())
            {
                TempConnection.ConnectionString = "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false";
                using (var TempCommand = TempConnection.CreateCommand())
                {
                    try
                    {
                        TempCommand.CommandText = "Create Table TestTable(ID INT PRIMARY KEY IDENTITY,StringValue1 NVARCHAR(100),StringValue2 NVARCHAR(MAX),BigIntValue BIGINT,BitValue BIT,DecimalValue DECIMAL(12,6),FloatValue FLOAT,DateTimeValue DATETIME,GUIDValue UNIQUEIDENTIFIER,TimeSpanValue TIME(7))";
                        TempCommand.Open(3);
                        TempCommand.ExecuteNonQuery();
                        TempCommand.CommandText = "Create Table TestTableNotNull(ID INT PRIMARY KEY IDENTITY,UShortValue_ SMALLINT NOT NULL)";
                        TempCommand.ExecuteNonQuery();
                    }
                    catch { }
                    finally { TempCommand.Close(); }
                }
            }
        }

        public void Dispose()
        {
            using (var TempConnection = SqlClientFactory.Instance.CreateConnection())
            {
                TempConnection.ConnectionString = "Data Source=localhost;Initial Catalog=master;Integrated Security=SSPI;Pooling=false";
                using (var TempCommand = TempConnection.CreateCommand())
                {
                    try
                    {
                        TempCommand.CommandText = "ALTER DATABASE TestDatabase SET OFFLINE WITH ROLLBACK IMMEDIATE\r\nALTER DATABASE TestDatabase SET ONLINE\r\nDROP DATABASE TestDatabase";
                        TempCommand.Open(3);
                        TempCommand.ExecuteNonQuery();
                    }
                    finally { TempCommand.Close(); }
                }
            }
        }
    }
}