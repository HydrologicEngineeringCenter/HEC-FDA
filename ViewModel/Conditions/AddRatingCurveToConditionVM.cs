using System;
using System.Collections.Generic;
using Statistics;
using ViewModel.Utilities;
using System.Windows;
using ViewModel.StageTransforms;
using Model;
using Functions;

namespace ViewModel.Conditions
{
    //[Author(q0heccdm, 12 / 1 / 2017 3:35:31 PM)]
    public class AddRatingCurveToConditionVM : Editors.BaseEditorVM,Plots.iConditionsImporter
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
        public bool IsPoppedOut {
            get;
            set;
        }
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
            get { return GetCurrentListOfRatingCurves(); }
            set { _ListOfRatingCurves = value; NotifyPropertyChanged(); }
        }

        private List<RatingCurveElement> GetCurrentListOfRatingCurves()
        {
            return StudyCache.GetChildElementsOfType<RatingCurveElement>();

        }

        public void UpdateListOfRatingCurves()
        {
            ListOfRatingCurves = GetCurrentListOfRatingCurves();
        }

        public IFdaFunction SelectedCurve
        {
            get
            {
                //this is a rating curve and we need to switch the x and y values.

                //return ((StageTransforms.RatingCurveElement)SelectedElement).Curve;

                List<double> ys = new List<double>();
                List<double> xs = new List<double>();
                foreach(ICoordinate coord in SelectedElement.Curve.Coordinates)
                {
                    xs.Add( coord.Y.Value());
                    ys.Add(coord.X.Value());
                }
                ICoordinatesFunction coordFunc = ICoordinatesFunctionsFactory.Factory(xs, ys, SelectedElement.Curve.Interpolator);
                return IFdaFunctionFactory.Factory(IParameterEnum.Rating, (IFunction)coordFunc);
            }
        }

        //public BaseFunction BaseFunction
        //{
        //    get
        //    {
        //        return new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(SelectedCurve, FunctionTypes.Rating);
        //    }
        //}

        
        #endregion
        #region Constructors
        public AddRatingCurveToConditionVM(List<StageTransforms.RatingCurveElement> listOfRatingCurves ):base(null)
        {
            ListOfRatingCurves = listOfRatingCurves;

            StudyCache.RatingAdded += RatingWasUpdated;
            StudyCache.RatingUpdated += RatingWasUpdated;
            StudyCache.RatingRemoved += RatingWasUpdated;
        }

        public AddRatingCurveToConditionVM(List<StageTransforms.RatingCurveElement> listOfRatingCurves, StageTransforms.RatingCurveElement selectedRatingElement ):base(selectedRatingElement,null)
        {
            SelectedElement = selectedRatingElement;
            ListOfRatingCurves = listOfRatingCurves;

            StudyCache.RatingAdded += RatingWasUpdated;
            StudyCache.RatingUpdated += RatingWasUpdated;
            StudyCache.RatingRemoved += RatingWasUpdated;
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
                string header = "No Rating Curve";
                DynamicTabVM tab = new DynamicTabVM(header, custmb, "ErrorMessage");
                Navigate(tab, true, true);
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
        private void RatingWasUpdated(object sender, EventArgs args)
        {          
            List<RatingCurveElement> tempList = StudyCache.GetChildElementsOfType<RatingCurveElement>();
            ListOfRatingCurves = tempList;//this is to hit the notify prop changed
        }
        //private void RatingAdded(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    List<RatingCurveElement> tempList = ListOfRatingCurves;
        //    tempList.Add((RatingCurveElement)e.Element);
        //    ListOfRatingCurves = tempList;//this is to hit the notify prop changed
        //}
        #endregion
        #region Functions
        #endregion

        public override void AddValidationRules()
        {
            AddRule(nameof(SelectedElement), () => { return (SelectedElement != null); }, "A Rating Curve has not been selected.");
        }

        public override void Save()
        {
            OKClicked();
        }
    }
}
