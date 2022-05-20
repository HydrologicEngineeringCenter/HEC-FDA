using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.ViewModel.Validation;
using Statistics.Distributions;
using System.Collections.Generic;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    internal class UniformRow : SequentialRow
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
                ((UniformRow)PreviousRow)?.NotifyPropertyChanged(nameof(X));
                ((UniformRow)NextRow)?.NotifyPropertyChanged(nameof(X));
            }

        }

        [DisplayAsColumn("Min")]
        [DisplayAsLine("Min", Enumerables.ColorEnum.Blue, true)]
        public double Min
        {
            get
            {
                return ((Uniform)Y).Min;
            }
            set
            {
                ((Uniform)Y).Min = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Max));
                ((UniformRow)PreviousRow)?.NotifyPropertyChanged(nameof(Min));
                ((UniformRow)NextRow)?.NotifyPropertyChanged(nameof(Min));
            }
        }
        [DisplayAsColumn("Max")]
        [DisplayAsLine("Max", Enumerables.ColorEnum.Blue, true)]
        public double Max
        {
            get
            {
                return ((Uniform)Y).Max;
            }
            set
            {
                ((Uniform)Y).Max = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Min));
                ((UniformRow)PreviousRow)?.NotifyPropertyChanged(nameof(Max));
                ((UniformRow)NextRow)?.NotifyPropertyChanged(nameof(Max));
            }
        }
        protected override List<string> YMinProperties
        {
            get
            {
                return new List<string>() { nameof(Min) };
            }
        }
        protected override List<string> YMaxProperties
        {
            get
            {
                return new List<string>() { nameof(Max) };
            }
        }
        public UniformRow(double x, Uniform y):base(x, y)
        {
            AddSinglePropertyRule(nameof(Min), new Rule(() => { return Min <= Max; }, "Min is greater than max", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Max), new Rule(() => { return Max >= Min; }, "Max is less than min", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Min), new Rule(() => { if (PreviousRow == null) return true; return Min >= ((UniformRow)PreviousRow).Min; }, "Min values are not increasing.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Max), new Rule(() => { if (PreviousRow == null) return true; return Max >= ((UniformRow)PreviousRow).Max; }, "Min values are not increasing.", ErrorLevel.Severe));
        }
        public override void UpdateRow(int col, double value)
        {
            switch (col)
            {
                case 0:
                    X = value;
                    break;
                case 1:
                    Min = value;
                    break;
                case 2:
                    Max = value;
                    break;
            }
        }
    }

}
