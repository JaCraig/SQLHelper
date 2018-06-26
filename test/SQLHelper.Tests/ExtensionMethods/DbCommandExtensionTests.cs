using SQLHelperDB.ExtensionMethods;
using System;
using System.Data;
using System.Data.SqlClient;
using Xunit;

namespace SQLHelperDB.Tests.ExtensionMethods
{
    public class DbCommandExtensionTests
    {
        public static readonly TheoryData<object> ParameterTypes = new TheoryData<object>
        {
            {  (sbyte)123},
            {  (byte)123},
            {  123},
            {  (uint)123},
            {  (short)123},
            {  (ushort)123},
            {  (long)123},
            {  (ulong)123},
            { "asdf" },
            { 'a' },
            { 0.43 },
            { (decimal)0.53 },
            { (float)0.42 },
            {  (sbyte?)123},
            {  (byte?)123},
            {  (int?)123},
            {  (uint?)123},
            {  (short?)123},
            {  (ushort?)123},
            {  (long?)123},
            {  (ulong?)123},
            { (char?)'a' },
            { (double?)0.43 },
            { (decimal?)0.53 },
            { (float?)0.42 },
            { (DateTime?)new DateTime(2000,1,1) },
            { (TimeSpan?)new TimeSpan(1,1,1) },
            { (bool?)true  },
            { new byte[]{1,2,3,4 } },
            { (DateTimeOffset?)new DateTimeOffset(new DateTime(2000,1,1),new TimeSpan(1,1,0)) },
            { (Guid?)Guid.NewGuid() }
        };

        [Theory]
        [MemberData(nameof(ParameterTypes))]
        public void AddParameter(object value)
        {
            using (var TempConnection = SqlClientFactory.Instance.CreateConnection())
            {
                using (var TempCommand = TempConnection.CreateCommand())
                {
                    TempCommand.AddParameter("0", value);
                    Assert.Equal(TempCommand.Parameters[0].Value, value);
                    Assert.Equal(TempCommand.Parameters[0].IsNullable, value is Nullable);
                    Assert.Equal(ParameterDirection.Input, TempCommand.Parameters[0].Direction);
                }
            }
        }
    }
}