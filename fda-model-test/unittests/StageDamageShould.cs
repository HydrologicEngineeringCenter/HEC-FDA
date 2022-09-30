using Xunit;
using RasMapperLib;
using System.Collections.Generic;
using System;
using HEC.FDA.Model.stageDamage;
using HEC.FDA.Model.structures;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Statistics.Convergence;
using HEC.FDA.Statistics.Distributions;

namespace HEC.FDA.ModelTest.unittests
{
    public class StageDamageShould
    {

        //structure data
        private static int[] structureIDs = new int[] { 0, 1, 2, 3, };
        private static PointM pointM = new PointM();
        private static double[] firstFloorElevations = new double[] { 5, 6, 7, 8 };
        private static double[] structureValues = new double[] { 500, 600, 700, 800 };
        private static string residentialDamageCategory = "Residential";
        private static string commercialDamageCategory = "Commercial";
        private static string residentialOccupancyTypeName = "Residential_One_Story_No_Basement";
        private static string commercialOccupancyTypeName = "Commercial_Warehouse";
        private static string[] damageCategories = new string[] { residentialDamageCategory, residentialDamageCategory, commercialDamageCategory, commercialDamageCategory };
        private static string[] occupancyTypes = new string[] { residentialOccupancyTypeName, residentialOccupancyTypeName, commercialOccupancyTypeName, commercialOccupancyTypeName };
        private static int impactAreaID = 1;

        //occupancy type data
        private static double[] depths = new double[] { 0, 1, 2, 3, 4, 5 };
        private static IDistribution[] residentialPercentDamage = new IDistribution[]
        {
            new Normal(0,0),
            new Normal(.10,.05),
            new Normal(.20,.06),
            new Normal(.30,.07),
            new Normal(.40,.08),
            new Normal(.50,.09)
        };
        private static IDistribution[] commercialPercentDamage = new IDistribution[]
        {
            new Normal(0,0),
            new Normal(.20,.05),
            new Normal(.30,.06),
            new Normal(.40,.07),
            new Normal(.50,.08),
            new Normal(.60,.09)
        };
        private static CurveMetaData metaData = new CurveMetaData("Depths", "Percent Damage", "Depth-Percent Damage Function");
        private static UncertainPairedData _ResidentialDepthPercentDamageFunction = new UncertainPairedData(depths, residentialPercentDamage, metaData);
        private static UncertainPairedData _CommercialDepthPercentDamageFunction = new UncertainPairedData(depths, commercialPercentDamage, metaData);
        private static FirstFloorElevationUncertainty firstFloorElevationUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Normal, 0.5);
        private static ValueUncertainty _structureValueUncertainty = new ValueUncertainty(IDistributionEnum.Normal, .1);
        private static ValueRatioWithUncertainty _contentToStructureValueRatio = new ValueRatioWithUncertainty(IDistributionEnum.Normal, .1, .9);
        private static MedianRandomProvider medianRandomProvider = new MedianRandomProvider();

        private static OccupancyType residentialOccupancyType = OccupancyType.builder()
            .withName(residentialOccupancyTypeName)
            .withDamageCategory(residentialDamageCategory)
            .withStructureDepthPercentDamage(_ResidentialDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ResidentialDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType commercialOccupancyType = OccupancyType.builder()
            .withName(commercialOccupancyTypeName)
            .withDamageCategory(commercialDamageCategory)
            .withStructureDepthPercentDamage(_CommercialDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_CommercialDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
        private static string contentAssetCategory = "Content";
        private static string structureAssetCategory = "Structure";

        //Calculations for this test can be found here: https://docs.google.com/spreadsheets/d/1jeTPOIi20Bz-CWIxM9jIUQz6pxNjwKt1/edit?usp=sharing&ouid=105470256128470573157&rtpof=true&sd=true
        [Theory]
        [InlineData(340, 306, 540, 486)]
        public void ComputeDamageOneCoordinateShouldComputeCorrectly(double expectedResidentialStructureDamage, double expectedResidentialContentDamage, double expectedCommercialStructureDamage, double expectedCommercialContentDamage)
        {
            //Arrange
            List<Structure> structures = new List<Structure>();
            for (int i = 0; i < structureIDs.Length; i++)
            {
                Structure structure = new Structure(structureIDs[i], pointM, firstFloorElevations[i], structureValues[i], damageCategories[i], occupancyTypes[i], impactAreaID);
                structures.Add(structure);
            }
            List<OccupancyType> occupancyTypesList = new List<OccupancyType>() { residentialOccupancyType, commercialOccupancyType };
            Inventory inventory = new Inventory(structures, occupancyTypesList);
            float[] WSEs = new float[] { 7, 10, 8, 12 };

            //Act
            ConsequenceDistributionResults consequenceDistributionResults = ImpactAreaStageDamage.ComputeDamageOneCoordinate(medianRandomProvider, convergenceCriteria, inventory, impactAreaID, WSEs);
            double actualResidentialStructureDamage = consequenceDistributionResults.MeanDamage(residentialDamageCategory, structureAssetCategory);
            double actualResidentialContentDamage = consequenceDistributionResults.MeanDamage(residentialDamageCategory, contentAssetCategory);
            double actualCommercialStructureDamage = consequenceDistributionResults.MeanDamage(commercialDamageCategory, structureAssetCategory);
            double actualCommercialContentDamage = consequenceDistributionResults.MeanDamage(commercialDamageCategory, contentAssetCategory);

            //Assert
            double relativeDifferenceResidentialStructureDamage = Math.Abs(actualResidentialStructureDamage - expectedResidentialStructureDamage) / expectedResidentialStructureDamage;
            double relativeDifferenceREsidentialCOntentDamage = Math.Abs(actualResidentialContentDamage - expectedResidentialContentDamage) / expectedResidentialContentDamage;
            double relativeDifferenceCommercialStructureDamage = Math.Abs(actualCommercialStructureDamage - expectedCommercialStructureDamage) / expectedCommercialStructureDamage;
            double relativeDifferenceCommercialContentDamage = Math.Abs(actualCommercialContentDamage - expectedCommercialContentDamage) / expectedCommercialContentDamage;

            double tolerance = 0.05;

            Assert.True(relativeDifferenceResidentialStructureDamage < tolerance);
            Assert.True(relativeDifferenceREsidentialCOntentDamage < tolerance);
            Assert.True(relativeDifferenceCommercialStructureDamage < tolerance);
            Assert.True(relativeDifferenceCommercialContentDamage < tolerance);

        }
    }
}
