using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using System.Data;
using System.Text;

namespace SQLHelperDB.SpeedTests.Tests
{
    [MemoryDiagnoser]
    public class AddQuery
    {
        private SQLHelper Helper { get; set; }

        private string QueryText { get; } = "IF NOT EXISTS (SELECT TOP 1 ID_ FROM [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade] WHERE [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = 6701 AND [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_] = 1341) BEGIN INSERT INTO [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade]([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_],[dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_]) VALUES (6701,1341) END;";

        [Benchmark(Baseline = true)]
        public void Run() => Helper.AddQuery(CommandType.Text, QueryText, 0, 1, 2);

        [GlobalSetup]
        public void Setup()
        {
            var Services = new ServiceCollection().AddCanisterModules(x => x.AddAssembly(typeof(Program).Assembly)
                .RegisterSQLHelper()).BuildServiceProvider();
            Helper = new SQLHelper(Services.GetService<ObjectPool<StringBuilder>>(), Services.GetService<IConfiguration>(), null);
        }
    }
}