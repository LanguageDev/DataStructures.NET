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
using Towel;

namespace Benchmarks;

[MemoryDiagnoser]
public class AvlTreeInsertBenchmarks
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

    private readonly BinarySearchTreeSet<int, IntComparer> avlSet = new(default);
    private readonly BinarySearchTreeSet<int, IComparer<int>> avlSetIComparer = new(Comparer<int>.Default);
    private readonly Towel.DataStructures.AvlTreeLinked<int, IntComparer> towelSet = new();

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rnd = new Random(63463523);
        for (var i = 0; i < this.ElementCount; ++i) this.numbers.Add(rnd.Next(this.ElementCount * 2));
    }

    [IterationSetup]
    public void IterationSetup()
    {
        this.avlSet.Clear();
        this.avlSetIComparer.Clear();
        this.towelSet.Clear();
    }
    
    [Benchmark]
    public void DataStructuresNET()
    {
        foreach (var item in this.numbers) this.avlSet.Add(item);
    }

    [Benchmark]
    public void DataStructuresNET_IComparer()
    {
        foreach (var item in this.numbers) this.avlSetIComparer.Add(item);
    }

    [Benchmark]
    public void Towel()
    {
        foreach (var item in this.numbers) this.towelSet.TryAdd(item);
    }
}

[MemoryDiagnoser]
public class AvlTreeDeleteBenchmarks
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

    private readonly BinarySearchTreeSet<int, IntComparer> avlSet = new(default);
    private readonly BinarySearchTreeSet<int, IComparer<int>> avlSetIComparer = new(Comparer<int>.Default);
    private readonly Towel.DataStructures.AvlTreeLinked<int, IntComparer> towelSet = new();

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
        this.avlSet.Clear();
        this.avlSetIComparer.Clear();
        this.towelSet.Clear();

        foreach (var item in this.numbersToAdd)
        {
            this.avlSet.Add(item);
            this.avlSetIComparer.Add(item);
            this.towelSet.TryAdd(item);
        }
    }

    [Benchmark]
    public void DataStructuresNET()
    {
        foreach (var item in this.numbersToRemove) this.avlSet.Remove(item);
    }

    [Benchmark]
    public void DataStructuresNET_IComparer()
    {
        foreach (var item in this.numbersToRemove) this.avlSetIComparer.Remove(item);
    }

    [Benchmark]
    public void Towel()
    {
        foreach (var item in this.numbersToRemove) this.towelSet.TryRemove(item);
    }
}
