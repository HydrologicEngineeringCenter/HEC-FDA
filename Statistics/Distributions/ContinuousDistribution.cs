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
            XElement ele = new XElement(this.GetType().Name);
            PropertyInfo[] pilist = this.GetType().GetProperties();
            foreach (PropertyInfo pi in pilist)
            {
                Distributions.StoredAttribute sa = (Distributions.StoredAttribute)pi.GetCustomAttribute(typeof(Distributions.StoredAttribute));
                if (sa != null)
                {
                    ele.SetAttributeValue(sa.Name, pi.GetValue(this));
                }
            }
            return ele;
        }
        public static IDistribution FromXML(XElement ele)
        {
            string n = ele.Name.ToString();
            string ns = "Statistics";//this libraries name and the appropriate namespace.
            ObjectHandle oh = System.Activator.CreateInstance(ns, ns + ".Distributions." + n);//requires empty constructor
            IDistribution dist = oh.Unwrap() as IDistribution;
            PropertyInfo[] pilist = dist.GetType().GetProperties();
            foreach (PropertyInfo pi in pilist)
            {
                Distributions.StoredAttribute sa = (Distributions.StoredAttribute)pi.GetCustomAttribute(typeof(Distributions.StoredAttribute));
                if (sa != null)
                {
                    switch (sa.type.Name)
                    {
                        case "double":
                            double vald = 0.0;
                            if (ele.Attribute(sa.Name).Value == "Infinity")
                            {
                                vald = double.PositiveInfinity;
                            }
                            else if (ele.Attribute(sa.Name).Value == "-Infinity")
                            {
                                vald = double.NegativeInfinity;
                            }
                            else
                            {
                                vald = Convert.ToDouble(ele.Attribute(sa.Name).Value);
                            }
                            pi.SetValue(dist, vald);
                            break;
                        case "Double":
                            double valD = 0.0;
                            if (ele.Attribute(sa.Name).Value == "Infinity")
                            {
                                valD = double.PositiveInfinity;
                            }
                            else if (ele.Attribute(sa.Name).Value == "-Infinity")
                            {
                                valD = double.NegativeInfinity;
                            }
                            else
                            {
                                valD = Convert.ToDouble(ele.Attribute(sa.Name).Value);
                            }
                            pi.SetValue(dist, valD);
                            break;
                        case "Boolean":
                            bool valB = Convert.ToBoolean(ele.Attribute(sa.Name).Value);
                            pi.SetValue(dist, valB);
                            break;
                        case "Int32":
                            int vali = Convert.ToInt32(ele.Attribute(sa.Name).Value);
                            pi.SetValue(dist, vali);
                            break;
                        default:
                            throw new ArgumentException("unrecognized type in serialization " + sa.type.Name);
                    }

                }
            }
            return dist;
        }
    }
}
