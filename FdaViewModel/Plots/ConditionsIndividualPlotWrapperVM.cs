using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.Plots
{
    //[Author(q0heccdm, 12 / 20 / 2017 1:24:49 PM)]
    public class ConditionsIndividualPlotWrapperVM : BaseViewModel, IIndividualLinkedPlotWrapper
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/20/2017 1:24:49 PM
        #endregion
        #region Fields
        private bool _FlipXAxis;
        private bool _SetYAxisToLog;
        private string _Title;
        private string _XAxisLabel;
        private string _YAxisLabel;
        private string _SubTitle;
        private bool _DisplayImportButton;


        public event EventHandler ShowImportButton;
        public event EventHandler ShowTheImporter;
        public event EventHandler CurveUpdated;

        private IndividualLinkedPlotVM _PlotVM;

        private bool _OutOfRange;
        #endregion
        #region Properties
        public bool DisplayOutOfRange
        {
            get { return _OutOfRange; }
            set { _OutOfRange = value;NotifyPropertyChanged(); }
        }

        public bool DisplayImportButton
        {
            get { return _DisplayImportButton; }
            set { _DisplayImportButton = value; }
        }
        public string Title
        {
            get { return _Title; }
            set { _Title = value; NotifyPropertyChanged(); }
        }
        public string SubTitle
        {
            get { return _SubTitle; }
            set { _SubTitle = value; NotifyPropertyChanged(); }
        }
        public string XAxisLabel
        {
            get { return _XAxisLabel; }
            set { _XAxisLabel = value; NotifyPropertyChanged(); }
        }

        public string YAxisLabel
        {
            get { return _YAxisLabel; }
            set { _YAxisLabel = value; NotifyPropertyChanged(); }
        }

        public bool FlipXAxis
        {
            get { return _FlipXAxis; }
            set { _FlipXAxis = value; NotifyPropertyChanged(); }
        }

        public bool SetYAxisToLog
        {
            get { return _SetYAxisToLog; }
            set { _SetYAxisToLog = value; NotifyPropertyChanged(); }
        }
        public IndividualLinkedPlotVM PlotVM
        {
            get { return _PlotVM; }
            set { _PlotVM = value; }// _PlotVM.CurveUpdated += CurveHasBeenUpdated; }
        }
        #endregion
        #region Constructors
        public ConditionsIndividualPlotWrapperVM():base()
        {

        }
        public ConditionsIndividualPlotWrapperVM(bool setYAxisToLog, bool flipFreqAxis, string title, string xAxisLabel, string yAxisLabel, bool displayImportButton = true)
        {
            DisplayImportButton = displayImportButton;
            FlipXAxis = flipFreqAxis;
            SetYAxisToLog = setYAxisToLog;
            Title = title;
            XAxisLabel = xAxisLabel;
            YAxisLabel = yAxisLabel;
            
        }        

        #endregion
        #region Voids
        //public void CurveHasBeenUpdated(object sender, EventArgs e)
        //{
        //    CurveUpdated?.Invoke(sender, e);
        //}
        public void ShowTheImportButton(object sender, EventArgs e)
        {
            ShowImportButton?.Invoke(sender, e);
        }

        public void PlotIsInsideRange(object sender, EventArgs e)
        {
            DisplayOutOfRange = false;
        }
        public void PlotIsOutsideRange(object sender, EventArgs e)
        {
            DisplayOutOfRange = true;
        }
        public void ShowTheImporterForm(object sender, EventArgs e)
        {
            ShowTheImporter?.Invoke(sender, e);
        }
        public override void AddValidationRules()
        {
           // throw new NotImplementedException();
        }

     
        #endregion
        #region Functions
        #endregion
    }
}
