using BenchmarkDotNet.Running;


namespace Benchmark1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Testovac>();
        }
    }
}
