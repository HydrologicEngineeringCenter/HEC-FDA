using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public static class IRangeFactory
    {
        public static IRange<double> Factory(double min, double max, bool inclusiveMin = true, bool inclusiveMax = true, bool finiteRequirement = true, bool minNotEqualToMaxRequirement = true) => new Ranges.RangeDouble(min, max, inclusiveMin, inclusiveMax, finiteRequirement, minNotEqualToMaxRequirement);
        public static IRange<int> Factory(int min, int max) => new Ranges.RangeInteger(min, max, true, true);
    }
}
