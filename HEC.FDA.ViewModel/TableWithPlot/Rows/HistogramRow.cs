using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using Statistics.Histograms;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    public class HistogramRow : SequentialRow
    {
        private double _x;
        [DisplayAsColumn("X")]
        public override double X
        {
            get { return _x; }
            set
            {
                _x = value;
                NotifyPropertyChanged();
                ((HistogramRow)PreviousRow)?.NotifyPropertyChanged(nameof(X));
                ((HistogramRow)NextRow)?.NotifyPropertyChanged(nameof(X));
            }

        }
        [DisplayAsColumn("5th Percentile")]
        [DisplayAsLine("5th Percentile")]
        public double Percentile05
        {
            get
            {
                return ((IHistogram)Y).InverseCDF(.05);
            }
        }
        [DisplayAsColumn("Mean")]
        [DisplayAsLine("Mean")]
        public double Mean
        {
            get
            {
                return ((IHistogram)Y).Mean;
            }
        }
        [DisplayAsLine("95th Percentile")]
        [DisplayAsColumn("95th Percentile")]
        public double Percentile95
        {
            get
            {
                return ((IHistogram)Y).InverseCDF(.95);
            }
        }

        protected override List<string> YMinProperties
        {
            get
            {
                return new List<string>() { nameof(Mean), nameof(Percentile05), nameof(Percentile95) };
            }
        }
        protected override List<string> YMaxProperties
        {
            get
            {
                return new List<string>() { nameof(Mean), nameof(Percentile05), nameof(Percentile95) };
            }
        }
        public HistogramRow(double x, IHistogram y, bool isStrictMonotonic) : base(x, y)
        {
        }

        public override void UpdateRow(int col, double value)
        {
            switch (col)
            {
                case 0:
                    X = value;
                    break;
            }
        }
    
    }
}
