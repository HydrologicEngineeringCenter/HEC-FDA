using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;
using Statistics.Distributions;
using Statistics;
using System.Xml.Linq;

namespace StatisticsTests.Distributions
{
    [ExcludeFromCodeCoverage]
    public class SerializationTests
    {
        [Theory]
        [InlineData(2.33, 1d, 1)]
        public void SerializationRoundTrip_Normal(double mean, double sd, int n)
        {
            IDistribution d = new Normal(mean, sd, n);
            XElement ele = d.ToXML();
            IDistribution d2 = ContinuousDistribution.FromXML(ele);
            Assert.True(d.Equals(d2));
        }
        [Theory]
        [InlineData(2.33, 1d, 1d, 4d, 1)]
        public void SerializationRoundTrip_TruncatedNormal(double mean, double sd, double min, double max, int n)
        {
            IDistribution d = new TruncatedNormal(mean, sd, min, max, n);
            XElement ele = d.ToXML();
            IDistribution d2 = ContinuousDistribution.FromXML(ele);
            Assert.True(d.Equals(d2));
        }
        [Theory]
        [InlineData(2.33, 1d, 1)]
        public void SerializationRoundTrip_LogNormal(double mean, double sd, int n)
        {
            IDistribution d = new LogNormal(mean, sd, n);
            XElement ele = d.ToXML();
            IDistribution d2 = ContinuousDistribution.FromXML(ele);
            Assert.True(d.Equals(d2));
        }
        [Theory]
        [InlineData(2.33, 1d, 1d, 4d, 1)]
        public void SerializationRoundTrip_TruncatedLogNormal(double mean, double sd, double min, double max, int n)
        {
            IDistribution d = new TruncatedLogNormal(mean, sd, min, max, n);
            XElement ele = d.ToXML();
            IDistribution d2 = ContinuousDistribution.FromXML(ele);
            Assert.True(d.Equals(d2));
        }
        [Theory]
        [InlineData(2.33, .033d, .022, 1)]
        public void SerializationRoundTrip_LogPearsonIII(double mean, double sd, double skew, int n)
        {
            IDistribution d = new LogPearson3(mean, sd, skew, n);
            XElement ele = d.ToXML();
            IDistribution d2 = ContinuousDistribution.FromXML(ele);
            Assert.True(d.Equals(d2));
        }
        [Theory]
        [InlineData(3.4, 4.5, 1)]
        public void SerializationRoundTrip_Uniform(double min, double max, int n)
        {
            IDistribution d = new Uniform(min, max, n);
            XElement ele = d.ToXML();
            IDistribution d2 = ContinuousDistribution.FromXML(ele);
            Assert.True(d.Equals(d2));
        }
        [Theory]
        [InlineData(3.4, 4.5, 5.6, 1)]
        public void SerializationRoundTrip_Triangular(double min, double mostlikely, double max, int n)
        {
            IDistribution d = new Triangular(min, mostlikely,max ,n);
            XElement element = d.ToXML();
            IDistribution d2 = ContinuousDistribution.FromXML(element);
            Assert.True(d.Equals(d2));
        }
    }
}
