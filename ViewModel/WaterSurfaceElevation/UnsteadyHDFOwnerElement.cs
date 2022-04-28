using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
