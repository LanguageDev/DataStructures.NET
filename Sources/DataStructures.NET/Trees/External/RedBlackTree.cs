// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        var nodeLeft = nodeAdapter.GetLeftChild(node);
        var nodeRight = nodeAdapter.GetRightChild(node);

        // If we are deleting a root that's the only element, the tree becomes empty, which is valid R/B
        if (ReferenceEquals(root, node) && nodeLeft is null && nodeRight is null) return default;

        if (nodeLeft is not null && nodeRight is not null)
        {
            // Successor has at most one non-null child
            var successor = BinarySearchTree.Successor(node, nodeAdapter)!;
            // We swap out the two nodes
            // Neighbors
            var successorParent = nodeAdapter.GetParent(successor);
            nodeAdapter.SetLeftChild(node, nodeAdapter.GetLeftChild(successor));
            nodeAdapter.SetRightChild(node, nodeAdapter.GetRightChild(successor));
            nodeAdapter.SetLeftChild(successor, nodeLeft);
            nodeAdapter.SetParent(successor, nodeAdapter.GetParent(node));
            if (ReferenceEquals(node, successorParent))
            {
                nodeAdapter.SetParent(node, successor);
                nodeAdapter.SetRightChild(successor, node);
            }
            else
            {
                nodeAdapter.SetParent(node, successorParent);
                nodeAdapter.SetRightChild(successor, nodeRight);
            }
            // Colors
            var nodeColor = nodeAdapter.GetColor(node);
            nodeAdapter.SetColor(node, nodeAdapter.GetColor(successor));
            nodeAdapter.SetColor(successor, nodeColor);
            // Check for root
            if (ReferenceEquals(node, root)) root = successor;
        }

        nodeLeft = nodeAdapter.GetLeftChild(node);
        nodeRight = nodeAdapter.GetRightChild(node);
        var nodeParent = nodeAdapter.GetParent(node)!;
        var nodeIsLeftChild = ReferenceEquals(nodeAdapter.GetLeftChild(nodeParent), node);
        var child = nodeLeft ?? nodeRight;
        if (nodeAdapter.GetColor(node) == Color.Red || child is not null)
        {
            // A red node must not have children here, we can simply remove
            // If there is a child, it must be red
            // We can simply paint that black and make it the child
            if (nodeIsLeftChild) nodeAdapter.SetLeftChild(nodeParent, child);
            else nodeAdapter.SetRightChild(nodeParent, child);
            // If there was a child, we paint it black
            if (child is not null)
            {
                nodeAdapter.SetParent(child, nodeParent);
                nodeAdapter.SetColor(child, Color.Black);
            }
            // We are done
            return root;
        }

        // The hard case, non-root black node and only null children
        Debug.Assert(nodeLeft is null && nodeRight is null && nodeAdapter.GetColor(node) == Color.Black);

        // Replace the parent pointer to this to null
        if (nodeIsLeftChild) nodeAdapter.SetLeftChild(nodeParent, default);
        else nodeAdapter.SetRightChild(nodeParent, default);

        TNode? sibling, closeNephew, distantNephew;
        TNode rotationRoot;
        while (true)
        {
            sibling = nodeIsLeftChild
                ? nodeAdapter.GetRightChild(nodeParent)
                : nodeAdapter.GetLeftChild(nodeParent);
            var (dn, cn) = nodeIsLeftChild
                ? (nodeAdapter.GetRightChild(sibling!), nodeAdapter.GetLeftChild(sibling!))
                : (nodeAdapter.GetLeftChild(sibling!), nodeAdapter.GetRightChild(sibling!));
            distantNephew = dn;
            closeNephew = cn;
            if (nodeAdapter.GetColor(sibling!) == Color.Red) goto Case_D3;

            // Sibling is black
            if (distantNephew is not null && nodeAdapter.GetColor(distantNephew) == Color.Red) goto Case_D6;
            if (closeNephew is not null && nodeAdapter.GetColor(closeNephew) == Color.Red) goto Case_D5;

            // Both nephews are null
            if (nodeAdapter.GetColor(nodeParent) == Color.Red) goto Case_D4;

            // Case D1 (parent, close nephew, distant nephew and sibling are black)
            nodeAdapter.SetColor(sibling!, Color.Red);

            // Update for next iteration level
            node = nodeParent;
            nodeParent = nodeAdapter.GetParent(node);
            if (nodeParent is null) break;
            nodeIsLeftChild = ReferenceEquals(nodeAdapter.GetLeftChild(nodeParent), node);
        }

        // Delete case 2
        return root;

    Case_D3: // Sibling is red, parent, close- and distant nephew are black
        rotationRoot = nodeIsLeftChild
            ? BinarySearchTree.RotateLeft(nodeParent, nodeAdapter)
            : BinarySearchTree.RotateRight(nodeParent, nodeAdapter);
        if (ReferenceEquals(root, nodeParent)) root = rotationRoot;
        nodeAdapter.SetColor(nodeParent, Color.Red);
        nodeAdapter.SetColor(sibling!, Color.Black);
        sibling = closeNephew;

        // Parent is red, sibling is black
        distantNephew = nodeIsLeftChild
            ? nodeAdapter.GetRightChild(sibling!)
            : nodeAdapter.GetLeftChild(sibling!);
        if (distantNephew is not null && nodeAdapter.GetColor(distantNephew) == Color.Red) goto Case_D6;

        closeNephew = nodeIsLeftChild
            ? nodeAdapter.GetLeftChild(sibling!)
            : nodeAdapter.GetRightChild(sibling!);
        if (closeNephew is not null && nodeAdapter.GetColor(closeNephew) == Color.Red) goto Case_D5;

        // Close and distant nephew are black
    Case_D4:
        nodeAdapter.SetColor(sibling!, Color.Red);
        nodeAdapter.SetColor(nodeParent, Color.Black);
        return root;

    Case_D5:
        // Sibling can't be the root
        if (nodeIsLeftChild) BinarySearchTree.RotateRight(sibling!, nodeAdapter);
        else BinarySearchTree.RotateLeft(sibling!, nodeAdapter);
        nodeAdapter.SetColor(sibling!, Color.Red);
        nodeAdapter.SetColor(closeNephew!, Color.Black);
        distantNephew = sibling;
        sibling = closeNephew;

    Case_D6:
        rotationRoot = nodeIsLeftChild
            ? BinarySearchTree.RotateLeft(nodeParent, nodeAdapter)
            : BinarySearchTree.RotateRight(nodeParent, nodeAdapter);
        if (ReferenceEquals(root, nodeParent)) root = rotationRoot;
        nodeAdapter.SetColor(sibling!, nodeAdapter.GetColor(nodeParent));
        nodeAdapter.SetColor(nodeParent, Color.Black);
        nodeAdapter.SetColor(distantNephew!, Color.Black);
        return root;
    }
}
