using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures.NET;
using Xunit;
using Fuzzer;
using BstSet = DataStructures.NET.BinarySearchTreeSet<int, System.Collections.Generic.IComparer<int>>;

namespace Tests;

public class BstTests
{
    private static void ValidateTree(BstSet bst) =>
        TreeValidation.ValidateAdjacency(bst.Root, default(BstSet.NodeAdapter));

    private static void AssertTreeEquals(
        BstSet bst,
        BstSet.Node? node) => TreeValidation.AssertTreeEquals(
            root1: bst.Root,
            root2: node,
            nodeAdapter: default(BstSet.NodeAdapter),
            nodeEquals: (n1, n2) => n1.Key == n2.Key);

    private static BstSet.Node? SetParent(BstSet.Node? root)
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
    public void Insert123()
    {
        var set = new BstSet(Comparer<int>.Default);
        ValidateTree(set);
        Assert.True(set.Add(1));
        ValidateTree(set);
        Assert.True(set.Add(2));
        ValidateTree(set);
        Assert.True(set.Add(3));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            new(1)
            {
                Right = new(2)
                {
                    Right = new(3),
                }
            });
    }

    [Fact]
    public void Insert321()
    {
        var set = new BstSet(Comparer<int>.Default);
        ValidateTree(set);
        Assert.True(set.Add(3));
        ValidateTree(set);
        Assert.True(set.Add(2));
        ValidateTree(set);
        Assert.True(set.Add(1));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            new(3)
            {
                Left = new(2)
                {
                    Left = new(1),
                }
            });
    }

    [Theory]
    [InlineData(2, 3, 1)]
    [InlineData(2, 1, 3)]
    public void Insert231or213(int a, int b, int c)
    {
        var set = new BstSet(Comparer<int>.Default);
        ValidateTree(set);
        Assert.True(set.Add(a));
        ValidateTree(set);
        Assert.True(set.Add(b));
        ValidateTree(set);
        Assert.True(set.Add(c));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            new(2)
            {
                Left = new(1),
                Right = new(3),
            });
    }

    [Fact]
    public void RepeatedInsert()
    {
        var set = new BstSet(Comparer<int>.Default);
        ValidateTree(set);
        Assert.True(set.Add(1));
        ValidateTree(set);
        Assert.False(set.Add(1));
        ValidateTree(set);
        Assert.True(set.Add(2));
        ValidateTree(set);
        Assert.False(set.Add(2));
        ValidateTree(set);
    }

    [Fact]
    public void InsertMany()
    {
        var set = new BstSet(Comparer<int>.Default);
        foreach (var n in new[] { 14, 23, 29, 8, 25, 22, 1, 6, 24, 28 })
        {
            ValidateTree(set);
            Assert.True(set.Add(n));
        }
        AssertTreeEquals(
            set,
            new(14)
            {
                Left = new(8)
                {
                    Left = new(1)
                    {
                        Right = new(6),
                    },
                },
                Right = new(23)
                {
                    Left = new(22),
                    Right = new(29)
                    {
                        Left = new(25)
                        {
                            Left = new(24),
                            Right = new(28),
                        },
                    },
                },
            });
    }

    [Fact]
    public void DeleteRootSingleRightChild()
    {
        var set = new BstSet(Comparer<int>.Default)
        {
            Root = SetParent(new(1)
            {
                Right = new(3)
                {
                    Left = new(2),
                    Right = new(4),
                },
            })
        };
        ValidateTree(set);
        Assert.True(set.Remove(1));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            new(3)
            {
                Left = new(2),
                Right = new(4),
            });
    }

    [Fact]
    public void DeleteRootSingleLeftChild()
    {
        var set = new BstSet(Comparer<int>.Default)
        {
            Root = SetParent(new(5)
            {
                Left = new(3)
                {
                    Left = new(2),
                    Right = new(4),
                },
            })
        };
        ValidateTree(set);
        Assert.True(set.Remove(5));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            new(3)
            {
                Left = new(2),
                Right = new(4),
            });
    }

    [Fact]
    public void DeleteRootRightChildWithNoLeftChild()
    {
        var set = new BstSet(Comparer<int>.Default)
        {
            Root = SetParent(new(5)
            {
                Left = new(3),
                Right = new(8)
                {
                    Right = new(9),
                },
            })
        };
        ValidateTree(set);
        Assert.True(set.Remove(5));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            new(8)
            {
                Left = new(3),
                Right = new(9),
            });
    }

    [Fact]
    public void DeleteRootRightChildWithLeftChild()
    {
        var set = new BstSet(Comparer<int>.Default)
        {
            Root = SetParent(new(5)
            {
                Left = new(3),
                Right = new(8)
                {
                    Left = new(6)
                    {
                        Right = new(7),
                    },
                },
            })
        };
        ValidateTree(set);
        Assert.True(set.Remove(5));
        ValidateTree(set);
        AssertTreeEquals(
            set,
            new(6)
            {
                Left = new(3),
                Right = new(8)
                {
                    Left = new(7),
                },
            });
    }
}
