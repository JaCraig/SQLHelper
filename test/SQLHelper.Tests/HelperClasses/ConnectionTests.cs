using Microsoft.Extensions.Configuration;
using Xunit;

namespace SQLHelper.Tests.HelperClasses
{
    public class ConnectionTests
    {
        [Fact]
        public void Initialization()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var TestConnection = new SQLHelperDB.HelperClasses.Connection(Configuration, Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("Default"));
            Assert.Equal(30, TestConnection.CommandTimeout);
            Assert.Equal(Configuration.GetConnectionString("Default"), TestConnection.ConnectionString);
            Assert.Equal("TestDatabase", TestConnection.DatabaseName);
            Assert.Same(Microsoft.Data.SqlClient.SqlClientFactory.Instance, TestConnection.Factory);
            Assert.Equal("@", TestConnection.ParameterPrefix);
            Assert.Equal(0, TestConnection.Retries);
        }

        [Fact]
        public void InitializationWithTimeout()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var TestConnection = new SQLHelperDB.HelperClasses.Connection(Configuration, Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("DefaultWithTimeout"));
            Assert.Equal(100, TestConnection.CommandTimeout);
            Assert.Equal(Configuration.GetConnectionString("DefaultWithTimeout"), TestConnection.ConnectionString);
            Assert.Equal("TestDatabase", TestConnection.DatabaseName);
            Assert.Same(Microsoft.Data.SqlClient.SqlClientFactory.Instance, TestConnection.Factory);
            Assert.Equal("@", TestConnection.ParameterPrefix);
            Assert.Equal(0, TestConnection.Retries);
        }

        [Fact]
        public void InitializationWithTimeout2()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").AddEnvironmentVariables()
                .Build();
            var TestConnection = new SQLHelperDB.HelperClasses.Connection(Configuration, Microsoft.Data.SqlClient.SqlClientFactory.Instance, Configuration.GetConnectionString("DefaultWithTimeout2"));
            Assert.Equal(100, TestConnection.CommandTimeout);
            Assert.Equal(Configuration.GetConnectionString("DefaultWithTimeout2"), TestConnection.ConnectionString);
            Assert.Equal("TestDatabase", TestConnection.DatabaseName);
            Assert.Same(Microsoft.Data.SqlClient.SqlClientFactory.Instance, TestConnection.Factory);
            Assert.Equal("@", TestConnection.ParameterPrefix);
            Assert.Equal(0, TestConnection.Retries);
        }
    }
}