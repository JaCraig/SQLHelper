using Microsoft.Extensions.Configuration;
using SQLHelper.Tests.BaseClasses;
using SQLHelper.Tests.DataClasses;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace SQLHelper.Tests
{
    public class SQLHelperTests : TestingDirectoryFixture
    {
        [Fact]
        public void AddQuery()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new SQLHelper(Configuration, SqlClientFactory.Instance);
            Instance.AddQuery(CommandType.Text, "SELECT * FROM TestUsers");
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers", Instance.ToString());
        }

        [Fact]
        public void AddQueryFromCopy()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var CopyInstance = new SQLHelper(Configuration, SqlClientFactory.Instance);
            var Instance = new SQLHelper(Configuration, SqlClientFactory.Instance);
            CopyInstance.AddQuery(CommandType.Text, "SELECT * FROM TestUsers");
            Instance.AddQuery(CopyInstance);
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers", Instance.ToString());
        }

        [Fact]
        public void AddQuerys()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new SQLHelper(Configuration, SqlClientFactory.Instance);
            Instance.AddQuery(CommandType.Text, "SELECT * FROM TestUsers")
                .AddQuery(CommandType.Text, "SELECT * FROM TestGroups");
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers\r\nSELECT * FROM TestGroups", Instance.ToString());
        }

        [Fact]
        public void AddQuerysWithParameters()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new SQLHelper(Configuration, SqlClientFactory.Instance);
            Instance.AddQuery(CommandType.Text, "SELECT * FROM TestUsers WHERE UserID=@0", 1)
                .AddQuery(CommandType.Text, "SELECT * FROM TestGroups WHERE GroupID=@0", 10);
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers WHERE UserID=1\r\nSELECT * FROM TestGroups WHERE GroupID=10", Instance.ToString());
        }

        [Fact]
        public void AddQueryWithParameters()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new SQLHelper(Configuration, SqlClientFactory.Instance);
            Instance.AddQuery(CommandType.Text, "SELECT * FROM TestUsers WHERE UserID=@0", 1);
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers WHERE UserID=1", Instance.ToString());
        }

        [Fact]
        public void Create()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new SQLHelper(Configuration, SqlClientFactory.Instance);
            Assert.NotNull(Instance);
            Assert.Equal("", Instance.ToString());
        }

        [Fact]
        public void ExecuteInsert()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new SQLHelper(Configuration, SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false");
            var Result = Instance.AddQuery(CommandType.Text,
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
                .ExecuteScalar<int>();
            Assert.Equal(1, Result);
            Instance.CreateBatch();
            for (int x = 0; x < 50; ++x)
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
            Result = Instance.ExecuteScalar<int>();
            Assert.Equal(50, Result);
        }

        [Fact]
        public void ExecuteSelect()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(Configuration, SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false");
            for (int x = 0; x < 50; ++x)
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
            var Result = Instance.ExecuteScalar<int>();
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            var ListResult = Instance.AddQuery(CommandType.Text,
                "SELECT * FROM [TestDatabase].[dbo].[TestTable]")
                .Execute();
            Assert.Equal(1, ListResult.Count);
            Assert.Equal(50, ListResult[0].Count);
            for (int x = 0; x < 50; ++x)
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
        public void ExecuteSelectToObjectType()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(Configuration, SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false");
            for (int x = 0; x < 50; ++x)
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
            var Result = Instance.ExecuteScalar<int>();
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            var ListResult = Instance.AddQuery(CommandType.Text,
                "SELECT * FROM [TestDatabase].[dbo].[TestTable]")
                .Execute();
            Assert.Equal(1, ListResult.Count);
            Assert.Equal(50, ListResult[0].Count);
            var ConvertedResult = ListResult[0].Select(x => (TestTableClass)x).ToList();
            for (int x = 0; x < 50; ++x)
            {
                Assert.Equal("A", ConvertedResult[x].StringValue1);
                Assert.Equal("B", ConvertedResult[x].StringValue2);
                Assert.Equal(10, ConvertedResult[x].BigIntValue);
                Assert.Equal(true, ConvertedResult[x].BitValue);
                Assert.Equal(75.12m, ConvertedResult[x].DecimalValue);
                Assert.Equal(4.53f, ConvertedResult[x].FloatValue);
                Assert.Equal(new DateTime(2010, 1, 1), ConvertedResult[x].DateTimeValue);
                Assert.Equal(TempGuid, ConvertedResult[x].GUIDValue);
                Assert.Equal(new TimeSpan(1, 0, 0), ConvertedResult[x].TimeSpanValue);
            }
        }

        [Fact]
        public void ExecuteUpdate()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new SQLHelper(Configuration, SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false");
            for (int x = 0; x < 50; ++x)
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
            var Result = Instance.ExecuteScalar<int>();
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            Result = Instance.AddQuery(CommandType.Text,
                "UPDATE [TestDatabase].[dbo].[TestTable] SET StringValue1=@0",
                "C")
                .ExecuteScalar<int>();
            Assert.Equal(50, Result);
        }

        [Fact]
        public void RemoveDuplicateCommands()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new SQLHelper(Configuration, SqlClientFactory.Instance);
            Instance.AddQuery(CommandType.Text, "SELECT * FROM TestUsers")
                    .AddQuery(CommandType.Text, "SELECT * FROM TestUsers");
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers\r\nSELECT * FROM TestUsers", Instance.ToString());
            Instance.RemoveDuplicateCommands();
            Assert.Equal("SELECT * FROM TestUsers", Instance.ToString());
        }
    }
}