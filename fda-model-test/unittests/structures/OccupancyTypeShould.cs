﻿using Xunit;
using structures;
using Statistics;
using paireddata;
using Statistics.Distributions;
using compute;

namespace fda_model_test.unittests.structures
{
    public class OccupancyTypeShould
    {
        private static double[] depths = new double[] { 0, 1, 2, 3, 4, 5 };
        private static IDistribution[] percentDamages = new IDistribution[]
        {
            new Normal(0,0),
            new Normal(.10,.05),
            new Normal(.20,.06),
            new Normal(.30,.07),
            new Normal(.40,.08),
            new Normal(.50,.09)
        };
        private static double[] expectedPercentDamage = new double[] { 0, .10, .20, .30, .40, .50 };
        private static CurveMetaData metaData = new CurveMetaData("Depths", "Percent Damage", "Depth-Percent Damage Function");
        private static UncertainPairedData _StructureDepthPercentDamageFunction = new UncertainPairedData(depths, percentDamages, metaData);
        private static UncertainPairedData _ContentDepthPercentDamageFunction = new UncertainPairedData(depths, percentDamages, metaData);
        private static FirstFloorElevationUncertainty firstFloorElevationUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Normal, 0.5);
        private static ValueUncertainty _structureValueUncertainty = new ValueUncertainty(IDistributionEnum.Normal, .1);
        private static ValueRatioWithUncertainty _contentToStructureValueRatio = new ValueRatioWithUncertainty(IDistributionEnum.Normal, .1, .9);
        private double expectedCSVR = 0.9;
        private static MedianRandomProvider medianRandomProvider = new MedianRandomProvider();
        private static string name = "MyOccupancyType";
        private static string damageCategory = "DamageCategory";

        [Theory]
        [InlineData(100,10)]
        public void OccupancyTypeShouldSampleCorrectly(double structureValue, double firstFloorElevation)
        {
            OccupancyType occupancyType = OccupancyType.builder()
                .withName(name)
                .withDamageCategory(damageCategory)
                .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
                .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
                .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
                .withStructureValueUncertainty(_structureValueUncertainty)
                .withContentToStructureValueRatio(_contentToStructureValueRatio)
                .build();

            SampledStructureParameters sampledStructureParameters = occupancyType.Sample(medianRandomProvider, structureValue, firstFloorElevation);
            double expectedContentValue = expectedCSVR * sampledStructureParameters.StructureValueSampled;

            Assert.Equal(name, sampledStructureParameters.OccupancyTypeName);
            Assert.Equal(damageCategory, sampledStructureParameters.OccupancyTypeDamageCategory);
            Assert.Equal(expectedPercentDamage, sampledStructureParameters.StructPercentDamagePairedData.Yvals);
            Assert.Equal(structureValue, sampledStructureParameters.StructureValueSampled);
            Assert.Equal(expectedContentValue, sampledStructureParameters.ContentValueSampled);
            Assert.Equal(expectedPercentDamage, sampledStructureParameters.ContentPercentDamagePairedData.Yvals);
            Assert.Equal(firstFloorElevation, sampledStructureParameters.FirstFloorElevationSampled);

        }

    }
}
