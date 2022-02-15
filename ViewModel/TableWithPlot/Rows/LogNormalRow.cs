using Statistics.Distributions;

using HEC.FDA.ViewModel.TableWithPlot.Rows.Base;
using Base.Enumerations;
using System.Collections.Generic;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    internal class LogNormalRow: SequentialRow
    {
        [DisplayAsColumn("Mean")]
        [DisplayAsLine("Max")]
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
        public LogNormalRow(double x, LogNormal y) : base(x, y)
        {
            AddSinglePropertyRule(nameof(Mean), new Rule(() => { if (PreviousRow == null) return true; return Mean > ((LogNormalRow)PreviousRow).Mean; }, "Mean values are not increasing.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Mean), new Rule(() => { return Mean > 0; }, "Mean value must be greater than 0.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Standard_Deviation), new Rule(() => { return Standard_Deviation > 0; }, "Standard deviation is less than 0.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Standard_Deviation), new Rule(() => { return CheckNormalDistExtremes(.00001); }, "A value of .00001 yeilds a non monotonic extreme", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Standard_Deviation), new Rule(() => { return CheckNormalDistExtremes(.99999); }, "A value of .99999 yeilds a non monotonic extreme", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Mean), new Rule(() => { return CheckNormalDistExtremes(.00001); }, "A value of .00001 yeilds a non monotonic extreme", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Mean), new Rule(() => { return CheckNormalDistExtremes(.99999); }, "A value of .99999 yeilds a non monotonic extreme", ErrorLevel.Severe));
        }
    }
}
