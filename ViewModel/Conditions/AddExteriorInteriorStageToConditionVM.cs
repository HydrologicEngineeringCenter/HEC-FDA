using System;
using System.Collections.Generic;
using ViewModel.Utilities;
using ViewModel.StageTransforms;
using Model;

namespace ViewModel.Conditions
{
    //[Author(q0heccdm, 12 / 6 / 2017 2:34:26 PM)]
    public class AddExteriorInteriorStageToConditionVM : BaseViewModel,Plots.iConditionsImporter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/6/2017 2:34:26 PM
        #endregion
        #region Fields
        //private StageTransforms.ExteriorInteriorElement _SelectedExteriorInteriorStageElement;
        private List<StageTransforms.ExteriorInteriorElement> _ListOfExteriorInteriorStageElements;

        public event EventHandler OKClickedEvent;
        public event EventHandler CancelClickedEvent;
        public event EventHandler PopImporterOut;

        #endregion
        #region Properties
        //public StageTransforms.ExteriorInteriorElement SelectedExteriorInteriorStageElement
        //{
        //    get { return _SelectedExteriorInteriorStageElement; }
        //    set { _SelectedExteriorInteriorStageElement = value; NotifyPropertyChanged(); }
        //}
        public ChildElement SelectedElement
        {
            get;set;
        }


        public List<StageTransforms.ExteriorInteriorElement> ListOfExteriorInteriorStageElements
        {
            get { return _ListOfExteriorInteriorStageElements; }
            set { _ListOfExteriorInteriorStageElements = value; NotifyPropertyChanged(); }
        }

        public IFdaFunction SelectedCurve
        {
            get
            {
                //todo: Refactor: What is this thing doing? Can't i just return the original curve. Looks like was making a new one
                //and returning the copy? I think we are getting rid of all this oxy plot stuff anyway.
                return  ((ExteriorInteriorElement)SelectedElement).Curve;

                //IFdaFunction extInt = 
                //    new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((UncertainCurveIncreasing)curve,FunctionTypes.ExteriorInteriorStage);
                //List<double> ys = new List<double>();
                //List<double> xs = new List<double>();
                //foreach (double y in (extInt.GetOrdinatesFunction().Function.YValues))
                //{
                //    ys.Add(y);
                //}
                //foreach (double x in (extInt.GetOrdinatesFunction().Function.XValues))
                //{
                //    xs.Add(x);
                //}
                //return new CurveIncreasing(xs.ToArray(), ys.ToArray(), true, false);
            }
        }

        //todo: Refactor: commenting this out
        //public BaseFunction BaseFunction
        //{
        //    get
        //    {
        //        return new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(SelectedCurve, FunctionTypes.ExteriorInteriorStage);
        //    }
        //}

        public string SelectedElementName
        {
            get
            {
                return SelectedElement.Name;
            }
        }

        public bool IsPoppedOut
        {
            get; set;
        }



        #endregion
        #region Constructors
        public AddExteriorInteriorStageToConditionVM(List<StageTransforms.ExteriorInteriorElement> listOfExIntStage ):this(listOfExIntStage,null)
        {

        }
        public AddExteriorInteriorStageToConditionVM(List<StageTransforms.ExteriorInteriorElement> listOfExIntStage, StageTransforms.ExteriorInteriorElement selectedElement ):base()
        {
            SelectedElement = selectedElement;
            ListOfExteriorInteriorStageElements = listOfExIntStage;

            StudyCache.ExteriorInteriorAdded += ExteriorInteriorWasUpdated;
            StudyCache.ExteriorInteriorUpdated += ExteriorInteriorWasUpdated;
            StudyCache.ExteriorInteriorRemoved += ExteriorInteriorWasUpdated;

        }
        #endregion
        #region Voids

        public void LaunchNewExteriorInteriorStage(object sender,EventArgs e)
        {
            ExteriorInteriorOwnerElement extIntParent = StudyCache.GetParentElementOfType<ExteriorInteriorOwnerElement>();
            extIntParent.AddNewExteriorInteriorCurve(sender, e);
            //at first i thought that if a user adds, updates, or removes and item, then maybe we would want to move the selected item, but i dont think
            //that is a good idea. First i don't know if they added and element from the conditios editor or the study. I don't want to be switching things around
            //without the user knowing it.
            //SelectedElement = ListOfStageDamageElements.LastOrDefault();//this works if an element was added or removed. Not so good if an element was updated.
        }
        //private void ExteriorInteriorAdded(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    List<ExteriorInteriorElement> tempList = ListOfExteriorInteriorStageElements;
        //    tempList.Add((ExteriorInteriorElement)e.Element);
        //    ListOfExteriorInteriorStageElements = tempList;//this is to hit the notify prop changed
        //}

        private void ExteriorInteriorWasUpdated(object sender, EventArgs e)
        {
            List<ExteriorInteriorElement> tempList = StudyCache.GetChildElementsOfType<ExteriorInteriorElement>();
            ListOfExteriorInteriorStageElements = tempList;

        }

        public override void AddValidationRules()
        {
            AddRule(nameof(SelectedElement), () => { return (SelectedElement != null); }, "An Exterior Interior Curve has not been selected.");
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
                CustomMessageBoxVM custmb = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "An Exterior Interior Curve has not been selected.");
                String header = "Error";
                DynamicTabVM tab = new DynamicTabVM(header, custmb, "ExtIntError1");
                Navigate(tab);
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
