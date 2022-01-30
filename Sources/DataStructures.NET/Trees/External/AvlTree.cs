// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DataStructures.NET.Trees.External;

/// <summary>
/// Operations on an AVL tree.
/// </summary>
public static class AvlTree
{
    /// <summary>
    /// Height getter and setter functionality.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    public interface IHeightSelector<TNode>
    {
        /// <summary>
        /// Accesses the height of a node.
        /// </summary>
        /// <param name="node">The node to get the height of.</param>
        /// <returns>The height of <paramref name="node"/>.</returns>
        public int GetHeight(TNode node);

        /// <summary>
        /// Sets the height of a node.
        /// </summary>
        /// <param name="node">The node to set the height of.</param>
        /// <param name="height">The height to set for <paramref name="node"/>.</param>
        public void SetHeight(TNode node, int height);
    }

    /// <summary>
    /// Represents the results of rebalancing an AVL tree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="Root">The new root of the subtree that was rebalanced.</param>
    /// <param name="Rebalanced">True, if rebalancing did happen, false otherwise.</param>
    public readonly record struct RebalanceResult<TNode>(TNode Root, bool Rebalanced);

    /// <summary>
    /// Updates the height of a node based on its children.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="node">The node to update the height of.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UpdateHeight<TNode, TNodeAdapter>(
        TNode node,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : BinarySearchTree.IChildSelector<TNode>, IHeightSelector<TNode>
    {
        var nodeLeft = nodeAdapter.GetLeftChild(node);
        var nodeRight = nodeAdapter.GetRightChild(node);
        var leftHeight = nodeLeft is null ? 0 : nodeAdapter.GetHeight(nodeLeft);
        var rightHeight = nodeRight is null ? 0 : nodeAdapter.GetHeight(nodeRight);
        nodeAdapter.SetHeight(node, Math.Max(leftHeight, rightHeight) + 1);
    }

    /// <summary>
    /// Retrieves the balance factor of a node.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="node">The node to get the balance factor of.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The balance factor of <paramref name="node"/>, which is the height of its right child subtracted from
    /// the height of its left child.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BalanceFactor<TNode, TNodeAdapter>(
        TNode node,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : BinarySearchTree.IChildSelector<TNode>, IHeightSelector<TNode>
    {
        var left = nodeAdapter.GetLeftChild(node);
        var right = nodeAdapter.GetRightChild(node);
        var leftHeight = left is null ? 0 : nodeAdapter.GetHeight(left);
        var rightHeight = right is null ? 0 : nodeAdapter.GetHeight(right);
        return leftHeight - rightHeight;
    }

    /// <summary>
    /// Rotates the subtree left and updates the heights.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="root">The root of the subtree to rotate.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The new root of the subtree.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNode RotateLeft<TNode, TNodeAdapter>(
        TNode root,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : BinarySearchTree.IChildSelector<TNode>,
                             BinarySearchTree.IParentSelector<TNode>,
                             IHeightSelector<TNode>
    {
        root = BinarySearchTree.RotateLeft(root, nodeAdapter);
        UpdateHeight(nodeAdapter.GetLeftChild(root)!, nodeAdapter);
        UpdateHeight(root, nodeAdapter);
        return root;
    }

    /// <summary>
    /// Rotates the subtree right and updates the heights.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="root">The root of the subtree to rotate.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The new root of the subtree.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNode RotateRight<TNode, TNodeAdapter>(
        TNode root,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : BinarySearchTree.IChildSelector<TNode>,
                             BinarySearchTree.IParentSelector<TNode>,
                             IHeightSelector<TNode>
    {
        root = BinarySearchTree.RotateRight(root, nodeAdapter);
        UpdateHeight(nodeAdapter.GetRightChild(root)!, nodeAdapter);
        UpdateHeight(root, nodeAdapter);
        return root;
    }

    /// <summary>
    /// Performs a rebalancing step on an AVL node.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="root">The node to perform the rebalancing on.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The results of the rebalancing.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RebalanceResult<TNode> Rebalance<TNode, TNodeAdapter>(
        TNode root,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : BinarySearchTree.IChildSelector<TNode>,
                             BinarySearchTree.IParentSelector<TNode>,
                             IHeightSelector<TNode>
    {
        var bf = BalanceFactor(root, nodeAdapter);
        if (bf < -1)
        {
            // Right-left
            var rootRight = nodeAdapter.GetRightChild(root)!;
            if (BalanceFactor(rootRight, nodeAdapter) > 0) RotateRight(rootRight, nodeAdapter);
            // Right-right
            root = RotateLeft(root, nodeAdapter);
            return new(Root: root, Rebalanced: true);
        }
        else if (bf > 1)
        {
            // Left-right
            var rootLeft = nodeAdapter.GetLeftChild(root)!;
            if (BalanceFactor(rootLeft, nodeAdapter) < 0) RotateLeft(rootLeft, nodeAdapter);
            // Left-left
            root = RotateRight(root, nodeAdapter);
            return new(Root: root, Rebalanced: true);
        }
        return new(Root: root, Rebalanced: false);
    }

    /// <summary>
    /// Performs an insertion on an AVL tree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TData">The inserted data type.</typeparam>
    /// <typeparam name="TKeyComparer">The key comparer type.</typeparam>
    /// <param name="root">The root of the AVL tree.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <param name="key">The key to insert.</param>
    /// <param name="data">The data to insert.</param>
    /// <param name="keyComparer">The key comparer.</param>
    /// <returns>The results of the insertion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BinarySearchTree.InsertResult<TNode> Insert<TNode, TNodeAdapter, TKey, TData, TKeyComparer>(
        TNode? root,
        TNodeAdapter nodeAdapter,
        TKey key,
        TData data,
        TKeyComparer keyComparer)
        where TNodeAdapter : BinarySearchTree.IChildSelector<TNode>,
                             BinarySearchTree.IParentSelector<TNode>,
                             BinarySearchTree.IKeySelector<TNode, TKey>,
                             BinarySearchTree.INodeBuilder<TKey, TData, TNode>,
                             IHeightSelector<TNode>
        where TKeyComparer : IComparer<TKey>
    {
        // First we use the trivial BST insertion
        var insertion = BinarySearchTree.Insert(
            root: root,
            nodeAdapter: nodeAdapter,
            key: key,
            data: data,
            keyComparer: keyComparer);

        // If nothing is inserted, return here
        if (insertion.Inserted is null) return new(Root: root!, Existing: insertion.Existing);

        // There was a node inserted
        root = insertion.Root;

        // We start walking up the tree, updating the heights
        // If we find a node that needs rebalancing, we rebalance it, then stop
        for (var n = insertion.Inserted; n is not null; n = nodeAdapter.GetParent(n))
        {
            UpdateHeight(n, nodeAdapter);
            var rebalance = Rebalance(n, nodeAdapter);
            if (ReferenceEquals(n, root)) root = rebalance.Root;
            if (rebalance.Rebalanced) break;
            n = rebalance.Root;
        }

        // We are done
        return new(Root: root, Inserted: insertion.Inserted);
    }

    /// <summary>
    /// Deletes a given node from an AVL tree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="root">The root of the tree.</param>
    /// <param name="node">The node to delete.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The new root of the AVL tree.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNode? Delete<TNode, TNodeAdapter>(
        TNode? root,
        TNode node,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : BinarySearchTree.IChildSelector<TNode>,
                             BinarySearchTree.IParentSelector<TNode>,
                             IHeightSelector<TNode>
    {
        // NOTE: Mostly copied from BST.Delete

        // NOTE: Same as BST.Delete.Shift
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Shift(TNode u, TNode? v)
        {
            var uParent = nodeAdapter.GetParent(u);
            if (uParent is null) root = v;
            else if (ReferenceEquals(u, nodeAdapter.GetLeftChild(uParent))) nodeAdapter.SetLeftChild(uParent, v);
            else nodeAdapter.SetRightChild(uParent, v);
            if (v is not null) nodeAdapter.SetParent(v, uParent);
        }

        TNode? updateStart;
        if (nodeAdapter.GetLeftChild(node) is null)
        {
            // 0 or 1 child
            updateStart = nodeAdapter.GetParent(node);
            Shift(node, nodeAdapter.GetRightChild(node));
        }
        else if (nodeAdapter.GetRightChild(node) is null)
        {
            // 0 or 1 child
            updateStart = nodeAdapter.GetParent(node);
            Shift(node, nodeAdapter.GetLeftChild(node));
        }
        else
        {
            // 2 children
            var y = BinarySearchTree.Successor(node, nodeAdapter);
            var yParent = nodeAdapter.GetParent(y!);
            if (!ReferenceEquals(yParent, node))
            {
                updateStart = yParent;
                Shift(y!, nodeAdapter.GetRightChild(y!));
                var nodeRight = nodeAdapter.GetRightChild(node);
                nodeAdapter.SetRightChild(y!, nodeRight);
                if (nodeRight is not null) nodeAdapter.SetParent(nodeRight, y);
            }
            else
            {
                updateStart = y;
            }
            Shift(node, y);
            var nodeLeft = nodeAdapter.GetLeftChild(node);
            nodeAdapter.SetLeftChild(y!, nodeLeft);
            if (nodeLeft is not null) nodeAdapter.SetParent(nodeLeft, y);
        }

        // Rebalance upwards
        for (var n = updateStart; n is not null; n = nodeAdapter.GetParent(n!))
        {
            UpdateHeight(n, nodeAdapter);
            var rebalance = Rebalance(n, nodeAdapter);
            if (ReferenceEquals(n, root)) root = rebalance.Root;
            n = rebalance.Root;
        }

        return root;
    }
}
