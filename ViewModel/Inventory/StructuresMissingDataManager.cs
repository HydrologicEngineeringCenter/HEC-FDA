using System.Collections.Generic;
using System.Linq;

namespace ViewModel.Inventory
{
    /// <summary>
    /// This class is used to help organize the missing data from the structures dbf file. 
    /// </summary>
    public class StructuresMissingDataManager
    {
        private Dictionary<string, StructureMissingDataRowItem> _MissingDataRows = new Dictionary<string, StructureMissingDataRowItem>();

        //there are three cases: First floor elev, from terrain file, from structure file
        public StructuresMissingDataManager()
        {

        }

        public void AddStructuresWithMissingData(List<StructureMissingDataRowItem> rows)
        {
            foreach(StructureMissingDataRowItem row in rows)
            {
                AddStructureWithMissingData(row);
            }
        }

        public void AddStructureWithMissingData(StructureMissingDataRowItem row)
        {
            if (_MissingDataRows.ContainsKey(row.ID))
            {
                UpdateRow(_MissingDataRows[row.ID], row);
            }
            else
            {
                _MissingDataRows.Add(row.ID, row);
            }
        }

        private void UpdateRow(StructureMissingDataRowItem rowInDictionary, StructureMissingDataRowItem newRow)
        {
            if(newRow.IsMissingFoundationHt)
            {
                rowInDictionary.IsMissingFoundationHt = true;
            }
            if(newRow.IsMissingGroundElevation)
            {
                rowInDictionary.IsMissingGroundElevation = true;
            }
            if (newRow.IsMissingOcctype)
            {
                rowInDictionary.IsMissingOcctype = true;
            }
            if (newRow.IsMissingStructureValue)
            {
                rowInDictionary.IsMissingStructureValue = true;
            }
            if (newRow.IsMissingTerrainElevation)
            {
                rowInDictionary.IsMissingTerrainElevation = true;
            }
            if(newRow.IsMissingFirstFloorElevation)
            {
                rowInDictionary.IsMissingFirstFloorElevation = true;
            }
        }

        public List<StructureMissingDataRowItem> GetRows()
        {
            return _MissingDataRows.Values.ToList();
        }
    }
}
