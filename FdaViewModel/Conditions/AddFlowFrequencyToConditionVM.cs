using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using FdaViewModel.Utilities;

namespace FdaViewModel.Conditions
{
    //[Author(q0heccdm, 12 / 1 / 2017 11:42:47 AM)]
    public class AddFlowFrequencyToConditionVM:BaseViewModel,Plots.iConditionsImporter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/1/2017 11:42:47 AM
        #endregion
        #region Fields

        public event EventHandler OKClickedEvent;
        public event EventHandler CancelClickedEvent;
        public event EventHandler PopImporterOut;

        //private FrequencyRelationships.AnalyticalFrequencyElement _SelectedFlowFrequencyElement;
       // private ParentElement _owner;
        private List<FrequencyRelationships.AnalyticalFrequencyElement> _InflowFrequencyCurves;
        private Statistics.CurveIncreasing _SelectedCurve;
        private FdaModel.Functions.BaseFunction _BaseFunction;
        #endregion
        #region Properties
        public bool IsPoppedOut { get; set; } 

        public string SelectedElementName
        {
            get { if (SelectedElement != null) { return SelectedElement.Name; } else { return ""; } }
        }
        public Utilities.ChildElement SelectedElement { get;
            set; }
        public Statistics.CurveIncreasing SelectedCurve
        {
            get { return new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(((FrequencyRelationships.AnalyticalFrequencyElement)SelectedElement).Distribution, FdaModel.Functions.FunctionTypes.InflowFrequency).GetOrdinatesFunction().Function; }
            
        }

        public FdaModel.Functions.BaseFunction BaseFunction
        {
            get { return new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(((FrequencyRelationships.AnalyticalFrequencyElement)SelectedElement).Distribution, FdaModel.Functions.FunctionTypes.InflowFrequency); }
        

        }
        //public FrequencyRelationships.AnalyticalFrequencyElement SelectedFlowFrequencyElement
        //{
        //    get { return _SelectedFlowFrequencyElement; }
        //    set { _SelectedFlowFrequencyElement = value; SelectedElement = value; NotifyPropertyChanged(); }
        //}
            public List<FrequencyRelationships.AnalyticalFrequencyElement> InflowFrequencyCurves
        {
            get { return _InflowFrequencyCurves; }
            set { _InflowFrequencyCurves = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        //public AddFlowFrequencyToConditionVM():base()
        //{

        //}
        public AddFlowFrequencyToConditionVM(List<FrequencyRelationships.AnalyticalFrequencyElement> lp3Curves):this(lp3Curves,null)
        {

        }
        public AddFlowFrequencyToConditionVM(List<FrequencyRelationships.AnalyticalFrequencyElement> lp3Curves, FrequencyRelationships.AnalyticalFrequencyElement selectedElement) : base()
        {
            SelectedElement = selectedElement;
            InflowFrequencyCurves = lp3Curves;
            StudyCache.FlowFrequencyAdded += FlowFrequencyAdded;

        }


        #endregion
        #region Voids
        public void LauchNewFlowFrequency(object sender, EventArgs e)
        {
            FrequencyRelationships.AnalyticalFrequencyOwnerElement flowFreqParent = StudyCache.GetParentElementOfType<FrequencyRelationships.AnalyticalFrequencyOwnerElement>();
            flowFreqParent.AddNewFlowFrequencyCurve(sender, e);
          
        }

        private void FlowFrequencyAdded(object sender, Saving.ElementAddedEventArgs e)
        {
            List<FrequencyRelationships.AnalyticalFrequencyElement> tempList = InflowFrequencyCurves;
            tempList.Add((FrequencyRelationships.AnalyticalFrequencyElement)e.Element);
            InflowFrequencyCurves = tempList;//this is to hit the notify prop changed
        }

        public void OKClicked()
        {
            Validate();
            if (!HasFatalError)
            {
                this.OKClickedEvent?.Invoke(this, new EventArgs());
            }
            else
            {
                CustomMessageBoxVM custmb = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "A frequency Curve has not been selected.");
                Navigate(custmb);
            }
        }
        public void CancelClicked()
        {
            this.CancelClickedEvent?.Invoke(this, new EventArgs());
        }

        public void PopTheImporterOut()
        {
            PopImporterOut?.Invoke(this, new EventArgs());
        }
        public override void AddValidationRules()
        {
            AddRule(nameof(SelectedElement), () => { return (SelectedElement != null); }, "A frequency Curve has not been selected.");
        }

       
        #endregion
        #region Functions
        #endregion
    }
}
