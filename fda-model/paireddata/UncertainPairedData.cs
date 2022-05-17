using interfaces;
using Statistics;
using System;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Enumerations;

namespace paireddata
{
    public class UncertainPairedData : HEC.MVVMFramework.Base.Implementations.Validation, IPairedDataProducer, ICanBeNull, IReportMessage
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
        public event MessageReportedEventHandler MessageReport;

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
            _metadata = new CurveMetaData(xlabel,ylabel,name);
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
            switch (_metadata.CurveType)
            {
                case CurveTypesEnum.StrictlyMonotonicallyIncreasing:
                    AddSinglePropertyRule(nameof(Xvals), new Rule(() => IsArrayValid(Xvals, (a, b) => (a < b)), "X must be strictly monotonically increasing"));
                    AddSinglePropertyRule(nameof(Yvals), new Rule(() => IsDistributionArrayValid(Yvals,.9999, (a, b) => (a < b)), "Y must be strictly monotonically increasing"));
                    AddSinglePropertyRule(nameof(Yvals), new Rule(() => IsDistributionArrayValid(Yvals, .0001, (a, b) => (a < b)), "Y must be strictly monotonically increasing"));
                    break;
                case CurveTypesEnum.MonotonicallyIncreasing:
                    AddSinglePropertyRule(nameof(Xvals), new Rule(() => IsArrayValid(Xvals, (a, b) => (a < b)), "X must be strictly monotonically increasing"));
                    AddSinglePropertyRule(nameof(Yvals), new Rule(() => IsDistributionArrayValid(Yvals, .9999, (a, b) => (a <= b)), "Y must be weakly monotonically increasing"));
                    AddSinglePropertyRule(nameof(Yvals), new Rule(() => IsDistributionArrayValid(Yvals, .0001, (a, b) => (a <= b)), "Y must be weakly monotonically increasing"));
                    break;
                default:
                    break;
            }

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
        private bool IsDistributionArrayValid(IDistribution[] arrayOfData,double prob, Func<double, double, bool> comparison)
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
        public IPairedData SamplePairedData(double probability)
        {
            double[] y = new double[_yvals.Length];
            for (int i = 0; i < _xvals.Length; i++)
            {
                y[i] = _yvals[i].InverseCDF(probability);
            }
            PairedData pairedData = new PairedData(_xvals, y, _metadata);//mutability leakage on xvals
            pairedData.Validate();
            if (pairedData.HasErrors){
                
                if (pairedData.RuleMap[nameof(pairedData.Yvals)].ErrorLevel > ErrorLevel.Unassigned)
                {
                    pairedData.ForceMonotonic();
                    ReportMessage(this, new MessageEventArgs(new Message("Sampled Y Values were not monotonically increasing as required and were forced to be monotonic")));
                }
                if (pairedData.RuleMap[nameof(pairedData.Xvals)].ErrorLevel > ErrorLevel.Unassigned)
                {
                    ReportMessage(this, new MessageEventArgs(new Message("X values are not monotonically decreasing as required")));
                }
                pairedData.Validate();
                if (pairedData.HasErrors)
                {
                    //TODO: do something
                }

                
            }
            return pairedData;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
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
        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("UncertainPairedData");
            XElement curveMetaDataElement = _metadata.WriteToXML();
            curveMetaDataElement.Name = "CurveMetaData";
            masterElement.Add(curveMetaDataElement);
            if(_metadata.IsNull)
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
                        else
                        {
                            yValues[i] = Statistics.ContinuousDistribution.FromXML(ordinateElements);
                        }
                    }
                    i++;
                }
                return new UncertainPairedData(xValues, yValues, curveMetaData);
            }

        }
        #endregion
    }
}