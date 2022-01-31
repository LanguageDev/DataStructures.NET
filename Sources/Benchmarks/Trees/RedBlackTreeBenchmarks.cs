// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using DataStructures.NET.Trees.Linked;
using Towel;

namespace Benchmarks.Trees;

[MemoryDiagnoser]
public class RedBlackTreeInsertBenchmarks
{
    private readonly struct IntComparer : IComparer<int>, Towel.IFunc<int, int, Towel.CompareResult>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(int x, int y) => x - y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CompareResult Invoke(int arg1, int arg2)
        {
            if (arg1 < arg2) return CompareResult.Less;
            if (arg1 > arg2) return CompareResult.Greater;
            return CompareResult.Equal;
        }
    }

    [Params(100, 1000, 10000)]
    public int ElementCount { get; set; }

    private readonly List<int> numbers = new();

    private readonly RedBlackTreeSetLinked<int, IntComparer> rbSet = new(default);
    private readonly RedBlackTreeSetLinked<int, IComparer<int>> rbSetIComparer = new(Comparer<int>.Default);
    private readonly Towel.DataStructures.RedBlackTreeLinked<int, IntComparer> towelSet = new();
    private readonly SortedSet<int> bclSortedSet = new();

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rnd = new Random(63463523);
        for (var i = 0; i < this.ElementCount; ++i) this.numbers.Add(rnd.Next(this.ElementCount * 2));
    }

    [IterationSetup]
    public void IterationSetup()
    {
        this.rbSet.Clear();
        this.rbSetIComparer.Clear();
        this.towelSet.Clear();
        this.bclSortedSet.Clear();
    }
    
    [Benchmark]
    public void DataStructuresNET()
    {
        foreach (var item in this.numbers) this.rbSet.Add(item);
    }

    [Benchmark]
    public void DataStructuresNET_IComparer()
    {
        foreach (var item in this.numbers) this.rbSetIComparer.Add(item);
    }

    [Benchmark]
    public void Towel()
    {
        foreach (var item in this.numbers) this.towelSet.TryAdd(item);
    }

    [Benchmark]
    public void BclSortedSet()
    {
        foreach (var item in this.numbers) this.bclSortedSet.Add(item);
    }
}

[MemoryDiagnoser]
public class RedBlackTreeDeleteBenchmarks
{
    private readonly struct IntComparer : IComparer<int>, Towel.IFunc<int, int, Towel.CompareResult>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(int x, int y) => x - y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CompareResult Invoke(int arg1, int arg2)
        {
            if (arg1 < arg2) return CompareResult.Less;
            if (arg1 > arg2) return CompareResult.Greater;
            return CompareResult.Equal;
        }
    }

    [Params(100, 1000, 10000)]
    public int ElementCount { get; set; }

    private readonly List<int> numbersToAdd = new();
    private readonly List<int> numbersToRemove = new();

    private readonly RedBlackTreeSetLinked<int, IntComparer> rbSet = new(default);
    private readonly RedBlackTreeSetLinked<int, IComparer<int>> rbSetIComparer = new(Comparer<int>.Default);
    private readonly Towel.DataStructures.RedBlackTreeLinked<int, IntComparer> towelSet = new();
    private readonly SortedSet<int> bclSortedSet = new();

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
        this.rbSet.Clear();
        this.rbSetIComparer.Clear();
        this.towelSet.Clear();
        this.bclSortedSet.Clear();

        foreach (var item in this.numbersToAdd)
        {
            this.rbSet.Add(item);
            this.rbSetIComparer.Add(item);
            this.towelSet.TryAdd(item);
            this.bclSortedSet.Add(item);
        }
    }

    [Benchmark]
    public void DataStructuresNET()
    {
        foreach (var item in this.numbersToRemove) this.rbSet.Remove(item);
    }

    [Benchmark]
    public void DataStructuresNET_IComparer()
    {
        foreach (var item in this.numbersToRemove) this.rbSetIComparer.Remove(item);
    }

    [Benchmark]
    public void Towel()
    {
        foreach (var item in this.numbersToRemove) this.towelSet.TryRemove(item);
    }

    [Benchmark]
    public void BclSortedSet()
    {
        foreach (var item in this.numbersToRemove) this.bclSortedSet.Remove(item);
    }
}
