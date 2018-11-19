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
using System.Windows;
using FdaViewModel.StageTransforms;

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

        //private StageTransforms.RatingCurveElement _SelectedRatingElement;
        private List<StageTransforms.RatingCurveElement> _ListOfRatingCurves;

        public event EventHandler OKClickedEvent;
        public event EventHandler CancelClickedEvent;
        public event EventHandler PopImporterOut;
        #endregion
        #region Properties
        public ChildElement SelectedElement
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
                UncertainCurveDataCollection curve = ((StageTransforms.RatingCurveElement)SelectedElement).Curve;
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
        public AddRatingCurveToConditionVM(List<StageTransforms.RatingCurveElement> listOfRatingCurves ):this(listOfRatingCurves,null)
        {

        }

        public AddRatingCurveToConditionVM(List<StageTransforms.RatingCurveElement> listOfRatingCurves, StageTransforms.RatingCurveElement selectedRatingElement ):base()
        {
            SelectedElement = selectedRatingElement;
            ListOfRatingCurves = listOfRatingCurves;
            StudyCache.RatingAdded += RatingAdded;
            StudyCache.RatingUpdated += RatingWasUpdated;
        }
        #endregion
        #region Voids
        public void OKClicked()
        {
            Validate();
            if (!HasFatalError)
            {
                this.OKClickedEvent?.Invoke(this, new EventArgs());
            }
            else
            {
                CustomMessageBoxVM custmb = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "A Rating Curve has not been selected.");
                Navigate(custmb, true, true, "No Rating Curve");
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
            RatingCurveOwnerElement ratingParent = StudyCache.GetParentElementOfType<RatingCurveOwnerElement>();
            ratingParent.AddNewRatingCurve(sender, e);
        }

        public void EditRatingCurve(object sender, EventArgs e)
        {
            RatingCurveOwnerElement ratingParent = StudyCache.GetParentElementOfType<RatingCurveOwnerElement>();
            foreach(RatingCurveElement elem in ratingParent.Elements)
            {
                if (elem.Name.Equals(SelectedElement.Name))
                {
                    elem.EditRatingCurve(sender, e);
                }
            }
            //List<RatingCurveElement> ratings = StudyCache.GetChildElementsOfType<RatingCurveElement>();
            // foreach(RatingCurveElement elem in ratings)
            // {
            //     if(elem.Name.Equals(SelectedElement.Name))
            //     {
            //         elem.EditRatingCurve(sender, e);
            //     }
            // }
            //((RatingCurveElement)SelectedElement).EditRatingCurve(sender, e);
            //ratingParent.AddNewRatingCurve(sender, e);
        }
        private void RatingWasUpdated(object sender, Saving.ElementUpdatedEventArgs args)
        {
            ////i need to swap out the old element for the new one
            //AggregatedStageDamageElement oldElem = (AggregatedStageDamageElement)args.OldElement;
            //AggregatedStageDamageElement newElem = (AggregatedStageDamageElement)args.NewElement;
            //int i;
            //for (i=0;i<ListOfStageDamageElements.Count;i++)
            //{
            //    if(ListOfStageDamageElements[i].Name.Equals(oldElem.Name))
            //    {
            //        break;                   
            //    }
            //}
            //ListOfStageDamageElements.RemoveAt(i);

            List<RatingCurveElement> tempList = StudyCache.GetChildElementsOfType<RatingCurveElement>();
            ListOfRatingCurves = tempList;//this is to hit the notify prop changed
        }
        private void RatingAdded(object sender, Saving.ElementAddedEventArgs e)
        {
            List<RatingCurveElement> tempList = ListOfRatingCurves;
            tempList.Add((RatingCurveElement)e.Element);
            ListOfRatingCurves = tempList;//this is to hit the notify prop changed
        }
        #endregion
        #region Functions
        #endregion

        public override void AddValidationRules()
        {
            AddRule(nameof(SelectedElement), () => { return (SelectedElement != null); }, "A Rating Curve has not been selected.");
        }

       
    }
}
