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
            Name = "Empiracle";
            for (int i = 0; i < upd.Xvals.Length; i++)
            {
                Data.Add(new HistogramRow(upd.Xvals[i], (Histogram)upd.Yvals[i], IsStrictMonotonic));
            }
            LinkList();
        }
        override public void AddUnlinkedRow(int i)
        {
           //do nothing. we wont add rows yet. 
        }
    }
}
