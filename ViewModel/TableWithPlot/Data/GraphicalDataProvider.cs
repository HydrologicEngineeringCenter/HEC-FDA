using HEC.FDA.ViewModel.TableWithPlot.Data.Abstract;
using HEC.FDA.ViewModel.TableWithPlot.Data.ExtensionMethods;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.ViewModel.TableWithPlot.Data
{
    public class GraphicalDataProvider : BaseDataProvider
    {

        public double[] Xs
        {
            get
            {
                double[] xs = new double[Data.Count];
                int i = 0;
                foreach(GraphicalRow row in Data)
                {
                    xs[i] = row.X;
                    i++;
                }
                return xs;
            }
        }
        public double[] Ys
        {
            get
            {
                double[] ys = new double[Data.Count];
                int i = 0;
                foreach (GraphicalRow row in Data)
                {
                    ys[i] = row.Y.InverseCDF(.5);
                    i++;
                }
                return ys;
            }
        }

        public GraphicalDataProvider()
        {
            Name = "Deterministic";
            Data.Add(new GraphicalRow(.99, 500d));
            Data.Add(new GraphicalRow(.5, 2000d));
            Data.Add(new GraphicalRow(.1, 34900d));
            Data.Add(new GraphicalRow(.02, 66900d));
            Data.Add(new GraphicalRow(.01, 86000d));
            Data.Add(new GraphicalRow(.002,146000d));
            LinkList();
            xMax = .999999999999999999999;
            xMin = .000000000000000000001;
            SetGlobalMaxAndMin();
            AddHandlers();
        }
        override public void AddUnlinkedRow(int i)
        {
            DataProviderExtensions.AddRow(this, i, new GraphicalRow(0, 0));
        }
        private void AddHandlers()
        {
            foreach(GraphicalRow row in Data)
            {
                row.PropertyChanged += Row_PropertyChanged;
            }
        }

        private void Row_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           if(e.PropertyName != nameof(GraphicalRow.Confidence975) && e.PropertyName != nameof(GraphicalRow.Confidence025))
            {
                InvalidateRows();
            }
        }

        private void InvalidateRows()
        {
            foreach (var row in Data)
            {
                ((GraphicalRow)row).Confidence975 = 0;
                ((GraphicalRow)row).Confidence025 = 0;
            }
        }

        override public void RemoveRows(List<int> rowIndices)
        {
            for (int i = rowIndices.Count() - 1; i >= 0; i--)
            {
                Data.RemoveAt(rowIndices[i]);
            }
            LinkList();
            InvalidateRows();
        }
    }
}
