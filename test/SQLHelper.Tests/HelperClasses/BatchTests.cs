using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using SQLHelper.Tests.Utils;
using SQLHelperDB.HelperClasses;
using SQLHelperDB.Tests.BaseClasses;
using System.Data;
using System.Text;
using Xunit;

namespace SQLHelperDB.Tests.HelperClasses
{
    public class BatchTests : TestingDirectoryFixture
    {
        [Fact]
        public void AddQuery()
        {
            var Configuration = TestConfigurationFactory.Create();
            var Instance = new Batch(new Connection(Configuration,
                    Microsoft.Data.SqlClient.SqlClientFactory.Instance,
                    Configuration.GetConnectionString("Default"),
                    "DATABASE NAME"),
                    GetServiceProvider().GetService<ObjectPool<StringBuilder>>(),
                     null
                );
            Instance.AddQuery((___, __, _) => { }, 10, false, "SELECT * FROM TestUsers", CommandType.Text);
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers", Instance.ToString());
        }

        [Fact]
        public void AddQuerys()
        {
            var Configuration = TestConfigurationFactory.Create();
            var Instance = new Batch(new Connection(Configuration,
                    Microsoft.Data.SqlClient.SqlClientFactory.Instance,
                    Configuration.GetConnectionString("Default"),
                    "DATABASE NAME"),
                    GetServiceProvider().GetService<ObjectPool<StringBuilder>>(),
                     null
                );
            Instance.AddQuery((___, __, _) => { }, 10, false, "SELECT * FROM TestUsers", CommandType.Text)
                .AddQuery((___, __, _) => { }, 10, false, "SELECT * FROM TestGroups", CommandType.Text);
            Assert.NotNull(Instance);
            Assert.Equal(TestConnectionStrings.NormalizeLineEndings("SELECT * FROM TestUsers\r\nSELECT * FROM TestGroups"), TestConnectionStrings.NormalizeLineEndings(Instance.ToString()));
        }

        [Fact]
        public void AddQuerysWithParameters()
        {
            var Configuration = TestConfigurationFactory.Create();
            var Instance = new Batch(new Connection(Configuration,
                    Microsoft.Data.SqlClient.SqlClientFactory.Instance,
                    Configuration.GetConnectionString("Default"),
                    "DATABASE NAME"),
                    GetServiceProvider().GetService<ObjectPool<StringBuilder>>(),
                     null
                );
            Instance.AddQuery((___, __, _) => { }, 10, false, "SELECT * FROM TestUsers WHERE UserID=@0", CommandType.Text, 1)
                .AddQuery((___, __, _) => { }, 10, false, "SELECT * FROM TestGroups WHERE GroupID=@0", CommandType.Text, 10);
            Assert.NotNull(Instance);
            Assert.Equal(TestConnectionStrings.NormalizeLineEndings("SELECT * FROM TestUsers WHERE UserID=1\r\nSELECT * FROM TestGroups WHERE GroupID=10"), TestConnectionStrings.NormalizeLineEndings(Instance.ToString()));
        }

        [Fact]
        public void AddQueryWithParameters()
        {
            var Configuration = TestConfigurationFactory.Create();
            var Instance = new Batch(new Connection(Configuration,
                    Microsoft.Data.SqlClient.SqlClientFactory.Instance,
                    Configuration.GetConnectionString("Default"),
                    "DATABASE NAME"),
                    GetServiceProvider().GetService<ObjectPool<StringBuilder>>(),
                     null
                );
            Instance.AddQuery((___, __, _) => { }, 10, false, "SELECT * FROM TestUsers WHERE UserID=@0", CommandType.Text, 1);
            Assert.NotNull(Instance);
            Assert.Equal("SELECT * FROM TestUsers WHERE UserID=1", Instance.ToString());
        }

        [Fact]
        public void Creation()
        {
            var Configuration = TestConfigurationFactory.Create();
            var Instance = new Batch(new Connection(Configuration,
                    Microsoft.Data.SqlClient.SqlClientFactory.Instance,
                    Configuration.GetConnectionString("Default"),
                    "DATABASE NAME"),
                    GetServiceProvider().GetService<ObjectPool<StringBuilder>>(),
                     null
                );
            Assert.NotNull(Instance);
            Assert.Equal("", Instance.ToString());
            Assert.Equal(0, Instance.CommandCount);
        }
    }
}