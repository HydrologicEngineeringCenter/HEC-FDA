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

        private ConditionsOwnerElement _owner;

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
        public OwnedElement SelectedElement
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
        public AddStageDamageToConditionVM(List<AggregatedStageDamage.AggregatedStageDamageElement> listOfStageDamageElements, ConditionsOwnerElement owner):this(listOfStageDamageElements,null,owner)
        {
        }
        public AddStageDamageToConditionVM(List<AggregatedStageDamage.AggregatedStageDamageElement> listOfStageDamageElements, AggregatedStageDamage.AggregatedStageDamageElement selectedElement, ConditionsOwnerElement owner):base()
        {
            SelectedElement = selectedElement;
            ListOfStageDamageElements = listOfStageDamageElements;
            _owner = owner;

        }
        #endregion
        #region Voids
        public void LaunchNewStageDamage(object sender, EventArgs e)
        {
            if (_owner != null)
            {
                List<AggregatedStageDamage.AggregatedStageDamageOwnerElement> eles = _owner.GetElementsOfType<AggregatedStageDamage.AggregatedStageDamageOwnerElement>();
                if (eles.Count > 0)
                {
                    eles.FirstOrDefault().AddNewDamageCurve(sender, e);
                    //need to determine what the most recent element is and see if we already have it.
                    if (eles.FirstOrDefault().Elements.Count > 0)
                    {
                        if (eles.FirstOrDefault().Elements.Count > ListOfStageDamageElements.Count)
                        {
                            List<AggregatedStageDamage.AggregatedStageDamageElement> theNewList = new List<AggregatedStageDamage.AggregatedStageDamageElement>();
                            for (int i = 0; i < eles.FirstOrDefault().Elements.Count; i++)
                            {
                                theNewList.Add((AggregatedStageDamage.AggregatedStageDamageElement)eles.FirstOrDefault().Elements[i]);
                            }
                            ListOfStageDamageElements = theNewList;
                            //StageDamageRelationships.Add((AggregatedStageDamage.AggregatedStageDamageElement)eles.FirstOrDefault().Elements.Last());
                            SelectedElement = ListOfStageDamageElements.Last();
                        }
                    }
                }
            }
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
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
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
        #endregion
        #region Functions
        #endregion
    }
}
