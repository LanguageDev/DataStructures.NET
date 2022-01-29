// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fuzzer;
using Xunit;
using AvlTreeSet = DataStructures.NET.AvlTreeSet<int, System.Collections.Generic.IComparer<int>>;

namespace Tests;

public class AvlTreeTests
{
    private static void ValidateTree(AvlTreeSet.Node? root)
    {
        TreeValidation.ValidateAdjacency(root, default(AvlTreeSet.NodeAdapter));
        TreeValidation.ValidateBalanceAndHeight(root, default(AvlTreeSet.NodeAdapter));
    }

    private static void ValidateTree(AvlTreeSet set) => ValidateTree(set.Root);

    private static void AssertTreeEquals(
        AvlTreeSet.Node? root1,
        AvlTreeSet.Node? root2) => TreeValidation.AssertTreeEquals(
            root1: root1,
            root2: root2,
            nodeAdapter: default(AvlTreeSet.NodeAdapter),
            nodeEquals: (n1, n2) => n1.Key == n2.Key
                                 && n1.Height == n2.Height);

    private static void AssertTreeEquals(
        AvlTreeSet set,
        AvlTreeSet.Node? node) => AssertTreeEquals(set.Root, node);

    private static AvlTreeSet.Node? SetParentAndHeight(AvlTreeSet.Node? root)
    {
        if (root is null) return root;
        if (root.Left is not null)
        {
            root.Left.Parent = root;
            SetParentAndHeight(root.Left);
        }
        if (root.Right is not null)
        {
            root.Right.Parent = root;
            SetParentAndHeight(root.Right);
        }
        root.Height = Math.Max(root.Left?.Height ?? 0, root.Right?.Height ?? 0) + 1;
        return root;
    }

    private static AvlTreeSet.Node InsertCase1Root => SetParentAndHeight(new(20)
    {
        Left = new(4),
    })!;

    private static AvlTreeSet.Node InsertCase2Root => SetParentAndHeight(new(20)
    {
        Left = new(4)
        {
            Left = new(3),
            Right = new(9),
        },
        Right = new(26),
    })!;

    private static AvlTreeSet.Node InsertCase3Root => SetParentAndHeight(new(20)
    {
        Left = new(4)
        {
            Left = new(3)
            {
                Left = new(2),
            },
            Right = new(9)
            {
                Left = new(7),
                Right = new(11),
            },
        },
        Right = new(26)
        {
            Left = new(21),
            Right = new(30),
        },
    })!;

    [Theory]
    [InlineData("abc")]
    [InlineData("cba")]
    [InlineData("acb")]
    [InlineData("cab")]
    [InlineData("bac")]
    [InlineData("bca")]
    public void InsertAbcInAllOrders(string abc)
    {
        var set = new AvlTreeSet(Comparer<int>.Default);
        ValidateTree(set.Root);
        foreach (var letter in abc)
        {
            Assert.True(set.Add(letter));
            ValidateTree(set.Root);
        }
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new('b')
            {
                Left = new('a'),
                Right = new('c'),
            }));
    }

    // Insert 15

    [Fact]
    public void Case1Insert15()
    {
        var set = new AvlTreeSet(Comparer<int>.Default) { Root = InsertCase1Root };
        ValidateTree(set.Root);
        Assert.True(set.Add(15));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new(15)
            {
                Left = new(4),
                Right = new(20),
            }));
    }

    [Fact]
    public void Case2Insert15()
    {
        var set = new AvlTreeSet(Comparer<int>.Default) { Root = InsertCase2Root };
        ValidateTree(set.Root);
        Assert.True(set.Add(15));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new(9)
            {
                Left = new(4)
                {
                    Left = new(3),
                },
                Right = new(20)
                {
                    Left = new(15),
                    Right = new(26),
                },
            }));
    }

    [Fact]
    public void Case3Insert15()
    {
        var set = new AvlTreeSet(Comparer<int>.Default) { Root = InsertCase3Root };
        ValidateTree(set.Root);
        Assert.True(set.Add(15));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new(9)
            {
                Left = new(4)
                {
                    Left = new(3)
                    {
                        Left = new(2),
                    },
                    Right = new(7),
                },
                Right = new(20)
                {
                    Left = new(11)
                    {
                        Right = new(15),
                    },
                    Right = new(26)
                    {
                        Left = new(21),
                        Right = new(30),
                    },
                },
            }));
    }

    // Insert 8

    [Fact]
    public void Case1Insert8()
    {
        var set = new AvlTreeSet(Comparer<int>.Default) { Root = InsertCase1Root };
        ValidateTree(set.Root);
        Assert.True(set.Add(8));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new(8)
            {
                Left = new(4),
                Right = new(20),
            }));
    }

    [Fact]
    public void Case2Insert8()
    {
        var set = new AvlTreeSet(Comparer<int>.Default) { Root = InsertCase2Root };
        ValidateTree(set.Root);
        Assert.True(set.Add(8));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new(9)
            {
                Left = new(4)
                {
                    Left = new(3),
                    Right = new(8),
                },
                Right = new(20)
                {
                    Right = new(26),
                },
            }));
    }

    [Fact]
    public void Case3Insert8()
    {
        var set = new AvlTreeSet(Comparer<int>.Default) { Root = InsertCase3Root };
        ValidateTree(set.Root);
        Assert.True(set.Add(8));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new(9)
            {
                Left = new(4)
                {
                    Left = new(3)
                    {
                        Left = new(2),
                    },
                    Right = new(7)
                    {
                        Right = new(8),
                    },
                },
                Right = new(20)
                {
                    Left = new(11),
                    Right = new(26)
                    {
                        Left = new(21),
                        Right = new(30),
                    },
                },
            }));
    }

    [Fact]
    public void DeleteAFromBcad()
    {
        var set = new AvlTreeSet(Comparer<int>.Default)
        {
            Root = SetParentAndHeight(new('b')
            {
                Left = new('a'),
                Right = new('c')
                {
                    Right = new('d'),
                },
            })
        };
        ValidateTree(set.Root);
        Assert.True(set.Remove('a'));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new('c')
            {
                Left = new('b'),
                Right = new('d'),
            }));
    }

    [Fact]
    public void DeleteDFromCdba()
    {
        var set = new AvlTreeSet(Comparer<int>.Default)
        {
            Root = SetParentAndHeight(new('c')
            {
                Left = new('b')
                {
                    Left = new('a'),
                },
                Right = new('d'),
            })
        };
        ValidateTree(set.Root);
        Assert.True(set.Remove('d'));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new('b')
            {
                Left = new('a'),
                Right = new('c'),
            }));
    }

    [Fact]
    public void DeleteAFromBdac()
    {
        var set = new AvlTreeSet(Comparer<int>.Default)
        {
            Root = SetParentAndHeight(new('b')
            {
                Left = new('a'),
                Right = new('d')
                {
                    Left = new('c'),
                },
            })
        };
        ValidateTree(set.Root);
        Assert.True(set.Remove('a'));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new('c')
            {
                Left = new('b'),
                Right = new('d'),
            }));
    }

    [Fact]
    public void DeleteDFromCadb()
    {
        var set = new AvlTreeSet(Comparer<int>.Default)
        {
            Root = SetParentAndHeight(new('c')
            {
                Left = new('a')
                {
                    Right = new('b'),
                },
                Right = new('d'),
            })
        };
        ValidateTree(set.Root);
        Assert.True(set.Remove('d'));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new('b')
            {
                Left = new('a'),
                Right = new('c'),
            }));
    }

    [Fact]
    public void DeleteAFromCbedfag()
    {
        var set = new AvlTreeSet(Comparer<int>.Default)
        {
            Root = SetParentAndHeight(new('c')
            {
                Left = new('b')
                {
                    Left = new('a'),
                },
                Right = new('e')
                {
                    Left = new('d'),
                    Right = new('f')
                    {
                        Right = new('g'),
                    },
                },
            })
        };
        ValidateTree(set.Root);
        Assert.True(set.Remove('a'));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new('e')
            {
                Left = new('c')
                {
                    Left = new('b'),
                    Right = new('d'),
                },
                Right = new('f')
                {
                    Right = new('g'),
                },
            }));
    }

    [Fact]
    public void DeleteGFromEcfbdga()
    {
        var set = new AvlTreeSet(Comparer<int>.Default)
        {
            Root = SetParentAndHeight(new('e')
            {
                Left = new('c')
                {
                    Left = new('b')
                    {
                        Left = new('a'),
                    },
                    Right = new('d'),
                },
                Right = new('f')
                {
                    Right = new('g'),
                },
            })
        };
        ValidateTree(set.Root);
        Assert.True(set.Remove('g'));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new('c')
            {
                Left = new('b')
                {
                    Left = new('a'),
                },
                Right = new('e')
                {
                    Left = new('d'),
                    Right = new('f'),
                },
            }));
    }

    [Fact]
    public void DeleteTwoRotations()
    {
        var set = new AvlTreeSet(Comparer<int>.Default)
        {
            Root = SetParentAndHeight(new(5)
            {
                Left = new(2)
                {
                    Left = new(1),
                    Right = new(3)
                    {
                        Right = new(4),
                    },
                },
                Right = new(8)
                {
                    Left = new(7)
                    {
                        Left = new(6),
                    },
                    Right = new(10)
                    {
                        Left = new(9),
                        Right = new(11)
                        {
                            Right = new(12),
                        },
                    }
                },
            })
        };
        ValidateTree(set.Root);
        Assert.True(set.Remove(1));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new(8)
            {
                Left = new(5)
                {
                    Left = new(3)
                    {
                        Left = new(2),
                        Right = new(4),
                    },
                    Right = new(7)
                    {
                        Left = new(6),
                    }
                },
                Right = new(10)
                {
                    Left = new(9),
                    Right = new(11)
                    {
                        Right = new(12),
                    },
                },
            }));
    }

    [Fact]
    public void DeleteFuzzed01()
    {
        var set = new AvlTreeSet(Comparer<int>.Default)
        {
            Root = SetParentAndHeight(new(7)
            {
                Left = new(0),
                Right = new(11)
                {
                    Right = new(18),
                },
            })
        };
        ValidateTree(set.Root);
        Assert.True(set.Remove(0));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new(11)
            {
                Left = new(7),
                Right = new(18),
            }));
    }

    [Fact]
    public void DeleteFuzzed02()
    {
        var set = new AvlTreeSet(Comparer<int>.Default)
        {
            Root = SetParentAndHeight(new(8)
            {
                Left = new(5),
                Right = new(19),
            })
        };
        ValidateTree(set.Root);
        Assert.True(set.Remove(8));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new(19)
            {
                Left = new(5),
            }));
    }

    [Fact]
    public void DeleteFuzzed03()
    {
        var set = new AvlTreeSet(Comparer<int>.Default)
        {
            Root = SetParentAndHeight(new(41)
            {
                Left = new(20)
                {
                    Left = new(5)
                    {
                        Left = new(1),
                    },
                    Right = new(25)
                    {
                        Left = new(23),
                    },
                },
                Right = new(46)
                {
                    Left = new(45),
                    Right = new(57)
                    {
                        Left = new(53),
                        Right = new(58),
                    },
                },
            })
        };
        ValidateTree(set.Root);
        Assert.True(set.Remove(41));
        ValidateTree(set.Root);
        AssertTreeEquals(
            set.Root,
            SetParentAndHeight(new(45)
            {
                Left = new(20)
                {
                    Left = new(5)
                    {
                        Left = new(1),
                    },
                    Right = new(25)
                    {
                        Left = new(23),
                    },
                },
                Right = new(57)
                {
                    Left = new(46)
                    {
                        Right = new(53),
                    },
                    Right = new(58),
                },
            }));
    }
}
