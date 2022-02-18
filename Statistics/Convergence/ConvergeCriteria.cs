using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics
{
    public class ConvergenceCriteria
    {
        public Int64 MinIterations { get; }
        public Int64 MaxIterations { get; }
        public double ZAlpha { get; }
        public double Tolerance { get; }

        public ConvergenceCriteria(Int64 minIterations = 100, Int64 maxIterations = 100000, double zAlpha = 1.96039491692543, double tolerance = .01)
        {
            MinIterations = minIterations;
            MaxIterations = maxIterations;
            ZAlpha = zAlpha;
            Tolerance = tolerance;
        }        
    }
}
