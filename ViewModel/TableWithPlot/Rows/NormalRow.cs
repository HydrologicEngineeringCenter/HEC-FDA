using Base.Enumerations;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using Statistics.Distributions;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    internal class NormalRow : SequentialRow
    {
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
        public NormalRow(double x, Normal y): base(x, y)
        {   
            AddSinglePropertyRule(nameof(Mean), new Rule(() => { if (PreviousRow == null) return true; return Mean > ((NormalRow)PreviousRow).Mean; }, "Mean values are not increasing.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Standard_Deviation), new Rule(() => { return Standard_Deviation >= 0; }, "Standard deviation is less than 0.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Standard_Deviation), new Rule(() => { return CheckNormalDistExtremes(.01);}, "A value of .01 yeilds a non monotonic extreme", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Standard_Deviation), new Rule(() => { return CheckNormalDistExtremes(.99); }, "A value of .99 yeilds a non monotonic extreme", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Mean), new Rule(() => { return CheckNormalDistExtremes(.01); }, "A value of .01 yeilds a non monotonic extreme", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Mean), new Rule(() => { return CheckNormalDistExtremes(.99); }, "A value of .99 yeilds a non monotonic extreme", ErrorLevel.Severe));
        }

    }

}
