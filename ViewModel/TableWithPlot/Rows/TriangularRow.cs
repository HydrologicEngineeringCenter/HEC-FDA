using Statistics;
using System.Collections.Generic;
using Statistics.Distributions;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.MVVMFramework.ViewModel.Validation;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Interfaces;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    internal class TriangularRow : SequentialRow
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
                ((TriangularRow)PreviousRow)?.NotifyPropertyChanged(nameof(X));
                ((TriangularRow)NextRow)?.NotifyPropertyChanged(nameof(X));
            }

        }
        [DisplayAsColumn("Min")]
        [DisplayAsLine("Min", Enumerables.ColorEnum.Blue, true)]
        public double Min
        {
            get
            {
                return ((Triangular)Y).Min;
            }
            set
            {
                ((Triangular)Y).Min = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Max));
                NotifyPropertyChanged(nameof(MostLikely));
                ((TriangularRow)PreviousRow)?.NotifyPropertyChanged(nameof(Min));
                ((TriangularRow)NextRow)?.NotifyPropertyChanged(nameof(Min));
            }
        }

        [DisplayAsColumn("Most Likely")]
        [DisplayAsLine("Most Likely", Enumerables.ColorEnum.Red)]
        public double MostLikely
        {
            get
            {
                return ((Triangular)Y).MostLikely;
            }

            set
            {
                ((Triangular)Y).MostLikely = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Max));
                NotifyPropertyChanged(nameof(Min));
                ((TriangularRow)PreviousRow)?.NotifyPropertyChanged(nameof(MostLikely));
                ((TriangularRow)NextRow)?.NotifyPropertyChanged(nameof(MostLikely));
            }
        }

        [DisplayAsColumn("Max")]
        [DisplayAsLine("Max", Enumerables.ColorEnum.Blue, true)]
        public double Max
        {
            get
            {
                return ((Triangular)Y).Max;
            }
            set
            {
                ((Triangular)Y).Max = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(MostLikely));
                NotifyPropertyChanged(nameof(Min));
                ((TriangularRow)PreviousRow)?.NotifyPropertyChanged(nameof(Max));
                ((TriangularRow)NextRow)?.NotifyPropertyChanged(nameof(Max));
            }
        }

        protected override List<string> YMinProperties
        {
            get
            {
                return new List<string>() { nameof(Min), nameof(MostLikely)};
            }
        }
        protected override List<string> YMaxProperties
        {
            get
            {
                return new List<string>() { nameof(Max), nameof(MostLikely) };
            }
        }

        public TriangularRow(double x, IDistribution y, bool isStrictMonotonic) : base(x, y)
        {
            AddSinglePropertyRule(nameof(Min), CreateMinValuesIncreasingPreviousRowRule( isStrictMonotonic));
            AddSinglePropertyRule(nameof(Min), CreateMinValuesIncreasingNextRowRule( isStrictMonotonic));

            AddSinglePropertyRule(nameof(MostLikely), CreateMostLikelyValuesIncreasingPreviousRowRule(isStrictMonotonic));
            AddSinglePropertyRule(nameof(MostLikely), CreateMostLikelyValuesIncreasingNextRowRule(isStrictMonotonic));
                       
            AddSinglePropertyRule(nameof(Max),CreateMaxValuesIncreasingPreviousRowRule(isStrictMonotonic));
            AddSinglePropertyRule(nameof(Max), CreateMaxValuesIncreasingNextRowRule(isStrictMonotonic));
            
            AddMultiPropertyRule(new List<string> { "Min", "MostLikely", "Max" }, new Rule(() => { return ((Min <= MostLikely) && (MostLikely <= Max)); }, "Min must be less than most likely, which must be less than Max", ErrorLevel.Severe));
        }

        private IRule CreateMinValuesIncreasingPreviousRowRule( bool isStrictMonotonic)
        {
            return new Rule(() =>
            {
                if (PreviousRow == null)
                {
                    return true;
                }
                else
                {
                    double prevMin = ((Triangular)PreviousRow.Y).Min;
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

        private IRule CreateMinValuesIncreasingNextRowRule( bool isStrictMonotonic)
        {
            return new Rule(() =>
            {
                if (NextRow == null)
                {
                    return true;
                }
                else
                {
                    double nextMin = ((Triangular)NextRow.Y).Min;
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

        private IRule CreateMostLikelyValuesIncreasingPreviousRowRule(bool isStrictMonotonic)
        {
            return new Rule(() =>
            {
                if (PreviousRow == null)
                {
                    return true;
                }
                else
                {
                    double prevMostLikely = ((Triangular)PreviousRow.Y).MostLikely;
                    if (isStrictMonotonic)
                    {
                        return MostLikely > prevMostLikely;
                    }
                    else
                    {
                        return MostLikely >= prevMostLikely;
                    }
                }
            },
                "Most likely values are not increasing.", ErrorLevel.Severe);
        }

        private IRule CreateMostLikelyValuesIncreasingNextRowRule(bool isStrictMonotonic)
        {
            return new Rule(() =>
            {
                if (NextRow == null)
                {
                    return true;
                }
                else
                {
                    double nextMostLikely = ((Triangular)NextRow.Y).MostLikely;
                    if (isStrictMonotonic)
                    {
                        return MostLikely < nextMostLikely;
                    }
                    else
                    {
                        return MostLikely <= nextMostLikely;
                    }
                }
            },
                "Most likely values are not increasing.", ErrorLevel.Severe);
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
                    double prevMax = ((Triangular)PreviousRow.Y).Max;
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
                    double nextMax = ((Triangular)NextRow.Y).Max;
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
                    MostLikely = value;
                    break;
                case 3:
                    Max = value;
                    break;
            }
        }
    }
}
