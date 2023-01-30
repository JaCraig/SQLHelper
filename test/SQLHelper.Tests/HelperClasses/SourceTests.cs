using Microsoft.Extensions.Configuration;
using SQLHelperDB.HelperClasses;
using SQLHelperDB.Tests.BaseClasses;
using Xunit;

namespace SQLHelperDB.Tests.HelperClasses
{
    public class SourceTests : TestingDirectoryFixture
    {
        private const string DefaultConnectionString = "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false;TrustServerCertificate=True";

        [Fact]
        public void Create()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var TestItem = new Connection(Configuration, Microsoft.Data.SqlClient.SqlClientFactory.Instance, DefaultConnectionString, "DATABASE NAME");
            Assert.Equal(DefaultConnectionString, TestItem.ConnectionString);
            Assert.Equal("TestDatabase", TestItem.DatabaseName);
            Assert.Equal(Microsoft.Data.SqlClient.SqlClientFactory.Instance, TestItem.Factory);
            Assert.Equal("DATABASE NAME", TestItem.Name);
            Assert.Equal("@", TestItem.ParameterPrefix);
        }
    }
}