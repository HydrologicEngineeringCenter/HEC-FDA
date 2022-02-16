using System;

namespace HEC.FDA.ViewModel.Output
{
    public class HistogramBinVM
    {
        public double Min { get; }
        public double Max { get;  }
        public double MidPoint { get; }
        public int Count { get; }
        public double BinWidth { get;  }

        public HistogramBinVM(Int64 count, double min, double max)
        {
            Count = (int)count;
            Min = min;
            Max = max;
            BinWidth = Max - Min;
            MidPoint = BinWidth/2;
            
        }

    }
}
