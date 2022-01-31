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
        where TNodeAdapter : BinarySearchTree.INodeIdentity<TNode>,
                             BinarySearchTree.IChildSelector<TNode>,
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
        if (nodeAdapter.IsNil(insertion.Inserted)) return new(Root: root!, Existing: insertion.Existing, Inserted: nodeAdapter.NilNode);

        // There was a node inserted
        root = insertion.Root;

        // Color inserted to red
        var node = insertion.Inserted;
        nodeAdapter.SetColor(node, Color.Red);

        // While there is a parent, we go up and rebalance
        while (true)
        {
            var parent = nodeAdapter.GetParent(node);
            if (nodeAdapter.IsNil(parent)) break;

            // Case I1: Parent is black
            if (nodeAdapter.GetColor(parent) == Color.Black) return new(Root: root, Inserted: insertion.Inserted, Existing: nodeAdapter.NilNode);

            // Parent is red
            var grandparent = nodeAdapter.GetParent(parent);
            if (nodeAdapter.IsNil(grandparent))
            {
                // Case I4
                nodeAdapter.SetColor(parent, Color.Black);
                return new(Root: root, Inserted: insertion.Inserted, Existing: nodeAdapter.NilNode);
            }

            // Parent is red, grandparent is not null
            var grandparentLeft = nodeAdapter.GetLeftChild(grandparent);
            var isParentLeftChild = nodeAdapter.NodeEquals(grandparentLeft, parent);
            var uncle = isParentLeftChild
                ? nodeAdapter.GetRightChild(grandparent)
                : grandparentLeft;

            if (nodeAdapter.IsNil(uncle) || nodeAdapter.GetColor(uncle) == Color.Black)
            {
                // Parent is red, uncle is black
                // Case I56
                if (isParentLeftChild && nodeAdapter.NodeEquals(node, nodeAdapter.GetRightChild(parent)))
                {
                    // Case I5 (parent is red, uncle is black, node is the inner grandchild of grandparent)
                    BinarySearchTree.RotateLeft(parent, nodeAdapter);
                    // node = parent;
                    parent = nodeAdapter.GetLeftChild(grandparent)!;
                }
                else if (!isParentLeftChild && nodeAdapter.NodeEquals(node, nodeAdapter.GetLeftChild(parent)))
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
                if (nodeAdapter.NodeEquals(root, grandparent)) root = rotateRoot;
                nodeAdapter.SetColor(parent, Color.Black);
                nodeAdapter.SetColor(grandparent, Color.Red);
                return new(Root: root, Inserted: insertion.Inserted, Existing: nodeAdapter.NilNode);
            }

            // Case I2 (parent and uncle are red)
            nodeAdapter.SetColor(parent, Color.Black);
            nodeAdapter.SetColor(uncle, Color.Black);
            nodeAdapter.SetColor(grandparent, Color.Red);
            node = grandparent;
        }

        // Case I3
        return new(Root: root, Inserted: insertion.Inserted, Existing: nodeAdapter.NilNode);
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
        where TNodeAdapter : BinarySearchTree.INodeIdentity<TNode>,
                             BinarySearchTree.IChildSelector<TNode>,
                             BinarySearchTree.IParentSelector<TNode>,
                             IColorSelector<TNode>
    {
        var nodeLeft = nodeAdapter.GetLeftChild(node);
        var nodeRight = nodeAdapter.GetRightChild(node);
        var nodeParent = nodeAdapter.GetParent(node);

        // If we are deleting a root that's the only element, the tree becomes empty, which is valid R/B
        if (nodeAdapter.NodeEquals(root, node) && nodeAdapter.IsNil(nodeLeft) && nodeAdapter.IsNil(nodeRight)) return nodeAdapter.NilNode;

        if (nodeAdapter.IsNotNil(nodeLeft) && nodeAdapter.IsNotNil(nodeRight))
        {
            // TODO: There has to be a nicer way...
            // Successor has at most one non-null child
            var successor = BinarySearchTree.InOrderSuccessor(node, nodeAdapter)!;
            // We swap out the two nodes
            // Neighbors
            var successorLeft = nodeAdapter.GetLeftChild(successor);
            var successorRight = nodeAdapter.GetRightChild(successor);
            var successorParent = nodeAdapter.GetParent(successor);
            nodeAdapter.SetLeftChild(node, successorLeft);
            if (nodeAdapter.IsNotNil(successorLeft)) nodeAdapter.SetParent(successorLeft, node);
            nodeAdapter.SetRightChild(node, successorRight);
            if (nodeAdapter.IsNotNil(successorRight)) nodeAdapter.SetParent(successorRight, node);
            nodeAdapter.SetLeftChild(successor, nodeLeft);
            if (nodeAdapter.IsNotNil(nodeLeft)) nodeAdapter.SetParent(nodeLeft, successor);
            nodeAdapter.SetParent(successor, nodeParent);
            if (nodeAdapter.IsNotNil(nodeParent))
            {
                var nodeParentLeft = nodeAdapter.GetLeftChild(nodeParent);
                if (nodeAdapter.NodeEquals(nodeParentLeft, node)) nodeAdapter.SetLeftChild(nodeParent, successor);
                else nodeAdapter.SetRightChild(nodeParent, successor);
            }
            if (nodeAdapter.NodeEquals(node, successorParent))
            {
                nodeAdapter.SetParent(node, successor);
                nodeAdapter.SetRightChild(successor, node);
            }
            else
            {
                nodeAdapter.SetParent(node, successorParent);
                if (nodeAdapter.IsNotNil(successorParent))
                {
                    var successorParentLeft = nodeAdapter.GetLeftChild(successorParent);
                    if (nodeAdapter.NodeEquals(successorParentLeft, successor)) nodeAdapter.SetLeftChild(successorParent, node);
                    else nodeAdapter.SetRightChild(successorParent, node);
                }
                nodeAdapter.SetRightChild(successor, nodeRight);
                if (nodeAdapter.IsNotNil(nodeRight)) nodeAdapter.SetParent(nodeRight, successor);
            }
            // Colors
            var nodeColor = nodeAdapter.GetColor(node);
            nodeAdapter.SetColor(node, nodeAdapter.GetColor(successor));
            nodeAdapter.SetColor(successor, nodeColor);
            // Check for root
            if (nodeAdapter.NodeEquals(node, root)) root = successor;
        }

        nodeLeft = nodeAdapter.GetLeftChild(node);
        nodeRight = nodeAdapter.GetRightChild(node);
        nodeParent = nodeAdapter.GetParent(node)!;
        var child = nodeAdapter.IsNil(nodeLeft) ? nodeRight : nodeLeft;
        bool nodeIsLeftChild;
        if (nodeAdapter.GetColor(node) == Color.Red || nodeAdapter.IsNotNil(child))
        {
            // A red node must not have children here, we can simply remove
            // If there is a child, it must be red
            // We can simply paint that black and make it the child
            if (nodeAdapter.IsNotNil(nodeParent))
            {
                nodeIsLeftChild = nodeAdapter.NodeEquals(nodeAdapter.GetLeftChild(nodeParent), node);
                if (nodeIsLeftChild) nodeAdapter.SetLeftChild(nodeParent, child);
                else nodeAdapter.SetRightChild(nodeParent, child);
            }
            else
            {
                root = child;
            }
            // If there was a child, we paint it black
            if (nodeAdapter.IsNotNil(child))
            {
                nodeAdapter.SetParent(child, nodeParent);
                nodeAdapter.SetColor(child, Color.Black);
            }
            // We are done
            return root;
        }

        // The hard case, non-root black node and only null children
        Debug.Assert(nodeAdapter.IsNil(nodeLeft) && nodeAdapter.IsNil(nodeRight) && nodeAdapter.GetColor(node) == Color.Black);

        // Replace the parent pointer to this to null
        nodeIsLeftChild = nodeAdapter.NodeEquals(nodeAdapter.GetLeftChild(nodeParent), node);
        if (nodeIsLeftChild) nodeAdapter.SetLeftChild(nodeParent, nodeAdapter.NilNode);
        else nodeAdapter.SetRightChild(nodeParent, nodeAdapter.NilNode);

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
            if (nodeAdapter.IsNotNil(distantNephew) && nodeAdapter.GetColor(distantNephew) == Color.Red) goto Case_D6;
            if (nodeAdapter.IsNotNil(closeNephew) && nodeAdapter.GetColor(closeNephew) == Color.Red) goto Case_D5;

            // Both nephews are null
            if (nodeAdapter.GetColor(nodeParent) == Color.Red) goto Case_D4;

            // Case D1 (parent, close nephew, distant nephew and sibling are black)
            nodeAdapter.SetColor(sibling!, Color.Red);

            // Update for next iteration level
            node = nodeParent;
            nodeParent = nodeAdapter.GetParent(node);
            if (nodeAdapter.IsNil(nodeParent)) break;
            nodeIsLeftChild = nodeAdapter.NodeEquals(nodeAdapter.GetLeftChild(nodeParent), node);
        }

        // Delete case 2
        return root;

    Case_D3: // Sibling is red, parent, close- and distant nephew are black
        rotationRoot = nodeIsLeftChild
            ? BinarySearchTree.RotateLeft(nodeParent, nodeAdapter)
            : BinarySearchTree.RotateRight(nodeParent, nodeAdapter);
        if (nodeAdapter.NodeEquals(root, nodeParent)) root = rotationRoot;
        nodeAdapter.SetColor(nodeParent, Color.Red);
        nodeAdapter.SetColor(sibling!, Color.Black);
        sibling = closeNephew;

        // Parent is red, sibling is black
        distantNephew = nodeIsLeftChild
            ? nodeAdapter.GetRightChild(sibling!)
            : nodeAdapter.GetLeftChild(sibling!);
        if (nodeAdapter.IsNotNil(distantNephew) && nodeAdapter.GetColor(distantNephew) == Color.Red) goto Case_D6;

        closeNephew = nodeIsLeftChild
            ? nodeAdapter.GetLeftChild(sibling!)
            : nodeAdapter.GetRightChild(sibling!);
        if (nodeAdapter.IsNotNil(closeNephew) && nodeAdapter.GetColor(closeNephew) == Color.Red) goto Case_D5;

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
        if (nodeAdapter.NodeEquals(root, nodeParent)) root = rotationRoot;
        nodeAdapter.SetColor(sibling!, nodeAdapter.GetColor(nodeParent));
        nodeAdapter.SetColor(nodeParent, Color.Black);
        nodeAdapter.SetColor(distantNephew!, Color.Black);
        return root;
    }
}
