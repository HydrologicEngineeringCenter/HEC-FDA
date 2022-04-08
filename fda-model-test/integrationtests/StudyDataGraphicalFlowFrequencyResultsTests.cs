using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using metrics;
using compute;
using paireddata;
using Statistics;
using Statistics.Distributions;


namespace fda_model_test.integrationtests
{
    /// <summary>
    /// The example data in this test is based on Bear Creek Workshop 4 FDA study data
    /// The data is based on PYSR Without, 2024, S Fork Bear, SF-8
    /// see: https://drive.google.com/file/d/12WJL6ambACQLfqGUwbg7tv_wMxLn-a6t/view?usp=sharing
    /// </summary>
    public class StudyDataGraphicalFlowFrequencyResultsTests
    {
        static int equivalentRecordLength = 48;
        static double[] exceedanceProbabilities = new double[] { .999, .5, .2, .1, .04, .02, .01, .004, .002 };
        static double[] flowFrequencyDischarges = new double[] { 900, 1500, 2120, 3140, 4210, 5070, 6240, 7050, 9680 };
        static double[] ratingDischarges = new double[] { 0, 1500, 2120, 3140, 4210, 5070, 6240, 7050, 9680 };
        static IDistribution[] stageDischargeStageDistributions = new IDistribution[]
        {
            new Normal(458,0);


        }
    }
}
