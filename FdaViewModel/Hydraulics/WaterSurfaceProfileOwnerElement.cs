using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Hydraulics
{
    class WaterSurfaceProfileOwnerElement : Utilities.OwnerElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableName = "Water Surface Profiles";
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
        public WaterSurfaceProfileOwnerElement(BaseFdaElement owner):base(owner)
        {
            Name = _TableName;

            Utilities.NamedAction addWSPs = new Utilities.NamedAction();
            addWSPs.Header = "Import Water Surface Profile";
            addWSPs.Action = AddNewWSP;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addWSPs);

            Actions = localActions;
        }

        #endregion
        #region Voids
        private void AddNewWSP(object arg1, EventArgs arg2)
        {

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
