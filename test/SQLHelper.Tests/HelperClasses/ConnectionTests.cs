using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Xunit;

namespace SQLHelper.Tests.HelperClasses
{
    public class ConnectionTests
    {
        [Fact]
        public void Initialization()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var TestConnection = new SQLHelperDB.HelperClasses.Connection(Configuration, SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false");
            Assert.Equal(30, TestConnection.CommandTimeout);
            Assert.Same(Configuration, TestConnection.Configuration);
            Assert.Equal("Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false", TestConnection.ConnectionString);
            Assert.Equal("TestDatabase", TestConnection.DatabaseName);
            Assert.Same(SqlClientFactory.Instance, TestConnection.Factory);
            Assert.Equal("@", TestConnection.ParameterPrefix);
            Assert.Equal(0, TestConnection.Retries);
            Assert.Equal("System.Data.SqlClient.SqlClientFactory", TestConnection.SourceType);
        }

        [Fact]
        public void InitializationWithTimeout()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var TestConnection = new SQLHelperDB.HelperClasses.Connection(Configuration, SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false;connection timeout=100;");
            Assert.Equal(100, TestConnection.CommandTimeout);
            Assert.Same(Configuration, TestConnection.Configuration);
            Assert.Equal("Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false;connection timeout=100;", TestConnection.ConnectionString);
            Assert.Equal("TestDatabase", TestConnection.DatabaseName);
            Assert.Same(SqlClientFactory.Instance, TestConnection.Factory);
            Assert.Equal("@", TestConnection.ParameterPrefix);
            Assert.Equal(0, TestConnection.Retries);
            Assert.Equal("System.Data.SqlClient.SqlClientFactory", TestConnection.SourceType);
        }

        [Fact]
        public void InitializationWithTimeout2()
        {
            var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
            var TestConnection = new SQLHelperDB.HelperClasses.Connection(Configuration, SqlClientFactory.Instance, "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false;connect timeout=100;");
            Assert.Equal(100, TestConnection.CommandTimeout);
            Assert.Same(Configuration, TestConnection.Configuration);
            Assert.Equal("Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false;connect timeout=100;", TestConnection.ConnectionString);
            Assert.Equal("TestDatabase", TestConnection.DatabaseName);
            Assert.Same(SqlClientFactory.Instance, TestConnection.Factory);
            Assert.Equal("@", TestConnection.ParameterPrefix);
            Assert.Equal(0, TestConnection.Retries);
            Assert.Equal("System.Data.SqlClient.SqlClientFactory", TestConnection.SourceType);
        }
    }
}