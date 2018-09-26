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
    public class GriddedHydraulicsOwnerElement: Utilities.OwnerElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableName = "Gridded Water Surface Profiles";
        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return TableName;
        }
        public override string TableName
        {
            get
            {
                return _TableName;
            }
        }

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
            List<Watershed.TerrainElement> TerrainList = GetElementsOfType<Watershed.TerrainElement>();
            
            
            GridImporterVM vm = new GridImporterVM(TerrainList,2001,2002);
            Navigate(vm);
            if(!vm.HasError & !vm.WasCancled)
            {
                //create a child and add it
                GriddedHydraulicsElement element = new GriddedHydraulicsElement(this, vm);
                element.RequestNavigation += Navigate;
                AddElement(element);

            }
        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
        public override void AddBaseElements()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Functions
        public override string[] TableColumnNames()
        {
            throw new NotImplementedException();
        }

        public override Type[] TableColumnTypes()
        {
            throw new NotImplementedException();
        }
        public override OwnedElement CreateElementFromRowData(object[] rowData)
        {
            return null;
        }
        public override void AddElement(object[] rowData)
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
