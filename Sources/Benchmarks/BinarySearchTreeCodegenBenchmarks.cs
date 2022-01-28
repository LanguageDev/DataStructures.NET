// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CodegenAnalysis;
using CodegenAnalysis.Benchmarks;
using DataStructures.NET;

namespace Benchmarks;

[CAJob(Tier = CompilationTier.Tier1)]

[CAColumn(CAColumn.Branches),
 CAColumn(CAColumn.Calls),
 CAColumn(CAColumn.CodegenSize),
 CAColumn(CAColumn.StaticStackAllocations)]

[CAExport(Export.Html),
 CAExport(Export.Md)]
public class BinarySearchTreeCodegenBenchmarks
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

    [CAAnalyze(null, 0)]
    public static BinarySearchTree.SearchResult<Node> Search(Node? root, int k)
    {
        return BinarySearchTree.Search(
            root: root,
            nodeAdapter: default(NodeAdapter),
            key: k,
            keyComparer: default(IntComparer));
    }

    [CAAnalyze(null, 0)]
    public static BinarySearchTree.InsertResult<Node> Insert(Node? root, int k)
    {
        return BinarySearchTree.Insert(
            root: root,
            nodeAdapter: default(NodeAdapter),
            key: k,
            data: false,
            keyComparer: default(IntComparer));
    }

    [CAAnalyze(null, null)]
    public static Node? Delete(Node? root, Node node)
    {
        return BinarySearchTree.Delete(
            root: root,
            node: node,
            nodeAdapter: default(NodeAdapter));
    }
}
