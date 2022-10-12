using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.TableWithPlot.Data.Abstract;
using HEC.FDA.ViewModel.TableWithPlot.Data.ExtensionMethods;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using Statistics.Histograms;

namespace HEC.FDA.ViewModel.TableWithPlot.Data
{
    internal class HistogramDataProvider : BaseDataProvider
    {
        public HistogramDataProvider(UncertainPairedData upd, bool isStrictMonotonic)
        {
            IsStrictMonotonic = isStrictMonotonic;
            Name = "Empirical";
            for (int i = 0; i < upd.Xvals.Length; i++)
            {
                Data.Add(new HistogramRow(upd.Xvals[i], (Histogram)upd.Yvals[i], IsStrictMonotonic));
            }
            LinkList();
        }

        public HistogramDataProvider(bool isStrictMonotonic)
        {
            IsStrictMonotonic = isStrictMonotonic;
            Name = "Empirical";
            Data.Add(new HistogramRow(0.0d, new Histogram(0), IsStrictMonotonic));
            Data.Add(new HistogramRow(1.0d, new Histogram(1), IsStrictMonotonic));
            Data.Add(new HistogramRow(2.0d, new Histogram(2), IsStrictMonotonic));
            Data.Add(new HistogramRow(3.0d, new Histogram(3), IsStrictMonotonic));
            LinkList();
        }

        override public void AddUnlinkedRow(int i)
        {
           //do nothing. we wont add rows yet. 
        }
    }
}
