// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DataStructures.NET.Trees.External;

namespace DataStructures.NET.Trees.Linked;

// TODO: Finish implementation

/// <summary>
/// A reference set implementation based on a binary search tree.
/// </summary>
/// <typeparam name="T">The stored element type.</typeparam>
/// <typeparam name="TComparer">The comparer type.</typeparam>
public class BinarySearchTreeSetLinked<T, TComparer> : ISet<T>
    where TComparer : IComparer<T>
{
    /// <summary>
    /// The node type of the BST set.
    /// </summary>
    internal sealed class Node
    {
        /// <summary>
        /// The key stored in the node.
        /// </summary>
        public T Key { get; }

        /// <summary>
        /// The parent of this node.
        /// </summary>
        public Node? Parent { get; internal set; }

        /// <summary>
        /// The left child of this node.
        /// </summary>
        public Node? Left { get; internal set; }

        /// <summary>
        /// The right child of this node.
        /// </summary>
        public Node? Right { get; internal set; }

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="key">The key of the node.</param>
        public Node(T key)
        {
            this.Key = key;
        }
    }

    internal readonly struct NodeAdapter :
        BinarySearchTree.INodeIdentity<Node>,
        BinarySearchTree.IChildSelector<Node>,
        BinarySearchTree.IParentSelector<Node>,
        BinarySearchTree.IKeySelector<Node, T>,
        BinarySearchTree.INodeBuilder<T, bool, Node>
    {
        public Node? NilNode => null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NodeEquals(Node? n1, Node? n2) => ReferenceEquals(n1, n2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Node? GetLeftChild(Node node) => node.Left;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Node? GetRightChild(Node node) => node.Right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Node? GetParent(Node node) => node.Parent;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetLeftChild(Node node, Node? child) => node.Left = child;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRightChild(Node node, Node? child) => node.Right = child;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetParent(Node node, Node? parent) => node.Parent = parent;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetKey(Node node) => node.Key;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Node Build(T key, bool data) => new(key);
    }

    /// <inheritdoc/>
    public int Count { get; internal set; }

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// The root of the tree.
    /// </summary>
    internal Node? Root { get; set; }

    private readonly TComparer comparer;

    /// <summary>
    /// Initializes a new, empty BST-based set.
    /// </summary>
    /// <param name="comparer"></param>
    public BinarySearchTreeSetLinked(TComparer comparer)
    {
        this.comparer = comparer;
    }

    /// <inheritdoc/>
    public bool Contains(T element) => BinarySearchTree.Search(
        root: this.Root,
        nodeAdapter: default(NodeAdapter),
        key: element,
        keyComparer: this.comparer).Found is not null;

    /// <inheritdoc/>
    public bool Add(T element)
    {
        var insert = BinarySearchTree.Insert(
            root: this.Root,
            nodeAdapter: default(NodeAdapter),
            key: element,
            data: false,
            keyComparer: this.comparer);
        this.Root = insert.Root;
        if (insert.Inserted is null) return false;
        ++this.Count;
        return true;
    }

    /// <inheritdoc/>
    void ICollection<T>.Add(T item) => this.Add(item);

    /// <inheritdoc/>
    public bool Remove(T element)
    {
        var node = BinarySearchTree.Search(
            root: this.Root,
            nodeAdapter: default(NodeAdapter),
            key: element,
            keyComparer: this.comparer).Found;
        if (node is null) return false;
        this.Root = BinarySearchTree.Delete(
            root: this.Root,
            node: node,
            nodeAdapter: default(NodeAdapter)).Root;
        --this.Count;
        return true;
    }

    /// <inheritdoc/>
    public void Clear() => this.Root = null;

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (var item in this) array[arrayIndex++] = item;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other)
    {
        foreach (var item in other) this.Remove(item);
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<T> other) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other)
    {
        foreach (var item in other) this.Add(item);
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other) => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other) => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other) => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other) => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other) => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other) => throw new NotImplementedException();
}
