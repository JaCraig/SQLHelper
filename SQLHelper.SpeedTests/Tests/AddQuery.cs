using Microsoft.Extensions.Configuration;
using Sundial.Core.Attributes;
using Sundial.Core.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace SQLHelper.SpeedTests.Tests
{
    [Series("AddQuery", 100, "Console")]
    public class AddQuery : ITimedTask
    {
        public AddQuery()
        {
            Helper = new SQLHelper(Canister.Builder.Bootstrapper.Resolve<IConfiguration>(), SqlClientFactory.Instance);
        }

        public bool Baseline => true;
        public string Name => "AddQuery test";
        private SQLHelper Helper { get; }

        public void Dispose()
        {
        }

        public void Run()
        {
            Helper.AddQuery(CommandType.Text, "IF NOT EXISTS (SELECT TOP 1 ID_ FROM [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade] WHERE [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = 6701 AND [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_] = 1341) BEGIN INSERT INTO [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade]([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_],[dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_]) VALUES (6701,1341) END;", 0, 1, 2);
        }
    }
}