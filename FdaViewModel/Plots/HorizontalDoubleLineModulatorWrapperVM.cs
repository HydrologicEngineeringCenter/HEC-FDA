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

        private bool _TrackerVisible = true;
        private bool _AreaPlotVisible = true;
        private FdaModel.ComputationPoint.PerformanceThreshold _Threshold;

        #endregion
        #region Properties
        public FdaModel.ComputationPoint.PerformanceThreshold Threshold
        {
            get { return _Threshold; }
            set { _Threshold = value; NotifyPropertyChanged(); }
        }
        public bool AreaPlotVisible
        {
            get { return _AreaPlotVisible; }
            set { _AreaPlotVisible = value; NotifyPropertyChanged(); }
        }
        public bool TrackerVisible
        {
            get { return _TrackerVisible; }
            set { _TrackerVisible = value; NotifyPropertyChanged(); }
        }
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
