using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace SQLHelperDB.CanisterModules
{
    /// <summary>
    /// SQLHelper Canister module.
    /// </summary>
    /// <seealso cref="IModule"/>
    public class SQLHelperModule : IModule
    {
        /// <summary>
        /// Order to run this in
        /// </summary>
        public int Order { get; } = 1;

        /// <summary>
        /// Loads the module using the bootstrapper
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper.</param>
        public void Load(IBootstrapper bootstrapper)
        {
            if (bootstrapper == null)
                return;
            bootstrapper.Register<SQLHelper>(ServiceLifetime.Transient);
        }
    }
}