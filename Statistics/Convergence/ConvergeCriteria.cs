using System;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Enumerations;

namespace Statistics
{
    public class ConvergenceCriteria : Validation
    {
        public int MinIterations { get; }
        public int MaxIterations { get; }
        public double ZAlpha { get; }
        public double Tolerance { get; }

        public ConvergenceCriteria(int minIterations = 50000, int maxIterations = 100000, double zAlpha = 1.96039491692543, double tolerance = .01)
        {
            MinIterations = minIterations;
            MaxIterations = maxIterations;
            ZAlpha = zAlpha;
            Tolerance = tolerance;
            AddRules();
        }

        private void AddRules()
        {
            AddSinglePropertyRule(nameof(MaxIterations), new Rule(() => MaxIterations >= MinIterations, "Max iterations must be at least equal to min iterations but they are not."));
            AddSinglePropertyRule(nameof(ZAlpha), new Rule(() => -4 < ZAlpha && ZAlpha < 4, "Z Alpha must be between -4 and 4 but is not."));
            AddSinglePropertyRule(nameof(Tolerance), new Rule(() => 0 < Tolerance && Tolerance < 1, "Tolerance must be between 0 and 1 but is not."));
        }

        public bool Equals(ConvergenceCriteria convergenceCriteria)
        {
            bool minIterationsAreEqual = MinIterations.Equals(convergenceCriteria.MinIterations);
            if(!minIterationsAreEqual)
            { 
                return false; 
            }
            bool maxIterationsAreEqual = MaxIterations.Equals(convergenceCriteria.MaxIterations);
            if(!maxIterationsAreEqual)
            { 
                return false; 
            }
            bool zAlphaAreEqual = ZAlpha.Equals(convergenceCriteria.ZAlpha);
            if (!zAlphaAreEqual)
            { 
                return false; 
            }
            bool toleranceAreEqual = Tolerance.Equals(convergenceCriteria.Tolerance);
            if(!toleranceAreEqual)
            { 
                return false;
            }
           return true; 
        }
        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("Convergence_Criteria");
            masterElement.SetAttributeValue("Minimum_Iterations", MinIterations);
            masterElement.SetAttributeValue("Maximum_Iterations", MaxIterations);
            masterElement.SetAttributeValue("ZAlpha", ZAlpha);
            masterElement.SetAttributeValue("Tolerance", Tolerance);
            return masterElement;
        }

        public static ConvergenceCriteria ReadFromXML(XElement xElement)
        {
            int minIterations = Convert.ToInt32(xElement.Attribute("Minimum_Iterations").Value);
            int maxIterations = Convert.ToInt32(xElement.Attribute("Maximum_Iterations").Value);
            double zAlpha = Convert.ToDouble(xElement.Attribute("ZAlpha").Value);
            double tolerance = Convert.ToDouble(xElement.Attribute("Tolerance").Value);
            return new ConvergenceCriteria(minIterations, maxIterations, zAlpha, tolerance);
        }
    }
}
