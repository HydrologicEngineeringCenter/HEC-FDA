using Statistics;
using System;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Enumerations;
using Statistics.Distributions;
using HEC.FDA.Model.interfaces;
using Statistics.Histograms;
using HEC.MVVMFramework.Model.Messaging;
using System.Collections.Generic;

namespace HEC.FDA.Model.paireddata
{
    public class UncertainPairedData : ValidationErrorLogger, IPairedDataProducer, ICanBeNull
    {
        private double[] _RandomNumbers;
        public string XLabel
        {
            get { return CurveMetaData.XLabel; }
        }

        public string YLabel
        {
            get { return CurveMetaData.YLabel; }
        }

        public string Name
        {
            get { return CurveMetaData.Name; }
        }

        public string DamageCategory
        {
            get { return CurveMetaData.DamageCategory; }
        }
        public string AssetCategory
        {
            get { return CurveMetaData.AssetCategory; }
        }
        public int ImpactAreaID
        {
            get { return CurveMetaData.ImpactAreaID; }
        }
        public bool IsNull
        {
            get { return CurveMetaData.IsNull; }
        }
        public CurveMetaData CurveMetaData { get; }
        public double[] Xvals { get; }
        public IDistribution[] Yvals { get; }

        public UncertainPairedData()
        {
            CurveMetaData = new CurveMetaData();
            AddRules();
        }

        public UncertainPairedData(double[] xs, IDistribution[] ys, CurveMetaData metadata)
        {
            Xvals = xs;
            Yvals = ys;
            CurveMetaData = metadata;
            AddRules();
        }

        #region Methods 
        public void GenerateRandomNumbers(int seed, long size)
        {
            Random random = new Random(seed);
            double[] randos = new double[size];
            for (int i = 0; i < size; i++)
            {
                randos[i] = random.NextDouble();
            }
            _RandomNumbers = randos;
        }

        private void AddRules()
        {
                    AddSinglePropertyRule(nameof(Xvals), new Rule(() => (IsArrayValid(Xvals, (a, b) => a == b) || IsArrayValid(Xvals, (a, b) => a < b)), $"X must be deterministic or strictly monotonically increasing but is not for the function named {CurveMetaData.Name}.", ErrorLevel.Minor));
                    AddSinglePropertyRule(nameof(Yvals), new Rule(() => IsDistributionArrayValid(Yvals, .9999, (a, b) => a == b) || IsDistributionArrayValid(Yvals, .9999, (a, b) => a <= b), $"Y must be deterministic or weakly monotonically increasing but is not for the function named {CurveMetaData.Name} at the upper bound.", ErrorLevel.Minor));
                    AddSinglePropertyRule(nameof(Yvals), new Rule(() => IsDistributionArrayValid(Yvals, .0001, (a, b) => a == b) || IsDistributionArrayValid(Yvals, .0001, (a, b) => a <= b), $"Y must be deterministic or weakly monotonically increasing but is not for the function named {CurveMetaData.Name} at the lower found.", ErrorLevel.Minor));
        }
        private static bool IsArrayValid(double[] arrayOfData, Func<double, double, bool> comparison)
        {
            if (arrayOfData == null)
            {
                return false;
            }
            for (int i = 0; i < arrayOfData.Length - 1; i++)
            {
                if (comparison(arrayOfData[i], arrayOfData[i + 1]))
                {
                    return false;
                }
            }
            return true;
        }
        private static bool IsDistributionArrayValid(IDistribution[] arrayOfData, double prob, Func<double, double, bool> comparison)
        {
            if (arrayOfData == null) return false;
            for (int i = 0; i < arrayOfData.Length - 1; i++)
            {
                if (comparison(arrayOfData[i].InverseCDF(prob), arrayOfData[i + 1].InverseCDF(prob)))
                {
                    return false;
                }
            }
            return true;
        }

        public PairedData SamplePairedData(double probability)
        {
            double[] y = new double[Yvals.Length];
                for (int i = 0; i < Xvals.Length; i++)
                {
                    y[i] = Yvals[i].InverseCDF(probability);
                }
            PairedData pairedData = new(Xvals, y, CurveMetaData);//mutability leakage on xvals
            pairedData.ForceWeakMonotonicityBottomUp();
            return pairedData;
        }
        /// <summary>
        /// All sampling methods include a computeIsDeterministic argument that bypasses the iteration number for the retrieval of the deterministic representation of the variable 
        /// </summary>
        /// <param name="iterationNumber"></param> If this method is called during a full compute with uncertainty, random numbers need to have been previously generated, and the correct random number will be pulled for the given iteration number
        /// <param name="retrieveDeterministicRepresentation"></param> If the method is instead called during a deterministic compute, the iteration number will be bypassed and the deterministic representation will be returned
        /// <returns></returns> the x vals are returned alongside the sampled y vales for the random number that corresponds to the iteration index or the deterministic representation of the y vals 
        /// <exception cref="Exception"></exception>
        public PairedData SamplePairedData(long iterationNumber, bool retrieveDeterministicRepresentation)
        {
            double[] y = new double[Yvals.Length];
            if (retrieveDeterministicRepresentation)
            {
                for (int i = 0; i < Xvals.Length; i++)
                {
                    Deterministic deterministic = UncertainToDeterministicDistributionConverter.ConvertDistributionToDeterministic(Yvals[i]);
                    y[i] = (deterministic.Value);
                }
            }
            else
            {

            if (_RandomNumbers.Length ==0)
            {
                throw new Exception("Random numbers have not been created for UPD sampling");
            }
            if (iterationNumber < 0 || iterationNumber >= _RandomNumbers.Length)
            {
                throw new Exception("Iteration number cannot be less than 0 or greater than the size of the random number array");

            }
                for (int i = 0; i < Xvals.Length; i++)
                {
                    y[i] = Yvals[i].InverseCDF(_RandomNumbers[iterationNumber]);
                }
            }
            PairedData pairedData = new(Xvals, y, CurveMetaData);//mutability leakage on xvals
            pairedData.ForceWeakMonotonicityBottomUp();
            return pairedData;
        }

        public bool Equals(UncertainPairedData incomingUncertainPairedData)
        {
            bool nullMatches = CurveMetaData.IsNull.Equals(incomingUncertainPairedData.CurveMetaData.IsNull);
            if (nullMatches && IsNull)
            {
                return true;
            }
            bool nameIsTheSame = Name.Equals(incomingUncertainPairedData.Name);
            if (!nameIsTheSame)
            {
                return false;
            }
            for (int i = 0; i < Xvals.Length; i++)
            {
                bool probabilityIsTheSame = Xvals[i].Equals(incomingUncertainPairedData.Xvals[i]);
                bool distributionIsTheSame = Yvals[i].Equals(incomingUncertainPairedData.Yvals[i]);
                if (!probabilityIsTheSame || !distributionIsTheSame)
                {
                    return false;
                }
            }
            return true;
        }

        public XElement WriteToXML()
        {
            XElement masterElement = new("UncertainPairedData");
            XElement curveMetaDataElement = CurveMetaData.WriteToXML();
            curveMetaDataElement.Name = "CurveMetaData";
            masterElement.Add(curveMetaDataElement);
            if (CurveMetaData.IsNull)
            {
                return masterElement;
            }
            else
            {
                masterElement.SetAttributeValue("Ordinate_Count", Xvals.Length);
                XElement coordinatesElement = new("Coordinates");
                for (int i = 0; i < Xvals.Length; i++)
                {
                    XElement coordinateElement = new("Coordinate");
                    XElement xElement = new("X");
                    xElement.SetAttributeValue("Value", Xvals[i]);
                    XElement yElement = Yvals[i].ToXML();
                    coordinateElement.Add(xElement);
                    coordinateElement.Add(yElement);
                    coordinatesElement.Add(coordinateElement);
                }
                masterElement.Add(coordinatesElement);
                return masterElement;
            }

        }

        public static UncertainPairedData ReadFromXML(XElement element)
        {
            CurveMetaData curveMetaData = CurveMetaData.ReadFromXML(element.Element("CurveMetaData"));
            if (curveMetaData.IsNull)
            {
                return new UncertainPairedData();
            }
            else
            {
                int size = Convert.ToInt32(element.Attribute("Ordinate_Count").Value);
                double[] xValues = new double[size];
                IDistribution[] yValues = new IDistribution[size];
                int i = 0;
                foreach (XElement coordinateElement in element.Element("Coordinates").Elements())
                {
                    foreach (XElement ordinateElements in coordinateElement.Elements())
                    {
                        if (ordinateElements.Name.ToString().Equals("X"))
                        {
                            xValues[i] = Convert.ToDouble(ordinateElements.Attribute("Value").Value);
                        }
                        else if (ordinateElements.Name.ToString().Equals("ThreadsafeInlineHistogram"))
                        {
                            yValues[i] = ThreadsafeInlineHistogram.ReadFromXML(ordinateElements);
                        }
                        else if (ordinateElements.Name.ToString().Equals("Histogram"))
                        {
                            yValues[i] = DynamicHistogram.ReadFromXML(ordinateElements);
                        }
                        else
                        {
                            yValues[i] = ContinuousDistribution.FromXML(ordinateElements);
                        }
                    }
                    i++;
                }
                return new UncertainPairedData(xValues, yValues, curveMetaData);
            }

        }

        public static List<string> ConvertDamagedElementCountToText(List<UncertainPairedData> quantityDamagedElementsUPD, Dictionary<int,string> iaNames)
        {
            List<string> list = new();
            string header = "Impact Area Name," +
                " Damage Category," +
                " Asset Category," +
                " Stage," +
                " Damaged Element Count 0.95 AEP," +
                " Damaged Element Count 0.75 AEP," +
                " Damaged Element Count 0.5 AEP," +
                " Damaged Element Count 0.25 AEP," +
                " Damaged Element Count 0.05 AEP, " +
                "Damaged Element Count 0.01 AEP, " +
                "Damaged Element Count 0.002 AEP";
            list.Add(header);

            foreach(UncertainPairedData upd in quantityDamagedElementsUPD)
            {
                List<string> quantilesToText = QuantilesToText(upd, iaNames);
                list.AddRange(quantilesToText);
            }

            return list;
        }

        private static List<string> QuantilesToText(UncertainPairedData upd, Dictionary<int, string> iaNames)
        {
            List<string> returnStrings = new();
            for (int i = 0; i < upd.Xvals.Length; i++)
            {
                string thisXValData = $"{iaNames[upd.ImpactAreaID]}," +
                $"{upd.DamageCategory}," +
                $"{upd.AssetCategory}," +
                $"{upd.Xvals[i]}," +
                $"{Math.Ceiling(upd.Yvals[i].InverseCDF(1-0.95))}," +
                $"{Math.Ceiling(upd.Yvals[i].InverseCDF(1 - 0.5))}," +
                $"{Math.Ceiling(upd.Yvals[i].InverseCDF(1 - 0.5))}," +
                $"{Math.Ceiling(upd.Yvals[i].InverseCDF(1 - 0.25))}," +
                $"{Math.Ceiling(upd.Yvals[i].InverseCDF(1 - 0.05))}," +
                $"{Math.Ceiling(upd.Yvals[i].InverseCDF(1 - 0.01))}," +
                $"{Math.Ceiling(upd.Yvals[i].InverseCDF(1 - 0.002))},";
                returnStrings.Add(thisXValData);
            }
            return returnStrings;
        }
        #endregion
    }
}