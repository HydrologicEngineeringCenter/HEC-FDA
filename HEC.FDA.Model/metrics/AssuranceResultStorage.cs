﻿using System;
using Statistics.Histograms;
using Statistics;
using System.Xml.Linq;

namespace HEC.FDA.Model.metrics;

public class AssuranceResultStorage
{
    #region Fields
    private readonly double[] _TempResults;
    #endregion

    #region Properties
    public string AssuranceType { get; }
    public DynamicHistogram AssuranceHistogram { get; }
    public double StandardNonExceedanceProbability { get; }
    #endregion

    #region Constructors
    internal AssuranceResultStorage(string dummyAsuranceType, double standardNonExceedanceProbability)
    {
        _TempResults = Array.Empty<double>();
        AssuranceHistogram = new DynamicHistogram();
        AssuranceType = dummyAsuranceType;
        StandardNonExceedanceProbability = standardNonExceedanceProbability;
    }
    public AssuranceResultStorage(string assuranceType, double binWidth, ConvergenceCriteria convergenceCriteria, double standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee = 0)
    {
        StandardNonExceedanceProbability = standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee;
        _TempResults = new double[convergenceCriteria.IterationCount];
        AssuranceHistogram = new DynamicHistogram(binWidth, convergenceCriteria);
        AssuranceType = assuranceType;
    }
    private AssuranceResultStorage(string assuranceType, double standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee, DynamicHistogram assuranceHistogram)
    {
        AssuranceType = assuranceType;
        StandardNonExceedanceProbability = standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee;
        AssuranceHistogram = assuranceHistogram;
        _TempResults = new double[assuranceHistogram.ConvergenceCriteria.IterationCount];
    }
    #endregion

    #region Methods

    public bool Equals(AssuranceResultStorage incomingAssuranceResultStorage)
    {
        if (AssuranceType == incomingAssuranceResultStorage.AssuranceType)
        {
            if (StandardNonExceedanceProbability == incomingAssuranceResultStorage.StandardNonExceedanceProbability)
            {
                if (AssuranceHistogram.Equals(incomingAssuranceResultStorage.AssuranceHistogram))
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
        AssuranceHistogram.AddObservationsToHistogram(_TempResults);
        Array.Clear(_TempResults);
    }

    public XElement WriteToXML()
    {
        XElement masterElement = new("Assurance");
        XElement histogramElement = AssuranceHistogram.ToXML();
        histogramElement.Name = "Histogram";
        masterElement.Add(histogramElement);
        masterElement.SetAttributeValue("Type", AssuranceType);
        masterElement.SetAttributeValue("ExceedanceProbability", StandardNonExceedanceProbability);
        return masterElement;
    }

    public static AssuranceResultStorage ReadFromXML(XElement xElement)
    {
            string type = xElement.Attribute("Type")?.Value;
            double probability = Convert.ToDouble(xElement.Attribute("ExceedanceProbability").Value);
            DynamicHistogram inlineHistogram = DynamicHistogram.ReadFromXML(xElement.Element("Histogram"));
            return new AssuranceResultStorage(type, probability, inlineHistogram);
    }

    
    #endregion
}
