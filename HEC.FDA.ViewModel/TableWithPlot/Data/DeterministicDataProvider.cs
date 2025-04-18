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
        public double[] Xs
        {
            get
            {
                double[] xs = new double[Data.Count];
                for(int i = 0; i < Data.Count; i++)
                {
                    xs[i] = ((DeterministicRow)Data[i]).X;
                }
                return xs;
            }
        }

        public double[] Ys
        {
            get
            {
                double[] ys = new double[Data.Count];
                for(int i = 0; i < Data.Count; i++)
                {
                    ys[i] = ((DeterministicRow)Data[i]).Value;
                }
                return ys;
            }
        }
        public DeterministicDataProvider()
        {
            Name = "Deterministic";
        }

        public DeterministicDataProvider(bool isStrictMonotonic, bool xIsDecreasing = false)
        {
            XisDecreasing = xIsDecreasing;
            IsStrictMonotonic = isStrictMonotonic;
            Name = "Deterministic";
            UncertainPairedData uncertainPairedData = DefaultData.GeneralUseDefaultCurve(IDistributionEnum.Deterministic);
            for (int i = 0; i < uncertainPairedData.Xvals.Length; i++)
            {
                Data.Add(new DeterministicRow(uncertainPairedData.Xvals[i], uncertainPairedData.Yvals[i].InverseCDF(.5), IsStrictMonotonic, xIsDecreasing));
            }
            LinkList();
        }
        public DeterministicDataProvider(UncertainPairedData uncertainPairedData, bool isStrictMonotonic, bool xIsDecreasing = false)
        {
            IsStrictMonotonic = isStrictMonotonic;
            XisDecreasing = xIsDecreasing;
            Name = "Deterministic";
            for(int i = 0; i < uncertainPairedData.Xvals.Length; i++)
            {
                Data.Add(new DeterministicRow(uncertainPairedData.Xvals[i], uncertainPairedData.Yvals[i].InverseCDF(.5), isStrictMonotonic, xIsDecreasing));
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
                DataProviderExtensions.AddRow(this, i, new DeterministicRow(x0, y0, IsStrictMonotonic, XisDecreasing));
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
                DataProviderExtensions.AddRow(this, i, new DeterministicRow(x, y, IsStrictMonotonic, XisDecreasing));
            }
            else
            {
                x0 = ((DeterministicRow)Data[Data.Count - 1]).X;
                y0 = ((DeterministicRow)Data[Data.Count - 1]).Y.InverseCDF(.5);
                DataProviderExtensions.AddRow(this, i, new DeterministicRow(x0, y0, IsStrictMonotonic, XisDecreasing));
                return;
            }

        }
    }
}
