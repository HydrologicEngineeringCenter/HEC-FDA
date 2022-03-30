using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.WaterSurfaceElevation
{
    //[Author(q0heccdm, 9 / 1 / 2017 8:46:34 AM)]
    public class WaterSurfaceElevationOwnerElement : ParentElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 9/1/2017 8:46:34 AM
        #endregion
        #region Fields
        #endregion
        #region Properties   
        #endregion
        #region Constructors
        public WaterSurfaceElevationOwnerElement( ):base()
        {
            Name = "Hydraulics";
            IsBold = true;
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction import = new NamedAction();
            import.Header = "Import Hydraulics";
            import.Action = ImportWaterSurfaceElevations;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(import);

            Actions = localActions;

            StudyCache.WaterSurfaceElevationAdded += AddWaterSurfaceElevationElement;
            StudyCache.WaterSurfaceElevationRemoved += RemoveWaterSurfaceElevationElement;
            StudyCache.WaterSurfaceElevationUpdated += UpdateWaterSurfaceElevationElement;
        }
        #endregion
        #region Voids
        private void UpdateWaterSurfaceElevationElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement( e.NewElement);
        }
        private void RemoveWaterSurfaceElevationElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void AddWaterSurfaceElevationElement(object sender, Saving.ElementAddedEventArgs e)
        {
            AddElement(e.Element);
        }

        public void ImportWaterSurfaceElevations(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
               .WithSiblingRules(this);

            WaterSurfaceElevationImporterVM vm = new WaterSurfaceElevationImporterVM(actionManager);

            string header = "Import Hydraulics";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "ImportWatSurfElev");
            Navigate(tab, false,false);
        }
        #endregion     
    }
}
