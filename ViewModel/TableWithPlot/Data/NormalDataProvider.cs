using HEC.FDA.ViewModel.TableWithPlot.Rows;
using Statistics.Distributions;
using HEC.FDA.ViewModel.TableWithPlot.Data.ExtensionMethods;
using HEC.FDA.ViewModel.TableWithPlot.Data.Abstract;
using HEC.FDA.Model.paireddata;

namespace HEC.FDA.ViewModel.TableWithPlot.Data
{
    internal class NormalDataProvider : BaseDataProvider
    {
        public NormalDataProvider()
        {
            Name = "Normal";
            Data.Add(new NormalRow(0.0d, new Normal(0,0), IsStrictMonotonic));
            Data.Add(new NormalRow(2.0d, new Normal(0, 2), IsStrictMonotonic));
            LinkList();
        }

        public NormalDataProvider(bool isStrictMonotonic)
        {
            IsStrictMonotonic = isStrictMonotonic;
            Name = "Normal";
            Data.Add(new NormalRow(0.0d, new Normal(0, 0), IsStrictMonotonic));
            Data.Add(new NormalRow(2.0d, new Normal(0, 2), IsStrictMonotonic));
            LinkList();
        }
        public NormalDataProvider(UncertainPairedData upd, bool isStrictMonotonic)
        {
            IsStrictMonotonic = isStrictMonotonic;
            Name = "Normal";
            for (int i = 0; i < upd.Xvals.Length; i++)
            {
                Data.Add(new NormalRow(upd.Xvals[i], (Normal)upd.Yvals[i], IsStrictMonotonic));
            }
            LinkList();
        }
        override public void AddUnlinkedRow(int i)
        {
            double mean0 = 0.0;
            double stdev0  = 0.0;
            double x0 = 0.0;
            if (i == 0)
            {
                if (Data.Count != 0)
                {
                    x0 = ((NormalRow)Data[i]).X;
                    mean0 = ((Normal)((NormalRow)Data[i]).Y).Mean;
                    stdev0 = ((Normal)((NormalRow)Data[i]).Y).StandardDeviation;
                }
                DataProviderExtensions.AddRow(this, i, new NormalRow(x0, new Normal(mean0, stdev0), IsStrictMonotonic));
                return;
            }
            if (Data.Count > i)
            {
                x0 = ((NormalRow)Data[i - 1]).X;
                mean0 = ((Normal)((NormalRow)Data[i - 1]).Y).Mean;
                stdev0 = ((Normal)((NormalRow)Data[i - 1]).Y).StandardDeviation;
                double x1 = 0.0;
                double mean1 = 0.0;
                double stdev1 = 0.0;
                x1 = ((NormalRow)Data[i]).X;
                mean1 = ((Normal)((NormalRow)Data[i]).Y).Mean;
                stdev1 = ((Normal)((NormalRow)Data[i]).Y).StandardDeviation;
                double x = x0 + ((x1 - x0) / 2.0);
                double mean = mean0 + ((mean1 - mean0) / 2.0);
                double stdev = stdev0 + ((stdev1 - stdev0) / 2.0);
                DataProviderExtensions.AddRow(this, i, new NormalRow(x, new Normal(mean, stdev), IsStrictMonotonic));
            }
            else
            {
                x0 = ((NormalRow)Data[Data.Count - 1]).X;
                mean0 = ((Normal)((NormalRow)Data[Data.Count - 1]).Y).Mean;
                stdev0 = ((Normal)((NormalRow)Data[Data.Count - 1]).Y).StandardDeviation;
                DataProviderExtensions.AddRow(this, i, new NormalRow(x0, new Normal(mean0, stdev0), IsStrictMonotonic));
                return;
            }
        }
    }
}
