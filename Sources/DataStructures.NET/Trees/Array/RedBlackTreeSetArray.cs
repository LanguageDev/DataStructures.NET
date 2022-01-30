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
    public int Count => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

    public bool Add(T item) => throw new NotImplementedException();
    public void Clear() => throw new NotImplementedException();
    public bool Contains(T item) => throw new NotImplementedException();
    public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
    public void ExceptWith(IEnumerable<T> other) => throw new NotImplementedException();
    public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();
    public void IntersectWith(IEnumerable<T> other) => throw new NotImplementedException();
    public bool IsProperSubsetOf(IEnumerable<T> other) => throw new NotImplementedException();
    public bool IsProperSupersetOf(IEnumerable<T> other) => throw new NotImplementedException();
    public bool IsSubsetOf(IEnumerable<T> other) => throw new NotImplementedException();
    public bool IsSupersetOf(IEnumerable<T> other) => throw new NotImplementedException();
    public bool Overlaps(IEnumerable<T> other) => throw new NotImplementedException();
    public bool Remove(T item) => throw new NotImplementedException();
    public bool SetEquals(IEnumerable<T> other) => throw new NotImplementedException();
    public void SymmetricExceptWith(IEnumerable<T> other) => throw new NotImplementedException();
    public void UnionWith(IEnumerable<T> other) => throw new NotImplementedException();
    void ICollection<T>.Add(T item) => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}
