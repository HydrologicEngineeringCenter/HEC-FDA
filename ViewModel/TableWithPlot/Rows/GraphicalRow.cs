using Statistics.Distributions;
using System.Collections.Generic;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.MVVMFramework.ViewModel.Validation;
using HEC.MVVMFramework.Base.Enumerations;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    public class GraphicalRow : SequentialRow
    {
        private double _Confidence975 = 0;
        private double _Confidence025 = 0;
        private double _x;

        [DisplayAsColumn("X Value")]
        public override double X
        {
            get { return _x; }
            set
            {
                _x = value;
                NotifyPropertyChanged();
                ((GraphicalRow)PreviousRow)?.NotifyPropertyChanged(nameof(X));
                ((GraphicalRow)NextRow)?.NotifyPropertyChanged(nameof(X));
                ClearConfidenceLimits();
            }

        }
        [DisplayAsColumn("Y Value")]
        [DisplayAsLine("Y Value",Enumerables.ColorEnum.Black)]
        public double Value
        {
            get
            {
                return ((Deterministic)Y).Value;
            }
            set
            {
                Y = new Deterministic(value);
                ((GraphicalRow)PreviousRow)?.NotifyPropertyChanged(nameof(Value));
                ((GraphicalRow)NextRow)?.NotifyPropertyChanged(nameof(Value));
                ClearConfidenceLimits();
                NotifyPropertyChanged();
            }
        }
        [DisplayAsColumn("2.5% Confidence")]
        [DisplayAsLine("2.5% Confidence",Enumerables.ColorEnum.Blue,true)]
        public double Confidence025
        {
            get
            {
                return _Confidence025;
            }
            set
            {
                ClearConfidenceLimits();
            }
        }

        [DisplayAsColumn("97.5% Confidence")]
        [DisplayAsLine("97.5% Confidence", Enumerables.ColorEnum.Blue,true)]
        public double Confidence975 { 
            get
            {
                return _Confidence975;
            }
            set
            {
                ClearConfidenceLimits();
            }
        }


        protected override List<string> YMinProperties
        {
            get
            {
                return new List<string>() { nameof(Value) };
            }
        }
        protected override List<string> YMaxProperties
        {
            get
            {
                return new List<string>() { nameof(Value) };
            }
        }
        public GraphicalRow(double x, double y, bool isMonotonicallyIncreasing = false) : base(x, new Deterministic(y), isMonotonicallyIncreasing)
        {
            AddSinglePropertyRule(nameof(Value), new Rule(() => { if (NextRow == null) return true; return Value < ((GraphicalRow)NextRow).Value; }, "Y values are not increasing.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Value), new Rule(() => { if (PreviousRow == null) return true; return Value > ((GraphicalRow)PreviousRow).Value; }, "Y values are not increasing.", ErrorLevel.Severe));
        }

        private void ClearConfidenceLimits()
        {
            GraphicalRow row = this;
            //fix this row
            row._Confidence025 = 0;
            row._Confidence975 = 0;
            row.NotifyPropertyChanged(nameof(Confidence025));
            row.NotifyPropertyChanged(nameof(Confidence975));

            //Check next rows
            while(row.NextRow != null)
            {
                ((GraphicalRow)row.NextRow)._Confidence025 = 0;
                ((GraphicalRow)row.NextRow)._Confidence975 = 0;
                row = ((GraphicalRow)row.NextRow);
                row.NotifyPropertyChanged(nameof(Confidence025));
                row.NotifyPropertyChanged(nameof(Confidence975));
            }
            //Check prior rows
            while(((GraphicalRow)row.PreviousRow) != null)
            {
                ((GraphicalRow)row.PreviousRow)._Confidence025 = 0;
                ((GraphicalRow)row.PreviousRow)._Confidence975 = 0;
                row = ((GraphicalRow)row.PreviousRow);
                row.NotifyPropertyChanged(nameof(Confidence025));
                row.NotifyPropertyChanged(nameof(Confidence975));
            }
        }

        public void SetConfidenceLimits(double conf05, double conf95)
        {
            _Confidence975 = conf95;
            _Confidence025 = conf05;
            NotifyPropertyChanged(nameof(Confidence975));
            NotifyPropertyChanged(nameof(Confidence025));
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
