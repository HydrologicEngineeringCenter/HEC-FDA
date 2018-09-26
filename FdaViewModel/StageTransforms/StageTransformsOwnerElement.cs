using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.StageTransforms
{
    class StageTransformsOwnerElement: Utilities.OwnerElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return TableName;
        }
        #endregion
        #region Constructors
        public StageTransformsOwnerElement(BaseFdaElement owner) : base(owner)
        {
            Name = "Stage Transforms";
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);
            //Utilities.NamedAction add = new Utilities.NamedAction();
            //add.Header = "Create New Levee Feature";
            //add.Action = AddNewLeveeFeature;

            //List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            //localActions.Add(add);

            //Actions = localActions;
        }
        #endregion
        #region Voids
        public override void AddBaseElements()
        {
            RatingCurveOwnerElement r = new RatingCurveOwnerElement(this);
            AddElement(r);

            ExteriorInteriorOwnerElement i = new ExteriorInteriorOwnerElement(this);
            AddElement(i);
        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
        #endregion
        #region Functions
        public override string TableName
        {
            get
            {
                return "";
            }
        }
        public override void Save()
        {
            foreach(Utilities.OwnedElement ele in _Elements)
            {
                ele.Save();
            }
        }
        public override string[] TableColumnNames()
        {
            throw new NotImplementedException();
        }
        public override Type[] TableColumnTypes()
        {
            throw new NotImplementedException();
        }
        public override bool SavesToTable()
        {
            return false;
        }
        public override OwnedElement CreateElementFromRowData(object[] rowData)
        {
            return null;
        }
        public override void AddElement(object[] rowData)
        {
            
        }
        #endregion
    }
}
