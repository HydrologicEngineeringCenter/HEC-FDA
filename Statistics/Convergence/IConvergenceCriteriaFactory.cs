using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    public static class IConvergenceCriteriaFactory
    {
        public static IConvergenceCriteria Factory(double quantile = 0.5, double tolerance = 0.1, int minNewObservations = 30, Utilities.IRange<int> testRange = null) => 
            new ConvergeCriteria(quantile, tolerance, minNewObservations, testRange);

    }
}
