using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using SQLHelperDB.Tests.BaseClasses;
using SQLHelperDB.Tests.DataClasses;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SQLHelperDB.Tests
{
    public class SQLHelperAsyncTests : TestingDirectoryFixture
    {
        [Fact]
        public async Task ExecuteInsertAndGetBackIdAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            var Result1 = await Instance.AddQuery(CommandType.Text,
                @"INSERT INTO [TestDatabase].[dbo].[TestTable](StringValue1,StringValue2,BigIntValue,BitValue,DecimalValue,FloatValue,DateTimeValue,GUIDValue,TimeSpanValue) VALUES(@0,@1,@2,@3,@4,@5,@6,@7,@8)
                SELECT scope_identity() as [ID]",
                "A",
                "B",
                10,
                true,
                75.12m,
                4.53f,
                new DateTime(2010, 1, 1),
                Guid.NewGuid(),
                new TimeSpan(1, 0, 0))
                .ExecuteScalarAsync<int>();
            Assert.True(Result1 > 0);
            Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            var Result2 = await Instance.AddQuery(CommandType.Text,
                @"INSERT INTO [TestDatabase].[dbo].[TestTable](StringValue1,StringValue2,BigIntValue,BitValue,DecimalValue,FloatValue,DateTimeValue,GUIDValue,TimeSpanValue) VALUES(@0,@1,@2,@3,@4,@5,@6,@7,@8)
                SELECT scope_identity() as [ID]",
                "A",
                "B",
                10,
                true,
                75.12m,
                4.53f,
                new DateTime(2010, 1, 1),
                Guid.NewGuid(),
                new TimeSpan(1, 0, 0))
                .ExecuteScalarAsync<int>();
            Assert.True(Result1 < Result2);
        }

        [Fact]
        public async Task ExecuteInsertAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            var Result = await Instance.AddQuery(CommandType.Text,
                "INSERT INTO [TestDatabase].[dbo].[TestTable](StringValue1,StringValue2,BigIntValue,BitValue,DecimalValue,FloatValue,DateTimeValue,GUIDValue,TimeSpanValue) VALUES(@0,@1,@2,@3,@4,@5,@6,@7,@8)",
                "A",
                "B",
                10,
                true,
                75.12m,
                4.53f,
                new DateTime(2010, 1, 1),
                Guid.NewGuid(),
                new TimeSpan(1, 0, 0))
                .ExecuteScalarAsync<int>();
            Assert.Equal(1, Result);
            Instance.CreateBatch();
            for (var X = 0; X < 50; ++X)
            {
                Instance.AddQuery(CommandType.Text,
                    "INSERT INTO [TestDatabase].[dbo].[TestTable](StringValue1,StringValue2,BigIntValue,BitValue,DecimalValue,FloatValue,DateTimeValue,GUIDValue,TimeSpanValue) VALUES(@0,@1,@2,@3,@4,@5,@6,@7,@8)",
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
            Result = await Instance.ExecuteScalarAsync<int>();
            Assert.Equal(50, Result);
        }

        [Fact]
        public async Task ExecuteScalarAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            for (var X = 0; X < 50; ++X)
            {
                Instance.AddQuery(CommandType.Text,
                    "INSERT INTO [TestDatabase].[dbo].[TestTable](StringValue1,StringValue2,BigIntValue,BitValue,DecimalValue,FloatValue,DateTimeValue,GUIDValue,TimeSpanValue) VALUES(@0,@1,@2,@3,@4,@5,@6,@7,@8)",
                    "A",
                    "B",
                    10,
                    true,
                    75.12m,
                    4.53f,
                    new DateTime(2010, 1, 1),
                    TempGuid,
                    new TimeSpan(1, 0, 0));
            }
            var Result = await Instance.ExecuteScalarAsync<int>();
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            var ListResult = await Instance.AddQuery(CommandType.Text,
                "SELECT COUNT(*) FROM [TestDatabase].[dbo].[TestTable]")
                .ExecuteScalarAsync<int>();
            Assert.Equal(50, ListResult);
        }

        [Fact]
        public async Task ExecuteSelectAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            for (var X = 0; X < 50; ++X)
            {
                Instance.AddQuery(CommandType.Text,
                    "INSERT INTO [TestDatabase].[dbo].[TestTable](StringValue1,StringValue2,BigIntValue,BitValue,DecimalValue,FloatValue,DateTimeValue,GUIDValue,TimeSpanValue) VALUES(@0,@1,@2,@3,@4,@5,@6,@7,@8)",
                    "A",
                    "B",
                    10,
                    true,
                    75.12m,
                    4.53f,
                    new DateTime(2010, 1, 1),
                    TempGuid,
                    new TimeSpan(1, 0, 0));
            }
            var Result = await Instance.ExecuteScalarAsync<int>();
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            var ListResult = await Instance.AddQuery(CommandType.Text,
                "SELECT * FROM [TestDatabase].[dbo].[TestTable]")
                .ExecuteAsync();
            Assert.Single(ListResult);
            Assert.Equal(50, ListResult[0].Count);
            for (var X = 0; X < 50; ++X)
            {
                Assert.Equal("A", ListResult[0][X].StringValue1);
                Assert.Equal("B", ListResult[0][X].StringValue2);
                Assert.Equal(10, ListResult[0][X].BigIntValue);
                Assert.Equal(true, ListResult[0][X].BitValue);
                Assert.Equal(75.12m, ListResult[0][X].DecimalValue);
                Assert.Equal(4.53f, ListResult[0][X].FloatValue);
                Assert.Equal(new DateTime(2010, 1, 1), ListResult[0][X].DateTimeValue);
                Assert.Equal(TempGuid, ListResult[0][X].GUIDValue);
                Assert.Equal(new TimeSpan(1, 0, 0), ListResult[0][X].TimeSpanValue);
            }
        }

        [Fact]
        public async Task ExecuteSelectHundredsOfParamtersAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            var Builder = new StringBuilder();
            var Splitter = "";
            for (var X = 0; X < 200; ++X)
            {
                Builder.AppendFormat("{1}@{0}", X, Splitter);
                Splitter = ",";
            }
            var ListResult = await Instance.AddQuery(CommandType.Text,
                string.Format("SELECT {0} FROM [TestDatabase].[dbo].[TestTable]", Builder.ToString()),
                new object[200])
                .ExecuteAsync();
            Assert.Single(ListResult);
        }

        [Fact]
        public async Task ExecuteSelectToObjectTypeAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            for (var X = 0; X < 50; ++X)
            {
                Instance.AddQuery(CommandType.Text,
                    "INSERT INTO [TestDatabase].[dbo].[TestTable](StringValue1,StringValue2,BigIntValue,BitValue,DecimalValue,FloatValue,DateTimeValue,GUIDValue,TimeSpanValue) VALUES(@0,@1,@2,@3,@4,@5,@6,@7,@8)",
                    "A",
                    "B",
                    10,
                    true,
                    75.12m,
                    4.53f,
                    new DateTime(2010, 1, 1),
                    TempGuid,
                    new TimeSpan(1, 0, 0));
            }
            var Result = await Instance.ExecuteScalarAsync<int>();
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            var ListResult = await Instance.AddQuery(CommandType.Text,
                "SELECT * FROM [TestDatabase].[dbo].[TestTable]")
                .ExecuteAsync();
            Assert.Single(ListResult);
            Assert.Equal(50, ListResult[0].Count);
            var ConvertedResult = ListResult[0].ConvertAll(x => (TestTableClass)x);
            for (var X = 0; X < 50; ++X)
            {
                Assert.Equal("A", ConvertedResult[X].StringValue1);
                Assert.Equal("B", ConvertedResult[X].StringValue2);
                Assert.Equal(10, ConvertedResult[X].BigIntValue);
                Assert.True(ConvertedResult[X].BitValue);
                Assert.Equal(75.12m, ConvertedResult[X].DecimalValue);
                Assert.Equal(4.53f, ConvertedResult[X].FloatValue);
                Assert.Equal(new DateTime(2010, 1, 1), ConvertedResult[X].DateTimeValue);
                Assert.Equal(TempGuid, ConvertedResult[X].GUIDValue);
                Assert.Equal(new TimeSpan(1, 0, 0), ConvertedResult[X].TimeSpanValue);
            }
        }

        [Fact]
        public async Task ExecuteUpdateAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            for (var X = 0; X < 50; ++X)
            {
                Instance.AddQuery(CommandType.Text,
                    "INSERT INTO [TestDatabase].[dbo].[TestTable](StringValue1,StringValue2,BigIntValue,BitValue,DecimalValue,FloatValue,DateTimeValue,GUIDValue,TimeSpanValue) VALUES(@0,@1,@2,@3,@4,@5,@6,@7,@8)",
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
            var Result = await Instance.ExecuteScalarAsync<int>();
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            Result = await Instance.AddQuery(CommandType.Text,
                "UPDATE [TestDatabase].[dbo].[TestTable] SET StringValue1=@0",
                "C")
                .ExecuteScalarAsync<int>();
            Assert.Equal(50, Result);
        }

        [Fact]
        public async Task InsertWithAtSymbolAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            for (var X = 0; X < 50; ++X)
            {
                Instance.AddQuery(CommandType.Text,
                    "INSERT INTO [TestDatabase].[dbo].[TestTable](StringValue1,StringValue2,BigIntValue,BitValue,DecimalValue,FloatValue,DateTimeValue,GUIDValue,TimeSpanValue) VALUES('email@address.com',@0,@1,@2,@3,@4,@5,@6,@7)",
                    "email@address.com",
                    10,
                    true,
                    75.12m,
                    4.53f,
                    new DateTime(2010, 1, 1),
                    TempGuid,
                    new TimeSpan(1, 0, 0));
            }
            var Result = await Instance.ExecuteScalarAsync<int>();
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            var ListResult = await Instance.AddQuery(CommandType.Text,
                "SELECT * FROM [TestDatabase].[dbo].[TestTable]")
                .ExecuteAsync();
            Assert.Single(ListResult);
            Assert.Equal(50, ListResult[0].Count);
            for (var X = 0; X < 50; ++X)
            {
                Assert.Equal("email@address.com", ListResult[0][X].StringValue1);
                Assert.Equal("email@address.com", ListResult[0][X].StringValue2);
                Assert.Equal(10, ListResult[0][X].BigIntValue);
                Assert.Equal(true, ListResult[0][X].BitValue);
                Assert.Equal(75.12m, ListResult[0][X].DecimalValue);
                Assert.Equal(4.53f, ListResult[0][X].FloatValue);
                Assert.Equal(new DateTime(2010, 1, 1), ListResult[0][X].DateTimeValue);
                Assert.Equal(TempGuid, ListResult[0][X].GUIDValue);
                Assert.Equal(new TimeSpan(1, 0, 0), ListResult[0][X].TimeSpanValue);
            }
        }
    }
}