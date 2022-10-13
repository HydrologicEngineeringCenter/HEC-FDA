using HEC.FDA.ViewModel.TableWithPlot.Rows;
using HEC.FDA.ViewModel.TableWithPlot.Data.ExtensionMethods;
using HEC.FDA.ViewModel.TableWithPlot.Data.Abstract;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Utilities;
using Statistics;

namespace HEC.FDA.ViewModel.TableWithPlot.Data
{
    public class DeterministicDataProvider : BaseDataProvider
    {
        public DeterministicDataProvider()
        {
            Name = "Deterministic";
            UncertainPairedData uncertainPairedData = DefaultData.GeneralUseDefaultCurve(IDistributionEnum.Deterministic);
            for (int i = 0; i < uncertainPairedData.Xvals.Length; i++)
            {
                Data.Add(new DeterministicRow(uncertainPairedData.Xvals[i], uncertainPairedData.Yvals[i].InverseCDF(.5), IsStrictMonotonic));
            }
            LinkList();
        }

        public DeterministicDataProvider(bool isStrictMonotonic)
        {
            IsStrictMonotonic = isStrictMonotonic;
            Name = "Deterministic";
            UncertainPairedData uncertainPairedData = DefaultData.GeneralUseDefaultCurve(IDistributionEnum.Deterministic);
            for (int i = 0; i < uncertainPairedData.Xvals.Length; i++)
            {
                Data.Add(new DeterministicRow(uncertainPairedData.Xvals[i], uncertainPairedData.Yvals[i].InverseCDF(.5), IsStrictMonotonic));
            }
            LinkList();
        }
        public DeterministicDataProvider(UncertainPairedData uncertainPairedData, bool isStrictMonotonic)
        {
            IsStrictMonotonic = isStrictMonotonic;
            Name = "Deterministic";
            for(int i = 0; i < uncertainPairedData.Xvals.Length; i++)
            {
                Data.Add(new DeterministicRow(uncertainPairedData.Xvals[i], uncertainPairedData.Yvals[i].InverseCDF(.5), isStrictMonotonic));
            }
            LinkList();
        }
        override public void AddUnlinkedRow(int i)
        {
            double x0 = 0.0;
            double y0 = 0.0;
            if (i == 0)
            {
                if (Data.Count != 0)
                {
                    x0 = ((DeterministicRow)Data[i]).X;
                    y0 = ((DeterministicRow)Data[i]).Y.InverseCDF(.5);
                }
                DataProviderExtensions.AddRow(this, i, new DeterministicRow(x0, y0, IsStrictMonotonic));
                return;
            }
            if (Data.Count > i)
            {
                x0 = ((DeterministicRow)Data[i - 1]).X;
                y0 = ((DeterministicRow)Data[i - 1]).Y.InverseCDF(.5);
                double x1 = 0.0;
                double y1 = 0.0;
                x1 = ((DeterministicRow)Data[i]).X;
                y1 = ((DeterministicRow)Data[i]).Y.InverseCDF(.5);
                double x = x0 + ((x1 - x0) / 2.0);
                double y = y0 + ((y1 - y0) / 2.0);
                DataProviderExtensions.AddRow(this, i, new DeterministicRow(x, y, IsStrictMonotonic));
            }
            else
            {
                x0 = ((DeterministicRow)Data[Data.Count - 1]).X;
                y0 = ((DeterministicRow)Data[Data.Count - 1]).Y.InverseCDF(.5);
                DataProviderExtensions.AddRow(this, i, new DeterministicRow(x0, y0, IsStrictMonotonic));
                return;
            }

        }
    }
}
