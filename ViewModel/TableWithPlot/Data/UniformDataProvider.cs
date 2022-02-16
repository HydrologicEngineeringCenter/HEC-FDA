using HEC.FDA.ViewModel.TableWithPlot.Rows;
using Statistics.Distributions;
using HEC.FDA.ViewModel.TableWithPlot.Data.ExtensionMethods;
using HEC.FDA.ViewModel.TableWithPlot.Data.Abstract;

namespace HEC.FDA.ViewModel.TableWithPlot.Data
{
    internal class UniformDataProvider: BaseDataProvider
    {
        public UniformDataProvider()
        {
            Name = "Uniform";
            Data.Add(new UniformRow(0.0d, new Uniform(0, 0)));
            Data.Add(new UniformRow(2.0d, new Uniform(0, 2)));
            LinkList();
        }
        override public void AddUnlinkedRow(int i)
        {
            double min0 = 0.0;
            double max0 = 0.0;
            double x0 = 0.0;
            if (i == 0)
            {
                if (Data.Count != 0)
                {
                    x0 = ((UniformRow)Data[i]).X;
                    min0 = ((Uniform)((UniformRow)Data[i]).Y).Min;
                    max0 = ((Uniform)((UniformRow)Data[i]).Y).Max;
                }
                DataProviderExtensions.AddRow(this, i, new UniformRow(x0, new Uniform(min0, max0)));
                return;
            }
            if (Data.Count > i)
            {
                x0 = ((UniformRow)Data[i - 1]).X;
                min0 = ((Uniform)((UniformRow)Data[i - 1]).Y).Min;
                max0 = ((Uniform)((UniformRow)Data[i - 1]).Y).Max;
                double x1 = 0.0;
                double min1 = 0.0;
                double max1 = 0.0;
                x1 = ((UniformRow)Data[i]).X;
                min1 = ((Uniform)((UniformRow)Data[i]).Y).Min;
                max1 = ((Uniform)((UniformRow)Data[i]).Y).Max;
                double x = x0 + ((x1 - x0) / 2.0);
                double min = min0 + ((min1 - min0) / 2.0);
                double max = max0 + ((max1 - max0) / 2.0);
                DataProviderExtensions.AddRow(this, i, new UniformRow(x, new Uniform(min, max)));
            }
            else
            {
                x0 = ((UniformRow)Data[Data.Count - 1]).X;
                min0 = ((Uniform)((UniformRow)Data[Data.Count - 1]).Y).Min;
                max0 = ((Uniform)((UniformRow)Data[Data.Count - 1]).Y).Max;
                DataProviderExtensions.AddRow(this, i, new UniformRow(x0, new Uniform(min0, max0)));
                return;
            }
        }
    }
}
