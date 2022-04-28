using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.FlowTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 9:23:47 AM)]
    class FlowTransformsOwnerElement : ParentElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 9:23:47 AM
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public FlowTransformsOwnerElement( ) : base()
        {
            Name = "Flow Transforms";
            CustomTreeViewHeader = new CustomHeaderVM(Name);
        }
        #endregion

        public  void AddBaseElements(Study.FDACache cache)
        {
            InflowOutflowOwnerElement io = new InflowOutflowOwnerElement();
            AddElement(io);
            cache.InflowOutflowParent = io;
        }

    }
}
