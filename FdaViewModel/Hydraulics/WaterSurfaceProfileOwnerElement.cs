using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Hydraulics
{
    class WaterSurfaceProfileOwnerElement : Utilities.ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableName = "Water Surface Profiles";
        #endregion
        #region Properties


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
        
        #endregion
        #region Functions
    
        #endregion
    }
}
