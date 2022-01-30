// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System.Collections.Generic;
using CodegenAnalysis;
using CodegenAnalysis.Benchmarks;
using DataStructures.NET.Trees.External;
using BstSet = DataStructures.NET.Trees.Linked.BinarySearchTreeSet<int, Benchmarks.Trees.BstCodegenBenchmarks.IntComparer>;

namespace Benchmarks.Trees;

[CAJob(Tier = CompilationTier.Tier1)]

[CAColumn(CAColumn.Branches),
 CAColumn(CAColumn.Calls),
 CAColumn(CAColumn.CodegenSize),
 CAColumn(CAColumn.StaticStackAllocations)]

[CAExport(Export.Html),
 CAExport(Export.Md)]
public class BstCodegenBenchmarks
{
    internal readonly struct IntComparer : IComparer<int>
    {
        public int Compare(int x, int y) => x - y;
    }

    [CAAnalyze(null!, 0)]
    internal static BinarySearchTree.SearchResult<BstSet.Node> Search(BstSet.Node? root, int k)
    {
        return BinarySearchTree.Search(
            root: root,
            nodeAdapter: default(BstSet.NodeAdapter),
            key: k,
            keyComparer: default(IntComparer));
    }

    [CAAnalyze(null!, 0)]
    internal static BinarySearchTree.InsertResult<BstSet.Node> Insert(BstSet.Node? root, int k)
    {
        return BinarySearchTree.Insert(
            root: root,
            nodeAdapter: default(BstSet.NodeAdapter),
            key: k,
            data: false,
            keyComparer: default(IntComparer));
    }

    [CAAnalyze(null!, null!)]
    internal static BstSet.Node? Delete(BstSet.Node? root, BstSet.Node node)
    {
        return BinarySearchTree.Delete(
            root: root,
            node: node,
            nodeAdapter: default(BstSet.NodeAdapter)).Root;
    }
}
