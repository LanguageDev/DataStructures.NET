using System;
using System.Collections.Generic;
using System.Diagnostics;
using DataStructures.NET.Trees.Linked;
using Fuzzer;
using BstSet = DataStructures.NET.Trees.Linked.BinarySearchTreeSet<int, System.Collections.Generic.IComparer<int>>;

internal class Program
{
    internal static void Validate(BstSet tested, HashSet<int> oracle)
    {
        TreeValidation.ValidateAdjacency(tested.Root, default(BstSet.NodeAdapter));
        TreeValidation.ValidateData(tested.Root, default(BstSet.NodeAdapter), n => n.Key, oracle);
    }

    internal static void FuzzBstSet(int maxElements)
    {
        var rnd = new Random();
        for (var epoch = 0; ; ++epoch)
        {
            if (epoch % 100 == 0) Console.WriteLine($"Epoch {epoch}...");

            var tested = new BstSet(Comparer<int>.Default);
            var oracle = new HashSet<int>();
            try
            {
                Validate(tested, oracle);
            }
            catch (ValidationException v)
            {
                throw new FuzzerException(v, "<empty>", "ctor");
            }

            while (tested.Count < maxElements)
            {
                var testCase = TreeValidation.ToTestCaseString(
                    tested.Root,
                    default(BstSet.NodeAdapter),
                    n => n.Key.ToString());

                var n = rnd.Next(0, maxElements * 4);
                var operation = $"Insert({n})";
                var testedInsert = tested.Add(n);
                var oracleInsert = oracle.Add(n);
                if (testedInsert != oracleInsert) throw new FuzzerException($"Insertion return value mismatch (oracle: {oracleInsert}, tested: {testedInsert})", testCase, operation);
                try
                {
                    Validate(tested, oracle);
                }
                catch (ValidationException v)
                {
                    throw new FuzzerException(v, testCase, operation);
                }
            }

            while (tested.Count > 0)
            {
                var testCase = TreeValidation.ToTestCaseString(
                    tested.Root,
                    default(BstSet.NodeAdapter),
                    n => n.Key.ToString());

                Debug.Assert(tested.Count == oracle.Count);

                var n = rnd.Next(0, maxElements * 4);
                var operation = $"Delete({n})";
                var testedDelete = tested.Remove(n);
                var oracleDelete = oracle.Remove(n);
                if (testedDelete != oracleDelete) throw new FuzzerException($"Deletion return value mismatch (oracle: {oracleDelete}, tested: {testedDelete})", testCase, operation);
                try
                {
                    Validate(tested, oracle);
                }
                catch (ValidationException v)
                {
                    throw new FuzzerException(v, testCase, operation);
                }
            }
        }
    }

    internal static void Main(string[] args)
    {
        try
        {
            FuzzBstSet(100);
        }
        catch (FuzzerException f)
        {
            Console.WriteLine(f.Message);
        }
    }
}
