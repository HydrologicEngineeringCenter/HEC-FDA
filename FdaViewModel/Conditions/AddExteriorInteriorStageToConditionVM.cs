using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using Statistics;
using FdaModel.Functions;



namespace FdaViewModel.Conditions
{
    //[Author(q0heccdm, 12 / 6 / 2017 2:34:26 PM)]
    public class AddExteriorInteriorStageToConditionVM : BaseViewModel,Plots.iConditionsImporter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/6/2017 2:34:26 PM
        #endregion
        #region Fields
        private ConditionsOwnerElement _owner;
        private StageTransforms.ExteriorInteriorElement _SelectedExteriorInteriorStageElement;
        private List<StageTransforms.ExteriorInteriorElement> _ListOfExteriorInteriorStageElements;

        public event EventHandler OKClickedEvent;
        public event EventHandler CancelClickedEvent;
        public event EventHandler PopImporterOut;

        #endregion
        #region Properties
        public StageTransforms.ExteriorInteriorElement SelectedExteriorInteriorStageElement
        {
            get { return _SelectedExteriorInteriorStageElement; }
            set { _SelectedExteriorInteriorStageElement = value; NotifyPropertyChanged(); }
        }
        public List<StageTransforms.ExteriorInteriorElement> ListOfExteriorInteriorStageElements
        {
            get { return _ListOfExteriorInteriorStageElements; }
            set { _ListOfExteriorInteriorStageElements = value; NotifyPropertyChanged(); }
        }

        public CurveIncreasing SelectedCurve
        {
            get
            {
                FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction extInt = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)SelectedExteriorInteriorStageElement.ExteriorInteriorCurve, FdaModel.Functions.FunctionTypes.ExteriorInteriorStage);
                List<double> ys = new List<double>();
                List<double> xs = new List<double>();
                foreach (double y in (extInt.GetOrdinatesFunction().Function.YValues))
                {
                    ys.Add(y);
                }
                foreach (double x in (extInt.GetOrdinatesFunction().Function.XValues))
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
                return new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(SelectedCurve, FunctionTypes.ExteriorInteriorStage);
            }
        }

        public string SelectedElementName
        {
            get
            {
                return SelectedExteriorInteriorStageElement.Name;
            }
        }

        public bool IsPoppedOut
        {
            get; set;
        }

        #endregion
        #region Constructors
        public AddExteriorInteriorStageToConditionVM(List<StageTransforms.ExteriorInteriorElement> listOfExIntStage, ConditionsOwnerElement owner)
        {
            ListOfExteriorInteriorStageElements = listOfExIntStage;
            _owner = owner;
        }
        #endregion
        #region Voids

        public void LaunchNewExteriorInteriorStage(object sender,EventArgs e)
        {
            if (_owner != null)
            {
                List<StageTransforms.ExteriorInteriorOwnerElement> eles = _owner.GetElementsOfType<StageTransforms.ExteriorInteriorOwnerElement>();
                if (eles.Count > 0)
                {
                    eles.FirstOrDefault().AddNewExteriorInteriorCurve(sender, e);
                    //need to determine what the most recent element is and see if we already have it.
                    if (eles.FirstOrDefault().Elements.Count > 0)
                    {
                        if (eles.FirstOrDefault().Elements.Count > ListOfExteriorInteriorStageElements.Count)
                        {

                            List<StageTransforms.ExteriorInteriorElement> theNewList = new List<StageTransforms.ExteriorInteriorElement>();
                            for (int i = 0; i < eles.FirstOrDefault().Elements.Count; i++)
                            {
                                theNewList.Add((StageTransforms.ExteriorInteriorElement)eles.FirstOrDefault().Elements[i]);
                            }
                            ListOfExteriorInteriorStageElements = theNewList;
                            //ExteriorInteriorList.Add((StageTransforms.ExteriorInteriorElement)eles.FirstOrDefault().Elements.Last());
                            SelectedExteriorInteriorStageElement = ListOfExteriorInteriorStageElements.Last();
                        }
                    }
                }
            }
        }
        public override void AddValidationRules()
        {
           // throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
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
