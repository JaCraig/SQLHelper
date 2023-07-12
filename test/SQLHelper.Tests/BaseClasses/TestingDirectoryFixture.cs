using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SQLHelperDB.ExtensionMethods;
using System;
using Xunit;

namespace SQLHelperDB.Tests.BaseClasses
{
    [Collection("DirectoryCollection")]
    public class TestingDirectoryFixture : IDisposable
    {
        public TestingDirectoryFixture()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            using (var TempConnection = Microsoft.Data.SqlClient.SqlClientFactory.Instance.CreateConnection())
            {
                TempConnection.ConnectionString = Configuration.GetConnectionString("Master");
                using var TempCommand = TempConnection.CreateCommand();
                try
                {
                    TempCommand.CommandText = "Create Database TestDatabase";
                    TempCommand.Open(3);
                    TempCommand.ExecuteNonQuery();
                }
                catch { }
                finally { TempCommand.Close(); }
            }
            using (var TempConnection = Microsoft.Data.SqlClient.SqlClientFactory.Instance.CreateConnection())
            {
                TempConnection.ConnectionString = Configuration.GetConnectionString("Default");
                using var TempCommand = TempConnection.CreateCommand();
                try
                {
                    TempCommand.CommandText = "Create Table TestTable(ID INT PRIMARY KEY IDENTITY,StringValue1 NVARCHAR(100),StringValue2 NVARCHAR(MAX),BigIntValue BIGINT,BitValue BIT,DecimalValue DECIMAL(12,6),FloatValue FLOAT,DateTimeValue DATETIME,GUIDValue UNIQUEIDENTIFIER,TimeSpanValue TIME(7))";
                    TempCommand.Open(3);
                    TempCommand.ExecuteNonQuery();
                    TempCommand.CommandText = "Create Table TestTableNotNull(ID INT PRIMARY KEY IDENTITY,UShortValue_ SMALLINT NOT NULL)";
                    TempCommand.ExecuteNonQuery();
                    TempCommand.CommandText = @"Create PROCEDURE TestSP
@Value nvarchar(100)
AS
BEGIN
SELECT @Value as [Value]
END";
                    TempCommand.ExecuteNonQuery();
                }
                catch { }
                finally { TempCommand.Close(); }
            }
        }

        /// <summary>
        /// The service provider lock
        /// </summary>
        private static readonly object ServiceProviderLock = new object();

        /// <summary>
        /// The service provider
        /// </summary>
        private static IServiceProvider ServiceProvider;

        protected IConfiguration Configuration { get; }

        public void Dispose()
        {
            using var TempConnection = Microsoft.Data.SqlClient.SqlClientFactory.Instance.CreateConnection();
            TempConnection.ConnectionString = Configuration.GetConnectionString("Master");
            using var TempCommand = TempConnection.CreateCommand();
            try
            {
                TempCommand.CommandText = "ALTER DATABASE TestDatabase SET OFFLINE WITH ROLLBACK IMMEDIATE\r\nALTER DATABASE TestDatabase SET ONLINE\r\nDROP DATABASE TestDatabase";
                TempCommand.Open(3);
                TempCommand.ExecuteNonQuery();
            }
            finally { TempCommand.Close(); }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        /// <returns></returns>
        protected static IServiceProvider GetServiceProvider()
        {
            if (ServiceProvider is not null)
                return ServiceProvider;
            lock (ServiceProviderLock)
            {
                if (ServiceProvider is not null)
                    return ServiceProvider;
                ServiceProvider = new ServiceCollection().AddCanisterModules()?.BuildServiceProvider();
            }
            return ServiceProvider;
        }
    }
}