using FdaViewModel.Editors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FdaViewModel.WaterSurfaceElevation
{
    public class WaterSurfaceElevationImporterFDA1VM: BaseEditorVM
    {
        private ObservableCollection<WSERowItemVM> _elems;
        public ObservableCollection<WSERowItemVM> WaterSurfaceElevations
        {
            get { return _elems; }
            set { _elems = value; NotifyPropertyChanged(); }
        }
        public WaterSurfaceElevationImporterFDA1VM(EditorActionManager actionManager) : base(actionManager)
        {
            WaterSurfaceElevations = new ObservableCollection<WSERowItemVM>();
            //I think i need to just assign a name so that the ok button gets enabled
            Name = "name";
        }

        public void AddWSEElements(List<WaterSurfaceElevationElement> elements)
        {
            foreach (WaterSurfaceElevationElement elem in elements)
            {
                WaterSurfaceElevations.Add(new WSERowItemVM(elem));
            }
        }

        public override bool RunSpecialValidation()
        {
            //what about name conflicts here?
            if(WaterSurfaceElevations.Count == 0)
            {
                MessageBox.Show("No water surface profiles were selected. Select an FDA version 1 study to import profiles from.", "No Profiles to Import", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            else
            {
                return true;
            }

        }
        public override void Save()
        {
            foreach(WSERowItemVM row in WaterSurfaceElevations)
            {
                if(row.IsSelected)
                {
                    string newName = row.Name;
                    string newDesc = row.Description;
                    bool isDepthGrid = !row.IsUsingWSE;
                    WaterSurfaceElevationElement newElem = new WaterSurfaceElevationElement(newName, newDesc, row.Element.RelativePathAndProbability, isDepthGrid);
                    Saving.PersistenceFactory.GetWaterSurfaceManager().SaveNew(newElem);
                }
            }
        }
    }
}
