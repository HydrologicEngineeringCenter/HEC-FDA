using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.WaterSurfaceElevation
{
    public class UnsteadyHDFOwnerElement : ParentElement
    {
        public UnsteadyHDFOwnerElement() : base()
        {
            Name = StringConstants.UNSTEADY_HDF;
            CustomTreeViewHeader = new CustomHeaderVM(Name);
            IsBold = false;
        }
    }
}
