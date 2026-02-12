using BenchmarkDotNet.Attributes;
using Statistics;
using Statistics.Distributions;
using Statistics.Histograms;

namespace HEC.FDA.Benchmarks;

[MemoryDiagnoser]
public class DynamicHistogramBenchmarks
{
    private double[] _data = null!;

    [Params(10_000)]
    public int ObservationCount { get; set; }

    [Params(0.0001, 1.0, .000000001, 10000)]
    public double BinWidth { get; set; }

    [Params("uniform_0_to_1", "uniform_0_to_100000000", "triangular_small", "triangular_large")]
    public string Distribution { get; set; } = null!;

    [GlobalSetup]
    public void Setup()
    {
        var rng = new System.Random(42);
        _data = new double[ObservationCount];

        switch (Distribution)
        {
            case "uniform_0_to_1":
                for (int i = 0; i < ObservationCount; i++)
                    _data[i] = rng.NextDouble();
                break;
            case "uniform_0_to_100000000":
                for (int i = 0; i < ObservationCount; i++)
                    _data[i] = rng.NextDouble() * 100_000_000.0;
                break;
            case "triangular_small":
                var smallTriangular = new Triangular(0.0, 0.05, 1.0); //Heavily skewed small to stress bin reallocation
                for (int i = 0; i < ObservationCount; i++)
                    _data[i] = smallTriangular.InverseCDF(rng.NextDouble());
                break;
            case "triangular_large":
                var largeTriangular = new Triangular(0.0, 100_000.0, 100_000_000.0);//Heavily skewed small to stress bin reallocation
                for (int i = 0; i < ObservationCount; i++)
                    _data[i] = largeTriangular.InverseCDF(rng.NextDouble());
                break;
            default:
                throw new System.ArgumentException($"Unknown distribution: {Distribution}");
        }
    }

    [Benchmark]
    public DynamicHistogram AddObservations()
    {
        var cc = new ConvergenceCriteria(minIterations: 100, maxIterations: ObservationCount);
        var histogram = new DynamicHistogram(BinWidth, cc);
        foreach (double obs in _data)
        {
            histogram.AddObservationToHistogram(obs);
        }
        return histogram;
    }
}

// Benchmark Results — 2026-02-12
//
// | Method          | ObservationCount | BinWidth | Distribution         | Mean      | Error    | StdDev   | Gen0    | Gen1   | Allocated |
// |---------------- |----------------- |--------- |--------------------- |----------:|---------:|---------:|--------:|-------:|----------:|
// | AddObservations | 10000            | 1E-09    | triangular_large     | 111.56 us | 0.373 us | 0.349 us | 19.4092 | 1.4648 | 158.66 KB |
// | AddObservations | 10000            | 1E-09    | triangular_small     | 111.41 us | 0.423 us | 0.375 us | 20.9961 | 1.4648 | 172.16 KB |
// | AddObservations | 10000            | 1E-09    | uniform_0_to_1       | 103.42 us | 0.528 us | 0.493 us | 13.7939 | 0.6104 | 113.21 KB |
// | AddObservations | 10000            | 1E-09    | uniform_0_to_100M    | 101.32 us | 0.509 us | 0.477 us | 13.7939 | 0.6104 | 113.25 KB |
// | AddObservations | 10000            | 0.0001   | triangular_large     | 113.27 us | 0.543 us | 0.508 us | 19.4092 | 0.7324 | 158.63 KB |
// | AddObservations | 10000            | 0.0001   | triangular_small     | 111.65 us | 0.369 us | 0.345 us | 22.3389 | 1.7090 | 182.52 KB |
// | AddObservations | 10000            | 0.0001   | uniform_0_to_1       | 105.59 us | 0.336 us | 0.314 us | 14.6484 | 0.7324 | 120.65 KB |
// | AddObservations | 10000            | 0.0001   | uniform_0_to_100M    | 103.65 us | 1.248 us | 1.168 us | 13.7939 | 0.6104 | 113.21 KB |
// | AddObservations | 10000            | 1        | triangular_large     | 110.66 us | 1.327 us | 1.241 us | 19.4092 | 0.7324 | 158.63 KB |
// | AddObservations | 10000            | 1        | triangular_small     |  82.22 us | 0.085 us | 0.075 us |  0.1221 |      - |   1.29 KB |
// | AddObservations | 10000            | 1        | uniform_0_to_1       |  82.33 us | 0.080 us | 0.075 us |  0.1221 |      - |   1.29 KB |
// | AddObservations | 10000            | 1        | uniform_0_to_100M    | 104.74 us | 1.738 us | 1.541 us | 13.7939 | 0.6104 | 113.21 KB |
// | AddObservations | 10000            | 10000    | triangular_large     | 113.03 us | 0.808 us | 0.716 us | 19.5313 | 1.2207 | 161.06 KB |
// | AddObservations | 10000            | 10000    | triangular_small     |  83.03 us | 0.952 us | 0.795 us |  0.1221 |      - |   1.29 KB |
// | AddObservations | 10000            | 10000    | uniform_0_to_1       |  82.89 us | 0.759 us | 0.673 us |  0.1221 |      - |   1.29 KB |
// | AddObservations | 10000            | 10000    | uniform_0_to_100M    | 109.62 us | 0.643 us | 0.602 us | 14.6484 | 0.7324 | 120.65 KB |
//
// Findings:
// The initial bin width has negligible impact on execution time or memory allocation in most cases.
// This is because the histogram's early resize logic discards and rebuilds the initial bin structure
// almost immediately once observations arrive. The bin width is effectively overridden during the
// first few additions, so whether it starts at 1E-09 or 10000 makes little difference for performance.
//
// The exception is when the bin width is large relative to the data range (e.g., BinWidth=1 or 10000
// with triangular_small [0–1] or uniform_0_to_1). In those cases, all observations collapse into one
// or a few bins (~1.29 KB allocated, no GC pressure), which is faster but destroys distribution
// resolution entirely — the histogram becomes useless for representing the underlying distribution.
//
// Action: Histograms will now be initialized with bin widths small enough to fit typical AALL ranges
// (0.0001) to preserve distribution resolution without any meaningful performance penalty.
