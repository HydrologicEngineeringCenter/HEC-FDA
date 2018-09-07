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
        private ConditionsOwnerElement _owner;
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

        public OwnedElement SelectedElement
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
                UncertainCurveDataCollection curve = ((FlowTransforms.InflowOutflowElement)SelectedElement).InflowOutflowCurve;
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
            get;set;
        }

       
        #endregion
        #region Constructors
        public AddInflowOutflowToConditionVM(List<FlowTransforms.InflowOutflowElement> listOfinOut, ConditionsOwnerElement owner):this(listOfinOut,null,owner)
        {
        }

        public AddInflowOutflowToConditionVM(List<FlowTransforms.InflowOutflowElement> listOfinOut, FlowTransforms.InflowOutflowElement selectedElement, ConditionsOwnerElement owner):base()
        {
            SelectedElement = selectedElement;
            ListOfInflowOutflowElements = listOfinOut;
            _owner = owner;
        }


        #endregion
        #region Voids
        public void NewInflowOutflowCurve(object sender, EventArgs e)
        {
            if (_owner != null)
            {
                List<FlowTransforms.InflowOutflowOwnerElement> eles = _owner.GetElementsOfType<FlowTransforms.InflowOutflowOwnerElement>();
                if (eles.Count > 0)
                {
                    eles.FirstOrDefault().AddInflowOutflow(sender, e);
                    //need to determine what the most recent element is and see if we already have it.
                    if (eles.FirstOrDefault().Elements.Count > 0)
                    {
                        if (eles.FirstOrDefault().Elements.Count > ListOfInflowOutflowElements.Count)
                        {
                            //InflowOutflowList.Add((FlowTransforms.InflowOutflowElement)eles.FirstOrDefault().Elements.Last());
                            List<FlowTransforms.InflowOutflowElement> theNewList = new List<FlowTransforms.InflowOutflowElement>();
                            for (int i = 0; i < eles.FirstOrDefault().Elements.Count; i++)
                            {
                                theNewList.Add((FlowTransforms.InflowOutflowElement)eles.FirstOrDefault().Elements[i]);
                            }
                            ListOfInflowOutflowElements = theNewList;

                            SelectedElement = ListOfInflowOutflowElements.Last();    

                        }
                    }
                }
            }
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();

        }

        public override void Save()
        {
           // throw new NotImplementedException();
        }

        public void OKClicked()
        {
            //raise event that we are done
            if (this.OKClickedEvent != null)
            {
                this.OKClickedEvent(this, new EventArgs());
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
