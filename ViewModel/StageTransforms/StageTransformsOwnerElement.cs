using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.StageTransforms
{
    class StageTransformsOwnerElement: ParentElement
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
            Name = StringConstants.STAGE_TRANSFORM_FUNCTIONS;
            CustomTreeViewHeader = new CustomHeaderVM(Name);
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
    }
}
