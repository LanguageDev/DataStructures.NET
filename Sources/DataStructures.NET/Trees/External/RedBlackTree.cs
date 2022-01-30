// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DataStructures.NET.Trees.External;

/// <summary>
/// Operations on a Red-Black tree.
/// </summary>
public static class RedBlackTree
{
    /// <summary>
    /// Color of a R/B tree node.
    /// </summary>
    public enum Color : byte
    {
        /// <summary>
        /// Red node color.
        /// </summary>
        Red,

        /// <summary>
        /// Black node color.
        /// </summary>
        Black,
    }

    /// <summary>
    /// Color getter and setter functionality.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    public interface IColorSelector<TNode>
    {
        /// <summary>
        /// Accesses the color of a node.
        /// </summary>
        /// <param name="node">The node to get the color of.</param>
        /// <returns>The color of <paramref name="node"/>.</returns>
        public Color GetColor(TNode node);

        /// <summary>
        /// Sets the color of a node.
        /// </summary>
        /// <param name="node">The node to set the color of.</param>
        /// <param name="color">The color to set for <paramref name="node"/>.</param>
        public void SetColor(TNode node, Color color);
    }

    /// <summary>
    /// Performs an insertion on a R/B tree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TData">The inserted data type.</typeparam>
    /// <typeparam name="TKeyComparer">The key comparer type.</typeparam>
    /// <param name="root">The root of the R/B tree.</param>
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
                             IColorSelector<TNode>
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

        // Color inserted to red
        var node = insertion.Inserted;
        nodeAdapter.SetColor(node, Color.Red);

        // While there is a parent, we go up and rebalance
        while (true)
        {
            var parent = nodeAdapter.GetParent(node);
            if (parent is null) break;

            // Case I1: Parent is black
            if (nodeAdapter.GetColor(parent) == Color.Black) return new(Root: root, Inserted: insertion.Inserted);

            // Parent is red
            var grandparent = nodeAdapter.GetParent(parent);
            if (grandparent is null)
            {
                // Case I4
                nodeAdapter.SetColor(parent, Color.Black);
                return new(Root: root, Inserted: insertion.Inserted);
            }

            // Parent is red, grandparent is not null
            var grandparentLeft = nodeAdapter.GetLeftChild(grandparent);
            var isParentLeftChild = ReferenceEquals(grandparentLeft, parent);
            var uncle = isParentLeftChild
                ? nodeAdapter.GetRightChild(grandparent)
                : grandparentLeft;

            if (uncle is null || nodeAdapter.GetColor(uncle) == Color.Black)
            {
                // Parent is red, uncle is black
                // Case I56
                if (isParentLeftChild && ReferenceEquals(node, nodeAdapter.GetRightChild(parent)))
                {
                    // Case I5 (parent is red, uncle is black, node is the inner grandchild of grandparent)
                    BinarySearchTree.RotateLeft(parent, nodeAdapter);
                    // node = parent;
                    parent = nodeAdapter.GetLeftChild(grandparent)!;
                }
                else if (!isParentLeftChild && ReferenceEquals(node, nodeAdapter.GetLeftChild(parent)))
                {
                    // Case I5 (parent is red, uncle is black, node is the inner grandchild of grandparent)
                    BinarySearchTree.RotateRight(parent, nodeAdapter);
                    // node = parent;
                    parent = nodeAdapter.GetRightChild(grandparent)!;
                }
                // Case I6 (parent is red, uncle is black, node is outer granchild of grandparent)
                // Grandparent may be the root
                var rotateRoot = isParentLeftChild
                    ? BinarySearchTree.RotateRight(grandparent, nodeAdapter)
                    : BinarySearchTree.RotateLeft(grandparent, nodeAdapter);
                if (ReferenceEquals(root, grandparent)) root = rotateRoot;
                nodeAdapter.SetColor(parent, Color.Black);
                nodeAdapter.SetColor(grandparent, Color.Red);
                return new(Root: root, Inserted: insertion.Inserted);
            }

            // Case I2 (parent and uncle are red)
            nodeAdapter.SetColor(parent, Color.Black);
            nodeAdapter.SetColor(uncle, Color.Black);
            nodeAdapter.SetColor(grandparent, Color.Red);
            node = grandparent;
        }

        // Case I3
        return new(Root: root, Inserted: insertion.Inserted);
    }

    /// <summary>
    /// Deletes a given node from a R/B tree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="root">The root of the tree.</param>
    /// <param name="node">The node to delete.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The new root of the R/B tree.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNode? Delete<TNode, TNodeAdapter>(
        TNode? root,
        TNode node,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : BinarySearchTree.IChildSelector<TNode>,
                             BinarySearchTree.IParentSelector<TNode>,
                             IColorSelector<TNode>
    {
        var left = nodeAdapter.GetLeftChild(node);
        var right = nodeAdapter.GetRightChild(node);

        // If we are deleting the root, which has no non-null child, just empty the tree
        if (ReferenceEquals(root, node) && left is null &&right is null) return default;



        return root;
    }
}
