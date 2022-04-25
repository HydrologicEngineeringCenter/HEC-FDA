using paireddata;
using Statistics.Distributions;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Utilities
{
    public static class UncertainPairedDataFactory
    {

        public static UncertainPairedData CreateDefaultNormalData(string xLabel, string yLabel, string name)
        {
            double[] xs = new double[10];
            Normal[] ys = new Normal[10];
            for (int i = 0; i < 10; i++)
            {
                xs[i] = i;
                ys[i] = new Normal(i, 0);
            }
            UncertainPairedData curve = new UncertainPairedData(xs, ys, xLabel, yLabel, name, "");
            return curve;
        }
        public static UncertainPairedData CreateDefaultDeterminateData(string xLabel, string yLabel, string name)
        {
            double[] xs = new double[10];
            Deterministic[] ys = new Deterministic[10];
            for (int i = 0; i < 10; i++)
            {
                xs[i] = i;
                ys[i] = new Deterministic(i);
            }
            UncertainPairedData curve = new UncertainPairedData(xs, ys, xLabel, yLabel, name, "");
            return curve;
        }

        public static UncertainPairedData CreateDeterminateData(List<double> xs, List<double> ys, string xLabel, string yLabel, string name)
        {
            List<Deterministic> yVals = new List<Deterministic>();
            foreach(double d in ys)
            {
                yVals.Add(new Deterministic(d));
            }
            UncertainPairedData curve = new UncertainPairedData(xs.ToArray(), yVals.ToArray(), xLabel, yLabel, name, "");
            return curve;
        }
        public static UncertainPairedData CreateDeterminateData(double[] xs, double[] ys, string xLabel, string yLabel, string name)
        {
            List<Deterministic> yVals = new List<Deterministic>();
            foreach (double d in ys)
            {
                yVals.Add(new Deterministic(d));
            }
            UncertainPairedData curve = new UncertainPairedData(xs, yVals.ToArray(), xLabel, yLabel, name, "");
            return curve;
        }

        public static UncertainPairedData CreateLP3Data(LogPearson3 lp3)
        {
            double[] probs = new double[] { .001, .01, .05, .25, .5, .75, .95, .99, .999 };
            Deterministic[] yVals = new Deterministic[probs.Length];
            for(int i = 0; i<probs.Length;i++)
            {
                yVals[i] = new Deterministic( lp3.InverseCDF(probs[i]));
            }
            return new UncertainPairedData(probs, yVals, StringConstants.EXCEEDANCE_PROBABILITY, StringConstants.DISCHARGE, StringConstants.FREQUENCY_RELATIONSHIP, "");
        }


    }
}
