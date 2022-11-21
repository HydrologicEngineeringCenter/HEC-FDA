using Statistics.Distributions;
using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Xml.Linq;

namespace Statistics
{
    public abstract class ContinuousDistribution : HEC.MVVMFramework.Base.Implementations.Validation, IDistribution
    {   
        public abstract IDistributionEnum Type { get; }
        [Stored(Name = "SampleSize", type = typeof(Int32))]
        public Int64 SampleSize { get; protected set; }
        [Stored(Name = "Truncated", type = typeof(bool))]
        public bool Truncated { get; protected set; }
        public abstract double CDF(double x);
        public abstract bool Equals(IDistribution distribution);
        public abstract double InverseCDF(double p);
        public abstract double PDF(double x);
        public abstract string Print(bool round = false);
        public abstract string Requirements(bool printNotes);
        public abstract IDistribution Fit(double[] data);
        /// <summary>
        /// Generates a parametric bootstrap sample of the distribution.
        /// </summary>
        /// <param name="distribution"></param>
        /// <param name="packetOfRandomNumbers"> Random numbers used to generate the bootstrap sample values, the array length must be equal to or longer than <see cref="IDistribution.SampleSize"/>. </param>
        /// <returns> A new <see cref="IDistribution"/> constructed from a bootstrap sample from the underlying distribution. </returns>
        public IDistribution Sample(double[] packetOfRandomNumbers)
        {

            if (packetOfRandomNumbers.Length < SampleSize) throw new ArgumentException($"The parametric bootstrap sample cannot be constructed using the {Print(true)} distribution. It requires at least {SampleSize} random value but only {packetOfRandomNumbers.Length} were provided.");
            double[] samples = new double[SampleSize];
            for (int i = 0; i < SampleSize; i++) samples[i] = this.InverseCDF(packetOfRandomNumbers[i]);
            return this.Fit(samples);
        }
        public XElement ToXML()
        {
            XElement element = new XElement(this.GetType().Name);
            PropertyInfo[] propertyList = this.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in propertyList)
            {
                Distributions.StoredAttribute storedAttribute = (Distributions.StoredAttribute)propertyInfo.GetCustomAttribute(typeof(Distributions.StoredAttribute));
                if (storedAttribute != null)
                {
                    element.SetAttributeValue(storedAttribute.Name, propertyInfo.GetValue(this));
                }
            }
            return element;
        }
        public static IDistribution FromXML(XElement xElement)
        {           
            string name = xElement.Name.ToString();
            string libraryName = "Statistics";//this libraries name and the appropriate namespace.
            ObjectHandle objectHandle = null;
            if(name.Equals("Histogram"))
            {
                objectHandle = Activator.CreateInstance(libraryName, libraryName + ".Histograms." + name);//requires empty constructor
            }
            else
            {
                objectHandle = Activator.CreateInstance(libraryName, libraryName + ".Distributions." + name);//requires empty constructor
            }
            IDistribution iDistribution = objectHandle.Unwrap() as IDistribution;
  
                PropertyInfo[] propertyList = iDistribution.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in propertyList)
                {
                    Distributions.StoredAttribute storedAttribute = (Distributions.StoredAttribute)propertyInfo.GetCustomAttribute(typeof(Distributions.StoredAttribute));
                    if (storedAttribute != null)
                    {
                        switch (storedAttribute.type.Name)
                        {
                            case "double":
                                double vald = 0.0; //TODO: WHAT DOES VALD MEAN? 
                                if (xElement.Attribute(storedAttribute.Name).Value == "Infinity")
                                {
                                    vald = double.PositiveInfinity;
                                }
                                else if (xElement.Attribute(storedAttribute.Name).Value == "-Infinity")
                                {
                                    vald = double.NegativeInfinity;
                                }
                                else
                                {
                                    vald = Convert.ToDouble(xElement.Attribute(storedAttribute.Name).Value);
                                }
                                propertyInfo.SetValue(iDistribution, vald);
                                break;
                            case "Double":
                                double valD = 0.0;
                                if (xElement.Attribute(storedAttribute.Name).Value == "Infinity")
                                {
                                    valD = double.PositiveInfinity;
                                }
                                else if (xElement.Attribute(storedAttribute.Name).Value == "-Infinity")
                                {
                                    valD = double.NegativeInfinity;
                                }
                                else
                                {
                                    string valDstring = xElement.Attribute(storedAttribute.Name).Value;
                                    valD = Convert.ToDouble(valDstring);
                                }//TODO: WHAT ARE THE VAL D VAL B VAL I IT IS NOT CLEAR 
                                propertyInfo.SetValue(iDistribution, valD);
                                break;
                            case "Boolean":
                                bool valB = Convert.ToBoolean(xElement.Attribute(storedAttribute.Name).Value);
                                propertyInfo.SetValue(iDistribution, valB);
                                break;
                            case "Int32":
                                int vali = Convert.ToInt32(xElement.Attribute(storedAttribute.Name).Value);
                                propertyInfo.SetValue(iDistribution, vali);
                                break;
                            default:
                                throw new ArgumentException("unrecognized type in serialization " + storedAttribute.type.Name);
                        }

                    }
                }
            return iDistribution;
  
        }
        public Tuple<double[], double[]> ToCoordinates()
        {
            double[] _RequiredExceedanceProbabilities = { 0.99900, 0.99000, 0.95000, 0.90000, 0.85000, 0.80000, 0.75000, 0.70000, 0.65000, 0.60000, 0.55000, 0.50000, 0.47500, 0.45000, 0.42500, 0.40000, 0.37500, 0.35000, 0.32500, 0.30000, 0.29000, 0.28000, 0.27000, 0.26000, 0.25000, 0.24000, 0.23000, 0.22000, 0.21000, 0.20000, 0.19500, 0.19000, 0.18500, 0.18000, 0.17500, 0.17000, 0.16500, 0.16000, 0.15500, 0.15000, 0.14500, 0.14000, 0.13500, 0.13000, 0.12500, 0.12000, 0.11500, 0.11000, 0.10500, 0.10000, 0.09500, 0.09000, 0.08500, 0.08000, 0.07500, 0.07000, 0.06500, 0.06000, 0.05900, 0.05800, 0.05700, 0.05600, 0.05500, 0.05400, 0.05300, 0.05200, 0.05100, 0.05000, 0.04900, 0.04800, 0.04700, 0.04600, 0.04500, 0.04400, 0.04300, 0.04200, 0.04100, 0.04000, 0.03900, 0.03800, 0.03700, 0.03600, 0.03500, 0.03400, 0.03300, 0.03200, 0.03100, 0.03000, 0.02900, 0.02800, 0.02700, 0.02600, 0.02500, 0.02400, 0.02300, 0.02200, 0.02100, 0.02000, 0.01950, 0.01900, 0.01850, 0.01800, 0.01750, 0.01700, 0.01650, 0.01600, 0.01550, 0.01500, 0.01450, 0.01400, 0.01350, 0.01300, 0.01250, 0.01200, 0.01150, 0.01100, 0.01050, 0.01000, 0.00950, 0.00900, 0.00850, 0.00800, 0.00750, 0.00700, 0.00650, 0.00600, 0.00550, 0.00500, 0.00490, 0.00450, 0.00400, 0.00350, 0.00300, 0.00250, 0.00200, 0.00195, 0.00190, 0.00185, 0.00180, 0.00175, 0.00170, 0.00165, 0.00160, 0.00155, 0.00150, 0.00145, 0.00140, 0.00135, 0.00130, 0.00125, 0.00120, 0.00115, 0.00110, 0.00105, 0.00100, 0.00095, 0.00090, 0.00085, 0.00080, 0.00075, 0.00070, 0.00065, 0.00060, 0.00055, 0.00050, 0.00045, 0.00040, 0.00035, 0.00030, 0.00025, 0.00020, 0.00015, 0.00010 };
        
            double[] x = new double[_RequiredExceedanceProbabilities.Length];
            double[] y = new double[_RequiredExceedanceProbabilities.Length];
            for (int i = 0; i < _RequiredExceedanceProbabilities.Length; i++)
            {
                //same exceedance probs as graphical and as 1.4.3
                double prob = 1 - _RequiredExceedanceProbabilities[i];
                x[i] = prob;

                //y values in increasing order 
                y[i] = InverseCDF(prob);
            }
            Tuple<double[],double[]> rtn = new Tuple<double[], double[]>(x,y);
            return rtn;

        }
    }
}
