// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace DataStructures.NET.Trees.External;

/// <summary>
/// Extension functionality for node adapters.
/// </summary>
public static class NodeAdapterExtensions
{
    /// <summary>
    /// Checks if a given node is considered a nil node.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <param name="node">The node to compare to nil.</param>
    /// <returns>True, if <paramref name="node"/> is nil.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNil<TNode, TNodeAdapter>(
        this TNodeAdapter nodeAdapter,
        [NotNullWhen(false)] TNode? node)
        where TNodeAdapter : BinarySearchTree.INodeIdentity<TNode> =>
        nodeAdapter.NodeEquals(node, nodeAdapter.NilNode);

    /// <summary>
    /// Checks if a given node is not considered a nil node.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <param name="node">The node to compare to nil.</param>
    /// <returns>True, if <paramref name="node"/> is not nil.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNil<TNode, TNodeAdapter>(
        this TNodeAdapter nodeAdapter,
        [NotNullWhen(true)] TNode? node)
        where TNodeAdapter : BinarySearchTree.INodeIdentity<TNode> =>
        !nodeAdapter.NodeEquals(node, nodeAdapter.NilNode);
}
