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


}
