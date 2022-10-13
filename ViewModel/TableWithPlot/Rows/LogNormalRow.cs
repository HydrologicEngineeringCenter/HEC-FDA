using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.ViewModel.Validation;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using Statistics.Distributions;
using System.Collections.Generic;
using HEC.MVVMFramework.Base.Interfaces;
using RasMapperLib.Utilities;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    internal class LogNormalRow: SequentialRow
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
                ((LogNormalRow)PreviousRow)?.NotifyPropertyChanged(nameof(X));
                ((LogNormalRow)NextRow)?.NotifyPropertyChanged(nameof(X));
            }

        }
        [DisplayAsColumn("Mean")]
        [DisplayAsLine("Mean")]
        public double Mean
        {
            get
            {
                return ((LogNormal)Y).Mean;
            }

            set
            {
                ((LogNormal)Y).Mean = value;
                NotifyPropertyChanged();
                ((LogNormalRow)PreviousRow)?.NotifyPropertyChanged(nameof(Mean));
                ((LogNormalRow)NextRow)?.NotifyPropertyChanged(nameof(Mean));
            }
        }
        [DisplayAsColumn("Standard Deviation")]
        public double Standard_Deviation
        {
            get
            {
                return ((LogNormal)Y).StandardDeviation;
            }
            set
            {
                ((LogNormal)Y).StandardDeviation = value;
                NotifyPropertyChanged();
            }
        }

        [DisplayAsLine("95% CL", Enumerables.ColorEnum.Blue, true)]
        public double Upper
        {
            get
            {
                return System.Math.Log(Y.InverseCDF(.95));
            }
        }
    
        [DisplayAsLine("5% CL", Enumerables.ColorEnum.Blue, true)]
        public double Lower
        {
            get
            {
                return System.Math.Log(Y.InverseCDF(.05));
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
        public LogNormalRow(double x, LogNormal y, bool isStrictMonotonic) : base(x, y)
        {
            AddSinglePropertyRule(nameof(Mean),CreateMeanValuesIncreasingPreviousRowRule(isStrictMonotonic));
            AddSinglePropertyRule(nameof(Mean), CreateMeanValuesIncreasingNextRowRule(isStrictMonotonic));

            AddSinglePropertyRule(nameof(Mean), new Rule(() => { return Mean > 0; }, "Mean value must be greater than 0.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Standard_Deviation), new Rule(() => { return Standard_Deviation >= 0; }, "Standard deviation is less than 0.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Standard_Deviation), new Rule(() => { return CheckNormalDistExtremes(.00001); }, "The first percentile of this distribution (the lower confidence limit of .00001) yielded a non monotonic extreme for the uncertainty in this relationship", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Standard_Deviation), new Rule(() => { return CheckNormalDistExtremes(.99999); }, "The 99999th percentile of this distribution (the upper confidence limit of .99999) yeilded a non monotonic extreme for the uncertainty in this relationship", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Mean), new Rule(() => { return CheckNormalDistExtremes(.00001); }, "A value of .00001 yeilds a non monotonic extreme", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Mean), new Rule(() => { return CheckNormalDistExtremes(.99999); }, "A value of .99999 yeilds a non monotonic extreme", ErrorLevel.Severe));
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
                    double prevMean = ((LogNormalRow)PreviousRow).Mean;
                    if (isStrictMonotonic)
                    {
                        return Mean > prevMean;
                    }
                    else
                    {
                        return Mean >= prevMean;
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
                    double nextMean = ((LogNormalRow)NextRow).Mean;
                    if (isStrictMonotonic)
                    {
                        return Mean < nextMean;
                    }
                    else
                    {
                        return Mean <= nextMean;
                    }
                }
            },
                "Mean values are not increasing.", ErrorLevel.Severe);
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
