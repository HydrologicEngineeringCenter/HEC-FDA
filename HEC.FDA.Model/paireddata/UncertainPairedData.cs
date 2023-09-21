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
        #region Fields 
        private double[] _xvals;
        private IDistribution[] _yvals;
        private CurveMetaData _metadata;
        #endregion

        #region Properties 
        public string XLabel
        {
            get { return _metadata.XLabel; }
        }

        public string YLabel
        {
            get { return _metadata.YLabel; }
        }

        public string Name
        {
            get { return _metadata.Name; }
        }

        public string DamageCategory
        {
            get { return _metadata.DamageCategory; }
        }
        public string AssetCategory
        {
            get { return _metadata.AssetCategory; }
        }
        public int ImpactAreaID
        {
            get { return _metadata.ImpactAreaID; }
        }
        public bool IsNull
        {
            get { return _metadata.IsNull; }
        }
        public CurveMetaData CurveMetaData
        {
            get
            {
                return _metadata;
            }
        }
        public double[] Xvals
        {
            get { return _xvals; }
        }
        public IDistribution[] Yvals
        {
            get { return _yvals; }
        }

        #endregion

        #region Constructors 
        public UncertainPairedData()
        {
            _metadata = new CurveMetaData();
            AddRules();
        }
        [Obsolete("This constructor is deprecated. Construct a CurveMetaData, then inject into constructor")]
        public UncertainPairedData(double[] xs, IDistribution[] ys, string xlabel, string ylabel, string name)
        {
            _xvals = xs;
            _yvals = ys;
            _metadata = new CurveMetaData(xlabel, ylabel, name);
            AddRules();
        }
        [Obsolete("This constructor is deprecated. Construct a CurveMetaData, then inject into constructor")]
        public UncertainPairedData(double[] xs, IDistribution[] ys, string xlabel, string ylabel, string name, string category)
        {
            _xvals = xs;
            _yvals = ys;
            _metadata = new CurveMetaData(xlabel, ylabel, name, category);
            AddRules();
        }
        public UncertainPairedData(double[] xs, IDistribution[] ys, CurveMetaData metadata)
        {
            _xvals = xs;
            _yvals = ys;
            _metadata = metadata;
            AddRules();
        }
        #endregion

        #region Methods 
        private void AddRules()
        {
                    AddSinglePropertyRule(nameof(Xvals), new Rule(() => !(IsArrayValid(Xvals, (a, b) => a == b) || IsArrayValid(Xvals, (a, b) => a < b)), $"X must be deterministic or strictly monotonically increasing but is not for the function named {_metadata.Name}.", ErrorLevel.Minor));
                    AddSinglePropertyRule(nameof(Yvals), new Rule(() => IsDistributionArrayValid(Yvals, .9999, (a, b) => a == b) || IsDistributionArrayValid(Yvals, .9999, (a, b) => a <= b), $"Y must be deterministic or weakly monotonically increasing but is not for the function named {_metadata.Name} at the upper bound.", ErrorLevel.Minor));
                    AddSinglePropertyRule(nameof(Yvals), new Rule(() => IsDistributionArrayValid(Yvals, .0001, (a, b) => a == b) || IsDistributionArrayValid(Yvals, .0001, (a, b) => a <= b), $"Y must be deterministic or weakly monotonically increasing but is not for the function named {_metadata.Name} at the lower found.", ErrorLevel.Minor));
        }
        private bool IsArrayValid(double[] arrayOfData, Func<double, double, bool> comparison)
        {
            if (arrayOfData == null) return false;
            for (int i = 0; i < arrayOfData.Length - 1; i++)
            {
                if (comparison(arrayOfData[i], arrayOfData[i + 1]))
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsDistributionArrayValid(IDistribution[] arrayOfData, double prob, Func<double, double, bool> comparison)
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
        public PairedData SamplePairedData(double probability, bool retrieveDeterministicRepresentation = false)
        {
            double[] y = new double[_yvals.Length];
            if (retrieveDeterministicRepresentation)
            {
                for (int i = 0; i < _xvals.Length; i++)
                {
                    Deterministic deterministic = UncertainToDeterministicDistributionConverter.ConvertDistributionToDeterministic(_yvals[i]);
                    y[i] = (deterministic.Value);
                }

            } else
            {
                for (int i = 0; i < _xvals.Length; i++)
                {
                    y[i] = _yvals[i].InverseCDF(probability);
                }

            }
            PairedData pairedData = new PairedData(_xvals, y, _metadata);//mutability leakage on xvals
            pairedData.ForceMonotonicity();
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
            for (int i = 0; i < _xvals.Length; i++)
            {
                bool probabilityIsTheSame = _xvals[i].Equals(incomingUncertainPairedData._xvals[i]);
                bool distributionIsTheSame = _yvals[i].Equals(incomingUncertainPairedData._yvals[i]);
                if (!probabilityIsTheSame || !distributionIsTheSame)
                {
                    return false;
                }
            }
            return true;
        }
        public static IPairedData ConvertToPairedDataAtMeans(UncertainPairedData uncertainPairedData)
        {
            UncertainPairedData intermediatePairedData = ConvertToDeterministic(uncertainPairedData);
            double medianProb = 0.5;
            return intermediatePairedData.SamplePairedData(medianProb);
        }
        public static UncertainPairedData ConvertToDeterministic(UncertainPairedData uncertainPairedData)
        {
            Deterministic[] deterministicDistributions = new Deterministic[uncertainPairedData.Xvals.Length];
            int i = 0;
            foreach (IDistribution distribution in uncertainPairedData.Yvals)
            {
                deterministicDistributions[i] = UncertainToDeterministicDistributionConverter.ConvertDistributionToDeterministic(uncertainPairedData.Yvals[i]);
                i++;
            }
            UncertainPairedData deterministicUncertainPairedData = new UncertainPairedData(uncertainPairedData.Xvals, deterministicDistributions, uncertainPairedData.CurveMetaData);
            return deterministicUncertainPairedData;
        }
        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("UncertainPairedData");
            XElement curveMetaDataElement = _metadata.WriteToXML();
            curveMetaDataElement.Name = "CurveMetaData";
            masterElement.Add(curveMetaDataElement);
            if (_metadata.IsNull)
            {
                return masterElement;
            }
            else
            {
                masterElement.SetAttributeValue("Ordinate_Count", _xvals.Length);
                XElement coordinatesElement = new XElement("Coordinates");
                for (int i = 0; i < _xvals.Length; i++)
                {
                    XElement coordinateElement = new XElement("Coordinate");
                    XElement xElement = new XElement("X");
                    xElement.SetAttributeValue("Value", _xvals[i]);
                    XElement yElement = _yvals[i].ToXML();
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
                            yValues[i] = Histogram.ReadFromXML(ordinateElements);
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

        public static List<string> ConvertToText(List<UncertainPairedData> quantityDamagedElementsUPD)
        {
            List<string> list = new();
            string header = "Impact Area Row Number," +
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
                List<string> quantilesToText = QuantilesToText(upd);
                list.AddRange(quantilesToText);
            }

            return list;
        }

        private static List<string> QuantilesToText(UncertainPairedData upd)
        {
            List<string> returnStrings = new();
            for (int i = 0; i < upd.Xvals.Length; i++)
            {
                string thisXValData = $"{upd.ImpactAreaID}," +
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