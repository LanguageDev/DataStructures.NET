using System;
using System.Collections.Generic;
using System.Diagnostics;
using DataStructures.NET.Trees.Linked;
using Fuzzer;
using BstSet = DataStructures.NET.Trees.Linked.BinarySearchTreeSetLinked<int, System.Collections.Generic.IComparer<int>>;

internal class Program
{
    internal static void Main(string[] args)
    {
        try
        {
            //SetFuzzer.FuzzBinarySearchTreeSetLinked(100);
            //SetFuzzer.FuzzAvlTreeSetLinked(100);
            //SetFuzzer.FuzzRedBlackTreeSetLinked(100);
            SetFuzzer.FuzzBinarySearchTreeSetArray(100);
        }
        catch (FuzzerException f)
        {
            Console.WriteLine(f.Message);
        }
    }
}
