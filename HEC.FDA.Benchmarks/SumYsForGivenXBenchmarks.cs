using BenchmarkDotNet.Attributes;
using HEC.FDA.Model.paireddata;

namespace HEC.FDA.Benchmarks;

[MemoryDiagnoser]
public class SumYsForGivenXBenchmarks
{
    private PairedData _subject = null!;
    private PairedData _input = null!;

    [Params("same_grid", "disjoint_grids", "input_sparse", "subject_sparse")]
    public string Scenario { get; set; } = null!;

    [GlobalSetup]
    public void Setup()
    {
        switch (Scenario)
        {
            case "same_grid":
            {
                // Both curves have the same 200-point x-grid (typical stage-damage resolution)
                double[] x = new double[200];
                double[] ySubject = new double[200];
                double[] yInput = new double[200];
                for (int i = 0; i < 200; i++)
                {
                    x[i] = i * 0.1;
                    ySubject[i] = i * 10.0;
                    yInput[i] = i * 15.0;
                }
                _subject = new PairedData(x, ySubject);
                _input = new PairedData((double[])x.Clone(), yInput);
                break;
            }
            case "disjoint_grids":
            {
                // Two curves with completely different x-grids (worst case for union)
                double[] xSubject = new double[200];
                double[] ySubject = new double[200];
                double[] xInput = new double[200];
                double[] yInput = new double[200];
                for (int i = 0; i < 200; i++)
                {
                    xSubject[i] = i * 0.2;       // 0.0, 0.2, 0.4, ...
                    ySubject[i] = i * 10.0;
                    xInput[i] = i * 0.2 + 0.1;   // 0.1, 0.3, 0.5, ...
                    yInput[i] = i * 15.0;
                }
                _subject = new PairedData(xSubject, ySubject);
                _input = new PairedData(xInput, yInput);
                break;
            }
            case "input_sparse":
            {
                // Subject has 200 points, input has only 2 — the bug scenario
                double[] xSubject = new double[200];
                double[] ySubject = new double[200];
                for (int i = 0; i < 200; i++)
                {
                    xSubject[i] = i * 0.1;
                    ySubject[i] = i * 10.0;
                }
                double[] xInput = new double[] { 0.0, 19.9 };
                double[] yInput = new double[] { 0.0, 2985.0 };
                _subject = new PairedData(xSubject, ySubject);
                _input = new PairedData(xInput, yInput);
                break;
            }
            case "subject_sparse":
            {
                // Subject has only 2 points, input has 200
                double[] xSubject = new double[] { 0.0, 19.9 };
                double[] ySubject = new double[] { 0.0, 1990.0 };
                double[] xInput = new double[200];
                double[] yInput = new double[200];
                for (int i = 0; i < 200; i++)
                {
                    xInput[i] = i * 0.1;
                    yInput[i] = i * 15.0;
                }
                _subject = new PairedData(xSubject, ySubject);
                _input = new PairedData(xInput, yInput);
                break;
            }
        }
    }

    [Benchmark]
    public PairedData SumYsForGivenX()
    {
        return _subject.SumYsForGivenX(_input);
    }
}
