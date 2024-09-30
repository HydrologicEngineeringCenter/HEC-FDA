using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using Statistics.Histograms;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    public class LP3HistogramRow : SequentialRow
    {
        private double _x;
        [DisplayAsColumn("X Value")]
        public override double X
        {
            get { return _x; }
            set
            {
                _x = value;
                NotifyPropertyChanged();
                ((LP3HistogramRow)PreviousRow)?.NotifyPropertyChanged(nameof(X));
                ((LP3HistogramRow)NextRow)?.NotifyPropertyChanged(nameof(X));
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

        private double _inputFunction;
        [DisplayAsColumn("Input Discharge")]
        [DisplayAsLine("Input Discharge")]
        public double InputFunctionY
        {
            get
            {
                return _inputFunction;
            }
            set 
            { 
                _inputFunction = value; 
                NotifyPropertyChanged(nameof(InputFunctionY));
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
                return [nameof(Percentile05), nameof(Percentile95)];
            }
        }
        protected override List<string> YMaxProperties
        {
            get
            {
                return [nameof(Percentile05), nameof(Percentile95)];
            }
        }
        public LP3HistogramRow(double x, IHistogram y, bool isStrictMonotonic) : base(x, y)
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
