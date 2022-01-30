using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DataStructures.NET.Trees.External;

/// <summary>
/// Operations on a binary search tree.
/// </summary>
public static class BinarySearchTree
{
    /// <summary>
    /// An enumeration describing the two children of a node.
    /// </summary>
    public enum Child : byte
    {
        /// <summary>
        /// The left child.
        /// </summary>
        Left,

        /// <summary>
        /// The right child.
        /// </summary>
        Right,
    }

    /// <summary>
    /// Child getter and setter functionality.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    public interface IChildSelector<TNode>
    {
        /// <summary>
        /// Accesses the left child of a node.
        /// </summary>
        /// <param name="node">The node to get the left child of.</param>
        /// <returns>The left child of <paramref name="node"/>, or null if it has no left child.</returns>
        public TNode? GetLeftChild(TNode node);

        /// <summary>
        /// Accesses the right child of a node.
        /// </summary>
        /// <param name="node">The node to get the right child of.</param>
        /// <returns>The right child of <paramref name="node"/>, or null if it has no right child.</returns>
        public TNode? GetRightChild(TNode node);

        /// <summary>
        /// Sets the left child of a node.
        /// </summary>
        /// <param name="node">The node to set the left child of.</param>
        /// <param name="child">The node to set as the left child of <paramref name="node"/>.</param>
        public void SetLeftChild(TNode node, TNode? child);

        /// <summary>
        /// Sets the right child of a node.
        /// </summary>
        /// <param name="node">The node to set the right child of.</param>
        /// <param name="child">The node to set as the right child of <paramref name="node"/>.</param>
        public void SetRightChild(TNode node, TNode? child);
    }

    /// <summary>
    /// Parent getter and setter functionality.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    public interface IParentSelector<TNode>
    {
        /// <summary>
        /// Accesses the parent of a node.
        /// </summary>
        /// <param name="node">The node to get the parent of.</param>
        /// <returns>The parent of <paramref name="node"/>, or null if it has no parent.</returns>
        public TNode? GetParent(TNode node);

        /// <summary>
        /// Sets the parent of a node.
        /// </summary>
        /// <param name="node">The node to set the parent of.</param>
        /// <param name="parent">The node to set as the parent of <paramref name="node"/>.</param>
        public void SetParent(TNode node, TNode? parent);
    }

    /// <summary>
    /// Node key retrieval functionality.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    public interface IKeySelector<TNode, TKey>
    {
        /// <summary>
        /// Retrieves the key of a node.
        /// </summary>
        /// <param name="node">The node to get the key of.</param>
        /// <returns>The key of <paramref name="node"/>.</returns>
        public TKey GetKey(TNode node);
    }

    /// <summary>
    /// Node instantiation functionality.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TData">The type of the data inserted.</typeparam>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    public interface INodeBuilder<TKey, TData, TNode>
    {
        /// <summary>
        /// Constructs a new tree node.
        /// </summary>
        /// <param name="key">The key to create the node from.</param>
        /// <param name="data">The data to create the node from.</param>
        /// <returns>The created tree node.</returns>
        public TNode Build(TKey key, TData data);
    }

    /// <summary>
    /// Represents the result of a tree-search.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="Found">The node found. This is the exact match, if <paramref name="Hint"/> is null.</param>
    /// <param name="Hint">The hint for insertion, if an exact match is not found.</param>
    public readonly record struct SearchResult<TNode>(
        TNode? Found = default,
        (TNode Node, Child Child)? Hint = null);

    /// <summary>
    /// Represents the result of an insertion.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="Root">The root of the tree.</param>
    /// <param name="Inserted">The inserted node, if any.</param>
    /// <param name="Existing">The existing node that blocked the insertion, if any.</param>
    public readonly record struct InsertResult<TNode>(
        TNode Root,
        TNode? Inserted = default,
        TNode? Existing = default);

    /// <summary>
    /// Represents the result of a deletion.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="Root">The root of the tree.</param>
    /// <param name="Parent">The parent of the node that was removed or moved in the tree. This can be used for rebalancing.</param>
    public readonly record struct DeleteResult<TNode>(
        TNode? Root,
        TNode? Parent = default);

    /// <summary>
    /// Retrieves the minimum (leftmost leaf) of a given subtree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="root">The root of the subtree.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The minimum of the subtree with root <paramref name="root"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNode Minimum<TNode, TNodeAdapter>(
        TNode root,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : IChildSelector<TNode>
    {
        while (true)
        {
            var left = nodeAdapter.GetLeftChild(root);
            if (left is null) break;
            root = left;
        }
        return root;
    }

    /// <summary>
    /// Retrieves the maximum (rightmost leaf) of a given subtree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="root">The root of the subtree.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The maximum of the subtree with root <paramref name="root"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNode Maximum<TNode, TNodeAdapter>(
        TNode root,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : IChildSelector<TNode>
    {
        while (true)
        {
            var right = nodeAdapter.GetRightChild(root);
            if (right is null) break;
            root = right;
        }
        return root;
    }

    /// <summary>
    /// Retrieves the in-order predecessor of a given node.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="node">The node to get the predecessor of.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The predecessor of <paramref name="node"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNode? Predecessor<TNode, TNodeAdapter>(
        TNode node,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : IChildSelector<TNode>, IParentSelector<TNode>
    {
        var nodeLeft = nodeAdapter.GetLeftChild(node);
        if (nodeLeft is not null) return Maximum(nodeLeft, nodeAdapter);
        var y = nodeAdapter.GetParent(node);
        while (y is not null && ReferenceEquals(node, nodeAdapter.GetLeftChild(y)))
        {
            node = y;
            y = nodeAdapter.GetParent(y);
        }
        return y;
    }

    /// <summary>
    /// Retrieves the in-order successor of a given node.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="node">The node to get the successor of.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The successor of <paramref name="node"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNode? Successor<TNode, TNodeAdapter>(
        TNode node,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : IChildSelector<TNode>, IParentSelector<TNode>
    {
        var nodeRight = nodeAdapter.GetRightChild(node);
        if (nodeRight is not null) return Minimum(nodeRight, nodeAdapter);
        var y = nodeAdapter.GetParent(node);
        while (y is not null && ReferenceEquals(node, nodeAdapter.GetRightChild(y)))
        {
            node = y;
            y = nodeAdapter.GetParent(y);
        }
        return y;
    }

    /// <summary>
    /// Performs a search on a BST.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TKeyComparer">The key comparer type.</typeparam>
    /// <param name="root">The root of the BST.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <param name="key">The key to search for.</param>
    /// <param name="keyComparer">The key comparer.</param>
    /// <returns>The search results.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SearchResult<TNode> Search<TNode, TNodeAdapter, TKey, TKeyComparer>(
        TNode? root,
        TNodeAdapter nodeAdapter,
        TKey key,
        TKeyComparer keyComparer)
        where TNodeAdapter : IChildSelector<TNode>, IKeySelector<TNode, TKey>
        where TKeyComparer : IComparer<TKey>
    {
        (TNode Node, Child Child)? hint = null;
        while (root is not null)
        {
            var rootKey = nodeAdapter.GetKey(root);
            var cmp = keyComparer.Compare(key, rootKey);
            if (cmp < 0)
            {
                hint = (root, Child.Left);
                root = nodeAdapter.GetLeftChild(root);
            }
            else if (cmp > 0)
            {
                hint = (root, Child.Right);
                root = nodeAdapter.GetRightChild(root);
            }
            else
            {
                return new(Found: root);
            }
        }
        return new(Hint: hint);
    }

    /// <summary>
    /// Performs an insertion on a BST.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <typeparam name="TData">The inserted data type.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TKeyComparer">The key comparer type.</typeparam>
    /// <param name="root">The root of the BST.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <param name="key">The key to insert.</param>
    /// <param name="data">The data to insert.</param>
    /// <param name="keyComparer">The key comparer.</param>
    /// <returns>The results of the insertion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static InsertResult<TNode> Insert<TNode, TNodeAdapter, TData, TKey, TKeyComparer>(
        TNode? root,
        TNodeAdapter nodeAdapter,
        TKey key,
        TData data,
        TKeyComparer keyComparer)
        where TNodeAdapter : IChildSelector<TNode>,
                             IParentSelector<TNode>,
                             IKeySelector<TNode, TKey>,
                             INodeBuilder<TKey, TData, TNode>
        where TKeyComparer : IComparer<TKey>
    {
        // Try a search
        var (found, hint) = Search(root, nodeAdapter, key, keyComparer);
        // If found, we don't do an insertion
        if (found is not null) return new(Root: root!, Existing: found);
        // It's a new node, construct it
        var newNode = nodeAdapter.Build(key, data);
        // If there's a hint, use it
        if (hint is not null)
        {
            var (hNode, hChild) = hint.Value;
            if (hChild == Child.Left) nodeAdapter.SetLeftChild(hNode, newNode);
            else nodeAdapter.SetRightChild(hNode, newNode);
            nodeAdapter.SetParent(newNode, hNode);
            return new(Root: root!, Inserted: newNode);
        }
        // Otherwise, this has to be a new root
        return new(Root: newNode, Inserted: newNode);
    }

    /// <summary>
    /// Deletes a given node from a BST.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="root">The root of the tree.</param>
    /// <param name="node">The node to delete.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The results of the deletion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DeleteResult<TNode> Delete<TNode, TNodeAdapter>(
        TNode? root,
        TNode node,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : IChildSelector<TNode>, IParentSelector<TNode>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Shift(TNode u, TNode? v)
        {
            var uParent = nodeAdapter.GetParent(u);
            if (uParent is null) root = v;
            else if (ReferenceEquals(u, nodeAdapter.GetLeftChild(uParent))) nodeAdapter.SetLeftChild(uParent, v);
            else nodeAdapter.SetRightChild(uParent, v);
            if (v is not null) nodeAdapter.SetParent(v, uParent);
        }

        TNode? parent;
        if (nodeAdapter.GetLeftChild(node) is null)
        {
            // 0 or 1 child
            parent = nodeAdapter.GetParent(node);
            Shift(node, nodeAdapter.GetRightChild(node));
        }
        else if (nodeAdapter.GetRightChild(node) is null)
        {
            // 0 or 1 child
            parent = nodeAdapter.GetParent(node);
            Shift(node, nodeAdapter.GetLeftChild(node));
        }
        else
        {
            // 2 children
            var y = Successor(node, nodeAdapter);
            var yParent = nodeAdapter.GetParent(y!);
            if (!ReferenceEquals(yParent, node))
            {
                parent = yParent;
                Shift(y!, nodeAdapter.GetRightChild(y!));
                var nodeRight = nodeAdapter.GetRightChild(node);
                nodeAdapter.SetRightChild(y!, nodeRight);
                if (nodeRight is not null) nodeAdapter.SetParent(nodeRight, y);
            }
            else
            {
                parent = y;
            }
            Shift(node, y);
            var nodeLeft = nodeAdapter.GetLeftChild(node);
            nodeAdapter.SetLeftChild(y!, nodeLeft);
            if (nodeLeft is not null) nodeAdapter.SetParent(nodeLeft, y);
        }
        return new(Root: root, Parent: parent);
    }

    /// <summary>
    /// Rotates the subtree left.
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
        where TNodeAdapter : IChildSelector<TNode>, IParentSelector<TNode>
    {
        var p = nodeAdapter.GetParent(root);
        var y = nodeAdapter.GetRightChild(root)
             ?? throw new InvalidOperationException("The right child can not be null");
        if (p is not null)
        {
            var pLeft = nodeAdapter.GetLeftChild(p);
            if (ReferenceEquals(pLeft, root)) nodeAdapter.SetLeftChild(p, y);
            else nodeAdapter.SetRightChild(p, y);
        }
        nodeAdapter.SetParent(y, p);
        var t2 = nodeAdapter.GetLeftChild(y);
        nodeAdapter.SetLeftChild(y, root);
        nodeAdapter.SetParent(root, y);
        nodeAdapter.SetRightChild(root, t2);
        if (t2 is not null) nodeAdapter.SetParent(t2, root);
        return y;
    }

    /// <summary>
    /// Rotates the subtree right.
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
        where TNodeAdapter : IChildSelector<TNode>, IParentSelector<TNode>
    {
        var p = nodeAdapter.GetParent(root);
        var x = nodeAdapter.GetLeftChild(root)
             ?? throw new InvalidOperationException("The left child can not be null");
        if (p is not null)
        {
            var pLeft = nodeAdapter.GetLeftChild(p);
            if (ReferenceEquals(pLeft, root)) nodeAdapter.SetLeftChild(p, x);
            else nodeAdapter.SetRightChild(p, x);
        }
        nodeAdapter.SetParent(x, p);
        var t2 = nodeAdapter.GetRightChild(x);
        nodeAdapter.SetRightChild(x, root);
        nodeAdapter.SetParent(root, x);
        nodeAdapter.SetLeftChild(root, t2);
        if (t2 is not null) nodeAdapter.SetParent(t2, root);
        return x;
    }
}
