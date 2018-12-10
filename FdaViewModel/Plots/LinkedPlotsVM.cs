using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using FdaModel.Functions;
using FdaModel.ComputationPoint;
using System.Collections.ObjectModel;

namespace FdaViewModel.Plots
{
    //[Author(q0heccdm, 4 / 6 / 2017 2:48:04 PM)]
    public delegate void DisplaySpecificPoint(double x, double y);
    public class LinkedPlotsVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 4/6/2017 2:48:04 PM
        #endregion
        #region Fields
        private int _IterationNumber;
        private int _TotalNumberOfRealizations;

        private double _EAD;
        private double _AEP;
        private double _MeanEAD;
        private double _MeanAEP;

        PerformanceThresholdTypes _thresholdType;
        double _thresholdValue;
        private ObservableCollection<Plots.IndividualLinkedPlotVM> _AvailablePlots = new ObservableCollection<Plots.IndividualLinkedPlotVM>();

        #endregion
        #region Properties
        public ObservableCollection<Plots.IndividualLinkedPlotVM> AvailablePlots
        {
            get { return _AvailablePlots; }
            set { _AvailablePlots = value; NotifyPropertyChanged(); }
        }
        public PerformanceThresholdTypes ThresholdType
        {
            get { return _thresholdType; }
            set { _thresholdType = value; NotifyPropertyChanged(); }
        }

        public double ThresholdValue
        {
            get { return _thresholdValue; }
            set { _thresholdValue = value; NotifyPropertyChanged(); }
        }
        public double EAD
        {
            get { return _EAD; }
            set{ _EAD = value; NotifyPropertyChanged();}
        }
        public double AEP
        {
            get { return _AEP; }
            set { _AEP = Math.Round(1 - value, 4);  NotifyPropertyChanged(); }
        }
        public double MeanAEP
        {
            get { return _MeanAEP; }
            set { _MeanAEP = Math.Round(1 - value, 4);  NotifyPropertyChanged(); }
        }
        public double MeanEAD
        {
            get { return _MeanEAD; }
            set { _MeanEAD =value;  NotifyPropertyChanged(); }
        }
        public int IterationNumber
        {
            get { return _IterationNumber + 1; }
            set { _IterationNumber = value; NotifyPropertyChanged(); }
        }

        public int TotalRealizations
        {
            get { return _TotalNumberOfRealizations; }
            set { _TotalNumberOfRealizations = value; NotifyPropertyChanged(); }
        }
        
        public FdaModel.ComputationPoint.Outputs.Result Result { get; set; }
      

        public IndividualLinkedPlotVM Plot0VM { get; set; }
        //public Statistics.CurveIncreasing Plot0Curve { get; set; }
        public IndividualLinkedPlotVM Plot1VM { get; set; }
        //public Statistics.CurveIncreasing Plot1Curve { get; set; }
        public IndividualLinkedPlotVM Plot3VM { get; set; }
        //public Statistics.CurveIncreasing Plot3Curve { get; set; }
        public IndividualLinkedPlotVM Plot5VM { get; set; }
        //public Statistics.CurveIncreasing Plot5Curve { get; set; }
        public IndividualLinkedPlotVM Plot7VM { get; set; }
        //public Statistics.CurveIncreasing Plot7Curve { get; set; }
        public IndividualLinkedPlotVM Plot8VM { get; set; }
        //public Statistics.CurveIncreasing Plot8Curve { get; set; }


            public List<string> SelectedElementNames { get; set; }
        #endregion
        #region Constructors
        public LinkedPlotsVM() : base()
        {

        }
        public LinkedPlotsVM(FdaModel.ComputationPoint.Outputs.Realization realization)// List<FdaModel.Functions.BaseFunction> ordFunctions)
        {
            EAD = Math.Round(realization.ExpectedAnnualDamage,3);
            AEP = Math.Round(realization.AnnualExceedanceProbability,3);

            AssignTheCurvesToThePlots(realization.Functions);

            CheckIfAllPlotsExists();
        }
        public LinkedPlotsVM(FdaModel.ComputationPoint.Outputs.Result result, PerformanceThresholdTypes thresholdType, double thresholdValue, List<string> selectedElementNames)
        {
            SelectedElementNames = selectedElementNames;
            Result = result;
            if (result.AEP != null)
            {
                MeanAEP = result.AEP.GetMean;
            }
            else
            {
                //do what?
                int test = 0;
            }
            if (result.EAD != null)
            {
                MeanEAD = result.EAD.GetMean;
            }
            else
            {
                //do what?
                int test = 0;
            }
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;

            if(result.Realizations.Count>0)
            {
                FdaModel.ComputationPoint.Outputs.Realization realization = result.Realizations[0];
                EAD = Math.Round(realization.ExpectedAnnualDamage, 3);
                AEP = Math.Round(realization.AnnualExceedanceProbability, 3);
                AssignTheCurvesToThePlots(realization.Functions);
                CheckIfAllPlotsExists();
                TotalRealizations = result.Realizations.Count;
                IterationNumber = 0;
            }
        }
        #endregion
        #region Voids

        public void UpdateCurvesToIteration(int iteration)
        {
            if (Result.Realizations.Count > iteration)
            {
                FdaModel.ComputationPoint.Outputs.Realization realization = Result.Realizations[iteration];
                //AssignTheCurvesToThePlots(realization.Functions);
                //CheckIfAllPlotsExists();
                if(realization == null) { return; }
                UpdateCurves(realization.Functions);
                IterationNumber = iteration;
                EAD = Math.Round(realization.ExpectedAnnualDamage, 3);
                AEP = Math.Round(realization.AnnualExceedanceProbability, 3);
            }
        }

        private void UpdateCurves(List<BaseFunction> functions )
        {
            foreach (BaseFunction func in functions)
            {
                FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction ord = func.GetOrdinatesFunction();
                switch(func.FunctionType)
                {
                    case FunctionTypes.InflowFrequency:
                    case FunctionTypes.OutflowFrequency:
                        Plot0VM.Curve = ord.Function;
                        break;
                    case FunctionTypes.InflowOutflow:
                        Plot1VM.Curve = ord.Function;
                        break;
                    case FunctionTypes.Rating:
                        //need to flip the xs and ys
                        List<double> ys = new List<double>();
                        List<double> xs = new List<double>();
                        foreach (double y in ord.Function.YValues)
                        {
                            ys.Add(y);
                        }
                        foreach (double x in ord.Function.XValues)
                        {
                            xs.Add(x);
                        }
                        FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction rat = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(func.GetOrdinatesFunction().Function.YValues, func.GetOrdinatesFunction().Function.XValues, FunctionTypes.Rating);
                        Plot3VM.Curve = rat.Function;
                        break;
                    case FunctionTypes.ExteriorInteriorStage:
                        Plot5VM.Curve = ord.Function;
                        break;
                    case FunctionTypes.InteriorStageDamage:
                        Plot7VM.Curve = ord.Function;
                        break;
                    case FunctionTypes.DamageFrequency:
                        Plot8VM.Curve = ord.Function;
                        break;
                }

            }
        }

        private void CheckIfAllPlotsExists()
        {
            //*********************************************************
            //  below is done to turn off the plots that don't have a curve
            //*********************************************************
            if (Plot0VM.Curve == null)
            {
                Plot0VM.Curve = new Statistics.CurveIncreasing(true, true);
            }
            if (Plot1VM.Curve == null)
            {
                Plot1VM.Curve = new Statistics.CurveIncreasing(true, true);
            }
            if (Plot3VM.Curve == null)
            {
                Plot3VM.Curve = new Statistics.CurveIncreasing(true, true);
            }
            if (Plot5VM.Curve == null)
            {
                Plot5VM.Curve = new Statistics.CurveIncreasing(true, true);
            }
            if (Plot7VM.Curve == null)
            {
                Plot7VM.Curve = new Statistics.CurveIncreasing(true, true);
            }
            if (Plot8VM.Curve == null)
            {
                Plot8VM.Curve = new Statistics.CurveIncreasing(true, true);
            }
        }
        private void AssignTheCurvesToThePlots(List<FdaModel.Functions.BaseFunction> ordFunctions)
        {
            Plot0VM = new IndividualLinkedPlotVM();
            Plot1VM = new IndividualLinkedPlotVM();
            Plot3VM = new IndividualLinkedPlotVM();
            Plot5VM = new IndividualLinkedPlotVM();
            Plot7VM = new IndividualLinkedPlotVM();
            Plot8VM = new IndividualLinkedPlotVM();
            foreach (FdaModel.Functions.BaseFunction func in ordFunctions)
            {
                FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction ord = func.GetOrdinatesFunction();
                if (func.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency || func.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency)
                {
                    Plot0VM = new IndividualLinkedPlotVM(ord, ord.Function,"LP3","Probability","Outflow (cfs)");
                    Plot0VM.Curve = ord.Function;
                    AvailablePlots.Add(Plot0VM);

                    //Plot0Curve = ord.Function;
                }
                if (func.FunctionType == FdaModel.Functions.FunctionTypes.InflowOutflow)
                {
                    Plot1VM = new IndividualLinkedPlotVM(ord,ord.Function,"Inflow-Outflow","Inflow (cfs)", "Outflow (cfs)");
                    //Plot1Curve = ord.Function;
                    AvailablePlots.Add(Plot1VM);
                }
                if (func.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
                {
                    //swap the x and y values
                    List<double> ys = new List<double>();
                    List<double> xs = new List<double>();
                    foreach (double y in ord.Function.YValues)
                    {
                        ys.Add(y);
                    }
                    foreach (double x in ord.Function.XValues)
                    {
                        xs.Add(x);
                    }
                    FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction rat = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(func.GetOrdinatesFunction().Function.YValues, func.GetOrdinatesFunction().Function.XValues, FdaModel.Functions.FunctionTypes.Rating);
                    Plot3VM = new IndividualLinkedPlotVM(rat,new Statistics.CurveIncreasing(ys.ToArray(), xs.ToArray(), true, false), "Rating", "Exterior Stage (ft)", "Outflow (cfs)");
                    //Plot3Curve = new Statistics.CurveIncreasing(ys.ToArray(), xs.ToArray(), true, false);
                    AvailablePlots.Add(Plot3VM);

                }
                if (func.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
                {
                    Plot7VM = new IndividualLinkedPlotVM(func,ord.Function, "Interior Stage Damage", "Interior Stage (ft)", "Damage ($)");
                    //Plot7Curve = ord.Function;
                    AvailablePlots.Add(Plot7VM);

                }
                if (func.FunctionType == FdaModel.Functions.FunctionTypes.ExteriorInteriorStage)
                {
                    Plot5VM = new IndividualLinkedPlotVM(func,ord.Function, "Exterior-Interior Stage", "Exterior Stage (ft)", "Interior Stage (ft)");
                    //Plot5Curve = ord.Function;
                    AvailablePlots.Add(Plot5VM);

                }
                if (func.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
                {
                    Plot8VM = new IndividualLinkedPlotVM(func,ord.Function, "Damage Frequency", "Probability", "Damage ($)");
                    //Plot8Curve = ord.Function;
                    AvailablePlots.Add(Plot8VM);

                }

                if(AvailablePlots.Count == SelectedElementNames.Count)
                {
                    for(int i = 0;i<AvailablePlots.Count;i++)
                    {
                        AvailablePlots[i].SelectedElementName = SelectedElementNames[i];
                    }
                }

            }
        }
        public override void AddValidationRules()
        {
            //AddRule(nameof(XValue), () => XValue < 0, "X value cannot be less than zero.");

        }

       
        #endregion
        #region Functions
        #endregion

    }
}
