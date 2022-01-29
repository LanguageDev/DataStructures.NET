using BenchmarkDotNet.Running;
using Benchmarks.Trees;

// CodegenBenchmarkRunner.Run<BinarySearchTreeCodegenBenchmarks>();
BenchmarkRunner.Run<AvlTreeInsertBenchmarks>();
BenchmarkRunner.Run<AvlTreeDeleteBenchmarks>();
