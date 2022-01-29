// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataStructures.NET.Trees.External;

namespace Fuzzer;

public static class TreeValidation
{
    public static string ToTestCaseString<TNode, TNodeAdapter>(
        TNode? root,
        TNodeAdapter nodeAdapter,
        Func<TNode, string> nodeCtor)
        where TNodeAdapter : BinarySearchTree.IChildSelector<TNode>,
                             BinarySearchTree.IParentSelector<TNode>
    {
        var builder = new StringBuilder();

        void Impl(TNode? node)
        {
            if (node is null) return;
            builder.Append("new(").Append(nodeCtor(node)).Append(')');
            var left = nodeAdapter.GetLeftChild(node);
            var right = nodeAdapter.GetRightChild(node);
            if (left is not null || right is not null)
            {
                builder.Append(" { ");
                if (left is not null)
                {
                    builder.Append("Left = ");
                    Impl(left);
                    builder.Append(", ");
                }
                if (right is not null)
                {
                    builder.Append("Right = ");
                    Impl(right);
                    builder.Append(", ");
                }
                builder.Append('}');
            }
        }

        Impl(root);
        return builder.ToString();
    }

    public static void ValidateAdjacency<TNode, TNodeAdapter>(
        TNode? root,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : BinarySearchTree.IChildSelector<TNode>,
                             BinarySearchTree.IParentSelector<TNode>
    {
        void Impl(TNode? node)
        {
            // An empty subtree is always valid
            if (node is null) return;

            var left = nodeAdapter.GetLeftChild(node);
            var right = nodeAdapter.GetRightChild(node);
            if (left is not null)
            {
                // If there is a left child, its parent has to be this node
                var leftParent = nodeAdapter.GetParent(left);
                if (!ReferenceEquals(leftParent, node)) throw new ValidationException("Adjacency error: The left node's parent is not the node");
                // Recursively validate
                Impl(left);
            }
            if (right is not null)
            {
                // If there is a right child, its parent has to be this node
                var rightParent = nodeAdapter.GetParent(right);
                if (!ReferenceEquals(rightParent, node)) throw new ValidationException("Adjacency error: The right node's parent is not the node");
                // Recursively validate
                Impl(right);
            }
        }

        // An empty tree is always valid
        if (root is null) return;

        // The parent of a root node must always be valid
        if (nodeAdapter.GetParent(root) is not null) throw new ValidationException("Adjacency error: The parent of root is not null");

        // Recursively validate
        Impl(root);
    }

    public static void ValidateData<TNode, TNodeAdapter, TData>(
        TNode? root,
        TNodeAdapter nodeAdapter,
        Func<TNode, TData> dataSelector,
        IEnumerable<TData> expected)
        where TNodeAdapter : BinarySearchTree.IChildSelector<TNode>
    {
        // Keep the remaining elements in a set
        var remaining = expected.ToHashSet();

        void Impl(TNode? node)
        {
            // No more data in this subtree
            if (node is null) return;
            // The remaining set must have had this data, otherwise the tree contained something extra
            var data = dataSelector(node);
            if (!remaining!.Remove(data)) throw new ValidationException($"Content error: The element {data} was not expected to be present in the tree");
            // Recursively remove from left and right subtree
            Impl(nodeAdapter.GetLeftChild(node));
            Impl(nodeAdapter.GetRightChild(node));
        }

        // Recursively remove elements from the remaining set
        Impl(root);
        // If the remaining wasn't empty, the didn't contain something we expected to contain
        if (remaining.Count > 0) throw new ValidationException($"Content error: The elements [{string.Join(", ", remaining)}] were not found in the tree, but were expected");
    }

    public static void ValidateBalanceAndHeight<TNode, TNodeAdapter>(
        TNode? root,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : BinarySearchTree.IChildSelector<TNode>,
                             AvlTree.IHeightSelector<TNode>
    {
        int Impl(TNode? node)
        {
            if (node is null) return 0;

            var left = nodeAdapter.GetLeftChild(node);
            var right = nodeAdapter.GetRightChild(node);

            var leftExpHeight = Impl(left);
            var leftActHeight = left is null ? 0 : nodeAdapter.GetHeight(left);
            if (leftExpHeight != leftActHeight) throw new ValidationException($"Height error: The left node's height ({leftActHeight}) does not match the expected ({leftExpHeight})");

            var rightExpHeight = Impl(right);
            var rightActHeight = right is null ? 0 : nodeAdapter.GetHeight(right);
            if (rightExpHeight != rightActHeight) throw new ValidationException($"Height error: The right node's height ({rightActHeight}) does not match the expected ({rightExpHeight})");

            var balance = leftActHeight - rightActHeight;
            if (!(-1 <= balance && balance <= 1)) throw new ValidationException($"Balance error: The node is unbalanced (balance factor {balance})");

            return 1 + Math.Max(leftActHeight, rightActHeight);
        }

        var rootExpHeight = Impl(root);
        var rootActHeight = root is null ? 0 : nodeAdapter.GetHeight(root);
        if (rootExpHeight != rootActHeight) throw new ValidationException($"Height error: The root's height ({rootActHeight}) does not match the expected ({rootExpHeight})");
    }

    public static void AssertTreeEquals<TNode, TNodeAdapter>(
        TNode? root1,
        TNode? root2,
        TNodeAdapter nodeAdapter,
        Func<TNode, TNode, bool> nodeEquals)
        where TNodeAdapter : BinarySearchTree.IChildSelector<TNode>
    {
        if (ReferenceEquals(root1, root2)) return;
        if (root1 is null || root2 is null) throw new ValidationException($"Equality error: The left or right subtree terminated early");
        if (!nodeEquals(root1!, root2!)) throw new ValidationException($"Equality error: The contents of the nodes are not equal");

        var root1Left = nodeAdapter.GetLeftChild(root1!);
        var root2Left = nodeAdapter.GetLeftChild(root2!);
        var root1Right = nodeAdapter.GetRightChild(root1!);
        var root2Right = nodeAdapter.GetRightChild(root2!);

        AssertTreeEquals(root1Left, root2Left, nodeAdapter, nodeEquals);
        AssertTreeEquals(root1Right, root2Right, nodeAdapter, nodeEquals);
    }
}
