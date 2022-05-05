using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.WaterSurfaceElevation
{
    public class SteadyHDFOwnerElement: ParentElement
    {
        public SteadyHDFOwnerElement() : base()
        {
            Name = StringConstants.STEADY_HDF;
            CustomTreeViewHeader = new CustomHeaderVM(Name);
            IsBold = false;
        }
    }
}
