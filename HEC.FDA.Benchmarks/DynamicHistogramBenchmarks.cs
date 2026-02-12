using BenchmarkDotNet.Attributes;
using Statistics;
using Statistics.Histograms;

namespace HEC.FDA.Benchmarks;

[MemoryDiagnoser]
public class DynamicHistogramBenchmarks
{
    private double[] _data = null!;

    [Params(10_000)]
    public int ObservationCount { get; set; }

    [Params(0.0001, 1.0)]
    public double BinWidth { get; set; }

    [Params("0_to_10", "0_to_100000", "0_to_100000000")]
    public string Range { get; set; } = null!;

    [GlobalSetup]
    public void Setup()
    {
        var rng = new System.Random(42);
        (double min, double max) = Range switch
        {
            "0_to_10" => (0.0, 10.0),
            "0_to_100000" => (0.0, 100_000.0),
            "0_to_100000000" => (0.0, 100_000_000.0),
            _ => throw new System.ArgumentException($"Unknown range: {Range}")
        };

        _data = new double[ObservationCount];
        for (int i = 0; i < ObservationCount; i++)
            _data[i] = min + rng.NextDouble() * (max - min);
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
