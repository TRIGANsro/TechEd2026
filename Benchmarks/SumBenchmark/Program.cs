using BenchmarkDotNet.Running;

namespace SumBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Sumovac>();
        }
    }
}
