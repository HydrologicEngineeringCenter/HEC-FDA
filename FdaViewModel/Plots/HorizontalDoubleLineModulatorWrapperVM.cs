using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Plots
{
    //[Author(q0heccdm, 1 / 19 / 2018 11:18:59 AM)]
    public class HorizontalDoubleLineModulatorWrapperVM:BaseViewModel,IIndividualLinkedPlotWrapper
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 1/19/2018 11:18:59 AM
        #endregion
        #region Fields

        public event EventHandler ShowImportButton;
        public event EventHandler ShowTheImporter;
        public event EventHandler CurveUpdated;

        #endregion
        #region Properties
        public IndividualLinkedPlotVM PlotVM
        {
            get;
            set;
        }

        public string SubTitle
        {
            get;
            set;
        }
        #endregion
        #region Constructors
        public HorizontalDoubleLineModulatorWrapperVM()
        {
        }
        public HorizontalDoubleLineModulatorWrapperVM(IndividualLinkedPlotVM plotVM):this()
        {
            PlotVM = plotVM;
        }
        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

     
        #endregion
        #region Functions
        #endregion
    }
}
