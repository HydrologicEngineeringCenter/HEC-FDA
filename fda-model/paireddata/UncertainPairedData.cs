using System.Collections.Generic;
using Statistics;
using interfaces;
using System.Linq;
using Utilities;
using Utilities.Serialization;
using System.Xml.Linq;

namespace paireddata
{
    public class UncertainPairedData: IPairedDataProducer, ICategory, ICanBeNull
    {
        private double[] _xvals;
        private IDistribution[] _yvals;
        public string Category {get;}
        public bool IsNull { get; }
        public double[] xs(){
            return _xvals;
        }
        public IDistribution[] ys(){
            return _yvals;
        }
        public UncertainPairedData()
        {
            IsNull = true;
        }
        public UncertainPairedData(double[] xs, IDistribution[] ys){
            _xvals = xs;
            _yvals = ys;
            Category = "Default";
            IsNull = false;
        }
        public UncertainPairedData(double[] xs, IDistribution[] ys, string category){
            _xvals = xs;
            _yvals = ys;
            Category = category;
            IsNull = false;
        }
        public IPairedData SamplePairedData(double probability){
            double[] y = new double[_yvals.Length];
            for (int i=0;i<_xvals.Length; i++){
                y[i] = ys()[i].InverseCDF(probability);
            }
            return new PairedData(xs(), y, Category);
        }

        public static readonly string XML_UNCERTAIN_PAIRED_DATA = "Uncertain Paired Data";
        public static readonly string XML_X = "X Value";
        public static readonly string XML_Y_TYPE = "Y Distribution Type";
        public static readonly string XML_Y_SKEW = "Y Distribution Skewness";

        public XElement WriteToXML()
        {
            XElement masterElem = new XElement(XML_UNCERTAIN_PAIRED_DATA);
            for (int i=0; i<_xvals.Length; i++)
            {
                XElement rowElement = new XElement(SerializationConstants.COORDINATE);
                rowElement.SetAttributeValue(XML_X, _xvals[i]);
                rowElement.SetAttributeValue(XML_Y_TYPE, _yvals[i].Type);
                rowElement.SetAttributeValue(SerializationConstants.MIN, _yvals[i].Range.Min);
                rowElement.SetAttributeValue(SerializationConstants.MAX, _yvals[i].Range.Max);
                rowElement.SetAttributeValue(SerializationConstants.MEAN, _yvals[i].Mean);
                rowElement.SetAttributeValue(SerializationConstants.MODE, _yvals[i].Mode);//technically this should be most likely but the triangle dist constructor accepts mode 
                rowElement.SetAttributeValue(SerializationConstants.ST_DEV, _yvals[i].StandardDeviation);
                rowElement.SetAttributeValue(XML_Y_SKEW, _yvals[i].Skewness);
                rowElement.SetAttributeValue(SerializationConstants.SAMPLE_SIZE, _yvals[i].SampleSize);
                masterElem.Add(rowElement);
            }

            return masterElem;
        }
    }
}