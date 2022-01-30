using System;
using System.Collections.Generic;
using System.Diagnostics;
using DataStructures.NET.Trees.Linked;
using Fuzzer;
using BstSet = DataStructures.NET.Trees.Linked.BinarySearchTreeSet<int, System.Collections.Generic.IComparer<int>>;

internal class Program
{
    internal static void Main(string[] args)
    {
        try
        {
            SetFuzzer.FuzzRedBlackTree(100);
        }
        catch (FuzzerException f)
        {
            Console.WriteLine(f.Message);
        }
    }
}
