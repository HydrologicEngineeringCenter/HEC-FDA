using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Plots
{
    //[Author(q0heccdm, 1 / 12 / 2018 2:08:57 PM)]
    public class DoubleLineModulatorWrapperVM :BaseViewModel, IIndividualLinkedPlotWrapper
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 1/12/2018 2:08:57 PM
        #endregion
        #region Fields

        public event EventHandler ShowImportButton;
        public event EventHandler ShowTheImporter;
        public event EventHandler CurveUpdated;

        private bool _TrackerVisible = true;
        private bool _AreaPlotVisible = true;
        private Model.ComputationPoint.PerformanceThreshold _Threshold;

        #endregion
        #region Properties
        public bool DisplayImportButton
        {
            get;set;
        }
        public Model.ComputationPoint.PerformanceThreshold Threshold
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
        public DoubleLineModulatorWrapperVM()
        {
        }
        /// <summary>
        /// Call this when opening up a conditions node that already has values for this plot
        /// </summary>
        /// <param name="plotVM"></param>
        public DoubleLineModulatorWrapperVM(IndividualLinkedPlotVM plotVM):this()
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
