using BenchmarkDotNet.Running;

namespace SQLHelperDB.SpeedTests
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            new BenchmarkSwitcher(typeof(Program).Assembly).Run(args);
        }
    }
}