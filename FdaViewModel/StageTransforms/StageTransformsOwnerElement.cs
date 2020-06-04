using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public StageTransformsOwnerElement( ) : base()
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
        public  void AddBaseElements(Study.FDACache cache)
        {
            RatingCurveOwnerElement r = new RatingCurveOwnerElement();
            AddElement(r);
            cache.RatingCurveParent = r;


            ExteriorInteriorOwnerElement i = new ExteriorInteriorOwnerElement();
            AddElement(i);
            cache.ExteriorInteriorParent = i;
        }
      
        #endregion
        #region Functions
 
     
        #endregion
    }
}
