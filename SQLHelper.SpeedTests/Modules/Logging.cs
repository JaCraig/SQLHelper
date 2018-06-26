using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace SQLHelperDB.SpeedTests.Modules
{
    /// <summary>
    /// Logging module
    /// </summary>
    /// <seealso cref="Canister.Interfaces.IModule"/>
    public class LoggingModule : IModule
    {
        /// <summary>
        /// Order to run this in
        /// </summary>
        public int Order => 1;

        /// <summary>
        /// Loads the module using the bootstrapper
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper.</param>
        public void Load(IBootstrapper bootstrapper)
        {
            if (bootstrapper == null)
                return;
            Log.Logger = new LoggerConfiguration()
                                            .WriteTo
                                            .File("./Log.txt")
                                            .MinimumLevel
                                            .Error()
                                            .CreateLogger();
            bootstrapper.Register<ILogger>(Log.Logger,
                                        ServiceLifetime.Singleton);
        }
    }
}