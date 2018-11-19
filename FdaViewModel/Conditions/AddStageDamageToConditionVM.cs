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
using FdaViewModel.AggregatedStageDamage;

namespace FdaViewModel.Conditions
{
    //[Author(q0heccdm, 12 / 6 / 2017 2:34:58 PM)]
    public class AddStageDamageToConditionVM:BaseViewModel,Plots.iConditionsImporter
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 12/6/2017 2:34:58 PM
        #endregion
        #region Fields
        public event EventHandler OKClickedEvent;
        public event EventHandler CancelClickedEvent;
        public event EventHandler PopImporterOut;


        //private AggregatedStageDamage.AggregatedStageDamageElement _StageDamageElement;
        private List<AggregatedStageDamage.AggregatedStageDamageElement> _ListOfStageDamageElements;

        #endregion
        #region Properties
        public bool IsPoppedOut { get; set; }
        //public AggregatedStageDamage.AggregatedStageDamageElement StageDamageElement
        //{
        //    get { return _StageDamageElement; }
        //    set { _StageDamageElement = value; NotifyPropertyChanged(); }
        //}
        public ChildElement SelectedElement
        {
            get;set;
        }
        public List<AggregatedStageDamage.AggregatedStageDamageElement> ListOfStageDamageElements
        {
            get { return _ListOfStageDamageElements; }
            set { _ListOfStageDamageElements = value; NotifyPropertyChanged(); }
        }

        public CurveIncreasing SelectedCurve
        {
            get
            {
                UncertainCurveDataCollection curve = ((AggregatedStageDamage.AggregatedStageDamageElement)SelectedElement).Curve;
                 FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction stageDamage = 
                    new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((UncertainCurveIncreasing)curve, FunctionTypes.InteriorStageDamage);
                return stageDamage.GetOrdinatesFunction().Function;
            }
        }

        public BaseFunction BaseFunction
        {
            get
            {
                return new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(SelectedCurve, FunctionTypes.InteriorStageDamage);

            }
        }

        public string SelectedElementName
        {
            get
            {
                return SelectedElement.Name;
            }
        }

       

        #endregion
        #region Constructors
        public AddStageDamageToConditionVM(List<AggregatedStageDamage.AggregatedStageDamageElement> listOfStageDamageElements ):this(listOfStageDamageElements,null)
        {

        }
        public AddStageDamageToConditionVM(List<AggregatedStageDamage.AggregatedStageDamageElement> listOfStageDamageElements, AggregatedStageDamage.AggregatedStageDamageElement selectedElement ):base()
        {
            SelectedElement = selectedElement;
            ListOfStageDamageElements = listOfStageDamageElements;
            StudyCache.StageDamageAdded += StageDamageAdded;

        }
        #endregion
        #region Voids
        public void LaunchNewStageDamage(object sender, EventArgs e)
        {
            AggregatedStageDamageOwnerElement stageDamageParent = StudyCache.GetParentElementOfType<AggregatedStageDamageOwnerElement>();
            stageDamageParent.AddNewDamageCurve(sender, e);
           
        }

      
        private void StageDamageAdded(object sender, Saving.ElementAddedEventArgs e)
        {
            List<AggregatedStageDamageElement> tempList = ListOfStageDamageElements;
            tempList.Add((AggregatedStageDamageElement)e.Element);
            ListOfStageDamageElements = tempList;//this is to hit the notify prop changed
        }
        public void LaunchNewWaterSurfaceElevation()
        {

        }

        public void LaunchNewStructureInventory()
        {


        }

        public void LaunchNewTerrainFile()
        {


        }

        public void LaunchNewOccupancyType()
        {

        }
        public override void AddValidationRules()
        {
            AddRule(nameof(SelectedElement), () => { return (SelectedElement != null); }, "A Stage Damage Curve has not been selected.");
        }

       
        public void PopTheImporterOut()
        {
            if (PopImporterOut != null)
            {
                PopImporterOut(this, new EventArgs());
            }
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
                CustomMessageBoxVM custmb = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK, "A Stage Damage Curve has not been selected.");
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
        #endregion
        #region Functions
        #endregion
    }
}
