using System.Collections.Generic;
using CodegenAnalysis;
using CodegenAnalysis.Benchmarks;
using DataStructures.NET;

CodegenBenchmarkRunner.Run<BinarySearchTreeCodegenBenchmarks>();

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
        public Node? Left { get; set; }
        public Node? Right { get; set; }
    }

    private readonly struct NodeAdapter :
        BinarySearchTree.IChildSelector<Node>,
        BinarySearchTree.IKeySelector<Node, int>
    {
        public Node? GetLeftChild(Node node) => node.Left;
        public Node? GetRightChild(Node node) => node.Right;
        public int GetKey(Node node) => node.Key;
    }

    private readonly struct IntComparer : IComparer<int>
    {
        public int Compare(int x, int y) => x - y;
    }

    [CAAnalyze(null, 0)]
    public static Node? Search(Node? root, int k)
    {
        var ins = BinarySearchTree.Search(
            root: root,
            nodeAdapter: default(NodeAdapter),
            key: k,
            keyComparer: default(IntComparer));
        return ins.Hint is null ? ins.Node : null;
    }
}
