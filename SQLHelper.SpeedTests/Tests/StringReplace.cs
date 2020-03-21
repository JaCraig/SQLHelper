using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Text;

namespace SQLHelper.SpeedTests.Tests
{
    [MemoryDiagnoser]
    public class StringReplace
    {
        [Params(1, 10, 100, 1000, 10000)]
        public int Count { get; set; }

        public string Value { get; set; }
        private ObjectPool<StringBuilder> StringBuilderPool { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var objectPoolProvider = new DefaultObjectPoolProvider();
            StringBuilderPool = objectPoolProvider.CreateStringBuilderPool();
            for (var x = 0; x < char.MaxValue; ++x)
            {
                Value += (char)x + " ";
            }
        }

        [Benchmark(Baseline = true)]
        public void StringBuilderPools()
        {
            var Builder = StringBuilderPool.Get();
            Builder.Append(Value);
            for (var x = 0; x < Count; ++x)
            {
                Builder.Replace("A", "C");
            }
            _ = Builder.ToString();
            StringBuilderPool.Return(Builder);
        }

        [Benchmark]
        public void StringConcat() => _ = Value.Replace("A", "C", StringComparison.Ordinal);
    }
}