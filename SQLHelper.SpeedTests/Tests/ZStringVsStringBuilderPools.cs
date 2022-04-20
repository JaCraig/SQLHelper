using BenchmarkDotNet.Attributes;
using Cysharp.Text;
using Microsoft.Extensions.ObjectPool;
using System.Text;

namespace SQLHelper.SpeedTests.Tests
{
    [MemoryDiagnoser]
    public class ZStringVsStringBuilderPools
    {
        [Params(1, 10, 100, 1000, 10000)]
        public int Count { get; set; }

        private ObjectPool<StringBuilder> StringBuilderPool { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var objectPoolProvider = new DefaultObjectPoolProvider();
            StringBuilderPool = objectPoolProvider.CreateStringBuilderPool();
        }

        [Benchmark(Baseline = true)]
        public void StringBuilderPools()
        {
            var Builder = StringBuilderPool.Get();
            for (var x = 0; x < Count; ++x)
            {
                Builder.Append("Testing this");
                Builder.AppendFormat(" out {0}", 12);
                Builder.AppendLine("Blah");
            }
            _ = Builder.ToString();
            StringBuilderPool.Return(Builder);
        }

        [Benchmark]
        public void StringConcat()
        {
            var Builder = string.Empty;
            for (var x = 0; x < Count; ++x)
            {
                Builder += "Testing this" + string.Format(" out {0}", 12) + "Blah";
            }
        }

        [Benchmark]
        public void ZStringUse()
        {
            using var Builder = ZString.CreateUtf8StringBuilder();
            for (var x = 0; x < Count; ++x)
            {
                Builder.Append("Testing this");
                Builder.AppendFormat(" out {0}", 12);
                Builder.AppendLine("Blah");
            }
            _ = Builder.ToString();
        }
    }
}