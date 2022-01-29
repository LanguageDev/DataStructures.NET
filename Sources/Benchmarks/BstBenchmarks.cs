// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using DataStructures.NET;

namespace Benchmarks;

public class BstInsertBenchmarks
{
    private readonly struct IntComparer : IComparer<int>
    {
        public int Compare(int x, int y) => x - y;
    }

    [Params(100, 1000, 10000)]
    public int ElementCount { get; set; }

    private List<int> numbers = new();

    private BinarySearchTreeSet<int, IntComparer> dsnSet = new(default);
    private BinarySearchTreeSet<int, IComparer<int>> dsnSetIComparer = new(Comparer<int>.Default);
    private BinaryTree.BinaryTree<int> marusykSet = new();
    private SchuchmannBst.BinarySearchTree<int> schuchmannSet = new();

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rnd = new Random(63463523);
        for (var i = 0; i < this.ElementCount; ++i) this.numbers.Add(rnd.Next(this.ElementCount * 2));
    }

    [IterationSetup]
    public void IterationSetup()
    {
        this.dsnSet.Clear();
        this.dsnSetIComparer.Clear();
        this.marusykSet.Clear();
        this.schuchmannSet.Root = null;
    }
    
    [Benchmark]
    public void DataStructuresNET()
    {
        foreach (var item in this.numbers) this.dsnSet.Add(item);
    }

    [Benchmark]
    public void DataStructuresNET_IComparer()
    {
        foreach (var item in this.numbers) this.dsnSetIComparer.Add(item);
    }

    [Benchmark]
    public void Marusyk()
    {
        foreach (var item in this.numbers) this.marusykSet.Add(item);
    }

    [Benchmark]
    public void Schuchmann()
    {
        foreach (var item in this.numbers) this.schuchmannSet.Insert(item);
    }
}

public class BstDeleteBenchmarks
{
    private readonly struct IntComparer : IComparer<int>
    {
        public int Compare(int x, int y) => x - y;
    }

    [Params(100, 1000, 10000)]
    public int ElementCount { get; set; }

    private List<int> numbersToAdd = new();
    private List<int> numbersToRemove = new();

    private BinarySearchTreeSet<int, IntComparer> dsnSet = new(default);
    private BinarySearchTreeSet<int, IComparer<int>> dsnSetIComparer = new(Comparer<int>.Default);
    private BinaryTree.BinaryTree<int> marusykSet = new();
    private SchuchmannBst.BinarySearchTree<int> schuchmannSet = new();

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rnd = new Random(63463523);
        for (var i = 0; i < this.ElementCount; ++i)
        {
            this.numbersToAdd.Add(rnd.Next(this.ElementCount * 2));
            this.numbersToRemove.Add(rnd.Next(this.ElementCount * 2));
        }
    }

    [IterationSetup]
    public void IterationSetup()
    {
        this.dsnSet.Clear();
        this.dsnSetIComparer.Clear();
        this.marusykSet.Clear();
        this.schuchmannSet.Root = null;

        foreach (var item in this.numbersToAdd)
        {
            this.dsnSet.Add(item);
            this.dsnSetIComparer.Add(item);
            this.marusykSet.Add(item);
            this.schuchmannSet.Insert(item);
        }
    }

    [Benchmark]
    public void DataStructuresNET()
    {
        foreach (var item in this.numbersToRemove) this.dsnSet.Remove(item);
    }

    [Benchmark]
    public void DataStructuresNET_IComparer()
    {
        foreach (var item in this.numbersToRemove) this.dsnSetIComparer.Remove(item);
    }

    [Benchmark]
    public void Marusyk()
    {
        foreach (var item in this.numbersToRemove) this.marusykSet.Remove(item);
    }

    [Benchmark]
    public void Schuchmann()
    {
        foreach (var item in this.numbersToRemove)
        {
            var n = this.schuchmannSet.Search(item);
            if (n is not null) this.schuchmannSet.DeleteNode(n);
        }
    }
}
