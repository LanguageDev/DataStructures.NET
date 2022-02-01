// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DataStructures.NET.Trees.External;

namespace DataStructures.NET.Trees.Array;

// TODO: Finish implementation

/// <summary>
/// A reference set implementation based on a binary search tree.
/// </summary>
/// <typeparam name="T">The stored element type.</typeparam>
/// <typeparam name="TComparer">The comparer type.</typeparam>
public class BinarySearchTreeSetArray<T, TComparer> : ISet<T>
    where TComparer : IComparer<T>
{
    internal readonly record struct NeighborData(int Parent, int Left, int Right);

    internal class Container
    {
        public List<T> Keys { get; } = new();
        public List<int> Left { get; } = new();
        public List<int> Right { get; } = new();
        public List<int> Parent { get; } = new();
    }

    // TODO: This structure never shrinks, the abadoned nodes stay in the list
    internal readonly struct NodeAdapter :
        BinarySearchTree.INodeIdentity<int>,
        BinarySearchTree.IChildSelector<int>,
        BinarySearchTree.IParentSelector<int>,
        BinarySearchTree.IKeySelector<int, T>,
        BinarySearchTree.INodeBuilder<T, bool, int>
    {
        internal readonly Container container = new();

        public int NilNode => -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NodeEquals(int n1, int n2) => n1 == n2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLeftChild(int node) => this.container.Left[node];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetRightChild(int node) => this.container.Right[node];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetParent(int node) => this.container.Parent[node];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetLeftChild(int node, int child) => this.container.Left[node] = child;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRightChild(int node, int child) => this.container.Right[node] = child;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetParent(int node, int parent) => this.container.Parent[node] = parent;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetKey(int node) => this.container.Keys[node];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Build(T key, bool data)
        {
            var idx = this.container.Keys.Count;
            this.container.Keys.Add(key);
            this.container.Left.Add(-1);
            this.container.Right.Add(-1);
            this.container.Parent.Add(0);
            return idx;
        }
    }

    /// <inheritdoc/>
    public int Count { get; internal set; }

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// The root of the tree.
    /// </summary>
    internal int Root { get; set; } = -1;

    /// <summary>
    /// The node adapter.
    /// </summary>
    internal NodeAdapter Adapter { get; } = new();

    private readonly TComparer comparer;

    /// <summary>
    /// Initializes a new, empty BST-based set.
    /// </summary>
    /// <param name="comparer"></param>
    public BinarySearchTreeSetArray(TComparer comparer)
    {
        this.comparer = comparer;
    }

    /// <inheritdoc/>
    public bool Contains(T element) => BinarySearchTree.Search(
        root: this.Root,
        nodeAdapter: this.Adapter,
        key: element,
        keyComparer: this.comparer).Found != -1;

    /// <inheritdoc/>
    public bool Add(T element)
    {
        var insert = BinarySearchTree.Insert(
            root: this.Root,
            nodeAdapter: this.Adapter,
            key: element,
            data: false,
            keyComparer: this.comparer);
        this.Root = insert.Root;
        if (insert.Inserted == -1) return false;
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
            nodeAdapter: this.Adapter,
            key: element,
            keyComparer: this.comparer).Found;
        if (node == -1) return false;
        this.Root = BinarySearchTree.Delete(
            root: this.Root,
            node: node,
            nodeAdapter: this.Adapter).Root;
        --this.Count;
        return true;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        this.Root = -1;
        this.Adapter.container.Keys.Clear();
        this.Adapter.container.Left.Clear();
        this.Adapter.container.Right.Clear();
        this.Adapter.container.Parent.Clear();
    }

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
