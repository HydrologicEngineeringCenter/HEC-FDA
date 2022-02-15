using ViewModel.TableWithPlot.Rows;
using ViewModel.TableWithPlot.Data.ExtensionMethods;
using ViewModel.TableWithPlot.Data.Base;
using System.Xml.Linq;
using Statistics;
using Statistics.Distributions;

namespace ViewModel.TableWithPlot.Data
{
    public class DeterministicDataProvider : BaseDataProvider
    {
        public DeterministicDataProvider()
        {
            Name = "Deterministic";
            Data.Add(new DeterministicRow(0.0d, 0.0d));
            Data.Add(new DeterministicRow(1.0d, 1.0d));
            Data.Add(new DeterministicRow(2.0d, 2.0d));
            Data.Add(new DeterministicRow(3.0d, 3.0d));
            Data.Add(new DeterministicRow(4.0d, 4.0d));
            Data.Add(new DeterministicRow(5.0d, 5.0d));
            Data.Add(new DeterministicRow(10000.0d, 10000.0d));
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
                DataProviderExtensions.AddRow(this, i, new DeterministicRow(x0, y0));
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
                DataProviderExtensions.AddRow(this, i, new DeterministicRow(x, y));
            }
            else
            {
                x0 = ((DeterministicRow)Data[Data.Count - 1]).X;
                y0 = ((DeterministicRow)Data[Data.Count - 1]).Y.InverseCDF(.5);
                DataProviderExtensions.AddRow(this, i, new DeterministicRow(x0, y0));
                return;
            }

        }
    }
}
