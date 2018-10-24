using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.StageTransforms
{
    class StageTransformsOwnerElement: Utilities.ParentElement
    {
        #region Notes
        #endregion
        #region Fields
        #endregion
        #region Properties
     
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
        public  void AddBaseElements()
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
 
     
        #endregion
    }
}
