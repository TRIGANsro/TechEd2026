using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Benchmark1;

public class Testovac
{
    [Benchmark]
    public int TestAdd()
        { return 1+1; }
}
