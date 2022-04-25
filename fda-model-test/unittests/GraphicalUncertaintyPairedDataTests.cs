using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using paireddata;
using Statistics.Distributions;
using System.Xml.Linq;

namespace fda_model_test.unittests
{
    [Trait("Category", "Unit")]
    public class GraphicalUncertaintyPairedDataTests
    {
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string category = "residential";
        static CurveTypesEnum curveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
        static CurveMetaData curveMetaData = new CurveMetaData(xLabel, yLabel, name, category, curveType);

        [Theory]
        [InlineData(new double[] { .99, .5, .1, .02, .01, .002 }, new double[] { 500, 2000, 34900, 66900, 86000, 146000 }, 5)] //Based on Elkhorn River at Highway 91 Dodge County FIS 2008
        [InlineData(new double[] { .99, .95, .90, .85, .8, .75, .7, .65, .6, .55, .5, .45, .4, .35, .3, .25, .2, .15, .1, .05, .02, .01, .005, .0025 },
       new double[] { 6.6, 7.4, 8.55, 9.95, 11.5, 12.7, 13.85, 14.7, 15.8, 16.7, 17.5, 18.25, 19, 19.7, 20.3, 21.1, 21.95, 23, 24.2, 25.7, 27.4, 28.4, 29.1, 29.4 },
       20)]
        public void ReturnsDistributionsWhereMeanAndConfidenceLimitsAreMonotonicallyIncreasing(double[] probs, double[] flows, int erl)
        {
            GraphicalUncertainPairedData graphical = new GraphicalUncertainPairedData(probs, flows, erl, curveMetaData);
            List<IPairedData> pairedDataList = new List<IPairedData>();
            IPairedData lowerPairedData = graphical.SamplePairedData(0.05);
            pairedDataList.Add(lowerPairedData);
            IPairedData upperPairedData = graphical.SamplePairedData(0.95);
            pairedDataList.Add(upperPairedData);
            bool pass = true;
            foreach (IPairedData pairedData in pairedDataList)
            {
                for (int j = 1; j < pairedData.Xvals.Length; j++)
                {
                    if (pairedData.Yvals[j] < pairedData.Yvals[j-1])
                    {
                        pass = false;
                        break;
                    }
                }
                Assert.True(pass);
            }
           
        }

        [Theory]
        [InlineData(new double[] { .99, .5, .1, .02, .01, .002 }, new double[] { 500, 2000, 34900, 66900, 86000, 146000 }, 5)] //Based on Elkhorn River at Highway 91 Dodge County FIS 2008
        [InlineData(new double[] { .99, .95, .90, .85, .8, .75, .7, .65, .6, .55, .5, .45, .4, .35, .3, .25, .2, .15, .1, .05, .02, .01, .005, .0025 },
new double[] { 6.6, 7.4, 8.55, 9.95, 11.5, 12.7, 13.85, 14.7, 15.8, 16.7, 17.5, 18.25, 19, 19.7, 20.3, 21.1, 21.95, 23, 24.2, 25.7, 27.4, 28.4, 29.1, 29.4 },
20)]
        public void GraphicalShouldReadTheSameStuffItWrites(double[] probs, double[] flows, int erl)
        {
            GraphicalUncertainPairedData graphical = new GraphicalUncertainPairedData(probs, flows, erl, curveMetaData);
            XElement graphicalElement = graphical.WriteToXML();
            GraphicalUncertainPairedData graphicalFromXML = GraphicalUncertainPairedData.ReadFromXML(graphicalElement);
            bool success = graphical.Equals(graphicalFromXML);
            Assert.True(success);

        }
    }
}
