using System.Collections.Generic;
using System.Data;

namespace HEC.FDA.ViewModel.Inventory
{
    public class StructureMissingElevationEditorVM : BaseViewModel
    {
        private bool _UsingFirstFloorElevation;
        private bool _ElevsFromTerrainFile;

        public DataTable MissingDataTable { get; }

        public List<StructureMissingDataRowItem> Rows { get; } = new List<StructureMissingDataRowItem>();
        public bool UsingFirstFloorElevation
        {
            get { return _UsingFirstFloorElevation; }
            set { _UsingFirstFloorElevation = value; NotifyPropertyChanged(); }
        }
        public bool ElevsFromTerrainFile
        {
            get { return _ElevsFromTerrainFile; }
            set { _ElevsFromTerrainFile = value; NotifyPropertyChanged(); }
        }

        public bool FromStructureFile { get; set; }

        public StructureMissingElevationEditorVM(StructuresMissingDataManager manager)
        {
            
            MissingDataTable = new DataTable();
            MissingDataTable.Columns.Add("ID");
            foreach (string header in manager.ColumnsWithMissingData)
            {
                MissingDataTable.Columns.Add(header);
            }
            foreach (StructureMissingDataRowItem row in manager.GetRows())
            {
                DataRow myRow = MissingDataTable.NewRow();
                myRow["ID"] = row.ID;
                foreach (string header in manager.ColumnsWithMissingData)
                {
                    switch(header)
                    {
                        case "First Floor Elevation":
                            myRow[header] = StringIfMissing(row.IsMissingFirstFloorElevation);
                            break;
                        case "Ground Elevation":
                            myRow[header] = StringIfMissing(row.IsMissingGroundElevation);
                            break;
                        case "Foundation Height":
                            myRow[header] = StringIfMissing(row.IsMissingFoundationHt);
                            break;
                        case "Structure Value":
                            myRow[header] = StringIfMissing(row.IsMissingStructureValue);
                            break;
                        case "Occupancy Type":
                            myRow[header] = StringIfMissing(row.IsMissingOcctype);
                            break;
                        case "Terrain Elevation":
                            myRow[header] = StringIfMissing(row.IsMissingTerrainElevation);
                            break;
                        case "ID":
                            myRow[header] = StringIfMissing(row.IsMissingID);
                            break;
                    }
                }
                MissingDataTable.Rows.Add(myRow);
            }
        }

        private string StringIfMissing(bool foo)
        {
            if (foo)
            {
                return "Missing";
            }
            else
            {
                return "";
            }
        }
    }
}
