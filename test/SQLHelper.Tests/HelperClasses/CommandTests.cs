using SQLHelper.HelperClasses;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Xunit;

namespace SQLHelper.Tests.HelperClasses
{
    public class CommandTests
    {
        [Fact]
        public void Creation()
        {
            var TestItem = new Command<int>((x, y, z) => { }, 10, "SELECT * FROM TestUsers", CommandType.Text, "@", new object[] { 1, 2, 3 });
            Assert.NotNull(TestItem.CallBack);
            Assert.Equal(CommandType.Text, TestItem.CommandType);
            Assert.True(TestItem.Finalizable);
            Assert.Equal(10, TestItem.Object);
            Assert.Equal(3, TestItem.Parameters.Count);

            for (int x = 0; x < 3; ++x)
            {
                Assert.Equal(DbType.Int32, TestItem.Parameters.ElementAt(x).DatabaseType);
                Assert.Equal(ParameterDirection.Input, TestItem.Parameters.ElementAt(x).Direction);
                Assert.Equal(x.ToString(), TestItem.Parameters.ElementAt(x).ID);
                Assert.Equal(x + 1, TestItem.Parameters.ElementAt(x).InternalValue);
            }
            Assert.Equal("SELECT * FROM TestUsers", TestItem.SQLCommand);
        }

        [Fact]
        public void FinalizeTest()
        {
            StringBuilder Builder = new StringBuilder();
            var TestItem = new Command<int>((x, y, z) => { Builder.Append(z); }, 10, "SELECT * FROM TestUsers", CommandType.Text, "@", new object[] { 1, 2, 3 });
            TestItem.Finalize(new List<dynamic>());
            Assert.Equal("10", Builder.ToString());
        }
    }
}