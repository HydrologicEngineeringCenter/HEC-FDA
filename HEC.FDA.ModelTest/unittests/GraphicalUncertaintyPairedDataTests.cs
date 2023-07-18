using System.Collections.Generic;
using Xunit;
using System.Xml.Linq;
using HEC.FDA.Model.paireddata;
using Statistics.Distributions;
using Statistics.GraphicalRelationships;
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

        /// <summary>
        /// Test data: https://docs.google.com/spreadsheets/d/1GhRe3ECAFIKgRqEE8Xo6f_0lHYnHqUW0/edit?usp=sharing&ouid=105470256128470573157&rtpof=true&sd=true
        /// </summary>
        [Theory]
        [InlineData(new double[] { 0.999, 0.5, 0.2, 0.1, 0.02, 0.01, 0.005, 0.001, 0.0001 }, new double[] { 80, 11320, 18520, 23810, 35010, 39350, 42850, 47300, 52739.48924 }, 50, true, new double[] { 80, 2858.451639, 5337.228177, 6658.654778, 7550.215031, 8258.798858, 8866.700687, 9412.615319, 9918.486582, 10398.50894, 10862.93613, 11320, 11856.4513, 12395.02242, 12937.89249, 13487.3635, 14045.93339, 14616.3847, 15201.89798, 15806.20299, 16054.16048, 16306.16086, 16562.56442, 16823.7667, 17090.20399, 17362.35997, 17640.77365, 17926.04904, 18218.86703, 18520, 18736.39683, 18956.19424, 19179.57664, 19406.74265, 19637.90661, 19873.30033, 20113.17518, 20357.80435, 20607.48559, 20862.54433, 21123.33728, 21390.25671, 21663.73549, 21944.25297, 22232.34211, 22528.5979, 22833.68764, 23148.36336, 23473.47714, 23810, 24231.0174, 24668.69091, 25124.82597, 25601.54196, 26101.35072, 26627.26167, 27182.92484, 27772.8299, 27895.39, 28019.59076, 28145.48607, 28273.13255, 28402.58978, 28533.92048, 28667.19078, 28802.4704, 28939.83302, 29079.35648, 29221.1232, 29365.22049, 29511.74095, 29660.78296, 29812.45112, 29966.85685, 30124.11893, 30284.36424, 30447.72849, 30614.35707, 30784.40601, 30958.04304, 31135.44886, 31316.81843, 31502.36263, 31692.31001, 31886.90884, 32086.4295, 32291.1672, 32501.44516, 32717.61832, 32940.07765, 33169.25529, 33405.63061, 33649.73739, 33902.17257, 34163.60661, 34434.79638, 34716.60074, 35010, 35176.18123, 35346.0221, 35519.70763, 35697.43758, 35879.42803, 36065.91326, 36257.14785, 36453.40909, 36654.99989, 36862.25197, 37075.52972, 37295.23467, 37521.8108, 37755.75074, 37997.60331, 38247.98241, 38507.57791, 38777.16883, 39057.63956, 39350, 39619.12159, 39900.92632, 40196.79454, 40508.35015, 40837.52212, 41186.62691, 41558.48088, 41956.55709, 42385.20911, 42850, 42910.3666, 43163.374, 43509.54173, 43896.87678, 44337.52391, 44850.10471, 45465.41891, 45534.42774, 45605.06291, 45677.40789, 45751.55282, 45827.59528, 45905.64115, 45985.80556, 46068.21402, 46153.00372, 46240.32501, 46330.34321, 46423.24059, 46519.21889, 46618.50216, 46721.34017, 46828.01261, 46938.83406, 47054.16009, 47174.39478, 47300, 47431.50712, 47569.53177, 47714.79279, 47868.13687, 48030.57104, 48203.30621, 48387.81669, 48585.92322, 48799.9117, 49032.70799, 49288.14364, 49571.37673, 49889.59106, 50253.22991, 50678.34777, 51191.57749, 51842.25822, 52739.48924 })]
        [InlineData(new double[] { 0.999, 0.5, 0.2, 0.1, 0.02, 0.01, 0.005, 0.001, 0.0001 }, new double[] { 80, 11320, 18520, 23810, 35010, 39350, 42850, 47300, 52739.48924 }, 50, false, new double[] { 80, 272.1086624, 811.0588834, 1451.795015, 2150.324299, 2938.261, 3840.709292, 4885.076262, 6104.766029, 7542.590893, 9255.246058, 11320, 11742.90762, 12183.38049, 12644.09127, 13128.14736, 13639.21713, 14181.69845, 14760.94953, 15383.61278, 15646.64079, 15918.56566, 16200.09273, 16492.00886, 16795.19557, 17110.64497, 17439.47911, 17782.97376, 18142.58781, 18520, 18711.33028, 18907.69079, 19109.3655, 19316.66234, 19529.91605, 19749.49128, 19975.78631, 20209.23726, 20450.32303, 20699.57102, 20957.56392, 21224.94759, 21502.44052, 21790.84508, 22091.06095, 22404.10146, 22731.11338, 23073.40124, 23432.45748, 23810, 24157.57474, 24524.28001, 24912.37821, 25324.55265, 25764.01826, 26234.67117, 26741.29457, 27289.84851, 27405.22163, 27522.63682, 27642.16737, 27763.89058, 27887.88799, 28014.24575, 28143.05493, 28274.41192, 28408.41885, 28545.18405, 28684.82257, 28827.4567, 28973.21666, 29122.24124, 29274.67859, 29430.68705, 29590.43615, 29754.10764, 29921.89668, 30094.01326, 30270.68362, 30452.15206, 30638.68286, 30830.5625, 31028.10222, 31231.64096, 31441.54866, 31658.23024, 31882.13003, 32113.73709, 32353.59134, 32602.29088, 32860.50055, 33128.96224, 33408.50714, 33700.07064, 34004.71037, 34323.62835, 34658.19834, 35010, 35167.01119, 35328.20768, 35493.81734, 35664.08706, 35839.2849, 36019.70258, 36205.65836, 36397.50029, 36595.6101, 36800.40765, 37012.35612, 37231.96826, 37459.81357, 37696.52709, 37942.81972, 38199.49081, 38467.44341, 38747.7029, 39041.4401, 39350, 39608.66497, 39881.34288, 40169.6491, 40475.49493, 40801.16453, 41149.4185, 41523.63617, 41928.01517, 42367.85859, 42850, 42907.47196, 43149.18715, 43482.11233, 43857.67606, 44288.87749, 44795.80759, 45412.00763, 45481.6423, 45553.02864, 45626.25912, 45701.43374, 45778.66089, 45858.05838, 45939.75445, 46023.88917, 46110.61583, 46200.10275, 46292.53525, 46388.11806, 46487.07814, 46589.668, 46696.1697, 46806.8997, 46922.21462, 47042.51843, 47168.27119, 47300, 47424.64334, 47555.81733, 47694.26039, 47840.84444, 47996.60925, 48162.80858, 48340.9734, 48533.00045, 48741.27914, 48968.87903, 49219.83597, 49499.60692, 49815.82892, 50179.66509, 50608.38421, 51130.84475, 51800.98947, 52739.48924 })]
        public void ReturnsCorrectInterpolationFlowsOrStages(double[] exceedanceProbabilities, double[] flowOrStageValues, int equivalentRecordLength, bool usingStagesNotFlows, double[] expected)
        {
            GraphicalUncertainPairedData graphicalUncertainPairedData = new(exceedanceProbabilities, flowOrStageValues, equivalentRecordLength, curveMetaData, usingStagesNotFlows);
            IPairedData pairedData = graphicalUncertainPairedData.SamplePairedData(0.5);

            for (int i = 0; i < pairedData.Yvals.Length; i++)
            {
                double actual = pairedData.Yvals[i];
                double tolerance = 0.01;
                double relativeError = Math.Abs((actual - expected[i]) / expected[i]);
                Assert.True(relativeError < tolerance);
            }
        }

        /// <summary>
        /// This test demonstrates that our quantile interpolation reasonably matches direct quantile calculation
        /// </summary>
        /// <param name="probabilitiesAtWhichToTest"></param> these are probabilities for quantiles that are interpolated
        /// <param name="expectedQuantile"></param> these are interpolated quantiles 
        [Theory]
        [InlineData(new double[] {0.35, 0.75, 0.956, 0.9905}, new double[] {81.8684, 84.060773, 84.970707, 88.707344})]
        public void SamplePairedDataShould(double[] probabilitiesAtWhichToTest, double[] expectedQuantile)
        {
            int erl = 50;
            double[] inputProbabilities = new double[] { 0.999, 0.5, 0.2, 0.1, 0.04, 0.02, 0.01, 0.004, 0.002 };
            double[] inputStages = new double[] { 80, 82, 84, 84.5, 84.8, 85, 86, 88, 90 };
            GraphicalUncertainPairedData graphicalUncertainPairedData = new(inputProbabilities, inputStages, erl, new CurveMetaData("hello"), true);
            double probOneStandardDeviation = new Normal().CDF(1);
            PairedData oneStandardDeviationAboveMean = graphicalUncertainPairedData.SamplePairedData(probOneStandardDeviation);
            for (int i = 0; i < probabilitiesAtWhichToTest.Length; i++)
            {
                double probability = probabilitiesAtWhichToTest[i];
                double actual = oneStandardDeviationAboveMean.f(probability);
                double expected = expectedQuantile[i];
                Assert.Equal(expected, actual, 0.18);
            }


        }   

    }
}
