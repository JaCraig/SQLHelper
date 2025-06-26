using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using SQLHelperDB.HelperClasses;
using SQLHelperDB.Tests.BaseClasses;
using SQLHelperDB.Tests.DataClasses;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SQLHelperDB.Tests
{
    public class SQLHelperTests : TestingDirectoryFixture
    {
        [Fact]
        public void AddQuery()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance);
            Instance.AddQuery(CommandType.Text, "SELECT * FROM TestUsers");
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers", Instance.ToString());
        }

        [Fact]
        public void AddQueryFromCopy()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var CopyInstance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance);
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance);
            CopyInstance.AddQuery(CommandType.Text, "SELECT * FROM TestUsers");
            Instance.AddQuery(CopyInstance);
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers", Instance.ToString());
            Assert.Equal(1, Instance.Count);
        }

        [Fact]
        public void AddQuerys()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance);
            Instance.AddQuery(CommandType.Text, "SELECT * FROM TestUsers")
                .AddQuery(CommandType.Text, "SELECT * FROM TestGroups");
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers\r\nSELECT * FROM TestGroups", Instance.ToString());
            Assert.Equal(2, Instance.Count);
        }

        [Fact]
        public void AddQuerysWithParameters()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance);
            Instance.AddQuery(CommandType.Text, "SELECT * FROM TestUsers WHERE UserID=@0", 1)
                .AddQuery(CommandType.Text, "SELECT * FROM TestGroups WHERE GroupID=@0", 10);
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers WHERE UserID=1\r\nSELECT * FROM TestGroups WHERE GroupID=10", Instance.ToString());
            Assert.Equal(2, Instance.Count);
        }

        [Fact]
        public void AddQueryWithParameters()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance);
            Instance.AddQuery(CommandType.Text, "SELECT * FROM TestUsers WHERE UserID=@0", 1);
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers WHERE UserID=1", Instance.ToString());
            Assert.Equal(1, Instance.Count);
        }

        [Fact]
        public void Create()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance);
            Assert.NotNull(Instance);
            Assert.Equal("", Instance.ToString());
        }

        [Fact]
        public async Task ExecuteInsert()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
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
            Result = await Instance.ExecuteScalarAsync<int>();
            Assert.Equal(50, Result);
        }

        [Fact]
        public async Task ExecuteInsertAndGetBackId()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
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
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
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
        public async Task ExecuteScalar()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
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
            var Result = await Instance.ExecuteScalarAsync<int>();
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            var ListResult = await Instance.AddQuery(CommandType.Text,
                "SELECT COUNT(*) FROM [TestDatabase].[dbo].[TestTable]")
                .ExecuteScalarAsync<int>();
            Assert.Equal(50, ListResult);
        }

        [Fact]
        public async Task ExecuteSelect()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
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
            var Result = await Instance.ExecuteScalarAsync<int>();
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            var ListResult = await Instance.AddQuery(CommandType.Text,
                "SELECT * FROM [TestDatabase].[dbo].[TestTable]")
                .ExecuteAsync();
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
        public async Task ExecuteSelectHundredsOfParamters()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
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
                .ExecuteAsync();
            Assert.Single(ListResult);
        }

        [Fact]
        public async Task ExecuteSelectThousandsOfParameters()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            for (var x = 0; x < 4000; ++x)
            {
                Instance.AddQuery(CommandType.Text, "SELECT * FROM [TestDatabase].[dbo].[TestTable] WHERE [TestDatabase].[dbo].[TestTable].[ID]=@0", x);
            }
            var ListResult = await Instance.ExecuteAsync();
            Assert.Equal(4000, ListResult.Count);
        }

        [Fact]
        public async Task ExecuteSelectThousandsOfParametersWithHeader()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            Instance.AddHeader(CommandType.Text, "DECLARE @A as nvarchar(100);");
            Instance.AddHeader(CommandType.Text, "SET @A ='BLAH';");
            for (var x = 0; x < 4000; ++x)
            {
                Instance.AddQuery(CommandType.Text, "SELECT * FROM [TestDatabase].[dbo].[TestTable] WHERE [TestDatabase].[dbo].[TestTable].[ID]=@0 AND @A='BLAH'", x);
            }
            var ListResult = await Instance.ExecuteAsync();
            Assert.Equal(4000, ListResult.Count);
        }

        [Fact]
        public async Task ExecuteSelectToObjectType()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
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
            var Result = await Instance.ExecuteScalarAsync<int>();
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            var ListResult = await Instance.AddQuery(CommandType.Text,
                "SELECT * FROM [TestDatabase].[dbo].[TestTable]")
                .ExecuteAsync();
            Assert.Single(ListResult);
            Assert.Equal(50, ListResult[0].Count);
            var ConvertedResult = ListResult[0].ConvertAll(x => (TestTableClass)x);
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
        public async Task ExecuteSelectUri()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            for (var x = 0; x < 50; ++x)
            {
                Instance.AddQuery(CommandType.Text,
                    "INSERT INTO [TestDatabase].[dbo].[TestTable](StringValue1,StringValue2,BigIntValue,BitValue,DecimalValue,FloatValue,DateTimeValue,GUIDValue,TimeSpanValue) VALUES(@0,@1,@2,@3,@4,@5,@6,@7,@8)",
                    new Uri("http://A"),
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
            for (var x = 0; x < 50; ++x)
            {
                Assert.Equal("http://a/", ListResult[0][x].StringValue1);
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
        public async Task ExecuteStoredProcedure()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            var Result = (await Instance.AddQuery(CommandType.StoredProcedure, "TestSP @Value = @0", "Test String")
                                 .ExecuteAsync())
                        .FirstOrDefault()[0];
            Assert.NotNull(Result);
            Assert.Equal("Test String", Result.Value);
        }

        [Fact]
        public async Task ExecuteUpdate()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
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
        public async Task InsertWithAtSymbol()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var TempGuid = Guid.NewGuid();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
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
            var Result = await Instance.ExecuteScalarAsync<int>();
            Assert.Equal(50, Result);
            Instance.CreateBatch();
            var ListResult = await Instance.AddQuery(CommandType.Text,
                "SELECT * FROM [TestDatabase].[dbo].[TestTable]")
                .ExecuteAsync();
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

        [Fact]
        public async Task NotNullInsertParameters()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            Assert.NotNull(Instance);
            var Result = await Instance.CreateBatch().AddQuery(CommandType.Text,
                "INSERT INTO [TestDatabase].[dbo].[TestTableNotNull](UShortValue_) VALUES(@0)",
                0)
                .ExecuteScalarAsync<int>();
            Assert.Equal(1, Result);
            Result = await Instance.CreateBatch().AddQuery(
                CommandType.Text,
                "INSERT INTO [TestDatabase].[dbo].[TestTableNotNull](UShortValue_) VALUES(@0)",
                new Parameter<object>("0", SqlDbType.SmallInt, 0, ParameterDirection.Input))
                .ExecuteScalarAsync<int>();
            Assert.Equal(1, Result);
        }

        [Fact]
        public void RemoveDuplicateCommands()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance);
            Instance.AddQuery(CommandType.Text, "SELECT * FROM TestUsers")
                    .AddQuery(CommandType.Text, "SELECT * FROM TestUsers");
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers\r\nSELECT * FROM TestUsers", Instance.ToString());
            Assert.Equal(2, Instance.Count);
            Instance.RemoveDuplicateCommands();
            Assert.Equal("SELECT * FROM TestUsers", Instance.ToString());
            Assert.Equal(1, Instance.Count);
        }

        [Fact]
        public void SwitchConnection()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var Instance = new SQLHelper(GetServiceProvider().GetService<ObjectPool<StringBuilder>>(), Configuration, null)
                .CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase22222;Integrated Security=SSPI;Pooling=false;TrustServerCertificate=True");
            Instance.CreateBatch(Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            Assert.Equal(Configuration.GetConnectionString("Default"), Instance.DatabaseConnection.ConnectionString);
        }
    }
}