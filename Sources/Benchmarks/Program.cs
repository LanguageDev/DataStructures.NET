using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Running;
using Benchmarks;
using CodegenAnalysis;
using CodegenAnalysis.Benchmarks;
using DataStructures.NET;

// CodegenBenchmarkRunner.Run<BinarySearchTreeCodegenBenchmarks>();
BenchmarkRunner.Run<BinarySearchTreeBenchmarks>();
