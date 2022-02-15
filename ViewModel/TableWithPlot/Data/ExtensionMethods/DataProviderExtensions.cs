using ViewModel.TableWithPlot.Rows.Base;
using ViewModel.TableWithPlot.Data.Interfaces;

namespace ViewModel.TableWithPlot.Data.ExtensionMethods
{
    public static class DataProviderExtensions
    {
        public static void AddRow(this IDataProvider dataProvider, int index, SequentialRow row )
        {
            dataProvider.Data.Insert(index, row);
        }
    }
}
