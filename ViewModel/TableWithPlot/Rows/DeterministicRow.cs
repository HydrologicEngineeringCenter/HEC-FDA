using Statistics.Distributions;
using System.Collections.Generic;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.MVVMFramework.ViewModel.Validation;
using HEC.MVVMFramework.Base.Enumerations;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    public class DeterministicRow : SequentialRow
    {
        [DisplayAsColumn("Y Value")]
        [DisplayAsLine("Y Value", Enumerables.ColorEnum.Red)]
        public double Value
        {
            get
            {
                return ((Deterministic)Y).Value;
            }
            set
            {
                Y = new Deterministic(value);
                NotifyPropertyChanged();
                ((DeterministicRow)PreviousRow)?.NotifyPropertyChanged(nameof(Value));
                ((DeterministicRow)NextRow)?.NotifyPropertyChanged(nameof(Value));
            }
        }
        protected override List<string> YMinProperties { 
            get
            {
                return new List<string>() { nameof(Value)};
            } 
        }
        protected override List<string> YMaxProperties
        {
            get
            {
                return new List<string>() { nameof(Value) };
            }
        }

        public DeterministicRow(double x, double y): base(x, new Deterministic(y))
        {
            AddSinglePropertyRule(nameof(Value), new Rule( () => { if (PreviousRow == null) return true; return Value > ((DeterministicRow)PreviousRow).Value; }, "Y values are not increasing.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Value), new Rule(() => { if (NextRow == null) return true; return Value < ((DeterministicRow)NextRow).Value; }, "Y values are not increasing.", ErrorLevel.Severe));
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


