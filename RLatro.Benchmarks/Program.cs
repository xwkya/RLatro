using BenchmarkDotNet.Running;
using RLatro.Benchmarks.CoreRules;

namespace RLatro.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting benchmarks...");
            var summary = BenchmarkRunner.Run<HandRankGetterBenchmarks>();

            Console.WriteLine("Benchmarking complete.");
        }
    }
}