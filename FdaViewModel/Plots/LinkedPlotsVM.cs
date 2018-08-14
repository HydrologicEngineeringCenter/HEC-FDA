using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

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
       

        #endregion
        #region Properties
        public double EAD { get; set; }
        public double AEP { get; set; }
        
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
        public LinkedPlotsVM(FdaModel.ComputationPoint.Outputs.Result result)
        {
            Result = result;
            if(result.Realizations.Count>0)
            {
                AssignTheCurvesToThePlots(result.Realizations[0].Functions);
                CheckIfAllPlotsExists();
            }
        }
        #endregion
        #region Voids

        

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
                    Plot0VM = new IndividualLinkedPlotVM(ord, ord.Function,"LP3","Probability","Inflow (cfs)");

                    //Plot0Curve = ord.Function;
                }
                if (func.FunctionType == FdaModel.Functions.FunctionTypes.InflowOutflow)
                {
                    Plot1VM = new IndividualLinkedPlotVM(ord,ord.Function,"Inflow-Outflow","Inflow (cfs)", "Outflow (cfs)");
                    //Plot1Curve = ord.Function;
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
                    
                }
                if (func.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
                {
                    Plot7VM = new IndividualLinkedPlotVM(func,ord.Function, "Interior Stage Damage", "Interior Stage (ft)", "Damage ($)");
                    //Plot7Curve = ord.Function;
                }
                if (func.FunctionType == FdaModel.Functions.FunctionTypes.ExteriorInteriorStage)
                {
                    Plot5VM = new IndividualLinkedPlotVM(func,ord.Function, "Exterior-Interior Stage", "Exterior Stage (ft)", "Interior Stage (ft)");
                    //Plot5Curve = ord.Function;
                }
                if (func.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
                {
                    Plot8VM = new IndividualLinkedPlotVM(func,ord.Function, "Damage Frequency", "Probability", "Damage ($)");
                    //Plot8Curve = ord.Function;
                }


            }
        }
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            //AddRule(nameof(XValue), () => XValue < 0, "X value cannot be less than zero.");

        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
    }
}
