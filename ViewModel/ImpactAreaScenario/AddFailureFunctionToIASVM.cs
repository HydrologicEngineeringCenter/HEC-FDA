using ViewModel.GeoTech;
using ViewModel.Utilities;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario
{
    public class AddFailureFunctionToIASVM : BaseViewModel, Plots.iIASImporter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 8/2/2020 2:34:26 PM
        #endregion
        #region Fields
        private List<LeveeFeatureElement> _ListOfLeveeFeatureElements;

        public event EventHandler OKClickedEvent;
        public event EventHandler CancelClickedEvent;
        public event EventHandler PopImporterOut;

        #endregion
        #region Properties

        public ChildElement SelectedElement
        {
            get; set;
        }


        public List<LeveeFeatureElement> ListOfLeveeFeatureElements
        {
            get { return _ListOfLeveeFeatureElements; }
            set { _ListOfLeveeFeatureElements = value; NotifyPropertyChanged(); }
        }

        public IFdaFunction SelectedCurve
        {
            get
            {
                return ((LeveeFeatureElement)SelectedElement).Curve;
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
        public AddFailureFunctionToIASVM(List<LeveeFeatureElement> listOfLeveeFeatures) : this(listOfLeveeFeatures, null)
        {

        }
        public AddFailureFunctionToIASVM(List<LeveeFeatureElement> listOfLeveeFeatures, LeveeFeatureElement selectedElement) : base()
        {
            SelectedElement = selectedElement;
            ListOfLeveeFeatureElements = listOfLeveeFeatures;

            StudyCache.FailureFunctionAdded += LeveeFeatureWasUpdated;
            StudyCache.FailureFunctionUpdated += LeveeFeatureWasUpdated;
            StudyCache.FailureFunctionRemoved += LeveeFeatureWasUpdated;

        }
        #endregion
        #region Voids

        public void LaunchNewFailureFunction(object sender, EventArgs e)
        {
            LeveeFeatureOwnerElement parent = StudyCache.GetParentElementOfType<LeveeFeatureOwnerElement>();
            parent.AddNewLeveeFeature(sender, e);
            //FailureFunctionOwnerElement failureParent = StudyCache.GetParentElementOfType<FailureFunctionOwnerElement>();
            //failureParent.AddNewFailureFunction(sender, e);
        }

        private void LeveeFeatureWasUpdated(object sender, EventArgs e)
        {
            List<LeveeFeatureElement> tempList = StudyCache.GetChildElementsOfType<LeveeFeatureElement>();
            ListOfLeveeFeatureElements = tempList;

        }

        public override void AddValidationRules()
        {
            AddRule(nameof(SelectedElement), () => { return (SelectedElement != null); }, "A levee feature curve has not been selected.");
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
                CustomMessageBoxVM custmb = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "A levee feature has not been selected.");
                String header = "Error";
                DynamicTabVM tab = new DynamicTabVM(header, custmb, "LeveeFeatureError1");
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
