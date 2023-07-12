using Microsoft.Extensions.Configuration;
using SQLHelperDB.HelperClasses;
using SQLHelperDB.Tests.BaseClasses;
using Xunit;

namespace SQLHelperDB.Tests.HelperClasses
{
    public class SourceTests : TestingDirectoryFixture
    {
        [Fact]
        public void Create()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var TestItem = new Connection(Configuration, Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"), "DATABASE NAME");
            Assert.Equal(Configuration.GetConnectionString("Default"), TestItem.ConnectionString);
            Assert.Equal("TestDatabase", TestItem.DatabaseName);
            Assert.Equal(Microsoft.Data.SqlClient.SqlClientFactory.Instance, TestItem.Factory);
            Assert.Equal("DATABASE NAME", TestItem.Name);
            Assert.Equal("@", TestItem.ParameterPrefix);
        }
    }
}