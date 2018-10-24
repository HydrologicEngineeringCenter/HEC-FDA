using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using FdaViewModel.Utilities;

namespace FdaViewModel.Hydraulics
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
        public GriddedHydraulicsOwnerElement(BaseFdaElement owner):base(owner)
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
            List<Watershed.TerrainElement> TerrainList = StudyCache.TerrainElements;// GetElementsOfType<Watershed.TerrainElement>();
            
            
            GridImporterVM vm = new GridImporterVM(TerrainList,2001,2002);
            Navigate(vm);
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
