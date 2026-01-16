using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.Hydraulics;

public interface IHaveListOfWSERows
{
    public ObservableCollection<WaterSurfaceElevationRowItemVM> ListOfRows { get; }
        public void RemoveRows(List<int> rowIndices)
    {
        for (int i = rowIndices.Count - 1; i >= 0; i--)
        {
            ListOfRows.RemoveAt(rowIndices[i]);
        }
    }
}
