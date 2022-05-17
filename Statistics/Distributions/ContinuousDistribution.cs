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
        public int SampleSize { get; protected set; }
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
            ObjectHandle objectHandle = System.Activator.CreateInstance(libraryName, libraryName + ".Distributions." + name);//requires empty constructor
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
                                    valD = Convert.ToDouble(xElement.Attribute(storedAttribute.Name).Value);
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
    }
}
