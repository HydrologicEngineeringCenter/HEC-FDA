using FdaViewModel.Conditions;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        //private bool _FlipXAxis;
        private bool _TrackerVisible = true;
        private bool _AreaPlotVisible = true;

        //private bool _SetYAxisToLog;
        //private bool _SetXAxisToLog;

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
        private Model.IMetric  _Metric;
        #endregion
        #region Properties
     
        public Model.IMetric Metric
        {
            get { return _Metric; }
            set { _Metric = value; NotifyPropertyChanged(); }
        }
        public bool AreaPlotVisible
        {
            get { return _AreaPlotVisible; }
            set { _AreaPlotVisible = value;NotifyPropertyChanged(); }
        }
        public bool TrackerVisible
        {
            get { return _TrackerVisible; }
            set { _TrackerVisible = value;NotifyPropertyChanged(); }
        }
        public bool DisplayOutOfRange
        {
            get { return _OutOfRange; }
            set { _OutOfRange = value;NotifyPropertyChanged(); }
        }

        //public bool DisplayImportButton
        //{
        //    get { return _DisplayImportButton; }
        //    set { _DisplayImportButton = value; NotifyPropertyChanged(); }
        //}
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

        //public bool FlipXAxis
        //{
        //    get { return _FlipXAxis; }
        //    set { _FlipXAxis = value; NotifyPropertyChanged(); }
        //}

        //public bool SetYAxisToLog
        //{
        //    get { return _SetYAxisToLog; }
        //    set { _SetYAxisToLog = value; NotifyPropertyChanged(); }
        //}

        //public bool SetXAxisToLog
        //{
        //    get { return _SetXAxisToLog; }
        //    set { _SetXAxisToLog = value; NotifyPropertyChanged(); }
        //}

        public IndividualLinkedPlotVM PlotVM
        {
            get { return _PlotVM; }
            set { _PlotVM = value; }// _PlotVM.CurveUpdated += CurveHasBeenUpdated; }
        }

        public int SelectedElementID { get; set; }

        private bool _isXAxisLog;
        private bool _isYAxisLog;
        private bool _isProbabilityXAxis;
        private bool _isProbabilityYAxis;
        private bool _xAxisOnBottom;
        private bool _yAxisOnLeft;
        #endregion
        #region Constructors
        public ConditionsIndividualPlotWrapperVM():base()
        {

        }
        
        public ConditionsIndividualPlotWrapperVM(bool isXAxisLog, bool isYAxisLog, bool isProbabilityXAxis, bool isProbabilityYAxis, bool xAxisOnBottom, bool yAxisOnLeft)
        {
            _isXAxisLog = isXAxisLog;
            _isYAxisLog = isYAxisLog;
            _isProbabilityXAxis = isProbabilityXAxis;
            _isProbabilityYAxis = isProbabilityYAxis;
            _xAxisOnBottom = xAxisOnBottom;
            _yAxisOnLeft = yAxisOnLeft;
            //DisplayImportButton = displayImportButton;
            //FlipXAxis = flipFreqAxis;
            //SetYAxisToLog = setYAxisToLog;
             //SetXAxisToLog = setXAxisToLog;
            //Title = title;
            //XAxisLabel = xAxisLabel;
            //YAxisLabel = yAxisLabel;

        }

        #endregion
        #region Voids
        public void AddCurveToPlot(IFdaFunction function, string elementName,int selectedElemID, FdaCrosshairChartModifier ChartModifier)
        {
            SelectedElementID = selectedElemID;
            //from function: title, x axis label, y axis label
            PlotVM = new IndividualLinkedPlotVM(function, elementName, ChartModifier, _isXAxisLog, _isYAxisLog, _isProbabilityXAxis, _xAxisOnBottom, _yAxisOnLeft);
        }
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
