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

    [Params(100, 1000, 10000)]
    public int ElementCount { get; set; }

    private List<int> numbers = new();
    private BstSet ourSet = new();
    private BinaryTree.BinaryTree<int> marusykSet = new();

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(63463523);
        for (var i = 0; i < this.ElementCount; ++i)
        {
            this.numbers.Add(rnd.Next(this.ElementCount * 2));
            var toRemove = rnd.Next(this.ElementCount * 2);
            this.ourSet.Add(toRemove);
            this.marusykSet.Add(toRemove);
        }
    }

    [Benchmark]
    public void DataStructuresNET_BstAdd()
    {
        var bst = new BstSet();
        foreach (var item in this.numbers) bst.Add(item);
    }

    [Benchmark]
    public void DataStructuresNET_BstRemove()
    {
        foreach (var item in this.numbers) this.ourSet.Remove(item);
    }

    [Benchmark]
    public void Marusyk_BinaryTreeAdd()
    {
        var bst = new BinaryTree.BinaryTree<int>();
        foreach (var item in this.numbers) bst.Add(item);
    }

    [Benchmark]
    public void Marusyk_BinaryTreeRemove()
    {
        foreach (var item in this.numbers) this.marusykSet.Remove(item);
    }
}
