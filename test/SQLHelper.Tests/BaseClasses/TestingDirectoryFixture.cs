using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SQLHelper.Tests.Utils;
using SQLHelperDB.ExtensionMethods;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SQLHelperDB.Tests.BaseClasses
{
    [Collection("DirectoryCollection")]
    public class TestingDirectoryFixture : IAsyncLifetime, IDisposable
    {
        public TestingDirectoryFixture()
        {
            Configuration = TestConfigurationFactory.Create();
        }

        public async Task InitializeAsync()
        {
            await TestDatabaseManager.ResetKnownDatabasesAsync().ConfigureAwait(false);

            await using var TempConnection = Microsoft.Data.SqlClient.SqlClientFactory.Instance.CreateConnection();
            TempConnection.ConnectionString = TestConnectionStrings.Default;
            await TempConnection.OpenAsync().ConfigureAwait(false);

            await using var TempCommand = TempConnection.CreateCommand();
            await ExecuteSetupCommandAsync(TempCommand,
                "Create Table TestTable(ID INT PRIMARY KEY IDENTITY,StringValue1 NVARCHAR(100),StringValue2 NVARCHAR(MAX),BigIntValue BIGINT,BitValue BIT,DecimalValue DECIMAL(12,6),FloatValue FLOAT,DateTimeValue DATETIME,GUIDValue UNIQUEIDENTIFIER,TimeSpanValue TIME(7))")
                .ConfigureAwait(false);
            await ExecuteSetupCommandAsync(TempCommand,
                "Create Table TestTableNotNull(ID INT PRIMARY KEY IDENTITY,UShortValue_ SMALLINT NOT NULL)")
                .ConfigureAwait(false);
            await ExecuteSetupCommandAsync(TempCommand, @"Create PROCEDURE TestSP
@Value nvarchar(100)
AS
BEGIN
SELECT @Value as [Value]
END")
                .ConfigureAwait(false);
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
            GC.SuppressFinalize(this);
        }

        public Task DisposeAsync() => Task.CompletedTask;

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

        private static async Task ExecuteSetupCommandAsync(System.Data.Common.DbCommand command, string commandText)
        {
            command.CommandText = commandText;
            try
            {
                _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            catch (Microsoft.Data.SqlClient.SqlException Ex) when (Ex.Number == 2714)
            {
                // Ignore object-already-exists during setup when rerunning in partially initialized environments.
            }
        }
    }
}