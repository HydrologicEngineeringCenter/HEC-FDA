﻿using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.Hydraulics.SteadyHDF;
using HEC.FDA.ViewModel.Hydraulics.UnsteadyHDF;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Hydraulics
{
    //[Author(q0heccdm, 9 / 1 / 2017 8:46:34 AM)]
    public class HydraulicsOwnerElement : ParentElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 9/1/2017 8:46:34 AM
        #endregion

        #region Constructors
        public HydraulicsOwnerElement( ):base()
        {
            Name = StringConstants.HYDRAULICS;
            IsBold = true;
            CustomTreeViewHeader = new CustomHeaderVM(Name);
        }
        #endregion

        public void AddBaseElements(FDACache cache)
        {
            UnsteadyHDFOwnerElement unsteady = new UnsteadyHDFOwnerElement();
            AddElement(unsteady);
            SteadyHDFOwnerElement steady = new SteadyHDFOwnerElement();
            AddElement(steady);
            GriddedDataOwnerElement gridded = new GriddedDataOwnerElement();
            AddElement(gridded);
        }
    }
}
