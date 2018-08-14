using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Conditions
{
    public class ConditionsElement : Utilities.OwnedElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableConstant = "Conditions - ";


        private string _Description;
        private int _AnalysisYear;
        private ImpactArea.ImpactAreaElement _ImpactAreaSet;
        private ImpactArea.ImpactAreaRowItem _ImpactArea;
        private bool _UsesAnalyiticalFlowFrequency;
        private FrequencyRelationships.AnalyticalFrequencyElement _AnalyticalFlowFrequency;

        private bool _UseInflowOutflow;
        private FlowTransforms.InflowOutflowElement _InflowOutflowElement;

        private bool _UseRatingCurve;
        private StageTransforms.RatingCurveElement _RatingCurve;

        private bool _UseExteriorInteriorStage;
        private StageTransforms.ExteriorInteriorElement _ExteriorInteriorElement;

        private bool _UseLevee;
        private GeoTech.LeveeFeatureElement _LeveeElement;

        private bool _UseFailureFunction;
        private GeoTech.FailureFunctionElement _FailureFunctionElement;

        private bool _UseAggregatedStageDamage;
        private AggregatedStageDamage.AggregatedStageDamageElement _StageDamage;
        private bool _UseThreshold;
        private string _ThresholdType;//dollars or stage. need enum.
        private double _ThresholdValue;
        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return _TableConstant;
        }
        public GeoTech.FailureFunctionElement FailureFunctionElement
        {
            get { return _FailureFunctionElement; }
            set { _FailureFunctionElement = value; NotifyPropertyChanged(); }
        }
        public bool UseFailureFunction
        {
            get { return _UseFailureFunction; }
            set { _UseFailureFunction = value; NotifyPropertyChanged(); }
        }
        public GeoTech.LeveeFeatureElement LeveeElement
        {
            get { return _LeveeElement; }
            set { _LeveeElement = value; NotifyPropertyChanged(); }
        }
        public bool UseLevee
        {
            get { return _UseLevee; }
            set { _UseLevee = value; NotifyPropertyChanged(); }
        }
        public StageTransforms.ExteriorInteriorElement ExteriorInteriorElement
        {
            get { return _ExteriorInteriorElement; }
            set { _ExteriorInteriorElement = value; NotifyPropertyChanged(); }
        }
        public bool UseExteriorInteriorStage
        {
            get { return _UseExteriorInteriorStage; }
            set { _UseExteriorInteriorStage = value; NotifyPropertyChanged(); }
        }
        public FlowTransforms.InflowOutflowElement InflowOutflowElement
        {
            get { return _InflowOutflowElement; }
            set { _InflowOutflowElement = value; NotifyPropertyChanged(); }
        }
        public bool UseInflowOutflow
        {
            get { return _UseInflowOutflow; }
            set { _UseInflowOutflow = value; NotifyPropertyChanged(); }
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
        public ImpactArea.ImpactAreaElement ImpactAreaSet
        {
            get { return _ImpactAreaSet; }
            set { _ImpactAreaSet = value; NotifyPropertyChanged(); }
        }
        public ImpactArea.ImpactAreaRowItem ImpactArea
        {
            get { return _ImpactArea; }
            set { _ImpactArea = value; NotifyPropertyChanged(); }
        }
        public bool UseAnalyiticalFlowFrequency
        {
            get { return _UsesAnalyiticalFlowFrequency; }
            set { _UsesAnalyiticalFlowFrequency = value; NotifyPropertyChanged(); }
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
        public string ThresholdType {
            get { return _ThresholdType; }
            set { _ThresholdType = value; NotifyPropertyChanged(); }
        }
        public double ThresholdValue
        {
            get { return _ThresholdValue; }
            set { _ThresholdValue = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public ConditionsElement(string name, string description, int analysisYear,ImpactArea.ImpactAreaElement impactAreaElement, ImpactArea.ImpactAreaRowItem indexLocation, bool usesAnalyiticalFlowFrequency, FrequencyRelationships.AnalyticalFrequencyElement aFlowFreq, bool usesInflowOutflow, FlowTransforms.InflowOutflowElement inflowOutflowElement, bool useRating, StageTransforms.RatingCurveElement rc, bool useIntExtStage, StageTransforms.ExteriorInteriorElement extInt, bool useLevee, GeoTech.LeveeFeatureElement leveeElement, bool useFailureFunction, GeoTech.FailureFunctionElement failureFunctionElement, bool useAggStageDamage, AggregatedStageDamage.AggregatedStageDamageElement stageDamage, bool useThreshold, string thresholdType, double thresholdValue, BaseFdaElement owner) : base(owner)
        {
            Name = name;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/Condition.png");

            Description = description;
            AnalysisYear = analysisYear;
            ImpactAreaSet = impactAreaElement;
            ImpactArea = indexLocation;

            UseAnalyiticalFlowFrequency = usesAnalyiticalFlowFrequency;
            AnalyticalFlowFrequency = aFlowFreq;

            UseInflowOutflow = usesInflowOutflow;
            InflowOutflowElement = inflowOutflowElement;

            UseRatingCurve = useRating;
            RatingCurve = rc;

            UseExteriorInteriorStage = useIntExtStage;
            ExteriorInteriorElement = extInt;

            UseLevee = useLevee;
            LeveeElement = leveeElement;

            UseFailureFunction = useFailureFunction;
            FailureFunctionElement = failureFunctionElement;

            UseAggregatedStageDamage = useAggStageDamage;
            StageDamage = stageDamage;

            UseThreshold = useThreshold;
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;

            Utilities.NamedAction edit = new Utilities.NamedAction();
            edit.Header = "Edit Condition";
            edit.Action = EditCondition;

            Utilities.NamedAction compute = new Utilities.NamedAction();
            compute.Header = "Compute Condition";
            compute.Action = ComputeCondition;

            Utilities.NamedAction removeCondition = new Utilities.NamedAction();
            removeCondition.Header = "Remove";
            removeCondition.Action = Remove;

            Utilities.NamedAction renameElement = new Utilities.NamedAction();
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(edit);
            localActions.Add(compute);
            localActions.Add(removeCondition);
            localActions.Add(renameElement);



            Actions = localActions;
        }

        private void EditCondition(object arg1, EventArgs arg2)
        {
            List<ImpactArea.ImpactAreaElement> impactAreas = GetElementsOfType<ImpactArea.ImpactAreaElement>();
            List<FrequencyRelationships.AnalyticalFrequencyElement> freqeles = GetElementsOfType<FrequencyRelationships.AnalyticalFrequencyElement>();
            List<FlowTransforms.InflowOutflowElement> inflowOutflowList = GetElementsOfType<FlowTransforms.InflowOutflowElement>();
            List<StageTransforms.RatingCurveElement> ratingeles = GetElementsOfType<StageTransforms.RatingCurveElement>();

            List<StageTransforms.ExteriorInteriorElement> extIntList = GetElementsOfType<StageTransforms.ExteriorInteriorElement>();
            List<GeoTech.LeveeFeatureElement> leveeList = GetElementsOfType<GeoTech.LeveeFeatureElement>();
            List<GeoTech.FailureFunctionElement> failureFunctionList = GetElementsOfType<GeoTech.FailureFunctionElement>();


            List<AggregatedStageDamage.AggregatedStageDamageElement> damageles = GetElementsOfType<AggregatedStageDamage.AggregatedStageDamageElement>();
            if (impactAreas.Count == 0)
            {
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("No Impact Area Sets have been defined.", FdaModel.Utilities.Messager.ErrorMessageEnum.Report | FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel));
                return;
            }
            ConditionsEditorVM vm = new ConditionsEditorVM(Name,Description, AnalysisYear, impactAreas,ImpactAreaSet,ImpactArea, UseAnalyiticalFlowFrequency, freqeles, AnalyticalFlowFrequency, UseInflowOutflow, inflowOutflowList,InflowOutflowElement,UseRatingCurve, ratingeles,RatingCurve, UseExteriorInteriorStage, extIntList,ExteriorInteriorElement,UseLevee,leveeList,LeveeElement,UseFailureFunction,failureFunctionList,FailureFunctionElement,UseAggregatedStageDamage, damageles, StageDamage,UseThreshold,ThresholdType,ThresholdValue, (ConditionsOwnerElement)_Owner);
            Navigate(vm);
            if (!vm.WasCancled)
            {
                if (!vm.HasError)
                {
                    //this isnt necessary unless we clone (which we should do)
                    Name = vm.Name;
                    Description = vm.Description;
                    AnalysisYear = vm.AnalysisYear;
                    ImpactAreaSet = vm.ImpactAreaSet;
                    ImpactArea = vm.ImpactArea;

                    UseAnalyiticalFlowFrequency = vm.UseAnalyticalFlowFrequency;
                    AnalyticalFlowFrequency = vm.AnalyticalFlowFrequency;

                    UseInflowOutflow = vm.UsesInflowOutflow;
                    InflowOutflowElement = vm.InflowOutflowElement;

                    UseRatingCurve = vm.UseRatingCurve;
                    RatingCurve = vm.RatingCurve;

                    UseExteriorInteriorStage = vm.UsesExteriorInterior;
                    ExteriorInteriorElement = vm.ExteriorInteriorElement;

                    UseLevee = vm.UsesLevee;
                    LeveeElement = vm.LeveeElement;

                    UseFailureFunction = vm.UsesFailureFunction;
                    FailureFunctionElement = vm.FailureFunctionElement;

                    UseAggregatedStageDamage = vm.UseAggregatedStageDamage;
                    StageDamage = vm.StageDamage;

                    UseThreshold = vm.UseThreshold;
                    ThresholdType = vm.ThresholdType;
                    ThresholdValue = vm.ThresholdValue;
                }
            }
        }

        private void ComputeCondition(object arg1, EventArgs arg2)
        {
            //convert to model types, run model compute, show results in window.
            //ImpactArea.Name
            //AnalyticalFlowFrequency.Distribution;
            //RatingCurve.RatingCurve //isvalid //distribution //getx gety
            //StageDamage.Curve same as above.
            //convert to model objects
            //compute model objects
            //take result and pass to results viewmodel
            List<FdaModel.Functions.BaseFunction> listOfBaseFunctions = new List<FdaModel.Functions.BaseFunction>();
            FdaModel.Functions.FrequencyFunctions.LogPearsonIII LP3 = new FdaModel.Functions.FrequencyFunctions.LogPearsonIII(AnalyticalFlowFrequency.Distribution, FdaModel.Functions.FunctionTypes.InflowFrequency);

            listOfBaseFunctions.Add(LP3);

            //FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction inflowOutflow = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)InflowOutflowElement.InflowOutflowCurve, FdaModel.Functions.FunctionTypes.InflowOutflow);

           // listOfBaseFunctions.Add(inflowOutflow);

            FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction rating = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)RatingCurve.RatingCurve, FdaModel.Functions.FunctionTypes.Rating);

            listOfBaseFunctions.Add(rating);

            FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction extIntStage = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)ExteriorInteriorElement.ExteriorInteriorCurve, FdaModel.Functions.FunctionTypes.ExteriorInteriorStage);

            listOfBaseFunctions.Add(extIntStage);

            FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction stageDamage = new FdaModel.Functions.OrdinatesFunctions.UncertainOrdinatesFunction((Statistics.UncertainCurveIncreasing)StageDamage.Curve, FdaModel.Functions.FunctionTypes.InteriorStageDamage);

            listOfBaseFunctions.Add(stageDamage);

            FdaModel.ComputationPoint.PerformanceThreshold threshold;

            if (ThresholdType == "Stage")
            {
                threshold = new FdaModel.ComputationPoint.PerformanceThreshold(FdaModel.ComputationPoint.PerformanceThresholdTypes.InteriorStage, ThresholdValue);

            }
            else // ThresholdType == "Dollars"
            {
                threshold = new FdaModel.ComputationPoint.PerformanceThreshold(FdaModel.ComputationPoint.PerformanceThresholdTypes.Damage, ThresholdValue);

            }


            FdaModel.ComputationPoint.Condition condition = new FdaModel.ComputationPoint.Condition(AnalysisYear, ImpactArea.Name, listOfBaseFunctions, threshold, null); //this constructor will call Validate

            //FdaModel.ComputationPoint.Outputs.Realization realization = new FdaModel.ComputationPoint.Outputs.Realization(condition, false, false);
            //Random randomNumGenerator = new Random(0);
            //realization.Compute(randomNumGenerator);

            FdaModel.ComputationPoint.Outputs.Result result = new FdaModel.ComputationPoint.Outputs.Result(condition, 1000);


            // write out results for testing purposes.
            Plots.LinkedPlotsVM vem = new Plots.LinkedPlotsVM(result);
            Output.LinkedPlotsVM vm = new Output.LinkedPlotsVM(result);
            Navigate(vem);
            Navigate(vm);
        }
        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }

        public override object[] RowData()
        {
            return new object[] { Name };
        }

        public override bool SavesToRow()
        {
            return true;
        }
        public override bool SavesToTable()
        {
            return false;
        }
        #endregion
        #region Functions
        public override string TableName
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion



    }
}
