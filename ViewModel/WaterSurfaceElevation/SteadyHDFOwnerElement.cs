using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
