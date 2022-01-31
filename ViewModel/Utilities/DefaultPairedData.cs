using paireddata;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
