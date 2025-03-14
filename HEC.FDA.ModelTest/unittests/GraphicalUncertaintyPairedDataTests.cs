using System.Collections.Generic;
using Xunit;
using System.Xml.Linq;
using HEC.FDA.Model.paireddata;
using Statistics.Distributions;
using HEC.FDA.Model.extensions;
using System;

namespace HEC.FDA.ModelTest.unittests
{
    [Trait("RunsOn", "Remote")]
    public class GraphicalUncertaintyPairedDataTests
    {
        static string xLabel = "x label";
        static string yLabel = "y label";
        static string name = "name";
        static string category = "residential";
        static CurveMetaData curveMetaData = new CurveMetaData(xLabel, name, category);
        private static double[] _RequiredExceedanceProbabilities = { 0.99900, 0.99000, 0.95000, 0.90000, 0.85000, 0.80000, 0.75000, 0.70000, 0.65000, 0.60000, 0.55000, 0.50000, 0.47500, 0.45000, 0.42500, 0.40000, 0.37500, 0.35000, 0.32500, 0.30000, 0.29000, 0.28000, 0.27000, 0.26000, 0.25000, 0.24000, 0.23000, 0.22000, 0.21000, 0.20000, 0.19500, 0.19000, 0.18500, 0.18000, 0.17500, 0.17000, 0.16500, 0.16000, 0.15500, 0.15000, 0.14500, 0.14000, 0.13500, 0.13000, 0.12500, 0.12000, 0.11500, 0.11000, 0.10500, 0.10000, 0.09500, 0.09000, 0.08500, 0.08000, 0.07500, 0.07000, 0.06500, 0.06000, 0.05900, 0.05800, 0.05700, 0.05600, 0.05500, 0.05400, 0.05300, 0.05200, 0.05100, 0.05000, 0.04900, 0.04800, 0.04700, 0.04600, 0.04500, 0.04400, 0.04300, 0.04200, 0.04100, 0.04000, 0.03900, 0.03800, 0.03700, 0.03600, 0.03500, 0.03400, 0.03300, 0.03200, 0.03100, 0.03000, 0.02900, 0.02800, 0.02700, 0.02600, 0.02500, 0.02400, 0.02300, 0.02200, 0.02100, 0.02000, 0.01950, 0.01900, 0.01850, 0.01800, 0.01750, 0.01700, 0.01650, 0.01600, 0.01550, 0.01500, 0.01450, 0.01400, 0.01350, 0.01300, 0.01250, 0.01200, 0.01150, 0.01100, 0.01050, 0.01000, 0.00950, 0.00900, 0.00850, 0.00800, 0.00750, 0.00700, 0.00650, 0.00600, 0.00550, 0.00500, 0.00490, 0.00450, 0.00400, 0.00350, 0.00300, 0.00250, 0.00200, 0.00195, 0.00190, 0.00185, 0.00180, 0.00175, 0.00170, 0.00165, 0.00160, 0.00155, 0.00150, 0.00145, 0.00140, 0.00135, 0.00130, 0.00125, 0.00120, 0.00115, 0.00110, 0.00105, 0.00100, 0.00095, 0.00090, 0.00085, 0.00080, 0.00075, 0.00070, 0.00065, 0.00060, 0.00055, 0.00050, 0.00045, 0.00040, 0.00035, 0.00030, 0.00025, 0.00020, 0.00015, 0.00010 };


        [Theory]
        [InlineData(new double[] { .99, .5, .1, .02, .01, .002 }, new double[] { 500, 2000, 34900, 66900, 86000, 146000 }, 5, false)] //Based on Elkhorn River at Highway 91 Dodge County FIS 2008
        [InlineData(new double[] { .99, .95, .90, .85, .8, .75, .7, .65, .6, .55, .5, .45, .4, .35, .3, .25, .2, .15, .1, .05, .02, .01, .005, .0025 },
       new double[] { 6.6, 7.4, 8.55, 9.95, 11.5, 12.7, 13.85, 14.7, 15.8, 16.7, 17.5, 18.25, 19, 19.7, 20.3, 21.1, 21.95, 23, 24.2, 25.7, 27.4, 28.4, 29.1, 29.4 },
       20, true)]
        [InlineData(new double[] {.99, .5, .2, .1, .04, .02, .01, .004, .002}, new double[] {1087, 19, 8420, 10447, 12472, 14221, 15185, 18000, 19050}, 48, false)]
        public void ReturnsDistributionsWhereMeanAndConfidenceLimitsAreMonotonicallyIncreasing(double[] probs, double[] flows, int erl, bool usingStagesNotFlows)
        {
            GraphicalUncertainPairedData graphical = new GraphicalUncertainPairedData(probs, flows, erl, curveMetaData, usingStagesNotFlows);
            List<IPairedData> pairedDataList = new List<IPairedData>();

            foreach(var probability in _RequiredExceedanceProbabilities)
            {
                IPairedData pairedData = graphical.SamplePairedData(probability);
                pairedDataList.Add(pairedData);
            }

            bool pass = true;
            foreach (IPairedData pairedData in pairedDataList)
            {
                for (int j = 1; j < pairedData.Xvals.Length; j++)
                {
                    if (pairedData.Yvals[j] < pairedData.Yvals[j - 1])
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
            GraphicalUncertainPairedData graphical = new GraphicalUncertainPairedData(probs, flows, erl, curveMetaData, true);
            XElement graphicalElement = graphical.WriteToXML();
            GraphicalUncertainPairedData graphicalFromXML = GraphicalUncertainPairedData.ReadFromXML(graphicalElement);
            bool success = graphical.Equals(graphicalFromXML);
            Assert.True(success);

        }


        ///// <summary>
        ///// This test demonstrates that our quantile sampling reasonably matches direct quantile calculation
        ///// test data for the first case study can be found at: https://docs.google.com/spreadsheets/d/1aLnGuzmmopDID7ehb1Jux5IZtegMpmnX/edit?usp=drive_link&ouid=105470256128470573157&rtpof=true&sd=true
        ///// test data for the second case study was generated from HEC-FDA Version 1.4.3 from the Algiers feasibilty study data 
        ///// </summary>
        ///// <param name="probabilitiesAtWhichToTest"></param> these are probabilities for quantiles that are interpolated
        ///// <param name="expectedQuantile"></param> these are interpolated quantiles 
        //[Theory]
        //[InlineData(new double[] { 0.999, 0.5, 0.2, 0.1, 0.04, 0.02, 0.01, 0.004, 0.002 }, new double[] { 80, 82, 84, 84.5, 84.8, 85, 86, 88, 90 }, 50, 1, new double[] {0.35, 0.75, 0.956, 0.9905}, new double[] {81.8684, 84.060773, 84.970707, 88.707344})]
        //[InlineData(new double[] {0.5,0.2, 0.1, 0.04, 0.02, 0.01, 0.005, 0.002}, new double[] {1, 1.1, 4.93, 4.98, 5.02, 5.04, 5.18, 5.3}, 40, 2, new double[] { .1, .78, .8, .825, .99, .995 }, new double[] {1, 1.144, 3.398, 4.995, 5.216, 5.356})]
        //public void SamplePairedDataShould(double[] inputProbabilities, double[] inputStages, int erl, int standardDeviationAtWhichToTest, double[] probabilitiesAtWhichToTest, double[] expectedQuantile)
        //{

        //    GraphicalUncertainPairedData graphicalUncertainPairedData = new(inputProbabilities, inputStages, erl, new CurveMetaData("hello"), true);
        //    double probAtWhichToTest = new Normal().CDF(standardDeviationAtWhichToTest);
        //    PairedData sampledCurve = graphicalUncertainPairedData.SamplePairedData(probAtWhichToTest);
        //    for (int i = 0; i < probabilitiesAtWhichToTest.Length; i++)
        //    {
        //        double probability = probabilitiesAtWhichToTest[i];
        //        double actual = sampledCurve.f(probability);
        //        double expected = expectedQuantile[i];
        //        Assert.Equal(expected, actual, 0.2);
        //    }


        //}   

    }
}
