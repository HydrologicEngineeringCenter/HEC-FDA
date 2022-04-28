using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.WaterSurfaceElevation
{
    //[Author(q0heccdm, 9 / 1 / 2017 8:46:34 AM)]
    public class HydraulicsOwnerElement : ParentElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 9/1/2017 8:46:34 AM
        #endregion
        #region Fields
        #endregion
        #region Properties   
        #endregion
        #region Constructors
        public HydraulicsOwnerElement( ):base()
        {
            Name = StringConstants.HYDRAULICS;
            IsBold = true;
            CustomTreeViewHeader = new CustomHeaderVM(Name);
        }
        #endregion
        #region Voids
        
        #endregion


        public void AddBaseElements(Study.FDACache cache)
        {
            UnsteadyHDFOwnerElement impactAreaOwnerElem = new UnsteadyHDFOwnerElement();
            AddElement(impactAreaOwnerElem);
            SteadyHDFOwnerElement steady = new SteadyHDFOwnerElement();
            AddElement(steady);
            GriddedDataOwnerElement gridded = new GriddedDataOwnerElement();
            AddElement(gridded);
            //cache.ImpactAreaParent = impactAreaOwnerElem;
        }
    }
}
