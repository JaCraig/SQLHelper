using BigBook;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.ObjectPool;
using SQLHelperDB.Tests.BaseClasses;
using SQLHelperDB.Tests.DataClasses;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
                .AddInMemoryCollection()
                .Build();
            var Instance = new SQLHelper(Canister.Builder.Bootstrapper.Resolve<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false");
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
                .ExecuteScalarAsync<int>().ConfigureAwait(false);
            Assert.True(Result1 > 0);
            Instance = new SQLHelper(Canister.Builder.Bootstrapper.Resolve<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false");
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
                .ExecuteScalarAsync<int>().ConfigureAwait(false);
            Assert.True(Result1 < Result2);
        }

        [Fact]
        public async Task ExecuteInsertAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new SQLHelper(Canister.Builder.Bootstrapper.Resolve<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false");
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
                .ExecuteScalarAsync<int>().ConfigureAwait(false);
            Assert.Equal(1, Result);
            Instance.CreateBatch();
            for (var x = 0; x < 50; ++x)
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
            Result = await Instance.ExecuteScalarAsync<int>().ConfigureAwait(false);
            Assert.Equal(50, Result);
        }

        [Fact]
        public async Task ExecuteScalarAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(Canister.Builder.Bootstrapper.Resolve<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false");
            for (var x = 0; x < 50; ++x)
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
            var Result = await Instance.ExecuteScalarAsync<int>().ConfigureAwait(false);
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            var ListResult = await Instance.AddQuery(CommandType.Text,
                "SELECT COUNT(*) FROM [TestDatabase].[dbo].[TestTable]")
                .ExecuteScalarAsync<int>().ConfigureAwait(false);
            Assert.Equal(50, ListResult);
        }

        [Fact]
        public async Task ExecuteSelectAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(Canister.Builder.Bootstrapper.Resolve<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false");
            for (var x = 0; x < 50; ++x)
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
            var Result = await Instance.ExecuteScalarAsync<int>().ConfigureAwait(false);
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            var ListResult = await Instance.AddQuery(CommandType.Text,
                "SELECT * FROM [TestDatabase].[dbo].[TestTable]")
                .ExecuteAsync().ConfigureAwait(false);
            Assert.Single(ListResult);
            Assert.Equal(50, ListResult[0].Count);
            for (var x = 0; x < 50; ++x)
            {
                Assert.Equal("A", ListResult[0][x].StringValue1);
                Assert.Equal("B", ListResult[0][x].StringValue2);
                Assert.Equal(10, ListResult[0][x].BigIntValue);
                Assert.Equal(true, ListResult[0][x].BitValue);
                Assert.Equal(75.12m, ListResult[0][x].DecimalValue);
                Assert.Equal(4.53f, ListResult[0][x].FloatValue);
                Assert.Equal(new DateTime(2010, 1, 1), ListResult[0][x].DateTimeValue);
                Assert.Equal(TempGuid, ListResult[0][x].GUIDValue);
                Assert.Equal(new TimeSpan(1, 0, 0), ListResult[0][x].TimeSpanValue);
            }
        }

        [Fact]
        public async Task ExecuteSelectHundredsOfParamtersAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new SQLHelper(Canister.Builder.Bootstrapper.Resolve<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false");
            var Builder = new StringBuilder();
            var Splitter = "";
            for (var x = 0; x < 200; ++x)
            {
                Builder.AppendFormat("{1}@{0}", x, Splitter);
                Splitter = ",";
            }
            var ListResult = await Instance.AddQuery(CommandType.Text,
                string.Format("SELECT {0} FROM [TestDatabase].[dbo].[TestTable]", Builder.ToString()),
                new object[200])
                .ExecuteAsync().ConfigureAwait(false);
            Assert.Single(ListResult);
        }

        [Fact]
        public async Task ExecuteSelectToObjectTypeAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(Canister.Builder.Bootstrapper.Resolve<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false");
            for (var x = 0; x < 50; ++x)
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
            var Result = await Instance.ExecuteScalarAsync<int>().ConfigureAwait(false);
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            var ListResult = await Instance.AddQuery(CommandType.Text,
                "SELECT * FROM [TestDatabase].[dbo].[TestTable]")
                .ExecuteAsync().ConfigureAwait(false);
            Assert.Single(ListResult);
            Assert.Equal(50, ListResult[0].Count);
            var ConvertedResult = ListResult[0].Select(x => (TestTableClass)x).ToList();
            for (var x = 0; x < 50; ++x)
            {
                Assert.Equal("A", ConvertedResult[x].StringValue1);
                Assert.Equal("B", ConvertedResult[x].StringValue2);
                Assert.Equal(10, ConvertedResult[x].BigIntValue);
                Assert.True(ConvertedResult[x].BitValue);
                Assert.Equal(75.12m, ConvertedResult[x].DecimalValue);
                Assert.Equal(4.53f, ConvertedResult[x].FloatValue);
                Assert.Equal(new DateTime(2010, 1, 1), ConvertedResult[x].DateTimeValue);
                Assert.Equal(TempGuid, ConvertedResult[x].GUIDValue);
                Assert.Equal(new TimeSpan(1, 0, 0), ConvertedResult[x].TimeSpanValue);
            }
        }

        [Fact]
        public async Task ExecuteUpdateAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new SQLHelper(Canister.Builder.Bootstrapper.Resolve<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false");
            for (var x = 0; x < 50; ++x)
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
            var Result = await Instance.ExecuteScalarAsync<int>().ConfigureAwait(false);
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            Result = await Instance.AddQuery(CommandType.Text,
                "UPDATE [TestDatabase].[dbo].[TestTable] SET StringValue1=@0",
                "C")
                .ExecuteScalarAsync<int>().ConfigureAwait(false);
            Assert.Equal(50, Result);
        }

        [Fact]
        public async Task InsertWithAtSymbolAsync()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(Canister.Builder.Bootstrapper.Resolve<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false");
            for (var x = 0; x < 50; ++x)
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
            var Result = await Instance.ExecuteScalarAsync<int>().ConfigureAwait(false);
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            var ListResult = await Instance.AddQuery(CommandType.Text,
                "SELECT * FROM [TestDatabase].[dbo].[TestTable]")
                .ExecuteAsync().ConfigureAwait(false);
            Assert.Single(ListResult);
            Assert.Equal(50, ListResult[0].Count);
            for (var x = 0; x < 50; ++x)
            {
                Assert.Equal("email@address.com", ListResult[0][x].StringValue1);
                Assert.Equal("email@address.com", ListResult[0][x].StringValue2);
                Assert.Equal(10, ListResult[0][x].BigIntValue);
                Assert.Equal(true, ListResult[0][x].BitValue);
                Assert.Equal(75.12m, ListResult[0][x].DecimalValue);
                Assert.Equal(4.53f, ListResult[0][x].FloatValue);
                Assert.Equal(new DateTime(2010, 1, 1), ListResult[0][x].DateTimeValue);
                Assert.Equal(TempGuid, ListResult[0][x].GUIDValue);
                Assert.Equal(new TimeSpan(1, 0, 0), ListResult[0][x].TimeSpanValue);
            }
        }
    }
}