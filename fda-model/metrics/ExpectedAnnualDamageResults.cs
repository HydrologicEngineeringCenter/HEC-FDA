using System;
using System.Collections.Generic;
using Statistics;
using Statistics.Histograms;
namespace metrics
{
    public class ExpectedAnnualDamageResults: IContainResults
    {
        //needs access to get AEP and EAD results.
        private const double EAD_HISTOGRAM_BINWIDTH = 10;
        private Dictionary<string, Histogram> _ead; 
   
        

        public double AEPThreshold { 
            get {
                return _aepThreshold;
            } 
            set {
            _aepThreshold = value;
            }
        }
        public ExpectedAnnualDamageResults(){
            _aepThreshold = 0.0;
            _aep = null; //is this necessary?
            _ead = new Dictionary<string, Histogram>();
            _cnep = new Dictionary<double, Histogram>();
        }
        public void AddAEPEstimate(double aepEstimate)
        {
            double[] data = new double[1] { aepEstimate };
            IData aep = IDataFactory.Factory(data);
            if (_aep!=null)
            {
                _aep.AddObservationToHistogram(aep);
            } else
            {
                var histo = new Histogram(aep, AEP_HISTOGRAM_BINWIDTH);
                _aep = histo;
            }
            

        }

        public void AddStageForCNEP(double standardProbability, double stageForCNEP)
        {
            double[] data = new double[1] {stageForCNEP};
            IData stage = IDataFactory.Factory(data);
            if (_cnep.ContainsKey(standardProbability))
            {
                _cnep[standardProbability].AddObservationToHistogram(stage);
            } else
            {
                var histo = new Histogram(stage,CNEP_HISTOGRAM_BINWIDTH);
                _cnep.Add(standardProbability,histo);
            }
        }

        public double MeanAEP()
        {
            return _aep.Mean;
        }

        public double MedianAEP()
        {
            return _aep.Median;
        }

        public double AssuranceOfAEP(double exceedanceProbability)
        {   //assurance of AEP is a non-exceedance probability
            double assuranceOfAEP = _aep.CDF(exceedanceProbability);
            return assuranceOfAEP;
        }

        public double ConditionalNonExceedanceProbability(double exceedanceProbability)
        {
            double conditionalNonExceedanceProbability = 1-_cnep[exceedanceProbability].CDF(_aepThreshold);
            return conditionalNonExceedanceProbability;
        }

        public void AddEADEstimate(double eadEstimate, string category)
        {
            double[] data = new double[1] { eadEstimate };
            IData ead = IDataFactory.Factory(data);
            if (_ead.ContainsKey(category))
            {
                _ead[category].AddObservationToHistogram(ead); 
            }
            else
            {
                var histo = new Histogram(ead, EAD_HISTOGRAM_BINWIDTH);
                _ead.Add(category, histo);
            }
        }
        public double MeanEAD(string category)
        {
            return _ead[category].Mean;
        }

        public double EADExceededWithProbabilityQ(string category, double exceedanceProbability)
        {
            double nonExceedanceProbability = 1 - exceedanceProbability;
            double quartile = _ead[category].InverseCDF(nonExceedanceProbability);
            return quartile;
        }
        
        public double LongTermExceedanceProbability(double years)
        {
            double ltep = 1-Math.Pow((1-MeanAEP()),years);       
            return ltep;
        }


    }
}