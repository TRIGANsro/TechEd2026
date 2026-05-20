using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq;

namespace SumBenchmark
{
    [SimpleJob(RuntimeMoniker.Net48, baseline: true)]
    [SimpleJob(RuntimeMoniker.Net10_0)]
    [AllStatisticsColumn]
    public class Sumovac
    {
        private int[] pole;

        [GlobalSetup]
        public void Setup()
        {
            Random random = new Random(42);
            pole = new int[1_000_000];
            for (int i = 0; i < pole.Length; i++)
            {
                pole[i] = random.Next(100);
            }
        }

        [Benchmark]
        public int Sum()
        {
            int sum = 0;

            foreach (var item in pole)
                sum += item;
            
            return sum;
        }

        [Benchmark]
        public int SumLinq()
        {
            int sum = pole.Sum();
            return sum;
        }

        [Benchmark]
        public int SumPLinq()
        {
            int sum = pole.AsParallel().Sum();
            return sum;
        }
    }
}
