using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SQLHelper.Tests.Utils
{
    internal static class TestConfigurationFactory
    {
        public static IConfiguration Create()
        {
            var defaultConnection = TestConnectionStrings.Default;
            var masterConnection = TestConnectionStrings.Master;

            var overrides = new Dictionary<string, string>
            {
                ["ConnectionStrings:Master"] = masterConnection,
                ["ConnectionStrings:Default"] = defaultConnection,
                ["ConnectionStrings:DefaultWithTimeout"] = defaultConnection + ";connection timeout=100",
                ["ConnectionStrings:DefaultWithTimeout2"] = defaultConnection + ";connect timeout=100"
            };

            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .AddInMemoryCollection(overrides)
                .Build();
        }
    }
}