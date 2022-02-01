using System.Reflection;
using BenchmarkDotNet.Running;
using Benchmarks.Trees;

// CodegenBenchmarkRunner.Run<BinarySearchTreeCodegenBenchmarks>();
//BenchmarkRunner.Run(Assembly.GetExecutingAssembly());
BenchmarkRunner.Run<BstInsertBenchmarks>();
BenchmarkRunner.Run<BstDeleteBenchmarks>();
