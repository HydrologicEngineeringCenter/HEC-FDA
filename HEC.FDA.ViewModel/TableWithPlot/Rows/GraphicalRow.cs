using Statistics.Distributions;
using System.Collections.Generic;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Implementations;

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
                NotifyPropertyChanged();
            }
        }
        [DisplayAsColumn("2.5% Conf.")]
        [DisplayAsLine("2.5% Conf.",Enumerables.ColorEnum.Blue,true)]
        public double Confidence025
        {
            get
            {
                return _Confidence025;
            }
        }

        [DisplayAsColumn("97.5% Conf.")]
        [DisplayAsLine("97.5% Conf.", Enumerables.ColorEnum.Blue,true)]
        public double Confidence975 { 
            get
            {
                return _Confidence975;
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
        public GraphicalRow(double x, double y, bool isMonotonicallyIncreasing = false) : base(x, new Deterministic(y), isMonotonicallyIncreasing, true)
        {
            AddSinglePropertyRule(nameof(Value), new Rule(() => { if (NextRow == null) return true; return Value < ((GraphicalRow)NextRow).Value; }, "Y values are not increasing.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Value), new Rule(() => { if (PreviousRow == null) return true; return Value > ((GraphicalRow)PreviousRow).Value; }, "Y values are not increasing.", ErrorLevel.Severe));           
        }

        public GraphicalRow(double x, double y, double conf025, double conf0975, bool isMonotonicallyIncreasing = false) : base(x, new Deterministic(y), isMonotonicallyIncreasing, true)
        {
            AddSinglePropertyRule(nameof(Value), new Rule(() => { if (NextRow == null) return true; return Value < ((GraphicalRow)NextRow).Value; }, "Y values are not increasing.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Value), new Rule(() => { if (PreviousRow == null) return true; return Value > ((GraphicalRow)PreviousRow).Value; }, "Y values are not increasing.", ErrorLevel.Severe));
            SetConfidenceLimits(conf025, conf0975);
        }

        public void SetConfidenceLimits(double conf025, double conf975)
        {
            _Confidence975 = conf975;
            _Confidence025 = conf025;
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
                case 1:
                    Value = value;
                    break;
            }
        }
    }
}
