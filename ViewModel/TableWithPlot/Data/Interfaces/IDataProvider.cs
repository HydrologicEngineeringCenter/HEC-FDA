using HEC.FDA.Model.paireddata;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces
{
    public interface IDataProvider
    {
        string Name { get; }
        ObservableCollection<object> Data { get; }
        void RemoveRows(List<int> rowIndices);
        void AddRow(int i);
        void AddUnlinkedRow(int i);
        UncertainPairedData ToUncertainPairedData(string xlabel, string ylabel, string name, string description, string category);
    }
}

