using Xunit;
using Statistics.Distributions;
using HEC.FDA.Model.paireddata;
using System;
using System.Collections.Generic;
using Utility;
using System.Linq;

namespace HEC.FDA.ModelTest.unittests;

[Trait("RunsOn", "Remote")]
public class AlaiWaiCaseStudy
{
    double[] ExceedenceProbabilities = new double[]
    {
            .002,
            .005,
            .01,
            .02,
            .05,
            .1,
            .2,
            .5,
    };

    double[] NonExceedenceProbabilities = new double[]
    {
            0.998,
            0.995,
            0.99,
            0.98,
            0.95,
            0.9,
            0.8,
            0.5
    };

    // TimeOfDay = 2 (2am)
    double[] Reach1MeanLL = new double[]
    {
            2.186,
            0.162,
            0.099,
            0.04,
            0.017,
            0.009,
            0.003,
            0
    };

    double[] Reach2MeanLL_2 = new double[]
    {
            0.037,
            0,
            0,
            0,
            0,
            0,
            0,
            0
    };

    double[] Reach3MeanLL_2 = new double[]
    {
            0.003,
            0.003,
            0,
            0,
            0.001,
            0,
            0,
            0
    };

    double[] Reach4MeanLL_2 = new double[]
    {
            0.003,
            0,
            0.003,
            0,
            0,
            0,
            0,
            0
    };

    // TimeOfDay = 14 (2pm)
    double[] Reach1MeanLL_14 = new double[]
    {
            0.887,
            0.159,
            0.098,
            0.079,
            0.033,
            0.014,
            0.005,
            0
    };

    double[] Reach2MeanLL_14 = new double[]
    {
            0.036,
            0.006,
            0.002,
            0,
            0,
            0,
            0,
            0
    };

    double[] Reach3MeanLL_14 = new double[]
    {
            0.01,
            0.004,
            0.003,
            0,
            0,
            0,
            0,
            0
    };

    double[] Reach4MeanLL_14 = new double[]
    {
            0.003,
            0.001,
            0,
            0,
            0,
            0,
            0,
            0
    };

    //Test that integration works as expected. Represents Reach 1, with and without extrapolating lifeloss frequency between 1 and 0.
    // When Extrapolating, we assume rectangle from low freq to 0,and triangle from high freq to 1.
    [Theory]
    [InlineData(false, 0.0074245)]
    [InlineData(true, 0.0117965)]
    public void AlaiWaiReach1MeanLifeLossIntegrationTest(bool asCDF, double expectedAALL)
    {
        Array.Sort(NonExceedenceProbabilities, Reach1MeanLL);
        PairedData reach1MeanLLCurve = new PairedData(NonExceedenceProbabilities, Reach1MeanLL);
        double calculatedAnnualLifeLoss = reach1MeanLLCurve.Integrate(asCDF);
        Assert.Equal(expectedAALL, calculatedAnnualLifeLoss, 4, MidpointRounding.ToEven);
    }

    [Theory]
    [InlineData(2, 1, 0.0117965)]
    [InlineData(2, 2, 0.0001295)]
    [InlineData(2, 3, 0.0000625)]
    [InlineData(2, 4, 0.000033)]
    [InlineData(14, 1, 0.0094255)]
    [InlineData(14, 2, 0.000165)]
    [InlineData(14, 3, 0.0000735)]
    [InlineData(14, 4, 0.0000145)]
    public void AlaiWaiAllReachesMeanLifeLossIntegrationTest(int timeOfDay, int reach, double expectedAALL)
    {
        double[] reachMeanLL = GetReachMeanLL(timeOfDay, reach);
        double[] nonExceedenceProbsCopy = (double[])NonExceedenceProbabilities.Clone();
        double[] reachMeanLLCopy = (double[])reachMeanLL.Clone();
        Array.Sort(nonExceedenceProbsCopy, reachMeanLLCopy);
        PairedData reachMeanLLCurve = new PairedData(nonExceedenceProbsCopy, reachMeanLLCopy);
        double calculatedAnnualLifeLoss = reachMeanLLCurve.Integrate(true); //testing with paddding.
        Assert.Equal(expectedAALL, calculatedAnnualLifeLoss, 4, MidpointRounding.ToEven);
    }

    private double[] GetReachMeanLL(int timeOfDay, int reach)
    {
        return (timeOfDay, reach) switch
        {
            (2, 1) => Reach1MeanLL,
            (2, 2) => Reach2MeanLL_2,
            (2, 3) => Reach3MeanLL_2,
            (2, 4) => Reach4MeanLL_2,
            (14, 1) => Reach1MeanLL_14,
            (14, 2) => Reach2MeanLL_14,
            (14, 3) => Reach3MeanLL_14,
            (14, 4) => Reach4MeanLL_14,
            _ => throw new ArgumentException($"Invalid timeOfDay/reach combination: {timeOfDay}/{reach}")
        };
    }


}
