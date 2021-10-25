using System.Collections.Generic;
using Statistics;
using interfaces;
using System.Linq;
using System.Xml.Linq;
using System;

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

        public XElement WriteToXML()
        {
            XElement masterElem = new XElement("UncertainPairedData");
            masterElem.SetAttributeValue("Category", Category);
            masterElem.SetAttributeValue("Ordinate_Count", _xvals.Length);
            for (int i=0; i<_xvals.Length; i++)
            {
                XElement rowele = new XElement("Coordinate");
                XElement xele = new XElement("X");
                xele.SetAttributeValue("Value", _xvals[i]);
                XElement yele = _yvals[i].ToXML();
                rowele.Add(xele);
                rowele.Add(yele);
                masterElem.Add(rowele);
            }
            return masterElem;
        }

        public static UncertainPairedData ReadFromXML(XElement ele)
        {
            string cat = ele.Attribute("Category").Value;
            int size = Convert.ToInt32(ele.Attribute("Ordinate_Count").Value);
            double[] xvals = new double[size];
            IDistribution[] yvals = new IDistribution[size];
            int i = 0;
            foreach(XElement coordele in ele.Elements())
            {
                foreach(XElement ordeles in coordele.Elements())
                {
                    if (ordeles.Name.ToString().Equals("X"))
                    {
                        xvals[i] = Convert.ToDouble(ordeles.Attribute("Value").Value);
                    }
                    else
                    {
                        yvals[i] = Statistics.IDistributionExtensions.FromXML(ordeles);
                    }
                }
                i++;
            }
            return new UncertainPairedData(xvals,yvals,cat);
        }
    }
}