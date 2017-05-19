using Microsoft.Extensions.Configuration;
using SQLHelper.HelperClasses;
using SQLHelper.Tests.BaseClasses;
using System.Data;
using System.Data.SqlClient;
using Xunit;

namespace SQLHelper.Tests.HelperClasses
{
    public class BatchTests : TestingDirectoryFixture
    {
        [Fact]
        public void AddQuery()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new Batch(new Connection(Configuration,
                    SqlClientFactory.Instance,
                    "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false",
                    "DATABASE NAME")
                );
            Instance.AddQuery((x, y, z) => { }, 10, CommandType.Text, "SELECT * FROM TestUsers");
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers", Instance.ToString());
        }

        [Fact]
        public void AddQuerys()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new Batch(new Connection(Configuration,
                    SqlClientFactory.Instance,
                    "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false",
                    "DATABASE NAME")
                );
            Instance.AddQuery((x, y, z) => { }, 10, CommandType.Text, "SELECT * FROM TestUsers")
                .AddQuery((x, y, z) => { }, 10, CommandType.Text, "SELECT * FROM TestGroups");
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers\r\nSELECT * FROM TestGroups", Instance.ToString());
        }

        [Fact]
        public void AddQuerysWithParameters()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new Batch(new Connection(Configuration,
                    SqlClientFactory.Instance,
                    "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false",
                    "DATABASE NAME")
                );
            Instance.AddQuery((x, y, z) => { }, 10, "SELECT * FROM TestUsers WHERE UserID=@0", CommandType.Text, 1)
                .AddQuery((x, y, z) => { }, 10, "SELECT * FROM TestGroups WHERE GroupID=@0", CommandType.Text, 10);
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers WHERE UserID=1\r\nSELECT * FROM TestGroups WHERE GroupID=10", Instance.ToString());
        }

        [Fact]
        public void AddQueryWithParameters()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new Batch(new Connection(Configuration,
                    SqlClientFactory.Instance,
                    "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false",
                    "DATABASE NAME")
                );
            Instance.AddQuery((x, y, z) => { }, 10, "SELECT * FROM TestUsers WHERE UserID=@0", CommandType.Text, 1);
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers WHERE UserID=1", Instance.ToString());
        }

        [Fact]
        public void Creation()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var Instance = new Batch(new Connection(Configuration,
                    SqlClientFactory.Instance,
                    "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false",
                    "DATABASE NAME")
                );
            Assert.NotNull(Instance);
            Assert.Equal("", Instance.ToString());
            Assert.Equal(0, Instance.CommandCount);
        }
    }
}