using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using Statistics;
using FdaModel.Functions;
using FdaViewModel.Utilities;
using FdaViewModel.StageTransforms;

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

        public CurveIncreasing SelectedCurve
        {
            get
            {
                UncertainCurveDataCollection curve = ((StageTransforms.ExteriorInteriorElement)SelectedElement).Curve;

                FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction extInt = 
                    new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((UncertainCurveIncreasing)curve,FunctionTypes.ExteriorInteriorStage);
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
                return new CurveIncreasing(xs.ToArray(), ys.ToArray(), true, false);
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
            StudyCache.ExteriorInteriorAdded += ExteriorInteriorAdded;

        }
        #endregion
        #region Voids

        public void LaunchNewExteriorInteriorStage(object sender,EventArgs e)
        {
            ExteriorInteriorOwnerElement extIntParent = StudyCache.GetParentElementOfType<ExteriorInteriorOwnerElement>();
            extIntParent.AddNewExteriorInteriorCurve(sender, e);
            
        }
        private void ExteriorInteriorAdded(object sender, Saving.ElementAddedEventArgs e)
        {
            List<ExteriorInteriorElement> tempList = ListOfExteriorInteriorStageElements;
            tempList.Add((ExteriorInteriorElement)e.Element);
            ListOfExteriorInteriorStageElements = tempList;//this is to hit the notify prop changed
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
