using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.ImpactAreaScenario;
using Model;

namespace ViewModel.Plots
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
        private Model.IMetric _Metric;

        #endregion
        #region Properties
        //Maybe not ideal, but i need these on IConditionsPlotWrapper so 
        //that i can set them on plot 8. But that means they get put on
        //all these plot classes even though they will never get set.
        public string EAD { get; set; }

        public string AEP { get; set; }
        public bool DisplayImportButton
        {
            get; set;
        }
        public Model.IMetric Metric
        {
            get { return _Metric; }
            set { _Metric = value; NotifyPropertyChanged(); }
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

        public string Title => throw new NotImplementedException();

        public string XAxisLabel => throw new NotImplementedException();

        public string YAxisLabel => throw new NotImplementedException();

        public int SelectedElementID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
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

        public void AddCurveToPlot( IFdaFunction function, string elementName,int elementID, FdaCrosshairChartModifier chartModifier)
        {
            throw new NotImplementedException();
        }


        #endregion
        #region Functions
        #endregion
    }
}
