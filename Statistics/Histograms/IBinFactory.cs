using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics.Histograms
{
    public static class IBinFactory
    {
        public static IBin Factory(double min, double max, int count)
        {
            return new Bin(min, max, count);
        }
    }
}
