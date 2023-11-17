using System.Collections.Generic;
using System.Data;

namespace HEC.FDA.ViewModel.Inventory
{
    public class StructureMissingElevationEditorVM : BaseViewModel
    { 
        public DataTable MissingDataTable { get; }

        public StructureMissingElevationEditorVM(List<StructureMissingDataRowItem> rows,  InventoryColumnSelectionsVM inventoryColumnSelectionsVM)
        {
            
            MissingDataTable = new DataTable();
            CS.Collections.CustomObservableCollection<InventoryColumnSelectionsRowItem> requiredRows = inventoryColumnSelectionsVM.RequiredRows;

            foreach(InventoryColumnSelectionsRowItem item in requiredRows)
            {
                MissingDataTable.Columns.Add(item.MissingValueColumnHeader);
            }
            foreach (StructureMissingDataRowItem item in rows)
            {
                object[] rowVals = inventoryColumnSelectionsVM.GetRequiredRowValues(item.ID);
                DataRow myRow = MissingDataTable.NewRow();
                for(int i = 0;i<requiredRows.Count;i++)
                {
                    myRow[i] = rowVals[i];
                }
                MissingDataTable.Rows.Add(myRow);
            }
    
        }
    }
}
