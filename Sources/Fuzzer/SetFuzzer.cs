// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures.NET.Trees.Linked;
using BstSetLinked = DataStructures.NET.Trees.Linked.BinarySearchTreeSetLinked<int, System.Collections.Generic.IComparer<int>>;
using BstSetArray = DataStructures.NET.Trees.Array.BinarySearchTreeSetArray<int, System.Collections.Generic.IComparer<int>>;
using AvlTreeSetLinked = DataStructures.NET.Trees.Linked.AvlTreeSetLinked<int, System.Collections.Generic.IComparer<int>>;
using RedBlackTreeSetLinked = DataStructures.NET.Trees.Linked.RedBlackTreeSetLinked<int, System.Collections.Generic.IComparer<int>>;

namespace Fuzzer;

internal class SetFuzzer
{
    private class BstSetLinkedValidator : IValidator<BstSetLinked, ISet<int>>
    {
        public static BstSetLinkedValidator Instance { get; } = new();

        public string ToTestCase(BstSetLinked tested) =>
            TreeValidation.ToTestCaseString(tested.Root, default(BstSetLinked.NodeAdapter), n => n.Key.ToString());

        public void Validate(BstSetLinked tested, ISet<int> oracle)
        {
            TreeValidation.ValidateAdjacency(tested.Root, default(BstSetLinked.NodeAdapter));
            TreeValidation.ValidateData(tested.Root, default(BstSetLinked.NodeAdapter), n => n.Key, oracle);
        }
    }

    private class BstSetArrayValidator : IValidator<BstSetArray, ISet<int>>
    {
        public static BstSetArrayValidator Instance { get; } = new();

        public string ToTestCase(BstSetArray tested) =>
            TreeValidation.ToTestCaseString(tested.Root, tested.Adapter, n => tested.Adapter.container.Keys[n].ToString());

        public void Validate(BstSetArray tested, ISet<int> oracle)
        {
            TreeValidation.ValidateAdjacency(tested.Root, tested.Adapter);
            TreeValidation.ValidateData(tested.Root, tested.Adapter, n => tested.Adapter.container.Keys[n], oracle);
        }
    }

    private class AvlTreeValidator : IValidator<AvlTreeSetLinked, ISet<int>>
    {
        public static AvlTreeValidator Instance { get; } = new();

        public string ToTestCase(AvlTreeSetLinked tested) =>
            TreeValidation.ToTestCaseString(tested.Root, default(AvlTreeSetLinked.NodeAdapter), n => n.Key.ToString());

        public void Validate(AvlTreeSetLinked tested, ISet<int> oracle)
        {
            TreeValidation.ValidateAdjacency(tested.Root, default(AvlTreeSetLinked.NodeAdapter));
            TreeValidation.ValidateData(tested.Root, default(AvlTreeSetLinked.NodeAdapter), n => n.Key, oracle);
            TreeValidation.ValidateBalanceAndHeight(tested.Root, default(AvlTreeSetLinked.NodeAdapter));
        }
    }

    private class RedBlackTreeValidator : IValidator<RedBlackTreeSetLinked, ISet<int>>
    {
        public static RedBlackTreeValidator Instance { get; } = new();

        public string ToTestCase(RedBlackTreeSetLinked tested) =>
            TreeValidation.ToTestCaseString(
                tested.Root,
                default(RedBlackTreeSetLinked.NodeAdapter),
                n => $"{n.Key}, {n.Color}");

        public void Validate(RedBlackTreeSetLinked tested, ISet<int> oracle)
        {
            TreeValidation.ValidateAdjacency(tested.Root, default(RedBlackTreeSetLinked.NodeAdapter));
            TreeValidation.ValidateData(tested.Root, default(RedBlackTreeSetLinked.NodeAdapter), n => n.Key, oracle);
            TreeValidation.ValidateRedBlack(tested.Root, default(RedBlackTreeSetLinked.NodeAdapter));
        }
    }

    public static void FuzzBinarySearchTreeSetLinked(int maxElements) =>
        Fuzz(maxElements, () => new BstSetLinked(Comparer<int>.Default), BstSetLinkedValidator.Instance);

    public static void FuzzBinarySearchTreeSetArray(int maxElements) =>
        Fuzz(maxElements, () => new BstSetArray(Comparer<int>.Default), BstSetArrayValidator.Instance);

    public static void FuzzAvlTreeSetLinked(int maxElements) =>
        Fuzz(maxElements, () => new AvlTreeSetLinked(Comparer<int>.Default), AvlTreeValidator.Instance);

    public static void FuzzRedBlackTreeSetLinked(int maxElements) =>
        Fuzz(maxElements, () => new RedBlackTreeSetLinked(Comparer<int>.Default), RedBlackTreeValidator.Instance);

    public static void Fuzz<TTested>(
        int maxElements,
        Func<TTested> makeSet,
        IValidator<TTested, ISet<int>> validator)
        where TTested : ISet<int>
    {
        var rnd = new Random();
        for (var epoch = 0; ; ++epoch)
        {
            if (epoch % 100 == 0) Console.WriteLine($"Epoch {epoch}...");

            var tested = makeSet();
            var oracle = new HashSet<int>();
            try
            {
                validator.Validate(tested, oracle);
            }
            catch (Exception v)
            {
                throw new FuzzerException(v, "<empty>", "ctor");
            }

            while (tested.Count < maxElements)
            {
                var testCase = validator.ToTestCase(tested);

                var n = rnd.Next(0, maxElements * 4);
                var operation = $"Insert({n})";
                var testedInsert = tested.Add(n);
                var oracleInsert = oracle.Add(n);
                if (testedInsert != oracleInsert) throw new FuzzerException($"Insertion return value mismatch (oracle: {oracleInsert}, tested: {testedInsert})", testCase, operation);
                try
                {
                    validator.Validate(tested, oracle);
                }
                catch (ValidationException v)
                {
                    throw new FuzzerException(v, testCase, operation);
                }
            }

            while (tested.Count > 0)
            {
                var testCase = validator.ToTestCase(tested);

                var n = rnd.Next(0, maxElements * 4);
                var operation = $"Delete({n})";
                var testedDelete = tested.Remove(n);
                var oracleDelete = oracle.Remove(n);
                if (testedDelete != oracleDelete) throw new FuzzerException($"Deletion return value mismatch (oracle: {oracleDelete}, tested: {testedDelete})", testCase, operation);
                try
                {
                    validator.Validate(tested, oracle);
                }
                catch (Exception v)
                {
                    throw new FuzzerException(v, testCase, operation);
                }
            }
        }
    }
}
