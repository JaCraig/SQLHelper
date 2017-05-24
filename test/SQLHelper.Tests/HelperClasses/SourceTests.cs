using Microsoft.Extensions.Configuration;
using SQLHelper.HelperClasses;
using SQLHelper.Tests.BaseClasses;
using System.Data.SqlClient;
using Xunit;

namespace SQLHelper.Tests.HelperClasses
{
    public class SourceTests : TestingDirectoryFixture
    {
        [Fact]
        public void Create()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var TestItem = new Connection(Configuration, SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false", "DATABASE NAME");
            Assert.Equal("Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false", TestItem.ConnectionString);
            Assert.Equal("TestDatabase", TestItem.DatabaseName);
            Assert.Equal(SqlClientFactory.Instance, TestItem.Factory);
            Assert.Equal("DATABASE NAME", TestItem.Name);
            Assert.Equal("@", TestItem.ParameterPrefix);
            Assert.Equal("System.Data.SqlClient.SqlClientFactory", TestItem.SourceType);
        }
    }
}