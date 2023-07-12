using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;

namespace SQLHelper.Example
{
    /// <summary>
    /// This is an example program that shows how to use the SQLHelper.DB library to execute a batch of queries.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static async Task Main(string[] args)
        {
            // Start by creating a new ServiceCollection and adding the Canister modules to it (this will also add the SQLHelper module)
            var Services = new ServiceCollection().AddCanisterModules()?.BuildServiceProvider();

            // Get the SQLHelper instance from the ServiceCollection
            var Helper = Services.GetService<SQLHelperDB.SQLHelper>();

            // Execute a batch of queries and return the results (this will return a list of lists of rows. The first list contains the results of each query. The inner lists contain the rows.)
            var Results = await Helper.CreateBatch(SqlClientFactory.Instance)
                .AddQuery(System.Data.CommandType.Text, "SELECT * FROM [dbo].[TestTable]")
                .AddQuery(System.Data.CommandType.Text, "SELECT * FROM [dbo].[TestTable]")
                .ExecuteAsync()
                .ConfigureAwait(false);

            // Go through each result
            foreach (var Result in Results)
            {
                // Go through each row in the result
                foreach (var Row in Result)
                {
                    // Write the row to the console
                    Console.WriteLine(Row.ToString());
                }
            }
        }
    }
}