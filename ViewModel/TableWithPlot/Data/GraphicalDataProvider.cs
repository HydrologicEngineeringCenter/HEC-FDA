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
            Data.Add(new GraphicalRow(.99, 458d));
            Data.Add(new GraphicalRow(.5, 468.33d));
            Data.Add(new GraphicalRow(.2, 469.97d));
            Data.Add(new GraphicalRow(.1, 471.95d));
            Data.Add(new GraphicalRow(.04, 473.06d));
            Data.Add(new GraphicalRow(.02, 473.66d));
            Data.Add(new GraphicalRow(.01, 474.53d));
            Data.Add(new GraphicalRow(.004, 475.11d));
            Data.Add(new GraphicalRow(.002, 477.4d));
            LinkList();
            xMax = .999999999999999999999;
            xMin = .000000000000000000001;
            SetGlobalMaxAndMin();
            AddHandlers();
        }
        public GraphicalDataProvider(bool useFlow)
        {
            Name = "Deterministic";
            if (useFlow)
            {
                Data.Add(new GraphicalRow(.99, 1400d));
                Data.Add(new GraphicalRow(.5, 1500d));
                Data.Add(new GraphicalRow(.2, 2120d));
                Data.Add(new GraphicalRow(.1, 3140d));
                Data.Add(new GraphicalRow(.04, 4210d));
                Data.Add(new GraphicalRow(.02, 5070d));
                Data.Add(new GraphicalRow(.01, 6240d));
                Data.Add(new GraphicalRow(.004, 7050d));
                Data.Add(new GraphicalRow(.002, 9680d));
            } else
            {
                Data.Add(new GraphicalRow(.99, 458d));
                Data.Add(new GraphicalRow(.5, 468.33d));
                Data.Add(new GraphicalRow(.2,469.97d));
                Data.Add(new GraphicalRow(.1, 471.95d));
                Data.Add(new GraphicalRow(.04, 473.06d));
                Data.Add(new GraphicalRow(.02, 473.66d));
                Data.Add(new GraphicalRow(.01, 474.53d));
                Data.Add(new GraphicalRow(.004, 475.11d));
                Data.Add(new GraphicalRow(.002, 477.4d));
            }
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
