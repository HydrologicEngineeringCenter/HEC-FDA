# HEC.FDA.Benchmarks

BenchmarkDotNet project for profiling performance and memory allocations across the FDA codebase.

## Running Benchmarks

Benchmarks **must** be run in Release mode for accurate results.

```bash
# Run all benchmarks (will prompt to choose a class)
dotnet run --project HEC.FDA.Benchmarks/HEC.FDA.Benchmarks.csproj -c Release

# Run a specific benchmark class
dotnet run --project HEC.FDA.Benchmarks/HEC.FDA.Benchmarks.csproj -c Release -- --filter "*SampleBenchmarks*"

# Run a specific method
dotnet run --project HEC.FDA.Benchmarks/HEC.FDA.Benchmarks.csproj -c Release -- --filter "*SampleBenchmarks.Placeholder*"
```

## Adding a New Benchmark

1. Create a new `.cs` file in this project.
2. Add a class with methods marked `[Benchmark]`.
3. Optionally add `[MemoryDiagnoser]` to the class to track allocations.

```csharp
using BenchmarkDotNet.Attributes;

namespace HEC.FDA.Benchmarks;

[MemoryDiagnoser]
public class MyBenchmarks
{
    [GlobalSetup]
    public void Setup()
    {
        // One-time setup before benchmarks run.
    }

    [Benchmark]
    public void MyMethod()
    {
        // Code to benchmark.
    }
}
```

The `BenchmarkSwitcher` in `Program.cs` discovers all benchmark classes in the assembly automatically.

## Project References

This project references `HEC.FDA.Model` and `HEC.FDA.Statistics`. Add additional `<ProjectReference>` entries to the `.csproj` if you need to benchmark code from other projects.
