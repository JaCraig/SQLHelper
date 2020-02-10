using SQLHelperDB.HelperClasses;
using SQLHelperDB.Tests.BaseClasses;
using System.Data;
using Xunit;

namespace SQLHelperDB.Tests.HelperClasses
{
    public class ParameterTests : TestingDirectoryFixture
    {
        [Fact]
        public void Create()
        {
            var TestItem = new Parameter<int>("0", 10);
            Assert.Equal(DbType.Int32, TestItem.DatabaseType);
            Assert.Equal(ParameterDirection.Input, TestItem.Direction);
            Assert.Equal("0", TestItem.ID);
            Assert.Equal(10, TestItem.InternalValue);
            Assert.Equal("@", TestItem.ParameterStarter);
            Assert.Equal(10, TestItem.Value);
        }

        [Fact]
        public void CreateCopy()
        {
            var TestItem = new Parameter<int>("0", 10).CreateCopy("ABC");
            Assert.Equal(DbType.Int32, TestItem.DatabaseType);
            Assert.Equal(ParameterDirection.Input, TestItem.Direction);
            Assert.Equal("0ABC", TestItem.ID);
            Assert.Equal(10, TestItem.InternalValue);
        }

        [Fact]
        public void EqualsTest()
        {
            var TestItem1 = new Parameter<int>("0", 10);
            var TestItem2 = new Parameter<int>("0", 10);
            var TestItem3 = new Parameter<int>("1", 10);
            Assert.True(TestItem1.Equals(TestItem2));
            Assert.False(TestItem1.Equals(TestItem3));
            Assert.False(TestItem2.Equals(TestItem3));
            Assert.True(TestItem1 == TestItem2);
            Assert.False(TestItem1 == TestItem3);
            Assert.False(TestItem2 == TestItem3);
        }
    }
}