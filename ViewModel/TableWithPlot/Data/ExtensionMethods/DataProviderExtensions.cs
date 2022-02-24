using HEC.FDA.ViewModel.TableWithPlot.Rows;
using HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces;

namespace HEC.FDA.ViewModel.TableWithPlot.Data.ExtensionMethods
{
    public static class DataProviderExtensions
    {
        public static void AddRow(this IDataProvider dataProvider, int index, SequentialRow row )
        {
            dataProvider.Data.Insert(index, row);
        }
    }
}
