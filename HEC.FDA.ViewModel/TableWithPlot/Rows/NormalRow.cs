﻿using HEC.MVVMFramework.Base.Enumerations;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using Statistics.Distributions;
using System.Collections.Generic;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Implementations;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    internal class NormalRow : SequentialRow
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
                ((NormalRow)PreviousRow)?.NotifyPropertyChanged(nameof(X));
                ((NormalRow)NextRow)?.NotifyPropertyChanged(nameof(X));
            }

        }

        [DisplayAsColumn("Mean")]
        [DisplayAsLine("Mean", Enumerables.ColorEnum.Red)]
        public double Mean
        {
            get
            {
                return ((Normal)Y).Mean;
            }

            set
            {
                ((Normal)Y).Mean = value;
                NotifyPropertyChanged();
                ((NormalRow)PreviousRow)?.NotifyPropertyChanged(nameof(Mean));
                ((NormalRow)NextRow)?.NotifyPropertyChanged(nameof(Mean));
            }
        }
        [DisplayAsLine("95% CI", Enumerables.ColorEnum.Blue, true)]
        public double Upper
        {
            get
            {
                return Y.InverseCDF(.95);
            }
        }
        [DisplayAsLine("5% CI", Enumerables.ColorEnum.Blue, true)]
        public double Lower
        {
            get
            {
                return Y.InverseCDF(.05);
            }
        }
        [DisplayAsColumn("Standard Deviation")]
        public double Standard_Deviation
        {
            get
            {
                return ((Normal)Y).StandardDeviation;
            }
            set
            {
                ((Normal)Y).StandardDeviation = value;
                NotifyPropertyChanged();
            }
        }
        protected override List<string> YMinProperties
        {
            get
            {
                return new List<string>() { nameof(Mean), nameof(Standard_Deviation) };
            }
        }
        protected override List<string> YMaxProperties
        {
            get
            {
                return new List<string>() { nameof(Mean), nameof(Standard_Deviation) };
            }
        }
        public NormalRow(double x, Normal y, bool isStrictMonotonic) : base(x, y)
        {   
            AddSinglePropertyRule(nameof(Mean), CreateMeanValuesIncreasingPreviousRowRule(isStrictMonotonic));
            AddSinglePropertyRule(nameof(Mean), CreateMeanValuesIncreasingNextRowRule(isStrictMonotonic));

            AddSinglePropertyRule(nameof(Standard_Deviation), new Rule(() => { return Standard_Deviation >= 0; }, "Standard deviation is less than 0.", ErrorLevel.Severe));
            
            AddSinglePropertyRule(nameof(Standard_Deviation), new Rule(() => { return CheckNormalDistExtremes(.01);}, "The first percentile of this distribution (the lower confidence limit of .01) yielded a non monotonic extreme for the uncertainty in this relationship", ErrorLevel.Minor));
            AddSinglePropertyRule(nameof(Standard_Deviation), new Rule(() => { return CheckNormalDistExtremes(.99); }, "The 99th percentile of this distribution (the upper confidence limit of .99) yielded a non monotonic extreme for the uncertainty in this relationship", ErrorLevel.Minor));
            AddSinglePropertyRule(nameof(Mean), new Rule(() => { return CheckNormalDistExtremes(.99); }, "The upper confidence limit of .99 yeilded a non monotonic extreme for the uncertainty in this relationship", ErrorLevel.Minor));
            AddSinglePropertyRule(nameof(Mean), new Rule(() => { return CheckNormalDistExtremes(.01); }, "The lower confidence limit of .01 yeilded a non monotonic extreme for the uncertainty in this relationship", ErrorLevel.Minor));
        }

        private IRule CreateMeanValuesIncreasingPreviousRowRule(bool isStrictMonotonic)
        {
            return new Rule(() =>
            {
                if (PreviousRow == null)
                {
                    return true;
                }
                else
                {
                    double prevMin = ((NormalRow)PreviousRow).Mean;
                    if (isStrictMonotonic)
                    {
                        return Mean > prevMin;
                    }
                    else
                    {
                        return Mean >= prevMin;
                    }
                }
            },
                "Mean values are not increasing.", ErrorLevel.Severe);
        }

        private IRule CreateMeanValuesIncreasingNextRowRule(bool isStrictMonotonic)
        {
            return new Rule(() =>
            {
                if (NextRow == null)
                {
                    return true;
                }
                else
                {
                    double nextMin = ((NormalRow)NextRow).Mean;
                    if (isStrictMonotonic)
                    {
                        return Mean < nextMin;
                    }
                    else
                    {
                        return Mean <= nextMin;
                    }
                }
            },
                "Min values are not increasing.", ErrorLevel.Severe);
        }

        public override void UpdateRow(int col, double value)
        {
            switch (col)
            {
                case 0:
                    X = value;
                    break;
                case 1:
                    Mean = value;
                    break;
                case 2:
                    Standard_Deviation = value;
                    break;
            }
        }

    }

}
