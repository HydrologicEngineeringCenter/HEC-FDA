using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces
{
    public interface IDisplayWithFDADataGrid
    {
        ObservableCollection<object> Data { get; }
        void RemoveRows(List<int> rowIndices);
        void AddRow(int i);
        
    }
}
