using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Editors;

namespace ViewModel.Inventory
{
    public class StructureMissingElevationEditorVM : BaseEditorVM
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


        public StructureMissingElevationEditorVM(List<StructureMissingDataRowItem> rows, bool usingFirstFloorElevation, bool elevsFromTerrainFile) :base(null)
        {
            Rows = rows;
            UsingFirstFloorElevation = usingFirstFloorElevation;
            ElevsFromTerrainFile = elevsFromTerrainFile;
            if(!usingFirstFloorElevation && !elevsFromTerrainFile)
            {
                FromStructureFile = true;
            }
            else
            {
                FromStructureFile = false;
            }
        }

        public override void AddValidationRules()
        {
            //this is here so that we don't look for a name property.
        }

        public override void Save()
        {
            //validate that there is a value for each row.

            int i = 0;
        }
    }
}
