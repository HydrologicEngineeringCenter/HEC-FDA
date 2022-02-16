using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Hydraulics
{
    public class GriddedHydraulicsOwnerElement: Utilities.ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableName = "Gridded Water Surface Profiles";
        #endregion
        #region Properties
   

        #endregion
        #region Constructors
        public GriddedHydraulicsOwnerElement( ):base()
        {
            Name = _TableName;

            Utilities.NamedAction addGrids = new Utilities.NamedAction();
            addGrids.Header = "Import Gridded Hydraulics";
            addGrids.Action = AddNewGriddedHydraulics;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addGrids);

            Actions = localActions;
        }

        #endregion
        #region Voids
        private void AddNewGriddedHydraulics(object arg1, EventArgs arg2)
        {
            List<Watershed.TerrainElement> TerrainList = StudyCache.GetChildElementsOfType<Watershed.TerrainElement>();


            GridImporterVM vm = new GridImporterVM(TerrainList,2001,2002);
            string header = "New Hydraulic Data";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "NewHydraulicData");
            Navigate(tab);
            if(!vm.HasError & !vm.WasCanceled)
            {
                //create a child and add it
                GriddedHydraulicsElement element = new GriddedHydraulicsElement( vm);
                element.RequestNavigation += Navigate;
                AddElement(element);

            }
        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
     
        #endregion
        #region Functions
 
        #endregion
    }
}
