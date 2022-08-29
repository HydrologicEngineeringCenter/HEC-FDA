using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics.Histograms;
using Statistics;
using System.Xml.Linq;

namespace metrics
{
public class AssuranceResultStorage
{
        #region Fields
        ThreadsafeInlineHistogram _assurance;
        string _type;
        double _standardNonExceedanceProbability;
       
        #endregion

        #region Properties
        public string AssuranceType
        {
            get
            {
                return _type;
            }
        }
        public ThreadsafeInlineHistogram AssuranceHistogram
        {
            get
            {
                return _assurance;
            }
        }
        public double StandardNonExceedanceProbability
        {
            get
            {
                return _standardNonExceedanceProbability;
            }
        }
        #endregion

        #region Constructors
        internal AssuranceResultStorage(string dummyAsuranceType, double standardNonExceedanceProbability)
        {
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
            _assurance = new ThreadsafeInlineHistogram();
            _assurance.SetIterationSize(convergenceCriteria.MaxIterations);
            _type = dummyAsuranceType;
            _standardNonExceedanceProbability = standardNonExceedanceProbability;
        }
        public AssuranceResultStorage(string assuranceType, ConvergenceCriteria convergenceCriteria, double standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee = 0)
        {
            _standardNonExceedanceProbability = standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee;
            _assurance = new ThreadsafeInlineHistogram(convergenceCriteria);
            _assurance.SetIterationSize(convergenceCriteria.MaxIterations);
            _type = assuranceType;
        }
        public AssuranceResultStorage(string assuranceType, double binWidth, ConvergenceCriteria convergenceCriteria, double standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee = 0)
        {
            _standardNonExceedanceProbability = standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee;
            _assurance = new ThreadsafeInlineHistogram(binWidth,convergenceCriteria);
            _assurance.SetIterationSize(convergenceCriteria.MaxIterations);
            _type = assuranceType;
        }
        private AssuranceResultStorage(string assuranceType, double standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee, ThreadsafeInlineHistogram assuranceHistogram)
        {
            _type = assuranceType;
            _standardNonExceedanceProbability = standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee;
            _assurance = assuranceHistogram;
        }
        #endregion

        #region Methods

        public bool Equals(AssuranceResultStorage incomingAssuranceResultStorage)
        {
            if (_type == incomingAssuranceResultStorage.AssuranceType)
            {
                if(_standardNonExceedanceProbability == incomingAssuranceResultStorage.StandardNonExceedanceProbability)
                {
                    if (_assurance.Equals(incomingAssuranceResultStorage.AssuranceHistogram))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("Assurance");
            XElement histogramElement = _assurance.WriteToXML();
            histogramElement.Name = "Histogram";
            masterElement.Add(histogramElement);
            masterElement.SetAttributeValue("Type", _type);
            masterElement.SetAttributeValue("ExceedanceProbability", _standardNonExceedanceProbability);
            return masterElement;
        }

        public static AssuranceResultStorage ReadFromXML(XElement xElement)
        {
            string type = xElement.Attribute("Type").Value;
            double probability = Convert.ToDouble(xElement.Attribute("ExceedanceProbability").Value);
            ThreadsafeInlineHistogram threadsafeInlineHistogram = ThreadsafeInlineHistogram.ReadFromXML(xElement.Element("Histogram"));
            return new AssuranceResultStorage(type, probability, threadsafeInlineHistogram);
        }


        #endregion
    }
}
