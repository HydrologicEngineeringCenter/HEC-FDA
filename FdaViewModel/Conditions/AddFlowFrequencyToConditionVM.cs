using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using FdaViewModel.Utilities;
using Functions;
using Model;

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
        private IFdaFunction _SelectedCurve;
        private ChildElement _SelectedElement;
        //private FdaModel.Functions.BaseFunction _BaseFunction;
        #endregion
        #region Properties
        public bool IsPoppedOut { get; set; } 

        public string SelectedElementName
        {
            get { if (SelectedElement != null) { return SelectedElement.Name; } else { return ""; } }
        }
        public ChildElement SelectedElement 
        {
            get { return _SelectedElement; }
            set { _SelectedElement = value; }
        }
        public IFdaFunction SelectedCurve
        {
            
            get 
            {
                //this is weird, but this curve has a constant Y value but the interpolation
                //methid is statistical. Having a stats interp throws exceptions later. I am going
                //to return a clone of this curve with a linear interp since this curve is only used
                //for plotting in the conditions editor.
                ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(SelectedElement.Curve.Coordinates, InterpolationEnum.Linear);
                IFunction function = null;
                if (func is IFunction)
                {
                    function = (IFunction)func;
                }
                else 
                { 
                    function = func.Sample(.5);
                }
                return IFdaFunctionFactory.Factory(IParameterEnum.InflowFrequency, function);
                //return SelectedElement.Curve; 
            } 
            
        }

        //public FdaModel.Functions.BaseFunction BaseFunction
        //{
        //    get { return new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(((FrequencyRelationships.AnalyticalFrequencyElement)SelectedElement).Distribution, FdaModel.Functions.FunctionTypes.InflowFrequency); }
        

        //}
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

            StudyCache.FlowFrequencyAdded += FlowFrequencyWasUpdated;
            StudyCache.FlowFrequencyUpdated += FlowFrequencyWasUpdated;
            StudyCache.FlowFrequencyRemoved += FlowFrequencyWasUpdated;

        }


        #endregion
        #region Voids
        public void LauchNewFlowFrequency(object sender, EventArgs e)
        {
            FrequencyRelationships.AnalyticalFrequencyOwnerElement flowFreqParent = StudyCache.GetParentElementOfType<FrequencyRelationships.AnalyticalFrequencyOwnerElement>();
            flowFreqParent.AddNewFlowFrequencyCurve(sender, e);
          
        }

        private void FlowFrequencyWasUpdated(object sender, EventArgs e)
        {
            List<FrequencyRelationships.AnalyticalFrequencyElement> tempList = StudyCache.GetChildElementsOfType<FrequencyRelationships.AnalyticalFrequencyElement>();
            InflowFrequencyCurves = tempList;//this is to hit the notify prop changed
            //at first i thought that if a user adds, updates, or removes and item, then maybe we would want to move the selected item, but i dont think
            //that is a good idea. First i don't know if they added and element from the conditios editor or the study. I don't want to be switching things around
            //without the user knowing it.
            //SelectedElement = ListOfStageDamageElements.LastOrDefault();//this works if an element was added or removed. Not so good if an element was updated.
        }

        //private void FlowFrequencyAdded(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    List<FrequencyRelationships.AnalyticalFrequencyElement> tempList = InflowFrequencyCurves;
        //    tempList.Add((FrequencyRelationships.AnalyticalFrequencyElement)e.Element);
        //    InflowFrequencyCurves = tempList;//this is to hit the notify prop changed
        //}

        //private void SetSelectedCurve()
        //{
        //    SelectedElement.
        //}

        public void OKClicked()
        {
            //SetSelectedCurve();
            Validate();
            if (!HasFatalError)
            {
                this.OKClickedEvent?.Invoke(this, new EventArgs());
            }
            else
            {
                CustomMessageBoxVM custmb = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "A frequency Curve has not been selected.");
                string header = "Error";
                DynamicTabVM tab = new DynamicTabVM(header, custmb, "MessageBoxError");
                Navigate(tab);
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
