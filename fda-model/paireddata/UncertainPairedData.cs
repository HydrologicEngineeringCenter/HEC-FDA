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
        #region Fields 
        private double[] _xvals;
        private IDistribution[] _yvals;
        #endregion

        #region Properties 
        public string XLabel { get; }
        public string YLabel { get; }
        public string Name { get; }
        public string Description { get; }
        public int ID { get; }
        public string Category {get;}
        public bool IsNull { get; }
        public double[] xs(){
            return _xvals;
        }
        public IDistribution[] ys(){
            return _yvals;
        }
        #endregion

        #region Constructors 
        public UncertainPairedData()
        {
            IsNull = true;
        }
        //, string xlabel, string ylabel, string name, string description, int ID
        public UncertainPairedData(double[] xs, IDistribution[] ys, string xlabel, string ylabel, string name, string description, int id)
        {
            _xvals = xs;
            _yvals = ys;
            Category = "Default";
            IsNull = false;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            Description = description;
            ID = id;
        }
        public UncertainPairedData(double[] xs, IDistribution[] ys, string xlabel, string ylabel, string name, string description, int id, string category){
            _xvals = xs;
            _yvals = ys;
            Category = category;
            IsNull = false;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            Description = description;
            ID = id;
        }
        #endregion

        #region Methods 
        public IPairedData SamplePairedData(double probability){
            double[] y = new double[_yvals.Length];
            for (int i=0;i<_xvals.Length; i++){
                y[i] = ys()[i].InverseCDF(probability);
            }
            return new PairedData(xs(), y, Category);
        }

        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("UncertainPairedData");
            masterElement.SetAttributeValue("Category", Category);
            masterElement.SetAttributeValue("XLabel", XLabel);
            masterElement.SetAttributeValue("YLabel", YLabel);
            masterElement.SetAttributeValue("Name", Name);
            masterElement.SetAttributeValue("Description", Description);
            masterElement.SetAttributeValue("ID", ID);
            masterElement.SetAttributeValue("Ordinate_Count", _xvals.Length);
            for (int i=0; i<_xvals.Length; i++)
            {
                XElement rowElement = new XElement("Coordinate");
                XElement xElement = new XElement("X");
                xElement.SetAttributeValue("Value", _xvals[i]);
                XElement yElement = _yvals[i].ToXML();
                rowElement.Add(xElement);
                rowElement.Add(yElement);
                masterElement.Add(rowElement);
            }
            return masterElement;
        }

        public static UncertainPairedData ReadFromXML(XElement element)
        {
            string category = element.Attribute("Category").Value;
            string xLabel = element.Attribute("XLabel").Value;
            string yLabel = element.Attribute("YLabel").Value;
            string name = element.Attribute("Name").Value;
            string description = element.Attribute("Description").Value;
            int id = Convert.ToInt32(element.Attribute("ID").Value);
            int size = Convert.ToInt32(element.Attribute("Ordinate_Count").Value);
            double[] xValues = new double[size];
            IDistribution[] yValues = new IDistribution[size];
            int i = 0;
            foreach(XElement coordinateElement in element.Elements())
            {
                foreach(XElement ordinateElements in coordinateElement.Elements())
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
            return new UncertainPairedData(xValues,yValues,xLabel,yLabel,name,description,id,category);
        }
        #endregion
    }
}