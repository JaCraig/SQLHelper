using Microsoft.Extensions.Configuration;
using Xunit;

namespace SQLHelper.Tests.HelperClasses
{
    public class ConnectionTests
    {
        private static string ConnectionString = "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false;TrustServerCertificate=True";
        private static string ConnectionStringWithTimeout = "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false;connection timeout=100;TrustServerCertificate=True;";
        private static string ConnectionStringWithTimeout2 = "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false;connect timeout=100;TrustServerCertificate=True;";

        [Fact]
        public void Initialization()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var TestConnection = new SQLHelperDB.HelperClasses.Connection(Configuration, Microsoft.Data.SqlClient.SqlClientFactory.Instance, ConnectionString);
            Assert.Equal(30, TestConnection.CommandTimeout);
            Assert.Equal(ConnectionString, TestConnection.ConnectionString);
            Assert.Equal("TestDatabase", TestConnection.DatabaseName);
            Assert.Same(Microsoft.Data.SqlClient.SqlClientFactory.Instance, TestConnection.Factory);
            Assert.Equal("@", TestConnection.ParameterPrefix);
            Assert.Equal(0, TestConnection.Retries);
        }

        [Fact]
        public void InitializationWithTimeout()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var TestConnection = new SQLHelperDB.HelperClasses.Connection(Configuration, Microsoft.Data.SqlClient.SqlClientFactory.Instance, ConnectionStringWithTimeout);
            Assert.Equal(100, TestConnection.CommandTimeout);
            Assert.Equal(ConnectionStringWithTimeout, TestConnection.ConnectionString);
            Assert.Equal("TestDatabase", TestConnection.DatabaseName);
            Assert.Same(Microsoft.Data.SqlClient.SqlClientFactory.Instance, TestConnection.Factory);
            Assert.Equal("@", TestConnection.ParameterPrefix);
            Assert.Equal(0, TestConnection.Retries);
        }

        [Fact]
        public void InitializationWithTimeout2()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var TestConnection = new SQLHelperDB.HelperClasses.Connection(Configuration, Microsoft.Data.SqlClient.SqlClientFactory.Instance, ConnectionStringWithTimeout2);
            Assert.Equal(100, TestConnection.CommandTimeout);
            Assert.Equal(ConnectionStringWithTimeout2, TestConnection.ConnectionString);
            Assert.Equal("TestDatabase", TestConnection.DatabaseName);
            Assert.Same(Microsoft.Data.SqlClient.SqlClientFactory.Instance, TestConnection.Factory);
            Assert.Equal("@", TestConnection.ParameterPrefix);
            Assert.Equal(0, TestConnection.Retries);
        }
    }
}