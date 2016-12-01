using SQLHelper.HelperClasses;
using System.Data;
using Xunit;

namespace SQLHelper.Tests.HelperClasses
{
    public class StringParameterTests
    {
        [Fact]
        public void Create()
        {
            var TestItem = new StringParameter("0", "This is the value");
            Assert.Equal(DbType.String, TestItem.DatabaseType);
            Assert.Equal(ParameterDirection.Input, TestItem.Direction);
            Assert.Equal("0", TestItem.ID);
            Assert.Equal("This is the value", TestItem.InternalValue);
            Assert.Equal("@", TestItem.ParameterStarter);
            Assert.Equal("This is the value", TestItem.Value);
        }

        [Fact]
        public void CreateCopy()
        {
            var TestItem = new StringParameter("0", "This is the value").CreateCopy("ABC");
            Assert.Equal(DbType.String, TestItem.DatabaseType);
            Assert.Equal(ParameterDirection.Input, TestItem.Direction);
            Assert.Equal("0ABC", TestItem.ID);
            Assert.Equal("This is the value", TestItem.InternalValue);
        }

        [Fact]
        public void Equals()
        {
            var TestItem1 = new StringParameter("0", "This is the value");
            var TestItem2 = new StringParameter("0", "This is the value");
            var TestItem3 = new StringParameter("1", "This is the value");
            Assert.True(TestItem1.Equals(TestItem2));
            Assert.False(TestItem1.Equals(TestItem3));
            Assert.False(TestItem2.Equals(TestItem3));
            Assert.True(TestItem1 == TestItem2);
            Assert.False(TestItem1 == TestItem3);
            Assert.False(TestItem2 == TestItem3);
        }
    }
}