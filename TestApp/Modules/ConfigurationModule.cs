using Canister.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace TestApp.Modules
{
    public class ConfigurationModule : IModule
    {
        public int Order => 1;

        protected string ConnectionString => "Data Source=localhost;Initial Catalog=SereneCMS;Integrated Security=SSPI;Pooling=false";

        public void Load(IServiceCollection bootstrapper)
        {
            if (bootstrapper is null)
                return;
            var dict = new Dictionary<string, string>
                {
                    { "ConnectionStrings:Default", ConnectionString },
                };
            var Configuration = new ConfigurationBuilder()
                             .AddInMemoryCollection(dict)
                             .Build();
            bootstrapper.AddSingleton<IConfiguration>(_ => Configuration);
            bootstrapper.AddSingleton<IConfigurationRoot>(_ => Configuration);
        }
    }
}