using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DataStructures.NET;

/// <summary>
/// Operations on a binary search tree.
/// </summary>
public static class BinarySearchTree
{
    /// <summary>
    /// Child accessor functionality.
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
    /// <returns>The node with key <paramref name="key"/>, or null if not found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNode? Search<TNode, TNodeAdapter, TKey, TKeyComparer>(
        TNode? root,
        TNodeAdapter nodeAdapter,
        TKey key,
        TKeyComparer keyComparer)
        where TNodeAdapter : IChildSelector<TNode>, IKeySelector<TNode, TKey>
        where TKeyComparer : IComparer<TKey>
    {
        while (root is not null)
        {
            var rootKey = nodeAdapter.GetKey(root);
            var cmp = keyComparer.Compare(key, rootKey);
            if (cmp < 0) root = nodeAdapter.GetLeftChild(root);
            else if (cmp > 0) root = nodeAdapter.GetRightChild(root);
            else return root;
        }
        return default;
    }
}
