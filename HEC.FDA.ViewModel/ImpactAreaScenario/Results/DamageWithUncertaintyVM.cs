using HEC.Plotting.SciChart2D.DataModel;
using System.Collections.Generic;
using HEC.FDA.ViewModel.ImpactAreaScenario.Results.RowItems;
using HEC.FDA.Model.metrics;
using Statistics.Distributions;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Results
{
    public class DamageWithUncertaintyVM : BaseViewModel
    {
        public PlotModel MyPlot { get; set; } = new PlotModel();
        public List<EadRowItem> Rows { get; } = new List<EadRowItem>();
        public double Mean { get; set; }
        public DamageWithUncertaintyVM(ScenarioResults scenarioResults, int impactAreaID)
        {
            ImpactAreaScenarioResults iasResult = scenarioResults.GetResults(impactAreaID);
            Mean = iasResult.MeanExpectedAnnualConsequences(impactAreaID: impactAreaID);

            DefineHistogramSettings(iasResult);

            List<double> qValues = new List<double>();
            qValues.Add(scenarioResults.ConsequencesExceededWithProbabilityQ(.75, impactAreaID));
            qValues.Add(scenarioResults.ConsequencesExceededWithProbabilityQ(.5, impactAreaID));
            qValues.Add(scenarioResults.ConsequencesExceededWithProbabilityQ(.25, impactAreaID));

            loadTableValues(qValues);
        }

        private void DefineHistogramSettings(ImpactAreaScenarioResults iasResult)
        {
            Empirical empirical = iasResult.ConsequenceResults.GetAggregateEmpiricalDistribution(impactAreaID: iasResult.ImpactAreaID);
            (double min, double valueStep, double[] cumulativeRelativeFrequencies) = empirical.ComputeCumulativeFrequenciesForPlotting();
            double[] damageValues = new double[cumulativeRelativeFrequencies.Length];

            double damageValue = min;
            for (int i =0; i< cumulativeRelativeFrequencies.Length;i++)
            {
                damageValues[i] = (damageValue);
                damageValue += valueStep;
            }

            PlotModel model = new PlotModel();
            model.Title = "EAD Distribution";
            double MajorAxisTickInterval = (empirical.Max- empirical.Min)/5;
            model.Axes.Add(new LinearAxis() { Title = "Expected Annual Damage ($)", Position = AxisPosition.Bottom, MajorStep = MajorAxisTickInterval });
            model.Axes.Add(new LinearAxis() { Title = "Frequency", Position = AxisPosition.Left});
            var lineSeries = new LineSeries();
            for(int i =0; i < cumulativeRelativeFrequencies.Length; i++)
            {
                lineSeries.Points.Add(new DataPoint(damageValues[i], cumulativeRelativeFrequencies[i]));
            }
            lineSeries.Title = "EAD Distribution";
            model.Series.Add(lineSeries);
            MyPlot = model;
            MyPlot.InvalidatePlot(true);
        }

        private void loadTableValues(List<double> qValues)
        {
            List<EadRowItem> rows = new List<EadRowItem>();
            if (qValues.Count == 3)
            {
                List<double> xVals = new List<double>() { .75, .5, .25 };
                List<string> xValNames = new List<string>() { "First", "Second", "Third" };

                for (int i = 0; i < xValNames.Count; i++)
                {
                    rows.Add(new EadRowItem(xValNames[i], qValues[i]));
                }
            }
            Rows.AddRange(rows);
        }

    }
}
