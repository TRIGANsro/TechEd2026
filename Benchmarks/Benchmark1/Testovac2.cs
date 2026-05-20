using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Benchmark1;

[MemoryDiagnoser]
[AllStatisticsColumn]
public class Testovac2
{


    private int[][] batches = null!;
    private int index;

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(42);

        batches = Enumerable.Range(0, 128)
            .Select(i =>
            {
                int expensiveCount = i < 96 ? 10 : 2_000; // poslední čtvrtina výrazně dražší

                var data = new int[20_000];

                for (int j = 0; j < data.Length; j++)
                    data[j] = j < expensiveCount ? rnd.Next(10_000, 100_000) : 1;

                rnd.Shuffle(data);

                return data;
            })
            .ToArray();
    }

    [Benchmark]
    public long VariableWorkload()
    {
        var data = batches[index++ & 127];

        long sum = 0;

        foreach (var value in data)
        {
            if (value == 1)
            {
                sum += value;
            }
            else
            {
                for (int i = 0; i < 20000; i++)
                    sum += (value % 97) * i;
            }
        }

        return sum;
    }

}
