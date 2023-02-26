using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Utilities;

namespace Statistics.Distributions
{
    public class Empirical : ContinuousDistribution
    {
        #region Fields 
        internal bool _Constructed;
        #endregion
        #region EmpiricalProperties
        /// <summary>
        /// Cumulative probabilities are non-exceedance probabilities ONLY
        /// </summary>
        public double[] CumulativeProbabilities;
        public double[] ObservationValues;

        #endregion

        #region IDistributionProperties
        public override IDistributionEnum Type => IDistributionEnum.Empirical;

        public double Mean { get; set; }
        public double Median { get; set; }
        public double StandardDeviation { get; set; }
        public double Variance { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor with all defaults set to 0
        /// </summary>
        public Empirical()
        {
            Mean = 0;
            Median = 0;
            StandardDeviation = 0;
            Variance = 0;
            Min = 0; 
            Max = 0;
            CumulativeProbabilities = new double[] { 0 };
            ObservationValues = new double[] { 0 };
            _Constructed = true;
            
        }

        public Empirical(double[] probabilities, double[] observationValues)
        {
            CumulativeProbabilities = probabilities;
            ObservationValues = observationValues;
            Min = ObservationValues[0];
            Max = ObservationValues[ObservationValues.Length - 1];
            BuildFromProperties();
            addRules();
        }
        public Empirical(double[] probabilities, double[] observationValues, double min, double max)
        {
            CumulativeProbabilities = probabilities;
            ObservationValues = observationValues;
            Min = min;
            Max = max;
            Truncated = true;
            BuildFromProperties();
            addRules();
        }
        public void BuildFromProperties()
        {
            
            SampleSize = ObservationValues.Length;
            Mean = ComputeMean();
            Median = ComputeMedian();
            Variance = ComputeVariance();
            StandardDeviation = Math.Pow(Variance, 0.5);

            //Messages = msgs;
            _Constructed = true;

        }
        private void addRules()
        {
            AddSinglePropertyRule(nameof(SampleSize),
                new Rule(() => {
                    return SampleSize > 0;
                },
                "SampleSize must be greater than 0.",
                ErrorLevel.Fatal));
            //TODO: Add rule to test if not monotonically increasing
            //This should never occur because we only add data in probability steps 
        }
        #endregion

        #region EmpiricalFunctions
        //TODO: Is there anything we need this for? 
        private IRange<double> FiniteRange(double min = double.NegativeInfinity, double max = double.PositiveInfinity)
        {
            double pmin = 0, epsilon = 1 / 1000000000d;
            double pmax = 1 - pmin;
            if (min.IsFinite() || max.IsFinite())//we are not entirely sure how inclusive or works with one sided truncation and the while loop below.
            {
                pmin = CDF(min);
                pmax = CDF(max);
            }
            else
            {
                pmin = .0000001;
                pmax = 1 - pmin;
                min = InverseCDF(pmin);
                max = InverseCDF(pmax);
            }
            while (!(min.IsFinite() && max.IsFinite()))
            {
                pmin += epsilon;
                pmax -= epsilon;
                if (!min.IsFinite()) min = InverseCDF(pmin);
                if (!max.IsFinite()) max = InverseCDF(pmax);
                if (pmin > 0.25)
                    throw new InvalidConstructorArgumentsException($"The Empirical object is not constructable because 50% or more of its distribution returns {double.NegativeInfinity} and {double.PositiveInfinity}.");
            }
            return IRangeFactory.Factory(pmin, pmax);

        }

        private double ComputeMean()
        {
            if (SampleSize == 0)
            {
                return 0.0;
            }
            else if (SampleSize == 1)
            {
                return ObservationValues[0];
            }
            else
            {
                double mean = 0;
                Int64 i;
                double stepPDF, stepVal;
                double valL, valR, cdfL, cdfR;
                // left singleton
                i = 0;
                valR = ObservationValues[i];
                cdfR = CumulativeProbabilities[i];
                stepPDF = cdfR - 0.0;
                mean += valR * stepPDF;
                valL = valR;
                cdfL = cdfR;
                // add interval values
                for (i = 1; i < SampleSize; ++i)
                {
                    valR = ObservationValues[i];
                    cdfR = CumulativeProbabilities[i];
                    stepPDF = cdfR - cdfL;
                    stepVal = (valL + valR) / 2.0;
                    mean += stepPDF * stepVal;
                    valL = valR;
                    cdfL = cdfR;
                }
                // add right singleton 
                i = SampleSize - 1;
                valR = ObservationValues[i];
                cdfR = 1.0;
                stepPDF = cdfR - cdfL;
                mean += valR * stepPDF;
                return mean;
            }
        }

        public double ComputeMedian()
        {
            if (SampleSize == 0)
            {
                throw new ArgumentException("Sample cannot be null");
            }
            else if (SampleSize == 1)
            {
                return ObservationValues[SampleSize - 1];
            }
            else
            {
                if ((SampleSize % 2) == 0)
                {
                    return (ObservationValues[SampleSize / 2] + ObservationValues[SampleSize / 2 - 1]) / 2;
                }
                else
                {
                    return ObservationValues[(SampleSize - 1) / 2];
                }

            }
        }

        public (double, double, double[]) ComputeCumulativeFrequenciesForPlotting()
        {
            double range = Max - Min;
            int bins = 250;
            double valueStep = range / bins;
            double[] cumulativeFrequencies = new double[bins];
            for(int i = 0; i < bins; i++)
            {
                double cumulativeValueStep = Min + valueStep * i;
                double cumulativeProbability = CDF(cumulativeValueStep);
                cumulativeFrequencies[i] = cumulativeProbability;
            }
            return (Min, valueStep, cumulativeFrequencies);
            
        }

        private double ComputeVariance()
        {

            if (SampleSize == 0)
            {
                return 0.0;
            }
            else if (SampleSize == 1)
            {
                return 0.0;
            }
            else
            {
                double mean = Mean;
                double expect2 = 0.0;
                Int64 i;
                double stepPDF, stepVal;
                double valL, valR, cdfL, cdfR;
                // add left singleton 
                i = 0;
                valR = ObservationValues[i];
                cdfR = CumulativeProbabilities[i];
                stepPDF = cdfR - 0.0;
                expect2 += valR * valR * stepPDF;
                valL = valR;
                cdfL = cdfR;
                // add interval values
                for (i = 1; i < SampleSize; i++)
                {
                    valR = ObservationValues[i];
                    cdfR = CumulativeProbabilities[i];
                    stepPDF = cdfR - cdfL;
                    stepVal = (valL * valL + valL * valR + valR * valR) / 3.0;
                    expect2 += stepVal * stepPDF;
                    valL = valR;
                    cdfL = cdfR;
                }
                // add last singleton 
                i = SampleSize - 1;
                valR = ObservationValues[i];
                cdfR = 1.0;
                stepPDF = cdfR - cdfL;
                expect2 += valR * stepPDF;
                return expect2 - mean * mean;
            }
        }

        public bool IsMonotonicallyIncreasing(double[] arrayOfData)
        {

            for (int i = 0; i < arrayOfData.Length - 1; i++)
            {
                if (arrayOfData[i] >= arrayOfData[i + 1])
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
        #region IDistributionFunctions

        public override double CDF(double x)
        {
            int index = Array.BinarySearch(ObservationValues, x);
            if (index >= 0)
            {
                return CumulativeProbabilities[index];
            }
            else
            {
                Int64 size = SampleSize;
                index = -(index + 1);
                if (index == 0)
                {   // first value
                    return 0.0;
                }
                // in between index-1 and index: interpolate
                else if (index < size)
                {
                    double weight = (x - ObservationValues[index - 1]) / (ObservationValues[index] - ObservationValues[index - 1]);
                    return (1 - weight) * CumulativeProbabilities[index - 1] + weight * CumulativeProbabilities[index];
                }
                else
                {   // last value
                    return 1.0;
                }
            }
        }

        public override bool Equals(IDistribution distribution)
        {

            if (distribution.Type == IDistributionEnum.Empirical)
            {
                Empirical distCompared = distribution as Empirical;
                if (ObservationValues == distCompared.ObservationValues && CumulativeProbabilities == distCompared.ObservationValues)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override double InverseCDF(double p)
        {
            if (Truncated && _Constructed)
            {
                p = CumulativeProbabilities.Min() + (p) * (CumulativeProbabilities.Max() - CumulativeProbabilities.Min());
            }
            if (!p.IsFinite())
            {
                throw new ArgumentException($"The value of specified probability parameter: {p} is invalid because it is not on the valid probability range: [0, 1].");
            }
            else if (p <= CumulativeProbabilities.Min())
            {
                return Min;

            }
            else if (p >= CumulativeProbabilities.Max())
            {
                return Max;
            }
            else
            {
                int index = Array.BinarySearch(CumulativeProbabilities, p);
                if (index >= 0)
                {
                    return ObservationValues[index];
                }
                else
                {
                    index = -(index + 1);
                    // in between index-1 and index: interpolate
                    if (index == 0)
                    {   // first value
                        return ObservationValues[0];
                    }
                    else if (index < SampleSize)
                    {
                        double weight = (p - CumulativeProbabilities[index - 1]) / (CumulativeProbabilities[index] - CumulativeProbabilities[index - 1]);
                        return (1.0 - weight) * ObservationValues[index - 1] + weight * ObservationValues[index];
                    }
                    else
                    {   // last value
                        return ObservationValues[SampleSize - 1];
                    }
                }
            }

        }

        public override double PDF(double x)
        {
            int index = ObservationValues.ToList().IndexOf(x);
            if (index >= 0)
            {
                double pdfLeft;
                if (index == 0)
                {   // first value
                    pdfLeft = 0;
                }
                else
                {
                    pdfLeft = (CumulativeProbabilities[index] - CumulativeProbabilities[index - 1]) / (ObservationValues[index] - ObservationValues[index - 1]);
                }
                double pdfRight;
                if (index < SampleSize - 1)
                {
                    pdfRight = (CumulativeProbabilities[index + 1] - CumulativeProbabilities[index]) / (ObservationValues[index + 1] - ObservationValues[index]);
                }
                else
                {   //last value
                    pdfRight = 0;
                }
                double pdfValue = 0.5 * (pdfLeft + pdfRight);
                return pdfValue;

            }
            else
            {
                index = -(index + 1);
                // in between index-1 and index: interpolate
                if (index == 0)
                {   // first value
                    return 0.0;
                }
                else if (index < SampleSize)
                {
                    double pdfValue = (CumulativeProbabilities[index] - CumulativeProbabilities[index - 1]) / (ObservationValues[index] - ObservationValues[index - 1]);
                    return pdfValue;
                }
                else
                {   // last value
                    return 0.0;
                }

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="empiricalDistributionsForStacking"></param>
        /// <param name="addOrSubtract"></param> Either Empirical.Sum or Empirical.Subtract
        /// <returns></returns>
        public static Empirical StackEmpiricalDistributions(List<Empirical> empiricalDistributionsForStacking, Func<double, double, double> addOrSubtract)
        {
            //TODO: Check if list empty 
            int probabilitySteps = 2500;
            double[] cumulativeProbabilities = new double[probabilitySteps];
            double[] stackedInvCDFs = new double[probabilitySteps];
          
          
            for ( int i = 0; i < probabilitySteps; i++)
            {
                double probabilityStep = (0.5 + i) / probabilitySteps;
                double stackedValue = empiricalDistributionsForStacking[0].InverseCDF(probabilityStep);

                for (int j = 1; j < empiricalDistributionsForStacking.Count; j++)
                {
                    stackedValue = addOrSubtract(stackedValue, empiricalDistributionsForStacking[j].InverseCDF(probabilityStep));
                }

                cumulativeProbabilities[i] = probabilityStep;
                stackedInvCDFs[i] = stackedValue;
            }
            return FitToSample(stackedInvCDFs.ToList());
        }

        public static double Sum(double x1, double x2)
        {
            return x1 + x2;
        }
        public static double Subtract(double x1, double x2)
        {
            return x1 - x2;
        }
        public static string Print(double[] observationValues, double[] cumulativeProbabilities)
        {   //refactor this to be something like Head in R
            string returnString = "Empirical Distribution \n Observation Values | Cumulative Probabilities \n";
            for (int i = 0; i < observationValues.Length; i++)
            {
                returnString += $"{observationValues[i]} | {cumulativeProbabilities[i]}";
            }
            return returnString;
        }
        public override string Print(bool round = false) => round ? Print(ObservationValues, CumulativeProbabilities) : $"Empirical(Observation Values: {ObservationValues}, Cumulative Probabilities {CumulativeProbabilities})";

        public override string Requirements(bool printNotes)
        {
            return RequiredParameterization(printNotes);
        }
        public static string RequiredParameterization(bool printNotes = false)
        {
            return $"The empirical distribution requires the following parameterization: {Parameterization()}.";
        }
        internal static string Parameterization()
        {
            return $"Empirical(Observation Values: [{double.MinValue.Print()}, {double.MaxValue.Print()}], Cumulative Probabilities [0,1])";
        }

        public XElement WriteToXML()
        {
            XElement masterElem = new XElement("Empirical Distribution");
            masterElem.SetAttributeValue("Ordinate_Count", SampleSize);
            for (int i = 0; i < SampleSize; i++)
            {
                XElement rowElement = new XElement("Coordinate");
                XElement xRowElement = new XElement("X");
                xRowElement.SetAttributeValue("Value", ObservationValues[i]);
                XElement yRowElement = new XElement("Y");
                yRowElement.SetAttributeValue("Cumulative Probability", CumulativeProbabilities[i]);
                rowElement.Add(xRowElement);
                rowElement.Add(yRowElement);
                masterElem.Add(rowElement);
            }
            return masterElem;
        }
        public static Empirical ReadFromXML(XElement element)
        {
            string sampleSize = element.Attribute("Ordinate_Count").Value;
            int size = Convert.ToInt32(sampleSize);
            double[] observationValues = new double[size];
            double[] cumulativeProbabilities = new double[size];
            int i = 0;
            foreach (XElement coordinateElement in element.Elements())
            {
                foreach (XElement ordinateElements in coordinateElement.Elements())
                {
                    if (ordinateElements.Name.ToString().Equals("X"))
                    {
                        observationValues[i] = Convert.ToDouble(ordinateElements.Attribute("X").Value);
                    }
                    else
                    {
                        cumulativeProbabilities[i] = Convert.ToDouble(ordinateElements.Attribute("Y").Value);
                    }
                }
                i++;
            }
            return new Empirical(cumulativeProbabilities, observationValues);
        }
        public static Empirical FitToSample(List<double> sample)
        {
            int count = sample.Count;
            double[] probs = new double[count];
            double min = Double.MaxValue;
            double max = Double.MinValue;
            sample.Sort();//check if ascending or decending
            double[] sampleArray = new double[sample.Count];
            for (int i = 0; i < sample.Count(); i++)
            {
                if (sample[i] > max) max = sample[i];
                if (sample[i] < min) min = sample[i];
                probs[i] = (double)i / (double)count;
                sampleArray[i] = sample[i];
            }
            return new Empirical(probs, sampleArray, min, max);
        }
        //TODO: I don't think it makes sense for Fit to not be static 
        //Try to change that 
        public override Empirical Fit(double[] sample)
        {
            int count = sample.Length;
            double[] probs = new double[count];
            double min = Double.MaxValue;
            double max = Double.MinValue;
            Array.Sort(sample);//check if ascending or decending
            for (int i = 0; i < sample.Count(); i++)
            {
                if (sample[i] > max) max = sample[i];
                if (sample[i] < min) min = sample[i];
                probs[i] = (double)i / (double)count;
            }
            return new Empirical(probs, sample, min, max);
        }
        #endregion
    }

}