using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Output
{
    public class HistogramBinVM
    {
        public double Min { get; }
        public double Max { get;  }
        public double MidPoint { get; }
        public int Count { get; }
        public double BinWidth { get;  }

        public HistogramBinVM(IBin bin)
        {
            Count = bin.Count;
            Min = bin.Range.Min;
            Max = bin.Range.Max;
            MidPoint = bin.MidPoint;
            BinWidth = Max - Min;
        }

    }
}
