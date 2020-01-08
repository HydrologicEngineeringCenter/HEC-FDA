using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public static class IRangeFactory
    {
        public static IRange<double> Factory(double min, double max, bool inclusiveMin = true, bool inclusiveMax = true) => new Ranges.RangeDouble(min, max, inclusiveMin, inclusiveMax);
    }
}
