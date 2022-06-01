using HEC.FDA.ViewModel.TableWithPlot.Data.Abstract;
using HEC.FDA.ViewModel.TableWithPlot.Data.ExtensionMethods;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using paireddata;
using Statistics.Distributions;

namespace HEC.FDA.ViewModel.TableWithPlot.Data
{
    internal class TriangularDataProvider: BaseDataProvider
    {
        public TriangularDataProvider()
        {
            Name = "Triangular";
            Data.Add(new TriangularRow(0.0d, new Triangular(0, 0, 1), IsStrictMonotonic));
            Data.Add(new TriangularRow(2.0d, new Triangular(1, 2, 3), IsStrictMonotonic));
            LinkList();
        }

        public TriangularDataProvider(bool isStrictMonotonic)
        {
            IsStrictMonotonic = isStrictMonotonic;
            Name = "Triangular";
            Data.Add(new TriangularRow(0.0d, new Triangular(0, 0, 1), IsStrictMonotonic));
            Data.Add(new TriangularRow(2.0d, new Triangular(1, 2, 3), IsStrictMonotonic));
            LinkList();
        }
        public TriangularDataProvider(UncertainPairedData upd, bool isStrictMonotonic)
        {
            IsStrictMonotonic = isStrictMonotonic;
            Name = "Triangular";
            for (int i = 0; i < upd.Xvals.Length; i++)
            {
                Data.Add(new TriangularRow(upd.Xvals[i], (Triangular)upd.Yvals[i], IsStrictMonotonic));
            }
            LinkList();
        }
        override public void AddUnlinkedRow(int i)
        {
            double min0 = 0.0;
            double ml0 = 0.0;
            double max0 = 0.0;
            double x0 = 0.0;
            if (i == 0)
            {
                if (Data.Count != 0)
                {
                    x0 = ((TriangularRow)Data[i]).X;
                    min0 = ((Triangular)((TriangularRow)Data[i]).Y).Min;
                    ml0 = ((Triangular)((TriangularRow)Data[i]).Y).MostLikely;
                    max0 = ((Triangular)((TriangularRow)Data[i]).Y).Max;
                }
                DataProviderExtensions.AddRow(this, i, new TriangularRow(x0, new Triangular(min0,ml0,max0), IsStrictMonotonic));
                return;
            }
            if (Data.Count > i)
            {
                x0 = ((TriangularRow)Data[i-1]).X;
                min0 = ((Triangular)((TriangularRow)Data[i-1]).Y).Min;
                ml0 = ((Triangular)((TriangularRow)Data[i-1]).Y).MostLikely;
                max0 = ((Triangular)((TriangularRow)Data[i-1]).Y).Max;
                double x1 = 0.0;
                double min1 = 0.0;
                double ml1 = 0.0;
                double max1 = 0.0;
                x1 = ((TriangularRow)Data[i]).X;
                min1 = ((Triangular)((TriangularRow)Data[i]).Y).Min;
                ml1 = ((Triangular)((TriangularRow)Data[i]).Y).MostLikely;
                max1 = ((Triangular)((TriangularRow)Data[i]).Y).Max;
                double x = x0 + ((x1 - x0) / 2.0);
                double min = min0 + ((min1 - min0) / 2.0);
                double ml = ml0 + ((ml1 - ml0) / 2.0);
                double max = max0 + ((max1 - max0) / 2.0);
                DataProviderExtensions.AddRow(this, i, new TriangularRow(x, new Triangular(min, ml, max), IsStrictMonotonic));
            }
            else
            {
                x0 = ((TriangularRow)Data[Data.Count-1]).X;
                min0 = ((Triangular)((TriangularRow)Data[Data.Count - 1]).Y).Min;
                ml0 = ((Triangular)((TriangularRow)Data[Data.Count - 1]).Y).MostLikely;
                max0 = ((Triangular)((TriangularRow)Data[Data.Count - 1]).Y).Max;
                DataProviderExtensions.AddRow(this, i, new TriangularRow(x0, new Triangular(min0, ml0, max0), IsStrictMonotonic));
                return;
            }

        }
    }
}
