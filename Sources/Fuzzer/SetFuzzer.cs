// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures.NET.Trees.Linked;
using BstSet = DataStructures.NET.Trees.Linked.BinarySearchTreeSet<int, System.Collections.Generic.IComparer<int>>;
using AvlTreeSet = DataStructures.NET.Trees.Linked.AvlTreeSet<int, System.Collections.Generic.IComparer<int>>;
using RedBlackTreeSet = DataStructures.NET.Trees.Linked.RedBlackTreeSet<int, System.Collections.Generic.IComparer<int>>;

namespace Fuzzer;

internal class SetFuzzer
{
    private class BstValidator : IValidator<BstSet, ISet<int>>
    {
        public static BstValidator Instance { get; } = new();

        public string ToTestCase(BstSet tested) =>
            TreeValidation.ToTestCaseString(tested.Root, default(BstSet.NodeAdapter), n => n.Key.ToString());

        public void Validate(BstSet tested, ISet<int> oracle)
        {
            TreeValidation.ValidateAdjacency(tested.Root, default(BstSet.NodeAdapter));
            TreeValidation.ValidateData(tested.Root, default(BstSet.NodeAdapter), n => n.Key, oracle);
        }
    }

    private class AvlTreeValidator : IValidator<AvlTreeSet, ISet<int>>
    {
        public static AvlTreeValidator Instance { get; } = new();

        public string ToTestCase(AvlTreeSet tested) =>
            TreeValidation.ToTestCaseString(tested.Root, default(AvlTreeSet.NodeAdapter), n => n.Key.ToString());

        public void Validate(AvlTreeSet tested, ISet<int> oracle)
        {
            TreeValidation.ValidateAdjacency(tested.Root, default(AvlTreeSet.NodeAdapter));
            TreeValidation.ValidateData(tested.Root, default(AvlTreeSet.NodeAdapter), n => n.Key, oracle);
            TreeValidation.ValidateBalanceAndHeight(tested.Root, default(AvlTreeSet.NodeAdapter));
        }
    }

    private class RedBlackTreeValidator : IValidator<RedBlackTreeSet, ISet<int>>
    {
        public static RedBlackTreeValidator Instance { get; } = new();

        public string ToTestCase(RedBlackTreeSet tested) =>
            TreeValidation.ToTestCaseString(tested.Root, default(RedBlackTreeSet.NodeAdapter), n => n.Key.ToString());

        public void Validate(RedBlackTreeSet tested, ISet<int> oracle)
        {
            TreeValidation.ValidateAdjacency(tested.Root, default(RedBlackTreeSet.NodeAdapter));
            TreeValidation.ValidateData(tested.Root, default(RedBlackTreeSet.NodeAdapter), n => n.Key, oracle);
            TreeValidation.ValidateRedBlack(tested.Root, default(RedBlackTreeSet.NodeAdapter));
        }
    }

    public static void FuzzBst(int maxElements) =>
        Fuzz(maxElements, () => new BstSet(Comparer<int>.Default), BstValidator.Instance);

    public static void FuzzAvlTree(int maxElements) =>
        Fuzz(maxElements, () => new AvlTreeSet(Comparer<int>.Default), AvlTreeValidator.Instance);

    public static void FuzzRedBlackTree(int maxElements) =>
        Fuzz(maxElements, () => new RedBlackTreeSet(Comparer<int>.Default), RedBlackTreeValidator.Instance);

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
            catch (ValidationException v)
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
                catch (ValidationException v)
                {
                    throw new FuzzerException(v, testCase, operation);
                }
            }
        }
    }
}
