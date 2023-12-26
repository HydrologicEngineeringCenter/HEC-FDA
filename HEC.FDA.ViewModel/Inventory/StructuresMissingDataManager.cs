using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.ViewModel.Inventory
{
    /// <summary>
    /// This class is used to help organize the missing data from the structures dbf file. 
    /// </summary>
    public class StructuresMissingDataManager
    {
        //dictionary of row ID and row. Row ID is also carried in the row item, but having it in a dictionary makes it easier to know when to update rather than add to the list.
        private Dictionary<string, StructureMissingDataRowItem> _MissingDataRows = new Dictionary<string, StructureMissingDataRowItem>();

        public List<string> ColumnsWithMissingData { get; private set; } = new();
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
            AddMissingDataColumnToNeededList(row);
            if (_MissingDataRows.ContainsKey(row.ID))
            {
                UpdateRow(_MissingDataRows[row.ID], row);
            }
            else
            {
                _MissingDataRows.Add(row.ID, row);
            }
        }
        const string FIRSTFLOORELEVATION = "First Floor Elevation";
        const string GROUNDELEVATION = "Ground Elevation";
        const string FOUNDATIONHEIGHT = "Foundation Height";
        const string STRUCTUREVALUE = "Structure Value";
        const string OCCUPANCYTYPE = "Occupancy Type";
        const string TERRAINELEVATION = "Terrain Elevation";
        const string ID = "ID";

        private void AddMissingDataColumnToNeededList(StructureMissingDataRowItem row)
        {
            if(row.IsMissingFirstFloorElevation && !ColumnsWithMissingData.Contains(FIRSTFLOORELEVATION))
            {
                ColumnsWithMissingData.Add(FIRSTFLOORELEVATION);
            }
            if(row.IsMissingStructureValue && !ColumnsWithMissingData.Contains(STRUCTUREVALUE))
            {
                ColumnsWithMissingData.Add(STRUCTUREVALUE);
            }
            if(row.IsMissingGroundElevation && !ColumnsWithMissingData.Contains(GROUNDELEVATION))
            {
                ColumnsWithMissingData.Add(GROUNDELEVATION);
            }
            if(row.IsMissingFoundationHt && !ColumnsWithMissingData.Contains(FOUNDATIONHEIGHT))
            {
                ColumnsWithMissingData.Add(FOUNDATIONHEIGHT);
            }
            if(row.IsMissingOcctype && !ColumnsWithMissingData.Contains(OCCUPANCYTYPE))
            {
                ColumnsWithMissingData.Add(OCCUPANCYTYPE);
            }
            if(row.IsMissingTerrainElevation && !ColumnsWithMissingData.Contains(TERRAINELEVATION))
            {
                ColumnsWithMissingData.Add(TERRAINELEVATION);
            }
            if(row.IsMissingID && !ColumnsWithMissingData.Contains(ID))
            {
                ColumnsWithMissingData.Add(ID);
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
