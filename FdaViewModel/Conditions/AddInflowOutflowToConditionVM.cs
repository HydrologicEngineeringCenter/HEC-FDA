using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using FdaModel.Functions;
using Statistics;
using FdaViewModel.Utilities;
using FdaViewModel.FlowTransforms;

namespace FdaViewModel.Conditions
{
    //[Author(q0heccdm, 12 / 4 / 2017 9:51:41 AM)]
    public class AddInflowOutflowToConditionVM:BaseViewModel,Plots.iConditionsImporter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/4/2017 9:51:41 AM
        #endregion
        #region Fields

        //private FlowTransforms.InflowOutflowElement _SelectedInflowOutflowElement;
        private List<FlowTransforms.InflowOutflowElement> _ListOfInflowOutflowElements;

        public event EventHandler OKClickedEvent;
        public event EventHandler CancelClickedEvent;
        public event EventHandler PopImporterOut;
        #endregion
        #region Properties

        //public FlowTransforms.InflowOutflowElement SelectedInflowOutflowElement
        //{
        //    get { return _SelectedInflowOutflowElement; }
        //    set { _SelectedInflowOutflowElement = value; NotifyPropertyChanged(); }
        //}

        public ChildElement SelectedElement
        {
            get; set;
        }

        public List<FlowTransforms.InflowOutflowElement> ListOfInflowOutflowElements
        {
            get { return _ListOfInflowOutflowElements; }
            set { _ListOfInflowOutflowElements = value; NotifyPropertyChanged(); }
        }

        public CurveIncreasing SelectedCurve
        {
            get
            {
                UncertainCurveDataCollection curve = ((FlowTransforms.InflowOutflowElement)SelectedElement).Curve;
                FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction infOut = 
                    new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((UncertainCurveIncreasing)curve, FunctionTypes.InflowOutflow);
                List<double> ys = new List<double>();
                List<double> xs = new List<double>();
                foreach (double y in (infOut.GetOrdinatesFunction().Function.YValues))
                {
                    ys.Add(y);
                }
                foreach (double x in (infOut.GetOrdinatesFunction().Function.XValues))
                {
                    xs.Add(x);
                }
                return new Statistics.CurveIncreasing(xs.ToArray(), ys.ToArray(), true, false);
            }
        }

        public BaseFunction BaseFunction
        {
            get
            {
                return new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(SelectedCurve, FunctionTypes.InflowOutflow);
            }
        }

        public string SelectedElementName
        {
            get
            {
                return SelectedElement.Name;
            }
        }

        public bool IsPoppedOut
        {
            get;
            set;
        }

       
        #endregion
        #region Constructors
        public AddInflowOutflowToConditionVM(List<FlowTransforms.InflowOutflowElement> listOfinOut ):this(listOfinOut,null)
        {

        }

        public AddInflowOutflowToConditionVM(List<FlowTransforms.InflowOutflowElement> listOfinOut, FlowTransforms.InflowOutflowElement selectedElement ):base()
        {
            SelectedElement = selectedElement;
            ListOfInflowOutflowElements = listOfinOut;

            StudyCache.InflowOutflowAdded += InflowOutflowWasUpdated;
            StudyCache.InflowOutflowUpdated += InflowOutflowWasUpdated;
            StudyCache.InflowOutflowRemoved += InflowOutflowWasUpdated;

        }


        #endregion
        #region Voids
        public void NewInflowOutflowCurve(object sender, EventArgs e)
        {
            InflowOutflowOwnerElement infOutParent = StudyCache.GetParentElementOfType<InflowOutflowOwnerElement>();
            infOutParent.AddInflowOutflow(sender, e);
           
        }

        private void InflowOutflowWasUpdated(object sender, EventArgs e)
        {
            List<InflowOutflowElement> tempList = StudyCache.GetChildElementsOfType<InflowOutflowElement>();
            ListOfInflowOutflowElements = tempList;//this is to hit the notify prop changed
            //at first i thought that if a user adds, updates, or removes and item, then maybe we would want to move the selected item, but i dont think
            //that is a good idea. First i don't know if they added and element from the conditios editor or the study. I don't want to be switching things around
            //without the user knowing it.
            //SelectedElement = ListOfStageDamageElements.LastOrDefault();//this works if an element was added or removed. Not so good if an element was updated.
        }
        //private void InflowOutflowAdded(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    List<InflowOutflowElement> tempList = ListOfInflowOutflowElements;
        //    tempList.Add((InflowOutflowElement)e.Element);
        //    ListOfInflowOutflowElements = tempList;//this is to hit the notify prop changed
        //}

        public override void AddValidationRules()
        {
            AddRule(nameof(SelectedElement), () => { return (SelectedElement != null); }, "An Inflow Outflow Curve has not been selected.");
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
                CustomMessageBoxVM custmb = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "An Inflow Outflow Curve has not been selected.");
                Navigate(custmb);
            }
        }

        public void CancelClicked()
        {
            if (this.CancelClickedEvent != null)
            {
                this.CancelClickedEvent(this, new EventArgs());
            }
        }

        public void PopTheImporterOut()
        {
            if (PopImporterOut != null)
            {
                PopImporterOut(this, new EventArgs());
            }
        }
        #endregion
        #region Functions
        #endregion
    }
}
