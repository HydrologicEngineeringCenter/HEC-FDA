using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.ViewModel.Validation;
using Statistics.Distributions;
using System.Collections.Generic;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.MVVMFramework.Base.Interfaces;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    internal class UniformRow : SequentialRow
    {
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
        public UniformRow(double x, Uniform y, bool isStrictMonotonic) : base(x, y)
        {
            AddSinglePropertyRule(nameof(Min), new Rule(() => { return Min <= Max; }, "Min is greater than max", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Max), new Rule(() => { return Max >= Min; }, "Max is less than min", ErrorLevel.Severe));

            AddSinglePropertyRule(nameof(Min), CreateMinValuesIncreasingPreviousRowRule(isStrictMonotonic));
            AddSinglePropertyRule(nameof(Min), CreateMinValuesIncreasingNextRowRule(isStrictMonotonic));

            AddSinglePropertyRule(nameof(Max), CreateMaxValuesIncreasingPreviousRowRule(isStrictMonotonic));
            AddSinglePropertyRule(nameof(Max), CreateMaxValuesIncreasingNextRowRule(isStrictMonotonic));
        }

        private IRule CreateMinValuesIncreasingPreviousRowRule(bool isStrictMonotonic)
        {
            return new Rule(() =>
            {
                if (PreviousRow == null)
                {
                    return true;
                }
                else
                {
                    double prevMin = ((Uniform)PreviousRow.Y).Min;
                    if (isStrictMonotonic)
                    {
                        return Min > prevMin;
                    }
                    else
                    {
                        return Min >= prevMin;
                    }
                }
            },
                "Min values are not increasing.", ErrorLevel.Severe);
        }

        private IRule CreateMinValuesIncreasingNextRowRule(bool isStrictMonotonic)
        {
            return new Rule(() =>
            {
                if (NextRow == null)
                {
                    return true;
                }
                else
                {
                    double nextMin = ((Uniform)NextRow.Y).Min;
                    if (isStrictMonotonic)
                    {
                        return Min < nextMin;
                    }
                    else
                    {
                        return Min <= nextMin;
                    }
                }
            },
                "Min values are not increasing.", ErrorLevel.Severe);
        }

        private IRule CreateMaxValuesIncreasingPreviousRowRule(bool isStrictMonotonic)
        {
            return new Rule(() =>
            {
                if (PreviousRow == null)
                {
                    return true;
                }
                else
                {
                    double prevMax = ((Uniform)PreviousRow.Y).Max;
                    if (isStrictMonotonic)
                    {
                        return Max > prevMax;
                    }
                    else
                    {
                        return Max >= prevMax;
                    }
                }
            },
                "Max values are not increasing.", ErrorLevel.Severe);
        }

        private IRule CreateMaxValuesIncreasingNextRowRule(bool isStrictMonotonic)
        {
            return new Rule(() =>
            {
                if (NextRow == null)
                {
                    return true;
                }
                else
                {
                    double nextMax = ((Uniform)NextRow.Y).Max;
                    if (isStrictMonotonic)
                    {
                        return Max < nextMax;
                    }
                    else
                    {
                        return Max <= nextMax;
                    }
                }
            },
                "Max values are not increasing.", ErrorLevel.Severe);
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
