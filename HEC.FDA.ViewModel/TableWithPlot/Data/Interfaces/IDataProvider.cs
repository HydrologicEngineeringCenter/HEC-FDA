using HEC.FDA.Model.paireddata;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces
{
    public interface IDataProvider:IDisplayWithFDADataGrid
    {
        string Name { get; }
        void AddUnlinkedRow(int i);
        UncertainPairedData ToUncertainPairedData(string xlabel, string ylabel, string name, string description, string category = "Unassigned", string assetCategory = "Unassigned");
        void UpdateFromUncertainPairedData(UncertainPairedData data);
    }
}

