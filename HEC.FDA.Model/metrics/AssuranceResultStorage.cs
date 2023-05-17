using System;
using Statistics.Histograms;
using Statistics;
using System.Xml.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.Model.metrics
{
    public class AssuranceResultStorage
    {
        #region Fields
        private double[] _TempResults;
        Histogram _Assurance;
        string _Type;
        double _StandardNonExceedanceProbability;

        #endregion

        #region Properties
        public string AssuranceType
        {
            get
            {
                return _Type;
            }
        }
        public Histogram AssuranceHistogram
        {
            get
            {
                return _Assurance;
            }
        }
        public double StandardNonExceedanceProbability
        {
            get
            {
                return _StandardNonExceedanceProbability;
            }
        }
        #endregion

        #region Constructors
        internal AssuranceResultStorage(string dummyAsuranceType, double standardNonExceedanceProbability)
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
            _TempResults = new double[0];
            _Assurance = new Histogram();
            _Type = dummyAsuranceType;
            _StandardNonExceedanceProbability = standardNonExceedanceProbability;
        }
        public AssuranceResultStorage(string assuranceType, ConvergenceCriteria convergenceCriteria, double min, double binWidth, double standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee = 0)
        {
            _StandardNonExceedanceProbability = standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee;
            _TempResults = new double[convergenceCriteria.IterationCount];
            _Assurance = new Histogram(min, binWidth, convergenceCriteria);
            _Type = assuranceType;
        }
        public AssuranceResultStorage(string assuranceType, double binWidth, ConvergenceCriteria convergenceCriteria, double standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee = 0)
        {
            _StandardNonExceedanceProbability = standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee;
            _TempResults = new double[convergenceCriteria.IterationCount];
            _Assurance = new Histogram(binWidth, convergenceCriteria);
            _Type = assuranceType;
        }
        private AssuranceResultStorage(string assuranceType, double standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee, Histogram assuranceHistogram)
        {
            _Type = assuranceType;
            _StandardNonExceedanceProbability = standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee;
            _Assurance = assuranceHistogram;
            _TempResults = new double[assuranceHistogram.ConvergenceCriteria.IterationCount];
        }
        #endregion

        #region Methods

        public bool Equals(AssuranceResultStorage incomingAssuranceResultStorage)
        {
            if (_Type == incomingAssuranceResultStorage.AssuranceType)
            {
                if (_StandardNonExceedanceProbability == incomingAssuranceResultStorage.StandardNonExceedanceProbability)
                {
                    if (_Assurance.Equals(incomingAssuranceResultStorage.AssuranceHistogram))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void AddObservation(double result, int iteration)
        {
            _TempResults[iteration] = (result);
        }

        public void PutDataIntoHistogram()
        {
            _Assurance.AddObservationsToHistogram(_TempResults);
            Array.Clear(_TempResults);
        }

        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("Assurance");
            XElement histogramElement = _Assurance.ToXML();
            histogramElement.Name = "Histogram";
            masterElement.Add(histogramElement);
            masterElement.SetAttributeValue("Type", _Type);
            masterElement.SetAttributeValue("ExceedanceProbability", _StandardNonExceedanceProbability);
            return masterElement;
        }

        public static AssuranceResultStorage ReadFromXML(XElement xElement)
        {
            string type = xElement.Attribute("Type").Value;
            double probability = Convert.ToDouble(xElement.Attribute("ExceedanceProbability").Value);
            Histogram inlineHistogram = Histogram.ReadFromXML(xElement.Element("Histogram"));
            return new AssuranceResultStorage(type, probability, inlineHistogram);
        }

        
        #endregion
    }
}
