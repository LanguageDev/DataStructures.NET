// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using DataStructures.NET.Trees.Array;
using DataStructures.NET.Trees.Linked;

namespace Benchmarks.Trees;

[MemoryDiagnoser]
public class BstInsertBenchmarks
{
    private readonly struct IntComparer : IComparer<int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(int x, int y) => x - y;
    }

    [Params(100, 1000, 10000)]
    public int ElementCount { get; set; }

    private readonly List<int> numbers = new();

    private readonly BinarySearchTreeSetLinked<int, IntComparer> dsnSetLinked = new(default);
    private readonly BinarySearchTreeSetArray<int, IntComparer> dsnSetArray = new(default);
    private readonly BinarySearchTreeSetLinked<int, IComparer<int>> dsnSetLinkedIComparer = new(Comparer<int>.Default);
    private readonly BinaryTree.BinaryTree<int> marusykSet = new();
    private readonly SchuchmannBst.BinarySearchTree<int> schuchmannSet = new();

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rnd = new Random(63463523);
        for (var i = 0; i < this.ElementCount; ++i) this.numbers.Add(rnd.Next(this.ElementCount * 2));
    }

    [IterationSetup]
    public void IterationSetup()
    {
        this.dsnSetLinked.Clear();
        this.dsnSetArray.Clear();
        this.dsnSetLinkedIComparer.Clear();
        this.marusykSet.Clear();
        this.schuchmannSet.Root = null;
    }
    
    [Benchmark]
    public void DataStructuresNET_Linked()
    {
        foreach (var item in this.numbers) this.dsnSetLinked.Add(item);
    }

    [Benchmark]
    public void DataStructuresNET_Array()
    {
        foreach (var item in this.numbers) this.dsnSetArray.Add(item);
    }

    [Benchmark]
    public void DataStructuresNET_LinkedIComparer()
    {
        foreach (var item in this.numbers) this.dsnSetLinkedIComparer.Add(item);
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

[MemoryDiagnoser]
public class BstDeleteBenchmarks
{
    private readonly struct IntComparer : IComparer<int>
    {
        public int Compare(int x, int y) => x - y;
    }

    [Params(100, 1000, 10000)]
    public int ElementCount { get; set; }

    private readonly List<int> numbersToAdd = new();
    private readonly List<int> numbersToRemove = new();

    private readonly BinarySearchTreeSetLinked<int, IntComparer> dsnSetLinked = new(default);
    private readonly BinarySearchTreeSetArray<int, IntComparer> dsnSetArray = new(default);
    private readonly BinarySearchTreeSetLinked<int, IComparer<int>> dsnSetLinkedIComparer = new(Comparer<int>.Default);
    private readonly BinaryTree.BinaryTree<int> marusykSet = new();
    private readonly SchuchmannBst.BinarySearchTree<int> schuchmannSet = new();

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
        this.dsnSetLinked.Clear();
        this.dsnSetArray.Clear();
        this.dsnSetLinkedIComparer.Clear();
        this.marusykSet.Clear();
        this.schuchmannSet.Root = null;

        foreach (var item in this.numbersToAdd)
        {
            this.dsnSetLinked.Add(item);
            this.dsnSetArray.Add(item);
            this.dsnSetLinkedIComparer.Add(item);
            this.marusykSet.Add(item);
            this.schuchmannSet.Insert(item);
        }
    }

    [Benchmark]
    public void DataStructuresNET_Linked()
    {
        foreach (var item in this.numbersToRemove) this.dsnSetLinked.Remove(item);
    }

    [Benchmark]
    public void DataStructuresNET_Array()
    {
        foreach (var item in this.numbersToRemove) this.dsnSetArray.Remove(item);
    }

    [Benchmark]
    public void DataStructuresNET_LinkedIComparer()
    {
        foreach (var item in this.numbersToRemove) this.dsnSetLinkedIComparer.Remove(item);
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
