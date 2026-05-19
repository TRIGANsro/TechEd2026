using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Console;

namespace Rychlost;

class Program
{
    private const int POCET = 1_000_000;
    private const int HLEDEJ = 750_000;

    static void Main(string[] args)
    {
        Stopwatch stopky = new Stopwatch();

        List<int> listInt = new List<int>(POCET);
        HashSet<int> hashSet = new HashSet<int>(POCET);

        stopky.Start();
        
        for (int i = 0; i < POCET; i++)
        {
            hashSet.Add(i);
        }
        stopky.Stop();

        WriteLine("Plneni List<int>:");
        WriteLine(stopky.ElapsedTicks);

        stopky.Restart();
        for (int i = 0; i < POCET; i++)
        {
            listInt.Add(i);
        }

        stopky.Stop();
        WriteLine("Plneni HashSet<int>:");
        WriteLine(stopky.ElapsedTicks);

        WriteLine($"Hledani {HLEDEJ} v List<int>");
        stopky.Restart();
        listInt.IndexOf(HLEDEJ);
        stopky.Stop();

        WriteLine(stopky.ElapsedTicks);

        stopky.Restart();
        listInt.IndexOf(HLEDEJ + 1);
        stopky.Stop();

        WriteLine(stopky.ElapsedTicks);

        stopky.Restart();
        listInt.IndexOf(HLEDEJ + 2);
        stopky.Stop();

        WriteLine(stopky.ElapsedTicks);

        WriteLine($"Hledani {HLEDEJ} v HashSet<int>");
        stopky.Restart();
        hashSet.Contains(HLEDEJ);
        stopky.Stop();

        WriteLine(stopky.ElapsedTicks);

        stopky.Restart();
        hashSet.Contains(HLEDEJ + 1);
        stopky.Stop();

        WriteLine(stopky.ElapsedTicks);

        stopky.Restart();
        hashSet.Contains(HLEDEJ + 2);
        stopky.Stop();

        WriteLine(stopky.ElapsedTicks);

        WriteLine("HOTOVO A");
        //Sortovanie(1_000);
        //Sortovanie(1_000);
        //Sortovanie(1_000);
        //Sortovanie(10_000);
        //Sortovanie(10_000);
        //Sortovanie(10_000);
        //Sortovanie(100_000);
        //Sortovanie(100_000);
        //Sortovanie(100_000);

    }

    private static void Sortovanie(int pocet)
    {
        Stopwatch stopky = new Stopwatch();
        int[] pole = new int[pocet];

        Random random = new Random();

        for (int i = 0; i < pocet; i++)
        {
            pole[i] = random.Next(pocet);
        }

        stopky.Restart();
        Array.Sort(pole);
        stopky.Stop();

        WriteLine("Pole velke " + pocet + ":" + stopky.ElapsedTicks);
    }
}
