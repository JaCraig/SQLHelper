using System;

namespace SQLHelper.Tests.DataClasses
{
    public class TestTableClass
    {
        public long BigIntValue { get; set; }
        public bool BitValue { get; set; }
        public DateTime DateTimeValue { get; set; }
        public decimal DecimalValue { get; set; }
        public float FloatValue { get; set; }
        public Guid GUIDValue { get; set; }
        public string StringValue1 { get; set; }

        public string StringValue2 { get; set; }

        public TimeSpan TimeSpanValue { get; set; }
    }
}