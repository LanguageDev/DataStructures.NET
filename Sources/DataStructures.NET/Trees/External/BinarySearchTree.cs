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
    /// Node identify functionality.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    public interface INodeIdentity<TNode>
    {
        /// <summary>
        /// The null node.
        /// </summary>
        public TNode? NilNode { get; }

        /// <summary>
        /// Checks, if two nodes are equal. For actual tree-nodes this usually means referential equality.
        /// </summary>
        /// <param name="n1">The first node to equate.</param>
        /// <param name="n2">The second node to equate.</param>
        /// <returns>True, if <paramref name="n1"/> and <paramref name="n2"/> are considered equal.</returns>
        public bool NodeEquals(TNode? n1, TNode? n2);
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
        TNode? Found,
        (TNode Node, Child Child)? Hint = null); // NOTE: Can be null, since we know this is a nullable value-type

    /// <summary>
    /// Represents the result of an insertion.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="Root">The root of the tree.</param>
    /// <param name="Inserted">The inserted node, if any.</param>
    /// <param name="Existing">The existing node that blocked the insertion, if any.</param>
    public readonly record struct InsertResult<TNode>(
        TNode Root,
        TNode? Inserted,
        TNode? Existing);

    /// <summary>
    /// Represents the result of a deletion.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="Root">The root of the tree.</param>
    /// <param name="Parent">The parent of the node that was removed or moved in the tree. This can be used for rebalancing.</param>
    public readonly record struct DeleteResult<TNode>(
        TNode? Root,
        TNode? Parent);

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
        where TNodeAdapter : INodeIdentity<TNode>, IChildSelector<TNode>
    {
        while (true)
        {
            var left = nodeAdapter.GetLeftChild(root);
            if (nodeAdapter.IsNil(left)) break;
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
        where TNodeAdapter : INodeIdentity<TNode>, IChildSelector<TNode>
    {
        while (true)
        {
            var right = nodeAdapter.GetRightChild(root);
            if (nodeAdapter.IsNil(right)) break;
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
    public static TNode? InOrderPredecessor<TNode, TNodeAdapter>(
        TNode node,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : INodeIdentity<TNode>, IChildSelector<TNode>, IParentSelector<TNode>
    {
        var nodeLeft = nodeAdapter.GetLeftChild(node);
        if (nodeAdapter.IsNotNil(nodeLeft)) return Maximum(nodeLeft, nodeAdapter);
        var y = nodeAdapter.GetParent(node);
        while (nodeAdapter.IsNotNil(y) && nodeAdapter.NodeEquals(node, nodeAdapter.GetLeftChild(y)))
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
    public static TNode? InOrderSuccessor<TNode, TNodeAdapter>(
        TNode node,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : INodeIdentity<TNode>, IChildSelector<TNode>, IParentSelector<TNode>
    {
        var nodeRight = nodeAdapter.GetRightChild(node);
        if (nodeAdapter.IsNotNil(nodeRight)) return Minimum(nodeRight, nodeAdapter);
        var y = nodeAdapter.GetParent(node);
        while (nodeAdapter.IsNotNil(y) && nodeAdapter.NodeEquals(node, nodeAdapter.GetRightChild(y)))
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
        where TNodeAdapter : INodeIdentity<TNode>, IChildSelector<TNode>, IKeySelector<TNode, TKey>
        where TKeyComparer : IComparer<TKey>
    {
        (TNode Node, Child Child)? hint = null;
        while (nodeAdapter.IsNotNil(root))
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
        return new(Found: nodeAdapter.NilNode, Hint: hint);
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
        where TNodeAdapter : INodeIdentity<TNode>,
                             IChildSelector<TNode>,
                             IParentSelector<TNode>,
                             IKeySelector<TNode, TKey>,
                             INodeBuilder<TKey, TData, TNode>
        where TKeyComparer : IComparer<TKey>
    {
        // Try a search
        var (found, hint) = Search(root, nodeAdapter, key, keyComparer);
        // If found, we don't do an insertion
        if (nodeAdapter.IsNotNil(found)) return new(Root: root!, Existing: found, Inserted: nodeAdapter.NilNode);
        // It's a new node, construct it
        var newNode = nodeAdapter.Build(key, data);
        // If there's a hint, use it
        if (hint is not null)
        {
            var (hNode, hChild) = hint.Value;
            if (hChild == Child.Left) nodeAdapter.SetLeftChild(hNode, newNode);
            else nodeAdapter.SetRightChild(hNode, newNode);
            nodeAdapter.SetParent(newNode, hNode);
            return new(Root: root!, Inserted: newNode, Existing: nodeAdapter.NilNode);
        }
        // Otherwise, this has to be a new root
        return new(Root: newNode, Inserted: newNode, Existing: nodeAdapter.NilNode);
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
        where TNodeAdapter : INodeIdentity<TNode>, IChildSelector<TNode>, IParentSelector<TNode>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Shift(TNode u, TNode? v)
        {
            var uParent = nodeAdapter.GetParent(u);
            if (nodeAdapter.IsNil(uParent)) root = v;
            else if (nodeAdapter.NodeEquals(u, nodeAdapter.GetLeftChild(uParent))) nodeAdapter.SetLeftChild(uParent, v);
            else nodeAdapter.SetRightChild(uParent, v);
            if (nodeAdapter.IsNotNil(v)) nodeAdapter.SetParent(v, uParent);
        }

        TNode? parent;
        if (nodeAdapter.IsNil(nodeAdapter.GetLeftChild(node)))
        {
            // 0 or 1 child
            parent = nodeAdapter.GetParent(node);
            Shift(node, nodeAdapter.GetRightChild(node));
        }
        else if (nodeAdapter.IsNil(nodeAdapter.GetRightChild(node)))
        {
            // 0 or 1 child
            parent = nodeAdapter.GetParent(node);
            Shift(node, nodeAdapter.GetLeftChild(node));
        }
        else
        {
            // 2 children
            var y = InOrderSuccessor(node, nodeAdapter)!;
            var yParent = nodeAdapter.GetParent(y);
            if (!nodeAdapter.NodeEquals(yParent, node))
            {
                parent = yParent;
                Shift(y, nodeAdapter.GetRightChild(y));
                var nodeRight = nodeAdapter.GetRightChild(node);
                nodeAdapter.SetRightChild(y, nodeRight);
                if (nodeAdapter.IsNotNil(nodeRight)) nodeAdapter.SetParent(nodeRight, y);
            }
            else
            {
                parent = y;
            }
            Shift(node, y);
            var nodeLeft = nodeAdapter.GetLeftChild(node);
            nodeAdapter.SetLeftChild(y, nodeLeft);
            if (nodeAdapter.IsNotNil(nodeLeft)) nodeAdapter.SetParent(nodeLeft, y);
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
        where TNodeAdapter : INodeIdentity<TNode>, IChildSelector<TNode>, IParentSelector<TNode>
    {
        var p = nodeAdapter.GetParent(root);
        var y = nodeAdapter.GetRightChild(root);
        if (nodeAdapter.IsNil(y)) throw new InvalidOperationException("The right child can not be null");
        if (nodeAdapter.IsNotNil(p))
        {
            var pLeft = nodeAdapter.GetLeftChild(p);
            if (nodeAdapter.NodeEquals(pLeft, root)) nodeAdapter.SetLeftChild(p, y);
            else nodeAdapter.SetRightChild(p, y);
        }
        nodeAdapter.SetParent(y, p);
        var t2 = nodeAdapter.GetLeftChild(y);
        nodeAdapter.SetLeftChild(y, root);
        nodeAdapter.SetParent(root, y);
        nodeAdapter.SetRightChild(root, t2);
        if (nodeAdapter.IsNotNil(t2)) nodeAdapter.SetParent(t2, root);
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
        where TNodeAdapter : INodeIdentity<TNode>, IChildSelector<TNode>, IParentSelector<TNode>
    {
        var p = nodeAdapter.GetParent(root);
        var x = nodeAdapter.GetLeftChild(root);
        if (nodeAdapter.IsNil(x)) throw new InvalidOperationException("The left child can not be null");
        if (nodeAdapter.IsNotNil(p))
        {
            var pLeft = nodeAdapter.GetLeftChild(p);
            if (nodeAdapter.NodeEquals(pLeft, root)) nodeAdapter.SetLeftChild(p, x);
            else nodeAdapter.SetRightChild(p, x);
        }
        nodeAdapter.SetParent(x, p);
        var t2 = nodeAdapter.GetRightChild(x);
        nodeAdapter.SetRightChild(x, root);
        nodeAdapter.SetParent(root, x);
        nodeAdapter.SetLeftChild(root, t2);
        if (nodeAdapter.IsNotNil(t2)) nodeAdapter.SetParent(t2, root);
        return x;
    }
}
