using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using System.Xml.Linq;

namespace Statistics
{
    public class ConvergenceCriteria
    {
        public Int64 MinIterations { get; }
        public Int64 MaxIterations { get; }
        public double ZAlpha { get; }
        public double Tolerance { get; }

        public ConvergenceCriteria(Int64 minIterations = 100, Int64 maxIterations = 100000, double zAlpha = 1.96039491692543, double tolerance = .01)
        {
            MinIterations = minIterations;
            MaxIterations = maxIterations;
            ZAlpha = zAlpha;
            Tolerance = tolerance;
        }
        public bool Equals(ConvergenceCriteria convergenceCriteria)
        {
            if(MinIterations != convergenceCriteria.MinIterations) { return false; }
            else if(MaxIterations != convergenceCriteria.MaxIterations) { return false; }
            else if (ZAlpha != convergenceCriteria.ZAlpha) { return false; }
            else if(Tolerance != convergenceCriteria.Tolerance) { return false; }
            else { return true; }
        }
        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("Convergence Criteria");
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
