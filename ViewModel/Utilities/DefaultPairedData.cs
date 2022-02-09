using paireddata;
using Statistics.Distributions;
using System.Collections.Generic;

namespace ViewModel.Utilities
{
    public static class DefaultPairedData
    {

        public static UncertainPairedData CreateDefaultNormalUncertainPairedData(string xLabel, string yLabel, string name)
        {
            double[] xs = new double[10];
            Normal[] ys = new Normal[10];
            for (int i = 0; i < 10; i++)
            {
                xs[i] = i;
                ys[i] = new Normal(i, 0);
            }
            UncertainPairedData curve = new UncertainPairedData(xs, ys, xLabel, yLabel, name, "", -1);
            return curve;
        }
        public static UncertainPairedData CreateDefaultDeterminateUncertainPairedData(string xLabel, string yLabel, string name)
        {
            double[] xs = new double[10];
            Deterministic[] ys = new Deterministic[10];
            for (int i = 0; i < 10; i++)
            {
                xs[i] = i;
                ys[i] = new Deterministic(i);
            }
            UncertainPairedData curve = new UncertainPairedData(xs, ys, xLabel, yLabel, name, "", -1);
            return curve;
        }

        public static UncertainPairedData CreateDefaultDeterminateUncertainPairedData(List<double> xs, List<double> ys, string xLabel, string yLabel, string name)
        {
            List<Deterministic> yVals = new List<Deterministic>();
            foreach(double d in ys)
            {
                yVals.Add(new Deterministic(d));
            }
            UncertainPairedData curve = new UncertainPairedData(xs.ToArray(), yVals.ToArray(), xLabel, yLabel, name, "", -1);
            return curve;
        }


    }
}
