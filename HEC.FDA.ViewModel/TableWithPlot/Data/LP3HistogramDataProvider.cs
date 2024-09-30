using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.TableWithPlot.Data.Abstract;
using HEC.FDA.ViewModel.TableWithPlot.Data.ExtensionMethods;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using Statistics;
using Statistics.Histograms;
using System;

namespace HEC.FDA.ViewModel.TableWithPlot.Data
{
    public class LP3HistogramDataProvider : BaseDataProvider
    {
        /// <summary>
        /// Only used by the activator. Don't use this ctor.
        /// </summary>
        public LP3HistogramDataProvider()
        {
            Name = "LP3 Display";
        }
        public LP3HistogramDataProvider(UncertainPairedData upd, bool isStrictMonotonic)
        {
            IsStrictMonotonic = isStrictMonotonic;
            Name = "LP3 Display";
            for (int i = 0; i < upd.Xvals.Length; i++)
            {
                Data.Add(new LP3HistogramRow(upd.Xvals[i], (IHistogram)upd.Yvals[i], IsStrictMonotonic));
            }
            LinkList();
        }

        /// <summary>
        /// Assuems input function's x vals match the x vals from the uncertain paired data used to construct the histogram in the row.  
        /// Dangerous. Use with caution. I'm generous with exceptions here. 
        /// </summary>
        /// <param name="inputFunctionVals"></param>
        public void OverwriteInputFunctionVals(UncertainPairedData inputFunctionVals)
        {
            PairedData deterministicInputFunc = inputFunctionVals.SamplePairedData(0.5, true);
            deterministicInputFunc.SortToIncreasingXVals();
            for (int i = 0; i < Data.Count; i++)
            {
                LP3HistogramRow row = (LP3HistogramRow)Data[i];
                double probability = row.X;
                row.InputFunctionY = deterministicInputFunc.f(probability);
            }
        }

        override public void AddUnlinkedRow(int i)
        {
            DataProviderExtensions.AddRow(this,i,new LP3HistogramRow(0,new DynamicHistogram(), IsStrictMonotonic));
        }
    }
}
