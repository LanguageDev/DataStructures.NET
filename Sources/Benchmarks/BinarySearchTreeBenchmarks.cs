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

public class BinarySearchTreeBenchmarks
{
    public sealed class Node
    {
        public int Key { get; set; }
        public Node? Parent { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }
    }

    private readonly struct NodeAdapter :
        BinarySearchTree.INeighborSelector<Node>,
        BinarySearchTree.IKeySelector<Node, int>,
        BinarySearchTree.INodeBuilder<int, bool, Node>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Node? GetLeftChild(Node node) => node.Left;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Node? GetRightChild(Node node) => node.Right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Node? GetParent(Node node) => node.Parent;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetLeftChild(Node node, Node? child) => node.Left = child;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRightChild(Node node, Node? child) => node.Right = child;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetParent(Node node, Node? parent) => node.Parent = parent;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetKey(Node node) => node.Key;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Node Build(int key, bool data) => new() { Key = key };
    }

    private readonly struct IntComparer : IComparer<int>
    {
        public int Compare(int x, int y) => x - y;
    }

    private sealed class BstSet
    {
        private Node? root;

        public bool Contains(int n) => BinarySearchTree.Search(
            root: this.root,
            nodeAdapter: default(NodeAdapter),
            key: n,
            keyComparer: default(IntComparer)).Node is not null;

        public bool Add(int n)
        {
            var insert = BinarySearchTree.Insert(
                root: this.root,
                nodeAdapter: default(NodeAdapter),
                key: n,
                data: false,
                keyComparer: default(IntComparer));
            this.root = insert.Root;
            return insert.Inserted is not null;
        }

        public bool Remove(int n)
        {
            var node = BinarySearchTree.Search(
                root: this.root,
                nodeAdapter: default(NodeAdapter),
                key: n,
                keyComparer: default(IntComparer)).Node;
            if (node is null) return false;
            this.root = BinarySearchTree.Delete(
                root: this.root,
                node: node,
                nodeAdapter: default(NodeAdapter));
            return true;
        }
    }

    private sealed class BstSetWithIComparer
    {
        private Node? root;
        private IComparer<int> comparer = Comparer<int>.Default;

        public bool Contains(int n) => BinarySearchTree.Search(
            root: this.root,
            nodeAdapter: default(NodeAdapter),
            key: n,
            keyComparer: this.comparer).Node is not null;

        public bool Add(int n)
        {
            var insert = BinarySearchTree.Insert(
                root: this.root,
                nodeAdapter: default(NodeAdapter),
                key: n,
                data: false,
                keyComparer: this.comparer);
            this.root = insert.Root;
            return insert.Inserted is not null;
        }

        public bool Remove(int n)
        {
            var node = BinarySearchTree.Search(
                root: this.root,
                nodeAdapter: default(NodeAdapter),
                key: n,
                keyComparer: this.comparer).Node;
            if (node is null) return false;
            this.root = BinarySearchTree.Delete(
                root: this.root,
                node: node,
                nodeAdapter: default(NodeAdapter));
            return true;
        }
    }

    [Params(100, 10000, 100000)]
    public int ElementCount { get; set; }

    private List<int> numbersToAdd = new();
    private List<int> numbersToRemove = new();
    private BstSet ourSet = new();
    private BstSetWithIComparer ourIComparerSet = new();
    private BinaryTree.BinaryTree<int> marusykSet = new();
    private SchuchmannBst.BinarySearchTree<int> schuchmannSet = new();

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rnd = new Random(63463523);
        for (var i = 0; i < this.ElementCount; ++i)
        {
            this.numbersToAdd.Add(rnd.Next(this.ElementCount * 2));
            this.numbersToRemove.Add(this.ElementCount * 2);
        }
    }

    [IterationSetup]
    public void IterationSetup()
    {
        this.ourSet = new();
        this.ourIComparerSet = new();
        this.marusykSet = new();
        this.schuchmannSet = new();

        foreach (var item in this.numbersToAdd)
        {
            this.ourSet.Add(item);
            this.ourIComparerSet.Add(item);
            this.marusykSet.Add(item);
            this.schuchmannSet.Insert(item);
        }
    }
    
    [Benchmark]
    public void DataStructuresNET_BstAdd()
    {
        var bst = new BstSet();
        foreach (var item in this.numbersToAdd) bst.Add(item);
    }

    [Benchmark]
    public void DataStructuresNET_BstRemove()
    {
        foreach (var item in this.numbersToRemove) this.ourSet.Remove(item);
    }

    [Benchmark]
    public void DataStructuresNET_IComparerBstAdd()
    {
        var bst = new BstSetWithIComparer();
        foreach (var item in this.numbersToAdd) bst.Add(item);
    }

    [Benchmark]
    public void DataStructuresNET_IComparerBstRemove()
    {
        foreach (var item in this.numbersToRemove) this.ourIComparerSet.Remove(item);
    }

    [Benchmark]
    public void Marusyk_BinaryTreeAdd()
    {
        var bst = new BinaryTree.BinaryTree<int>();
        foreach (var item in this.numbersToAdd) bst.Add(item);
    }

    [Benchmark]
    public void Marusyk_BinaryTreeRemove()
    {
        foreach (var item in this.numbersToRemove) this.marusykSet.Remove(item);
    }

    [Benchmark]
    public void Schuchmann_BinarySearchTreeAdd()
    {
        var bst = new SchuchmannBst.BinarySearchTree<int>();
        foreach (var item in this.numbersToAdd) bst.Insert(item);
    }

    [Benchmark]
    public void Schuchmann_BinarySearchTreeRemove()
    {
        foreach (var item in this.numbersToRemove)
        {
            var n = this.schuchmannSet.Search(item);
            if (n is not null) this.schuchmannSet.DeleteNode(n);
        }
    }
}
