// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using Fuzzer;
using Xunit;
using RedBlackTreeSetLinked = DataStructures.NET.Trees.Linked.RedBlackTreeSetLinked<int, System.Collections.Generic.IComparer<int>>;
using Color = DataStructures.NET.Trees.External.RedBlackTree.Color;

namespace Tests.Trees;

public class RedBlackTreeTests
{
    private static void ValidateTree(RedBlackTreeSetLinked.Node? root)
    {
        TreeValidation.ValidateAdjacency(root, default(RedBlackTreeSetLinked.NodeAdapter));
        TreeValidation.ValidateRedBlack(root, default(RedBlackTreeSetLinked.NodeAdapter));
    }

    private static void ValidateTree(RedBlackTreeSetLinked set) => ValidateTree(set.Root);

    private static void AssertTreeEquals(
        RedBlackTreeSetLinked.Node? root1,
        RedBlackTreeSetLinked.Node? root2) => TreeValidation.AssertTreeEquals(
            root1: root1,
            root2: root2,
            nodeAdapter: default(RedBlackTreeSetLinked.NodeAdapter),
            nodeEquals: (n1, n2) => n1.Key == n2.Key
                                 && n1.Color == n2.Color);

    private static void AssertTreeEquals(
        RedBlackTreeSetLinked set,
        RedBlackTreeSetLinked.Node? node) => AssertTreeEquals(set.Root, node);

    private static RedBlackTreeSetLinked.Node? SetParent(RedBlackTreeSetLinked.Node? root)
    {
        if (root is null) return root;
        if (root.Left is not null)
        {
            root.Left.Parent = root;
            SetParent(root.Left);
        }
        if (root.Right is not null)
        {
            root.Right.Parent = root;
            SetParent(root.Right);
        }
        return root;
    }

    [Fact]
    public void Insert214()
    {
        var set = new RedBlackTreeSet(Comparer<int>.Default);
        ValidateTree(set);
        Assert.True(set.Add(2));
        ValidateTree(set);
        Assert.True(set.Add(1));
        ValidateTree(set);
        Assert.True(set.Add(4));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            SetParent(new(2)
            {
                Color = Color.Black,
                Left = new(1) { Color = Color.Red },
                Right = new(4) { Color = Color.Red },
            }));
    }

    [Fact]
    public void Insert5To214()
    {
        var set = new RedBlackTreeSet(Comparer<int>.Default)
        {
            Root = SetParent(new(2)
            {
                Color = Color.Black,
                Left = new(1) { Color = Color.Red },
                Right = new(4) { Color = Color.Red },
            })
        };
        ValidateTree(set);
        Assert.True(set.Add(5));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            SetParent(new(2)
            {
                Color = Color.Red,
                Left = new(1) { Color = Color.Black },
                Right = new(4)
                {
                    Color = Color.Black,
                    Right = new(5) { Color = Color.Red },
                },
            }));
    }

    [Fact]
    public void Insert9To2145()
    {
        var set = new RedBlackTreeSet(Comparer<int>.Default)
        {
            Root = SetParent(new(2)
            {
                Color = Color.Black,
                Left = new(1) { Color = Color.Black },
                Right = new(4)
                {
                    Color = Color.Black,
                    Right = new(5) { Color = Color.Red },
                },
            })
        };
        ValidateTree(set);
        Assert.True(set.Add(9));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            SetParent(new(2)
            {
                Color = Color.Black,
                Left = new(1) { Color = Color.Black },
                Right = new(5)
                {
                    Color = Color.Black,
                    Left = new(4) { Color = Color.Red },
                    Right = new(9) { Color = Color.Red },
                },
            }));
    }

    [Fact]
    public void Insert3To21459()
    {
        var set = new RedBlackTreeSet(Comparer<int>.Default)
        {
            Root = SetParent(new(2)
            {
                Color = Color.Black,
                Left = new(1) { Color = Color.Black },
                Right = new(5)
                {
                    Color = Color.Black,
                    Left = new(4) { Color = Color.Red },
                    Right = new(9) { Color = Color.Red },
                },
            })
        };
        ValidateTree(set);
        Assert.True(set.Add(3));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            SetParent(new(2)
            {
                Color = Color.Black,
                Left = new(1) { Color = Color.Black },
                Right = new(5)
                {
                    Color = Color.Red,
                    Left = new(4)
                    {
                        Color = Color.Black,
                        Left = new(3) { Color = Color.Red },
                    },
                    Right = new(9) { Color = Color.Black },
                },
            }));
    }

    [Fact]
    public void Insert6To214593()
    {
        var set = new RedBlackTreeSet(Comparer<int>.Default)
        {
            Root = SetParent(new(2)
            {
                Color = Color.Black,
                Left = new(1) { Color = Color.Black },
                Right = new(5)
                {
                    Color = Color.Red,
                    Left = new(4)
                    {
                        Color = Color.Black,
                        Left = new(3) { Color = Color.Red },
                    },
                    Right = new(9) { Color = Color.Black },
                },
            })
        };
        ValidateTree(set);
        Assert.True(set.Add(6));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            SetParent(new(2)
            {
                Color = Color.Black,
                Left = new(1) { Color = Color.Black },
                Right = new(5)
                {
                    Color = Color.Red,
                    Left = new(4)
                    {
                        Color = Color.Black,
                        Left = new(3) { Color = Color.Red },
                    },
                    Right = new(9)
                    {
                        Color = Color.Black,
                        Left = new(6) { Color = Color.Red },
                    },
                },
            }));
    }

    [Fact]
    public void Insert7To2145936()
    {
        var set = new RedBlackTreeSet(Comparer<int>.Default)
        {
            Root = SetParent(new(2)
            {
                Color = Color.Black,
                Left = new(1) { Color = Color.Black },
                Right = new(5)
                {
                    Color = Color.Red,
                    Left = new(4)
                    {
                        Color = Color.Black,
                        Left = new(3) { Color = Color.Red },
                    },
                    Right = new(9)
                    {
                        Color = Color.Black,
                        Left = new(6) { Color = Color.Red },
                    },
                },
            })
        };
        ValidateTree(set);
        Assert.True(set.Add(7));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            SetParent(new(2)
            {
                Color = Color.Black,
                Left = new(1) { Color = Color.Black },
                Right = new(5)
                {
                    Color = Color.Red,
                    Left = new(4)
                    {
                        Color = Color.Black,
                        Left = new(3) { Color = Color.Red },
                    },
                    Right = new(7)
                    {
                        Color = Color.Black,
                        Left = new(6) { Color = Color.Red },
                        Right = new(9) { Color = Color.Red },
                    },
                },
            }));
    }

    [Fact]
    public void Delete5From5267()
    {
        var set = new RedBlackTreeSet(Comparer<int>.Default)
        {
            Root = SetParent(new(5)
            {
                Color = Color.Red,
                Left = new(2) { Color = Color.Black },
                Right = new(6)
                {
                    Color = Color.Black,
                    Right = new(7) { Color = Color.Red },
                },
            })
        };
        ValidateTree(set);
        Assert.True(set.Remove(5));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            SetParent(new(6)
            {
                Color = Color.Red,
                Left = new(2) { Color = Color.Black },
                Right = new(7) { Color = Color.Black },
            }));
    }

    [Fact]
    public void Delete7From1413()
    {
        var set = new RedBlackTreeSet(Comparer<int>.Default)
        {
            Root = SetParent(new(4)
            {
                Color = Color.Red,
                Left = new(1) { Color = Color.Black },
                Right = new(7) { Color = Color.Black },
            })
        };
        ValidateTree(set);
        Assert.True(set.Remove(7));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            SetParent(new(4)
            {
                Color = Color.Black,
                Left = new(1) { Color = Color.Red },
            }));
    }

    [Fact]
    public void Delete2From21()
    {
        var set = new RedBlackTreeSet(Comparer<int>.Default)
        {
            Root = SetParent(new(2)
            {
                Color = Color.Black,
                Left = new(1) { Color = Color.Red },
            })
        };
        ValidateTree(set);
        Assert.True(set.Remove(2));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            SetParent(new(1) { Color = Color.Black }));
    }
}
