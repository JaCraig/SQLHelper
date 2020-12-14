using BenchmarkDotNet.Attributes;
using System;
using System.Text.RegularExpressions;

namespace SQLHelper.SpeedTests.Tests
{
    [MemoryDiagnoser]
    public class RegexOrContains
    {
        private static Regex FirstPass { get; } = new Regex("(INSERT)|(UPDATE)|(DELETE)|(CREATE)|(ALTER)|(INTO)|(DROP)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex SecondPass { get; } = new Regex("(ALTER DATABASE)|(CREATE DATABASE)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private string QueryText { get; } = "IF NOT EXISTS (SELECT TOP 1 ID_ FROM [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade] WHERE [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_] = 6701 AND [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_] = 1341) BEGIN INSERT INTO [dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade]([dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[AllReferencesAndID_ID_],[dbo].[AllReferencesAndID_ManyToManyPropertiesWithCascade].[ManyToManyPropertiesWithCascade_ID_]) VALUES (6701,1341) END;";

        [Benchmark]
        public void Contains()
        {
            _ = (QueryText.Contains("INSERT", StringComparison.Ordinal)
                                            || QueryText.Contains("UPDATE", StringComparison.Ordinal)
                                            || QueryText.Contains("DELETE", StringComparison.Ordinal)
                                            || QueryText.Contains("CREATE", StringComparison.Ordinal)
                                            || QueryText.Contains("ALTER", StringComparison.Ordinal)
                                            || QueryText.Contains("INTO", StringComparison.Ordinal)
                                            || QueryText.Contains("DROP", StringComparison.Ordinal)
                                          && (!(QueryText.Contains("ALTER DATABASE", StringComparison.Ordinal)
                                            || QueryText.Contains("CREATE DATABASE", StringComparison.Ordinal))));
        }

        [Benchmark(Baseline = true)]
        public void ContainsIgnoreCase()
        {
            _ = (QueryText.Contains("INSERT", StringComparison.OrdinalIgnoreCase)
                                            || QueryText.Contains("UPDATE", StringComparison.OrdinalIgnoreCase)
                                            || QueryText.Contains("DELETE", StringComparison.OrdinalIgnoreCase)
                                            || QueryText.Contains("CREATE", StringComparison.OrdinalIgnoreCase)
                                            || QueryText.Contains("ALTER", StringComparison.OrdinalIgnoreCase)
                                            || QueryText.Contains("INTO", StringComparison.OrdinalIgnoreCase)
                                            || QueryText.Contains("DROP", StringComparison.OrdinalIgnoreCase))
                                          && (!(QueryText.Contains("ALTER DATABASE", StringComparison.OrdinalIgnoreCase)
                                            || QueryText.Contains("CREATE DATABASE", StringComparison.OrdinalIgnoreCase)));
        }

        [Benchmark]
        public void RegexCall()
        {
            _ = FirstPass.IsMatch(QueryText) && !SecondPass.IsMatch(QueryText);
        }
    }
}