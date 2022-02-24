using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Inventory
{
    public class StructureMissingElevationEditorVM : BaseViewModel
    {
        private bool _UsingFirstFloorElevation;
        private bool _ElevsFromTerrainFile;
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

        public StructureMissingElevationEditorVM(List<StructureMissingDataRowItem> rows, bool usingFirstFloorElevation, bool elevsFromTerrainFile)
        {
            Rows = rows;
            UsingFirstFloorElevation = usingFirstFloorElevation;
            if (usingFirstFloorElevation)
            {
                ElevsFromTerrainFile = false;
            }
            else
            {
                ElevsFromTerrainFile = elevsFromTerrainFile;
            }
            FromStructureFile = !usingFirstFloorElevation && !elevsFromTerrainFile;      
        }
    }
}
