using ViewModel.Utilities;
using Model;
using Statistics;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ViewModel.Output
{
    //[Author(q0heccdm, 2 / 2 / 2017 11:42:33 AM)]
    public class LinkedPlotsVM : BaseViewModel
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 2/2/2017 11:42:33 AM
        #endregion
        #region Fields
        private IConditionLocationYearResult _Results;
            //private FdaModel.ComputationPoint.Outputs.Result _Result;
            //private List<FdaModel.Functions.BaseFunction> _FunctionsList;
            //private FdaModel.ComputationPoint.Outputs.Realization _Realization;
        #endregion
        #region Properties
            //public FdaModel.ComputationPoint.Outputs.Result Result
            //{
            //    get { return _Result; }
            //    set { _Result = value; }
            //}
            //public List<FdaModel.Functions.BaseFunction> FunctionsList
            //{
            //    get { return _FunctionsList; }
            //    set { _FunctionsList = value; }
            //}

            //public FdaModel.ComputationPoint.Outputs.Realization Realization
            //{
            //    get { return _Realization; }
            //    set { _Realization = value; }
            //}

            public double EADMean { get; set; }
        public double AEPMean { get; set; }
        public string MeanAEPLabel { get; set; }
        #endregion
        #region Constructors
        public LinkedPlotsVM() : base()
        {

        }

        public LinkedPlotsVM(IConditionLocationYearResult result) : base()
        {
            SetDimensions(800, 500, 500,400);
            _Results = result;
            IReadOnlyList<IConditionLocationYearRealizationSummary> realizations = result.Realizations;
            foreach (KeyValuePair<IMetric, Histogram> entry in result.Metrics)
            {
                if(entry.Key.ParameterType == IParameterEnum.EAD)
                {
                    EADMean = Math.Round(entry.Value.Mean, 2);
                }
                if (entry.Key.ParameterType == IParameterEnum.DamageAEP || entry.Key.ParameterType == IParameterEnum.ExteriorStageAEP ||
                    entry.Key.ParameterType == IParameterEnum.InteriorStageAEP)
                {
                    AEPMean = Math.Round(entry.Value.Mean, 3);
                    MeanAEPLabel = entry.Key.ParameterType.PrintLabel();
                }

            }
            
            //foreach(IConditionLocationYearRealizationSummary realization in realizations)
            //{
                
            //    //realization.Metrics.
            //}
        }

        //public LinkedPlotsVM(FdaModel.ComputationPoint.Outputs.Realization realization)// List<FdaModel.Functions.BaseFunction> realizationFunctions)
        //{

        //    FunctionsList = realization.Functions;//realizationFunctions;
        //}
        //public LinkedPlotsVM(FdaModel.ComputationPoint.Outputs.Result result)
        //{

        //    Result = result;
        //}
        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

       
        public void DisplayEADHistogram()
        {
            foreach (KeyValuePair<IMetric, Histogram> entry in _Results.Metrics)
            {
                if (entry.Key.ParameterType == IParameterEnum.EAD)
                {
                    HistogramViewerVM histVM = new HistogramViewerVM(entry.Key, entry.Value, EADMean);
                    string header = "EAD Histogram";
                    DynamicTabVM tab = new DynamicTabVM(header, histVM, "eadHistogram");

                    Navigate(tab, false,false);
                }
            }
        }

        public void DisplayAEPHistogram()
        {
            foreach (KeyValuePair<IMetric, Histogram> entry in _Results.Metrics)
            {
                if (entry.Key.ParameterType != IParameterEnum.EAD)
                {
                    HistogramViewerVM histVM = new HistogramViewerVM(entry.Key, entry.Value, AEPMean);
                    string header = "AEP Histogram";
                    DynamicTabVM tab = new DynamicTabVM(header, histVM, "aepHistogram");

                    Navigate(tab, false, false);
                }
            }
        }

        #endregion
        #region Functions
        #endregion
    }
}
