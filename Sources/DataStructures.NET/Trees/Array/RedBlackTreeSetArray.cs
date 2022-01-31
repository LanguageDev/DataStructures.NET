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
/// A reference set implementation based on a Red-Black tree.
/// </summary>
/// <typeparam name="T">The stored element type.</typeparam>
/// <typeparam name="TComparer">The comparer type.</typeparam>
public class RedBlackTreeSetArray<T, TComparer> : ISet<T>
    where TComparer : IComparer<T>
{
    /// <inheritdoc/>
    public int Count => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool IsReadOnly => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool Add(T item) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void Clear() => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool Contains(T item) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other) => throw new NotImplementedException();

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<T> other) => throw new NotImplementedException();

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
    public bool Remove(T item) => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other) => throw new NotImplementedException();

    /// <inheritdoc/>
    void ICollection<T>.Add(T item) => throw new NotImplementedException();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}
