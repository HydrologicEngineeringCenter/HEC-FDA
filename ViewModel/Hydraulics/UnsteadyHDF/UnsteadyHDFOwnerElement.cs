using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Hydraulics.UnsteadyHDF
{
    public class UnsteadyHDFOwnerElement : ParentElement
    {
        public UnsteadyHDFOwnerElement() : base()
        {
            Name = StringConstants.UNSTEADY_HDF;
            CustomTreeViewHeader = new CustomHeaderVM(Name);
            IsBold = false;

            NamedAction import = new NamedAction();
            import.Header = StringConstants.IMPORT_HYDRAULICS_MENU;
            import.Action = ImportWaterSurfaceElevations;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(import);
            if(!StringConstants.IS_BETA_RELEASE)
            {
                Actions = localActions;
            }

            StudyCache.WaterSurfaceElevationAdded += AddWaterSurfaceElevationElement;
            StudyCache.WaterSurfaceElevationRemoved += RemoveWaterSurfaceElevationElement;
            StudyCache.WaterSurfaceElevationUpdated += UpdateWaterSurfaceElevationElement;
        }

        private void UpdateWaterSurfaceElevationElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            UpdateElement(e.NewElement);
        }
        private void RemoveWaterSurfaceElevationElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RemoveElement(e.Element);
        }
        private void AddWaterSurfaceElevationElement(object sender, Saving.ElementAddedEventArgs e)
        {
            if(e.Element is HydraulicElement elem)
            {
                if(elem.HydroType == HydraulicType.Unsteady)
                {
                    AddElement(e.Element);
                }
            }
        }

        public void ImportWaterSurfaceElevations(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
               .WithSiblingRules(this);

            UnsteadyHDFImporterVM vm = new UnsteadyHDFImporterVM(actionManager);

            string header = StringConstants.IMPORT_HYDRAULICS_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, StringConstants.IMPORT_HYDRAULICS_HEADER + "Unsteady");
            Navigate(tab, false, false);
        }


    }
}
