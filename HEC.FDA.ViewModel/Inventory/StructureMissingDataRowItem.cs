﻿namespace HEC.FDA.ViewModel.Inventory
{
    public class StructureMissingDataRowItem
    {      
        public string ID { get; set; }
        public bool IsMissingID { get; set; } //if this is true, WTF is ID?
        public bool IsMissingFirstFloorElevation { get; set; }
        public bool IsMissingOcctype { get; set; }
        public bool IsMissingFoundationHt { get; set; }
        public bool IsMissingGroundElevation { get; set; }
        public bool IsMissingStructureValue { get; set; }
        public bool IsMissingTerrainElevation { get; set; }

        public object[] RowValues { get; }

        public StructureMissingDataRowItem(string id, object[] rowVals, MissingDataType missingType)
        {
            RowValues = rowVals;
            ID = id;
            switch(missingType)
            {
                case MissingDataType.FirstFloorElevation:
                    IsMissingFirstFloorElevation = true;
                    break;
                case MissingDataType.ID:
                    IsMissingID = true;
                    break;
                case MissingDataType.Occtype:
                    IsMissingOcctype = true;
                    break;
                case MissingDataType.FoundationHt:
                    IsMissingFoundationHt = true;
                    break;
                case MissingDataType.GroundElevation:
                    IsMissingGroundElevation = true;
                    break;
                case MissingDataType.StructureValue:
                    IsMissingStructureValue = true;
                    break;
                case MissingDataType.TerrainElevation:
                    IsMissingTerrainElevation = true;
                    break;
            }
        }

    }
}
