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
        private OwnerElement _owner;
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
        public Utilities.OwnedElement SelectedElement { get;
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
        public AddFlowFrequencyToConditionVM(List<FrequencyRelationships.AnalyticalFrequencyElement> lp3Curves, OwnerElement owner):this(lp3Curves,null,owner)
        {
            
        }
        public AddFlowFrequencyToConditionVM(List<FrequencyRelationships.AnalyticalFrequencyElement> lp3Curves, FrequencyRelationships.AnalyticalFrequencyElement selectedElement, OwnerElement owner) : base()
        {
            SelectedElement = selectedElement;
            InflowFrequencyCurves = lp3Curves;
            _owner = owner;
        }


        #endregion
        #region Voids
        public void LauchNewFlowFrequency(object sender, EventArgs e)
        {
            if (_owner != null)
            {
                List<FrequencyRelationships.AnalyticalFrequencyOwnerElement> eles = _owner.GetElementsOfType<FrequencyRelationships.AnalyticalFrequencyOwnerElement>();
                if (eles.Count > 0)
                {
                    eles.FirstOrDefault().AddNewFlowFrequencyCurve(sender, e);
                    //need to determine what the most recent element is and see if we already have it.
                    if (eles.FirstOrDefault().Elements.Count > 0)
                    {
                        if (eles.FirstOrDefault().Elements.Count > InflowFrequencyCurves.Count)
                        {
                            List<FrequencyRelationships.AnalyticalFrequencyElement> theNewList = new List<FrequencyRelationships.AnalyticalFrequencyElement>();
                            for (int i = 0; i < eles.FirstOrDefault().Elements.Count; i++)
                            {
                                theNewList.Add((FrequencyRelationships.AnalyticalFrequencyElement)eles.FirstOrDefault().Elements[i]);
                            }
                            InflowFrequencyCurves = theNewList;
                            //AnalyiticalRelationships.Add((FrequencyRelationships.AnalyticalFrequencyElement)eles.FirstOrDefault().Elements.Last());
                            SelectedElement = InflowFrequencyCurves.Last();
                        }
                    }
                }
            }
        }

        public void OKClicked()
        {
            //raise event that we are done
            if(this.OKClickedEvent != null)
            {
                this.OKClickedEvent(this,new EventArgs());
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
            if(PopImporterOut != null)
            {
                PopImporterOut(this, new EventArgs());
            }
        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
        #endregion
        #region Functions
        #endregion
    }
}
