﻿using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using SQLHelperDB.ExtensionMethods;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SQLHelperDB.SpeedTests.Tests
{
    /// <summary>
    /// Mass insert test.
    /// </summary>
    [MemoryDiagnoser]
    public class MassInsert
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        [Params(1, 10, 100)]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the helper.
        /// </summary>
        /// <value>The helper.</value>
        private SQLHelper? Helper { get; set; }

        /// <summary>
        /// Gets or sets the helper2.
        /// </summary>
        /// <value>The helper2.</value>
        private SQLHelperDBTests.SQLHelper? Helper2 { get; set; }

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        [GlobalCleanup]
        public void Cleanup()
        {
            using System.Data.Common.DbConnection TempConnection = Microsoft.Data.SqlClient.SqlClientFactory.Instance.CreateConnection();
            TempConnection.ConnectionString = "Data Source=localhost;Initial Catalog=master;Integrated Security=SSPI;Pooling=false;TrustServerCertificate=True";
            using System.Data.Common.DbCommand TempCommand = TempConnection.CreateCommand();
            try
            {
                TempCommand.CommandText = "ALTER DATABASE SpeedTestDatabase SET OFFLINE WITH ROLLBACK IMMEDIATE\r\nALTER DATABASE SpeedTestDatabase SET ONLINE\r\nDROP DATABASE SpeedTestDatabase";
                _ = TempCommand.Open(3);
                _ = TempCommand.ExecuteNonQuery();
            }
            finally { _ = TempCommand.Close(); }
        }

        /// <summary>
        /// Runs the changes.
        /// </summary>
        [Benchmark]
        public async Task RunChanges()
        {
            _ = Helper2.CreateBatch();
            for (var X = 0; X < Count; ++X)
            {
                _ = Helper2.AddQuery(CommandType.Text,
                    "INSERT INTO [SpeedTestDatabase].[dbo].[TestTable](StringValue1,StringValue2,BigIntValue,BitValue,DecimalValue,FloatValue,DateTimeValue,GUIDValue,TimeSpanValue) VALUES(@0,@1,@2,@3,@4,@5,@6,@7,@8)",
                    "A",
                    "B",
                    10,
                    true,
                    75.12m,
                    4.53f,
                    new DateTime(2010, 1, 1),
                    Guid.NewGuid(),
                    new TimeSpan(1, 0, 0));
            }
            _ = await Helper2.ExecuteScalarAsync<int>().ConfigureAwait(false);
        }

        /// <summary>
        /// Runs the current.
        /// </summary>
        [Benchmark(Baseline = true)]
        public async Task RunCurrent()
        {
            _ = Helper.CreateBatch();
            for (var X = 0; X < Count; ++X)
            {
                _ = Helper.AddQuery(CommandType.Text,
                    "INSERT INTO [SpeedTestDatabase].[dbo].[TestTable](StringValue1,StringValue2,BigIntValue,BitValue,DecimalValue,FloatValue,DateTimeValue,GUIDValue,TimeSpanValue) VALUES(@0,@1,@2,@3,@4,@5,@6,@7,@8)",
                    "A",
                    "B",
                    10,
                    true,
                    75.12m,
                    4.53f,
                    new DateTime(2010, 1, 1),
                    Guid.NewGuid(),
                    new TimeSpan(1, 0, 0));
            }
            _ = await Helper.ExecuteScalarAsync<int>().ConfigureAwait(false);
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            ServiceProvider Services = new ServiceCollection().AddCanisterModules(x => x.AddAssembly(typeof(Program).Assembly)
                .RegisterSQLHelper()).BuildServiceProvider();
            Helper = new SQLHelper(Services.GetService<ObjectPool<StringBuilder>>(), Services.GetService<IConfiguration>(), null);
            Helper2 = new SQLHelperDBTests.SQLHelper(Services.GetService<IConfiguration>(), Microsoft.Data.SqlClient.SqlClientFactory.Instance);

            using (System.Data.Common.DbConnection TempConnection = Microsoft.Data.SqlClient.SqlClientFactory.Instance.CreateConnection())
            {
                TempConnection.ConnectionString = "Data Source=localhost;Initial Catalog=master;Integrated Security=SSPI;Pooling=false;TrustServerCertificate=True";
                using System.Data.Common.DbCommand TempCommand = TempConnection.CreateCommand();
                try
                {
                    TempCommand.CommandText = "Create Database SpeedTestDatabase";
                    _ = TempCommand.Open(3);
                    _ = TempCommand.ExecuteNonQuery();
                }
                catch { }
                finally { _ = TempCommand.Close(); }
            }

            using (System.Data.Common.DbConnection TempConnection = Microsoft.Data.SqlClient.SqlClientFactory.Instance.CreateConnection())
            {
                TempConnection.ConnectionString = "Data Source=localhost;Initial Catalog=SpeedTestDatabase;Integrated Security=SSPI;Pooling=false;TrustServerCertificate=True";
                using System.Data.Common.DbCommand TempCommand = TempConnection.CreateCommand();
                try
                {
                    TempCommand.CommandText = "Create Table TestTable(ID INT PRIMARY KEY IDENTITY,StringValue1 NVARCHAR(100),StringValue2 NVARCHAR(MAX),BigIntValue BIGINT,BitValue BIT,DecimalValue DECIMAL(12,6),FloatValue FLOAT,DateTimeValue DATETIME,GUIDValue UNIQUEIDENTIFIER,TimeSpanValue TIME(7))";
                    _ = TempCommand.Open(3);
                    _ = TempCommand.ExecuteNonQuery();
                    TempCommand.CommandText = "Create Table TestTableNotNull(ID INT PRIMARY KEY IDENTITY,UShortValue_ SMALLINT NOT NULL)";
                    _ = TempCommand.ExecuteNonQuery();
                }
                catch { }
                finally { _ = TempCommand.Close(); }
            }
        }
    }
}