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
    //[Author(q0heccdm, 12 / 1 / 2017 3:35:31 PM)]
    public class AddRatingCurveToConditionVM : BaseViewModel,Plots.iConditionsImporter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/1/2017 3:35:31 PM
        #endregion
        #region Fields
        private ConditionsOwnerElement _owner;

        //private StageTransforms.RatingCurveElement _SelectedRatingElement;
        private List<StageTransforms.RatingCurveElement> _ListOfRatingCurves;

        public event EventHandler OKClickedEvent;
        public event EventHandler CancelClickedEvent;
        public event EventHandler PopImporterOut;
        #endregion
        #region Properties
        public OwnedElement SelectedElement
        {
            get;set;
        }
        public bool IsPoppedOut { get; set; }
        public string SelectedElementName
        {
            get { return SelectedElement.Name; }
        }

        //public StageTransforms.RatingCurveElement SelectedRatingElement
        //{
        //    get { return _SelectedRatingElement; }
        //    set { _SelectedRatingElement = value; NotifyPropertyChanged(); }
        //}
        
        public List<StageTransforms.RatingCurveElement> ListOfRatingCurves
        {
            get { return _ListOfRatingCurves; }
            set { _ListOfRatingCurves = value; NotifyPropertyChanged(); }
        }

        public CurveIncreasing SelectedCurve
        {
            get
            {
                UncertainCurveDataCollection curve = ((StageTransforms.RatingCurveElement)SelectedElement).RatingCurve;
                FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction rating = 
                    new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((UncertainCurveIncreasing)curve, FunctionTypes.Rating);

                List<double> ys = new List<double>();
                List<double> xs = new List<double>();
                foreach (double y in (rating.GetOrdinatesFunction().Function.YValues))
                {
                    ys.Add(y);
                }
                foreach (double x in (rating.GetOrdinatesFunction().Function.XValues))
                {
                    xs.Add(x);
                }
                return new Statistics.CurveIncreasing(ys.ToArray(), xs.ToArray(), true, false);
            }
        }

        public BaseFunction BaseFunction
        {
            get
            {
                return new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(SelectedCurve, FunctionTypes.Rating);
            }
        }

        
        #endregion
        #region Constructors
        public AddRatingCurveToConditionVM(List<StageTransforms.RatingCurveElement> listOfRatingCurves, ConditionsOwnerElement owner):this(listOfRatingCurves,null,owner)
        {
            
        }

        public AddRatingCurveToConditionVM(List<StageTransforms.RatingCurveElement> listOfRatingCurves, StageTransforms.RatingCurveElement selectedRatingElement, ConditionsOwnerElement owner):base()
        {
            SelectedElement = selectedRatingElement;
            _owner = owner;
            ListOfRatingCurves = listOfRatingCurves;
        }
        #endregion
        #region Voids
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
        public void LaunchNewRatingCurve(object sender, EventArgs e)
        {
            if (_owner != null)
            {
                List<StageTransforms.RatingCurveOwnerElement> eles = _owner.GetElementsOfType<StageTransforms.RatingCurveOwnerElement>();
                if (eles.Count > 0)
                {
                    eles.FirstOrDefault().AddNewRatingCurve(sender, e);
                    //need to determine what the most recent element is and see if we already have it.
                    if (eles.FirstOrDefault().Elements.Count > 0)
                    {
                        if (eles.FirstOrDefault().Elements.Count > ListOfRatingCurves.Count)
                        {
                            List<StageTransforms.RatingCurveElement> theNewList = new List<StageTransforms.RatingCurveElement>();
                            for (int i = 0; i < eles.FirstOrDefault().Elements.Count; i++)
                            {
                                theNewList.Add((StageTransforms.RatingCurveElement)eles.FirstOrDefault().Elements[i]);
                            }
                            ListOfRatingCurves = theNewList;
                            //RatingCurveRelationships.Add((StageTransforms.RatingCurveElement)eles.FirstOrDefault().Elements.Last());
                            SelectedElement = ListOfRatingCurves.Last();
                        }
                    }
                }
            }

        }
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
    }
}
