using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace StopwatchDemo
{
    public class Testovac
    {
        [Benchmark]
        public int TestAdd()
            { return 1+1; }
    }
}
