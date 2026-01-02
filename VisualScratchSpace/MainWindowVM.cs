using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.LifeLoss;
using Statistics;
using Statistics.Distributions;

namespace VisualScratchSpace;

public class MainWindowVM
{
    public LifeLossFnChartVM LifeLossFnChartViewModel { get; }

    public MainWindowVM()
    {
        var fakeData = CreateFakeLifeRiskData();
        LifeLossFnChartViewModel = new LifeLossFnChartVM(fakeData, "Life Loss Function - Test Data");
    }

    /// <summary>
    /// Creates fake UncertainPairedData for testing the Life Risk Matrix control.
    /// X values represent Average Life Loss, Y values represent AEP distributions.
    /// </summary>
    private static UncertainPairedData CreateFakeLifeRiskData()
    {
        // Average Life Loss values (X-axis) - spanning the log scale
        double[] lifeLossValues = [0.5, 1, 2, 5, 10, 20, 50, 100, 200, 500, 1000];

        // Create uncertain AEP distributions (Y-axis) for each life loss value
        // As life loss increases, AEP should generally decrease (inverse relationship)
        IDistribution[] aepDistributions = new IDistribution[lifeLossValues.Length];

        for (int i = 0; i < lifeLossValues.Length; i++)
        {
            double lifeLoss = lifeLossValues[i];

            // Base AEP decreases as life loss increases (roughly following societal risk line)
            // Using log-linear relationship: AEP ~ 0.01 / LifeLoss (with some variation)
            double baseAep = 0.05 / lifeLoss;

            // Clamp to valid AEP range
            baseAep = Math.Max(1E-06, Math.Min(0.5, baseAep));

            // Create a triangular distribution around the base AEP
            // This gives us min/median/max when sampled at different quantiles
            double minAep = baseAep * 0.3;   // Lower bound
            double maxAep = baseAep * 3.0;   // Upper bound

            // Clamp to valid range
            minAep = Math.Max(1E-06, minAep);
            maxAep = Math.Min(1.0, maxAep);

            // Ensure mode is between min and max
            double modeAep = baseAep;
            if (modeAep <= minAep) modeAep = minAep * 1.1;
            if (modeAep >= maxAep) modeAep = maxAep * 0.9;

            aepDistributions[i] = new Triangular(minAep, modeAep, maxAep);
        }

        var metadata = new CurveMetaData("Life Risk Data", "Average Life Loss", "Annual Exceedance Probability");
        return new UncertainPairedData(lifeLossValues, aepDistributions, metadata);
    }
}
