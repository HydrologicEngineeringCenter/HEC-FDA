using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Conditions
{
    public class ConditionsEditorVM : BaseViewModel
    {
        #region Notes
        #endregion
        #region Fields
        private string _Name;
        private string _Description;
        private int _AnalysisYear;
        private List<ImpactArea.ImpactAreaElement> _ImpactAreaSets;
        private ImpactArea.ImpactAreaElement _ImpactAreaSet;
        private ImpactArea.ImpactAreaRowItem _ImpactArea;

        private List<FrequencyRelationships.AnalyticalFrequencyElement> _AnalyiticalRelationships;
        private bool _UsesAnalyticalFlowFrequency;
        private FrequencyRelationships.AnalyticalFrequencyElement _AnalyticalFlowFrequency;

        private List<StageTransforms.RatingCurveElement> _RatingCurveRelationships;
        private bool _UseRatingCurve;
        private StageTransforms.RatingCurveElement _RatingCurve;

        private List<StageTransforms.ExteriorInteriorElement> _ExteriorInteriorList;
        private bool _UsesExteriorInterior;
        private StageTransforms.ExteriorInteriorElement _ExteriorInteriorElement;

        private List<AggregatedStageDamage.AggregatedStageDamageElement> _DamageRelationships;
        private bool _UseAggregatedStageDamage;
        private AggregatedStageDamage.AggregatedStageDamageElement _StageDamage;

        private bool _UseThreshold;
        private List<string> _ThresholdTypes = new List<string>() { "Dollars", "Stage" };//dollars or stage. need enum. should be based on users selection of inclusion of stage element or damage elements.
        private double _ThresholdValue;

        private List<FlowTransforms.InflowOutflowElement> _InflowOutflowList;
        private bool _UsesInflowOutflow;
        private FlowTransforms.InflowOutflowElement _InflowOutflowElement;

        private List<GeoTech.LeveeFeatureElement> _LeveeList;
        private bool _UsesLevee;
        private GeoTech.LeveeFeatureElement _LeveeElement;

        private List<GeoTech.FailureFunctionElement> _FailureFunctionList;
        private bool _UsesFailureFunction;
        private GeoTech.FailureFunctionElement _FailureFunctionElement;

        private List<Inventory.InventoryElement> _StructureInventoryList;
        private bool _UsesStructureInventory;
        private Inventory.InventoryElement _StructureInventoryElement;

        #endregion
        #region Properties
        public List<Inventory.InventoryElement> StructureInventoryList
        {
            get { return _StructureInventoryList; }
            set { _StructureInventoryList = value; NotifyPropertyChanged(); }
        }
        public bool UsesStructureInventory
        {
            get { return _UsesStructureInventory; }
            set { _UsesStructureInventory = value; NotifyPropertyChanged(); }
        }
        public Inventory.InventoryElement StructureInventoryElement
        {
            get { return _StructureInventoryElement; }
            set { _StructureInventoryElement = value; NotifyPropertyChanged(); }
        }

        public List<GeoTech.FailureFunctionElement> FailureFunctionList
        {
            get { return _FailureFunctionList; }
            set { _FailureFunctionList = value; NotifyPropertyChanged(); }
        }
        public bool UsesFailureFunction
        {
            get { return _UsesFailureFunction; }
            set { _UsesFailureFunction = value; NotifyPropertyChanged(); }
        }
        public GeoTech.FailureFunctionElement FailureFunctionElement
        {
            get { return _FailureFunctionElement; }
            set { _FailureFunctionElement = value; NotifyPropertyChanged(); }
        }
        public List<GeoTech.LeveeFeatureElement> LeveeList
        {
            get { return _LeveeList; }
            set { _LeveeList = value; NotifyPropertyChanged(); }
        }
        public bool UsesLevee
        {
            get { return _UsesLevee; }
            set { _UsesLevee = value; NotifyPropertyChanged(); }
        }
        public GeoTech.LeveeFeatureElement LeveeElement
        {
            get { return _LeveeElement; }
            set { _LeveeElement = value; NotifyPropertyChanged(); }
        }


        public List<StageTransforms.ExteriorInteriorElement> ExteriorInteriorList
        {
            get { return _ExteriorInteriorList; }
            set { _ExteriorInteriorList = value; NotifyPropertyChanged(); }
        }
        public bool UsesExteriorInterior
        {
            get { return _UsesExteriorInterior; }
            set { _UsesExteriorInterior = value; NotifyPropertyChanged(); }
        }
        public StageTransforms.ExteriorInteriorElement ExteriorInteriorElement
        {
            get { return _ExteriorInteriorElement; }
            set { _ExteriorInteriorElement = value; NotifyPropertyChanged(); }
        }
        public List<FlowTransforms.InflowOutflowElement> InflowOutflowList
        {
            get { return _InflowOutflowList; }
            set { _InflowOutflowList = value; NotifyPropertyChanged(); }
        }
        public bool UsesInflowOutflow
        {
            get { return _UsesInflowOutflow; }
            set { _UsesInflowOutflow = value;  NotifyPropertyChanged(); }
        }
        public FlowTransforms.InflowOutflowElement InflowOutflowElement
        {
            get { return _InflowOutflowElement; }
            set { _InflowOutflowElement = value; NotifyPropertyChanged(); }
        }
        public List<ImpactArea.ImpactAreaElement> ImpactAreaSets
        {
            get { return _ImpactAreaSets; }
            set { _ImpactAreaSets = value; NotifyPropertyChanged(); }
        }
        public List<FrequencyRelationships.AnalyticalFrequencyElement> AnalyiticalRelationships
        {
            get { return _AnalyiticalRelationships; }
            set { _AnalyiticalRelationships = value; NotifyPropertyChanged(); }
        }
        public List<StageTransforms.RatingCurveElement> RatingCurveRelationships
        {
            get { return _RatingCurveRelationships; }
            set { _RatingCurveRelationships = value; NotifyPropertyChanged(); }
        }
        public List<AggregatedStageDamage.AggregatedStageDamageElement> StageDamageRelationships
        {
            get { return _DamageRelationships; }
            set { _DamageRelationships = value; NotifyPropertyChanged(); }
        }
        public ImpactArea.ImpactAreaElement ImpactAreaSet
        {
            get { return _ImpactAreaSet; }
            set { _ImpactAreaSet = value; NotifyPropertyChanged(); }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
        public int AnalysisYear
        {
            get { return _AnalysisYear; }
            set { _AnalysisYear = value; NotifyPropertyChanged(); }
        }
        public ImpactArea.ImpactAreaRowItem ImpactArea
        {
            get { return _ImpactArea; }
            set { _ImpactArea = value; NotifyPropertyChanged(); }
        }
        public bool UseAnalyticalFlowFrequency
        {
            get { return _UsesAnalyticalFlowFrequency; }
            set { _UsesAnalyticalFlowFrequency = value; NotifyPropertyChanged(); }
        }
        public FrequencyRelationships.AnalyticalFrequencyElement AnalyticalFlowFrequency
        {
            get { return _AnalyticalFlowFrequency; }
            set { _AnalyticalFlowFrequency = value; NotifyPropertyChanged(); }
        }
        public bool UseRatingCurve
        {
            get { return _UseRatingCurve; }
            set { _UseRatingCurve = value; NotifyPropertyChanged(); }
        }
        public StageTransforms.RatingCurveElement RatingCurve
        {
            get { return _RatingCurve; }
            set { _RatingCurve = value; NotifyPropertyChanged(); }
        }
        public bool UseAggregatedStageDamage
        {
            get { return _UseAggregatedStageDamage; }
            set { _UseAggregatedStageDamage = value; NotifyPropertyChanged(); }
        }
        public AggregatedStageDamage.AggregatedStageDamageElement StageDamage
        {
            get { return _StageDamage; }
            set { _StageDamage = value; NotifyPropertyChanged(); }
        }
        public bool UseThreshold
        {
            get { return _UseThreshold; }
            set { _UseThreshold = value; NotifyPropertyChanged(); }
        }
        public List<string> ThresholdTypes
        {
            get { return _ThresholdTypes; }
            set { _ThresholdTypes = value; NotifyPropertyChanged(); }
        }
        public string ThresholdType { get; set; }
        public double ThresholdValue
        {
            get { return _ThresholdValue; }
            set { _ThresholdValue = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public ConditionsEditorVM( List<ImpactArea.ImpactAreaElement> impactAreaElements, List<FrequencyRelationships.AnalyticalFrequencyElement> analyiticalFrequencyRelationships, List<FlowTransforms.InflowOutflowElement> inflowOutflowList, List<StageTransforms.RatingCurveElement> ratingCurveRelationships, List<StageTransforms.ExteriorInteriorElement> intExtList, List<GeoTech.LeveeFeatureElement> leveeList, List<GeoTech.FailureFunctionElement> failureFunctionsList, List<AggregatedStageDamage.AggregatedStageDamageElement> damageFunctions, List<Inventory.InventoryElement> structureInventories, ConditionsOwnerElement owner)
        {
            

            if (impactAreaElements.Count > 0)
            {
                ImpactAreaSets = impactAreaElements;
                ImpactAreaSet = impactAreaElements.FirstOrDefault();
                ImpactArea = ImpactAreaSet.ImpactAreaRows.FirstOrDefault();
            }

            AnalysisYear = 2017;
            AnalyiticalRelationships = analyiticalFrequencyRelationships;
            if (AnalyiticalRelationships.Count > 0)
            {
                UseAnalyticalFlowFrequency = true;
            }
            AnalyticalFlowFrequency = _AnalyiticalRelationships.FirstOrDefault();

            InflowOutflowList = inflowOutflowList;
            if (InflowOutflowList.Count > 0)
            {
                UsesInflowOutflow = true;
            }
            InflowOutflowElement = _InflowOutflowList.FirstOrDefault();

            RatingCurveRelationships = ratingCurveRelationships;
            if (RatingCurveRelationships.Count > 0)
            {
                UseRatingCurve = true;
            }
            RatingCurve = _RatingCurveRelationships.FirstOrDefault();

            ExteriorInteriorList = intExtList;
            if (ExteriorInteriorList.Count > 0)
            {
                UsesExteriorInterior = true;
            }
            ExteriorInteriorElement = ExteriorInteriorList.FirstOrDefault();

            LeveeList = leveeList;
            if (LeveeList.Count > 0)
            {
                UsesLevee = true;
            }
            LeveeElement = LeveeList.FirstOrDefault();

            FailureFunctionList = failureFunctionsList;
            if (FailureFunctionList.Count > 0)
            {
                UsesFailureFunction = true;
            }
            FailureFunctionElement = FailureFunctionList.FirstOrDefault();

            StageDamageRelationships = damageFunctions;
            if (StageDamageRelationships.Count > 0)
            {
                UseAggregatedStageDamage = true;
            }
            StageDamage = _DamageRelationships.FirstOrDefault();

            StructureInventoryList = structureInventories;
            if(structureInventories.Count > 0)
            {
                UsesStructureInventory = true;
            }
            StructureInventoryElement = StructureInventoryList.FirstOrDefault();


        }
        public ConditionsEditorVM(String name, String description, int analysisYear, List<ImpactArea.ImpactAreaElement> impactAreaElements, ImpactArea.ImpactAreaElement IAElement, ImpactArea.ImpactAreaRowItem IARowItem, bool useAnalyticalFrequency, List<FrequencyRelationships.AnalyticalFrequencyElement> analyiticalFrequencyRelationships, FrequencyRelationships.AnalyticalFrequencyElement AFElement, bool useInflowOutflow, List<FlowTransforms.InflowOutflowElement> inflowOutflowList,FlowTransforms.InflowOutflowElement inflowOutflowElement, bool useRatingCurve, List<StageTransforms.RatingCurveElement> ratingCurveRelationships, StageTransforms.RatingCurveElement RCElement, bool useExteriorInterior, List<StageTransforms.ExteriorInteriorElement> exteriorInteriorList,StageTransforms.ExteriorInteriorElement exteriorInteriorElement, bool useLevee, List<GeoTech.LeveeFeatureElement> leveeList,GeoTech.LeveeFeatureElement leveeElement, bool useFailureFunction, List<GeoTech.FailureFunctionElement> failureFunctionList,GeoTech.FailureFunctionElement failureFunctionElement, bool useStageDamage, List<AggregatedStageDamage.AggregatedStageDamageElement> damageFunctions, AggregatedStageDamage.AggregatedStageDamageElement DamageElement, bool useThreshold,string thresholdType,double thresholdValue, ConditionsOwnerElement owner)
        {
            Name = name;
            Description = description;
            AnalysisYear = analysisYear;
            ImpactAreaSets = impactAreaElements;
            ImpactAreaSet = IAElement;
            ImpactArea = IARowItem;//what if the user has deleted a row item??

            UseAnalyticalFlowFrequency = useAnalyticalFrequency;
            AnalyiticalRelationships = analyiticalFrequencyRelationships;
            AnalyticalFlowFrequency = AFElement;

            UsesInflowOutflow = useInflowOutflow;
            InflowOutflowElement = inflowOutflowElement;
            InflowOutflowList = inflowOutflowList;

            RatingCurveRelationships = ratingCurveRelationships;
            UseRatingCurve = useRatingCurve; 
            RatingCurve = RCElement;

            UsesExteriorInterior = useExteriorInterior;
            ExteriorInteriorList = exteriorInteriorList;
            ExteriorInteriorElement = exteriorInteriorElement;

            UsesLevee = useLevee;
            LeveeList = leveeList;
            LeveeElement = leveeElement;

            UsesFailureFunction = useFailureFunction;
            FailureFunctionList = failureFunctionList;
            FailureFunctionElement = failureFunctionElement;

            UseAggregatedStageDamage = useStageDamage;
            StageDamageRelationships = damageFunctions;
            StageDamage = DamageElement;
            

            //thresholds etc.
            UseThreshold = useThreshold;
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
            

        }
        #endregion
        #region Voids
        public void ShowSelectedInflowOutflowCurve(object sender, EventArgs e)
        {
            if(InflowOutflowElement != null)
            {
                InflowOutflowElement.EditInflowOutflowCurve(sender, e);
            }
        }
        public void ShowSelectedExteriorInteriorCurve(object sender, EventArgs e)
        {
            if (ExteriorInteriorElement != null)
            {
                ExteriorInteriorElement.EditExteriorInteriorCurve(sender, e);
            }
        }
        public void ShowSelectedLateralStructure(object sender, EventArgs e)
        {
            if (LeveeElement != null)
            {
                LeveeElement.EditLeveeFeature(sender, e);
            }
        }
        public void ShowSelectedFailureFunctionCurve(object sender, EventArgs e)
        {
            if (FailureFunctionElement != null)
            {
                FailureFunctionElement.EditFailureFunctionCurve(sender, e);
            }
        }
        public void ShowSelectedFrequencyCurve(object sender, EventArgs e)
        {
            if (AnalyticalFlowFrequency != null)
            {
                AnalyticalFlowFrequency.EditFlowFreq(sender, e);
            }
        }
        public void ShowSelectedRatingCurve(object sender, EventArgs e)
        {
            if (RatingCurve != null)
            {
                RatingCurve.EditRatingCurve(sender, e);
            }
        }
        public void ShowSelectedDamageCurve(object sender, EventArgs e)
        {
            if (StageDamage != null)
            {
                StageDamage.EditDamageCurve(sender, e);
            }
        }
        public void ShowStructureInventory(object sender, EventArgs e)
        {
            if (StructureInventoryElement != null)
            {
                //StructureInventoryElement.Edit(sender, e);
            }
        }

        public void NewInflowOutflowCurve(object sender, EventArgs e)
        {
            //if (_owner != null)
            //{
            //    List<FlowTransforms.InflowOutflowOwnerElement> eles = _owner.GetElementsOfType<FlowTransforms.InflowOutflowOwnerElement>();
            //    if (eles.Count > 0)
            //    {
            //        eles.FirstOrDefault().AddInflowOutflow(sender, e);
            //        //need to determine what the most recent element is and see if we already have it.
            //        if (eles.FirstOrDefault().Elements.Count > 0)
            //        {
            //            if (eles.FirstOrDefault().Elements.Count > InflowOutflowList.Count)
            //            {
            //                //InflowOutflowList.Add((FlowTransforms.InflowOutflowElement)eles.FirstOrDefault().Elements.Last());
            //                List<FlowTransforms.InflowOutflowElement> theNewList = new List<FlowTransforms.InflowOutflowElement>();
            //                for(int i =0;i<eles.FirstOrDefault().Elements.Count;i++)
            //                {
            //                    theNewList.Add((FlowTransforms.InflowOutflowElement)eles.FirstOrDefault().Elements[i]);
            //                }
            //                InflowOutflowList = theNewList;

            //                InflowOutflowElement = InflowOutflowList.Last();
            //            }
            //        }
            //    }
            //}
        }

        public void NewExtIntStageCurve(object sender, EventArgs e)
        {
            //if (_owner != null)
            //{
            //    List<StageTransforms.ExteriorInteriorOwnerElement> eles = _owner.GetElementsOfType<StageTransforms.ExteriorInteriorOwnerElement>();
            //    if (eles.Count > 0)
            //    {
            //        eles.FirstOrDefault().AddNewExteriorInteriorCurve(sender, e);
            //        //need to determine what the most recent element is and see if we already have it.
            //        if (eles.FirstOrDefault().Elements.Count > 0)
            //        {
            //            if (eles.FirstOrDefault().Elements.Count > ExteriorInteriorList.Count)
            //            {

            //                List<StageTransforms.ExteriorInteriorElement> theNewList = new List<StageTransforms.ExteriorInteriorElement>();
            //                for (int i = 0; i < eles.FirstOrDefault().Elements.Count; i++)
            //                {
            //                    theNewList.Add((StageTransforms.ExteriorInteriorElement)eles.FirstOrDefault().Elements[i]);
            //                }
            //                ExteriorInteriorList = theNewList;
            //                //ExteriorInteriorList.Add((StageTransforms.ExteriorInteriorElement)eles.FirstOrDefault().Elements.Last());
            //                ExteriorInteriorElement = ExteriorInteriorList.Last();
            //            }
            //        }
            //    }
            //}
        }

        public void NewLeveeFeature(object sender, EventArgs e)
        {
            //if (_owner != null)
            //{
            //    List<GeoTech.LeveeFeatureOwnerElement> eles = _owner.GetElementsOfType<GeoTech.LeveeFeatureOwnerElement>();
            //    if (eles.Count > 0)
            //    {
            //        eles.FirstOrDefault().AddNewLeveeFeature(sender, e);
            //        //need to determine what the most recent element is and see if we already have it.
            //        if (eles.FirstOrDefault().Elements.Count > 0)
            //        {
            //            if (eles.FirstOrDefault().Elements.Count > LeveeList.Count)
            //            {

            //                List<GeoTech.LeveeFeatureElement> theNewList = new List<GeoTech.LeveeFeatureElement>();
            //                for (int i = 0; i < eles.FirstOrDefault().Elements.Count; i++)
            //                {
            //                    theNewList.Add((GeoTech.LeveeFeatureElement)eles.FirstOrDefault().Elements[i]);
            //                }
            //                LeveeList = theNewList;
            //                //LeveeList.Add((GeoTech.LeveeFeatureElement)eles.FirstOrDefault().Elements.Last());
            //                LeveeElement = LeveeList.Last();
            //            }
            //        }
            //    }
            //}
        }

        public void NewFailureFunctionCurve(object sender, EventArgs e)
        {
            //if (_owner != null)
            //{
            //    List<GeoTech.FailureFunctionOwnerElement> eles = _owner.GetElementsOfType<GeoTech.FailureFunctionOwnerElement>();
            //    if (eles.Count > 0)
            //    {
            //        eles.FirstOrDefault().AddNewFailureFunction(sender, e);
            //        //need to determine what the most recent element is and see if we already have it.
            //        if (eles.FirstOrDefault().Elements.Count > 0)
            //        {
            //            if (eles.FirstOrDefault().Elements.Count > FailureFunctionList.Count)
            //            {
            //                List<GeoTech.FailureFunctionElement> theNewList = new List<GeoTech.FailureFunctionElement>();
            //                for (int i = 0; i < eles.FirstOrDefault().Elements.Count; i++)
            //                {
            //                    theNewList.Add((GeoTech.FailureFunctionElement)eles.FirstOrDefault().Elements[i]);
            //                }
            //                FailureFunctionList = theNewList;
            //                //FailureFunctionList.Add((GeoTech.FailureFunctionElement)eles.FirstOrDefault().Elements.Last());
            //                FailureFunctionElement = FailureFunctionList.Last();
            //            }
            //        }
            //    }
            //}
        }

        public void NewFrequencyCurve(object sender, EventArgs e)
        {
            //if (_owner != null)
            //{
            //    List<FrequencyRelationships.AnalyticalFrequencyOwnerElement> eles = _owner.GetElementsOfType<FrequencyRelationships.AnalyticalFrequencyOwnerElement>();
            //    if (eles.Count > 0)
            //    {
            //        eles.FirstOrDefault().AddNewFlowFrequencyCurve(sender, e);
            //        //need to determine what the most recent element is and see if we already have it.
            //        if (eles.FirstOrDefault().Elements.Count > 0)
            //        {
            //            if (eles.FirstOrDefault().Elements.Count > AnalyiticalRelationships.Count)
            //            {
            //                List<FrequencyRelationships.AnalyticalFrequencyElement> theNewList = new List<FrequencyRelationships.AnalyticalFrequencyElement>();
            //                for (int i = 0; i < eles.FirstOrDefault().Elements.Count; i++)
            //                {
            //                    theNewList.Add((FrequencyRelationships.AnalyticalFrequencyElement)eles.FirstOrDefault().Elements[i]);
            //                }
            //                AnalyiticalRelationships = theNewList;
            //                //AnalyiticalRelationships.Add((FrequencyRelationships.AnalyticalFrequencyElement)eles.FirstOrDefault().Elements.Last());
            //                AnalyticalFlowFrequency = AnalyiticalRelationships.Last();
            //            }
            //        }
            //    }
            //}
        }
        public void NewRatingCurve(object sender, EventArgs e)
        {
            //if (_owner != null)
            //{
            //    List<StageTransforms.RatingCurveOwnerElement> eles = _owner.GetElementsOfType<StageTransforms.RatingCurveOwnerElement>();
            //    if (eles.Count > 0)
            //    {
            //        eles.FirstOrDefault().AddNewRatingCurve(sender, e);
            //        //need to determine what the most recent element is and see if we already have it.
            //        if (eles.FirstOrDefault().Elements.Count > 0)
            //        {
            //            if (eles.FirstOrDefault().Elements.Count > RatingCurveRelationships.Count)
            //            {
            //                List<StageTransforms.RatingCurveElement> theNewList = new List<StageTransforms.RatingCurveElement>();
            //                for (int i = 0; i < eles.FirstOrDefault().Elements.Count; i++)
            //                {
            //                    theNewList.Add((StageTransforms.RatingCurveElement)eles.FirstOrDefault().Elements[i]);
            //                }
            //                RatingCurveRelationships = theNewList;
            //                //RatingCurveRelationships.Add((StageTransforms.RatingCurveElement)eles.FirstOrDefault().Elements.Last());
            //                RatingCurve = RatingCurveRelationships.Last();
            //            }
            //        }
            //    }
            //}
        }
        public void NewDamageCurve(object sender, EventArgs e)
        {
            //if (_owner != null)
            //{
            //    List<AggregatedStageDamage.AggregatedStageDamageOwnerElement> eles = _owner.GetElementsOfType<AggregatedStageDamage.AggregatedStageDamageOwnerElement>();
            //    if (eles.Count > 0)
            //    {
            //        eles.FirstOrDefault().AddNewDamageCurve(sender, e);
            //        //need to determine what the most recent element is and see if we already have it.
            //        if (eles.FirstOrDefault().Elements.Count > 0)
            //        {
            //            if (eles.FirstOrDefault().Elements.Count > StageDamageRelationships.Count)
            //            {
            //                List<AggregatedStageDamage.AggregatedStageDamageElement> theNewList = new List<AggregatedStageDamage.AggregatedStageDamageElement>();
            //                for (int i = 0; i < eles.FirstOrDefault().Elements.Count; i++)
            //                {
            //                    theNewList.Add((AggregatedStageDamage.AggregatedStageDamageElement)eles.FirstOrDefault().Elements[i]);
            //                }
            //                StageDamageRelationships = theNewList;
            //                //StageDamageRelationships.Add((AggregatedStageDamage.AggregatedStageDamageElement)eles.FirstOrDefault().Elements.Last());
            //                StageDamage = StageDamageRelationships.Last();
            //            }
            //        }
            //    }
            //}
        }

        public void NewStructureInventory(object sender, EventArgs e)
        {
            //if (_owner != null)
            //{
            //    List<Inventory.StructureInventoryOwnerElement> eles = _owner.GetElementsOfType<Inventory.StructureInventoryOwnerElement>();
            //    if (eles.Count > 0)
            //    {
            //        eles.FirstOrDefault().AddStructureInventory(sender, e);
            //        //need to determine what the most recent element is and see if we already have it.
            //        if (eles.FirstOrDefault().Elements.Count > 0)
            //        {
            //            if (eles.FirstOrDefault().Elements.Count > StructureInventoryList.Count)
            //            {
            //                List<Inventory.InventoryElement> theNewList = new List<Inventory.InventoryElement>();
            //                for (int i = 0; i < eles.FirstOrDefault().Elements.Count; i++)
            //                {
            //                    theNewList.Add((Inventory.InventoryElement)eles.FirstOrDefault().Elements[i]);
            //                }
            //                StructureInventoryList = theNewList;
            //                //StageDamageRelationships.Add((AggregatedStageDamage.AggregatedStageDamageElement)eles.FirstOrDefault().Elements.Last());
            //                StructureInventoryElement = StructureInventoryList.Last();
            //            }
            //        }
            //    }
            //}
        }

        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => { if (Name == null) { return false; } else { return !Name.Equals(""); } }, "Name cannot be blank");

            //must have a year
            AddRule(nameof(AnalysisYear), () => AnalysisYear > 1500, "Analysis Year must be after 1500");
            //must have a threshold stage or dollars.
        }

      
    }
}
