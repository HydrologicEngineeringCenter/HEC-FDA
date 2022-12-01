using Xunit;
using Statistics;
using Statistics.Distributions;
using RasMapperLib;
using System.Collections.Generic;
using System;
using HEC.FDA.Model.stageDamage;
using HEC.FDA.Model.structures;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.Interfaces;
using HEC.FDA.Model.hydraulics.Mock;

namespace HEC.FDA.ModelTest.unittests
{
    [Trait("Category", "Disk")]
    public class StageDamageShould
    {

        //structure data
        private static int[] structureIDs = new int[] { 0, 1, 2, 3 };
        private static PointM pointM = new PointM(); // These won't get used. We're gonna do some goofy stuff with fake hydraulics.
        private static double[] firstFloorElevations = new double[] { 5, 6, 7, 8 };
        private static float[] GroundElevs = new float[] { 0, 0, 0, 0, 0 };
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

        //water data
        private const HydraulicDataSource hydraulicDataSource = HydraulicDataSource.SteadyHDF;
        private static DummyHydraulicProfile hydraulicProfile2 = new DummyHydraulicProfile(new float[] { 0, 0, 0, 0 }, 0.5);
        private static DummyHydraulicProfile hydraulicProfile5 = new DummyHydraulicProfile(new float[] { 1, 1, 1, 1 }, 0.2);
        private static DummyHydraulicProfile hydraulicProfile10 = new DummyHydraulicProfile(new float[] { 2, 2, 2, 2 }, 0.1);
        private static DummyHydraulicProfile hydraulicProfile25 = new DummyHydraulicProfile(new float[] { 3, 3, 3, 3 }, 0.04);
        private static DummyHydraulicProfile hydraulicProfile50 = new DummyHydraulicProfile(new float[] { 4, 4, 4, 4 }, .02);
        private static DummyHydraulicProfile hydraulicProfile100 = new DummyHydraulicProfile(new float[] { 5, 5, 5, 5 }, .01);
        private static DummyHydraulicProfile hydraulicProfile200 = new DummyHydraulicProfile(new float[] { 6, 6, 6, 6 }, .005);
        private static DummyHydraulicProfile hydraulicProfile500 = new DummyHydraulicProfile(new float[] { 7, 7, 7, 7 }, .002);
        private static List<IHydraulicProfile> hydraulicProfiles = new List<IHydraulicProfile>() { hydraulicProfile2, hydraulicProfile5, hydraulicProfile10, hydraulicProfile25, hydraulicProfile50, hydraulicProfile100, hydraulicProfile200, hydraulicProfile500 };
        private static HydraulicDataset hydraulicDataset = new HydraulicDataset(hydraulicProfiles, hydraulicDataSource);

        private static GraphicalUncertainPairedData stageFrequency = new GraphicalUncertainPairedData(new double[] { .5, .2, .1, .04, .02, .01, .005, .002 }
        , new double[] { 0, 1, 2, 3, 4, 5, 6, 7 }, 50, new CurveMetaData("Probability", "Stage", "Graphical Stage Frequency"));

        //Calculations for this test can be found here: https://docs.google.com/spreadsheets/d/1jeTPOIi20Bz-CWIxM9jIUQz6pxNjwKt1/edit?usp=sharing&ouid=105470256128470573157&rtpof=true&sd=true
        [Theory]
        [InlineData(340, 306, 540, 486)]
        public void ComputeDamageOneCoordinateShouldComputeCorrectly(double expectedResidentialStructureDamage, double expectedResidentialContentDamage, double expectedCommercialStructureDamage, double expectedCommercialContentDamage)
        {
            //Arrange
            Inventory inventory = CreateInventory();

            float[] WSEs = new float[] { 7, 10, 8, 12 };

            //Act
            ConsequenceDistributionResults consequenceDistributionResults = ImpactAreaStageDamage.ComputeDamageOneCoordinate(medianRandomProvider, convergenceCriteria, inventory, WSEs);
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


        private Inventory CreateInventory()
        {
            List<Structure> structures = new List<Structure>();
            for (int i = 0; i < structureIDs.Length; i++)
            {
                Structure structure = new Structure(structureIDs[i], pointM, firstFloorElevations[i], structureValues[i], damageCategories[i], occupancyTypes[i], impactAreaID);
                structures.Add(structure);
            }
            List<OccupancyType> occupancyTypesList = new List<OccupancyType>() { residentialOccupancyType, commercialOccupancyType };

            Inventory inventory = new Inventory(null, null, null, occupancyTypesList, null, false, null, structures);
            return inventory;
        }

        [Theory]
        [InlineData(5)]
        public void StructureDetailsShould(double expectedLength)
        {
            //Arrange
            Inventory inventory = CreateInventory();
            ImpactAreaStageDamage impactAreaStageDamage = new ImpactAreaStageDamage(impactAreaID, inventory, hydraulicDataset, convergenceCriteria, String.Empty,usingMockData: true);
            List<ImpactAreaStageDamage> impactAreaStageDamageList = new List<ImpactAreaStageDamage>() { impactAreaStageDamage };
            ScenarioStageDamage scenarioStageDamage = new ScenarioStageDamage(impactAreaStageDamageList);

            //Act
            List<string> structureDetails = scenarioStageDamage.ProduceStructureDetails();

            //Assert
            Assert.Equal(expectedLength, structureDetails.Count);
        }



        [Theory]
        [InlineData(new float[] {5,4,3}, new float[] {10,9,8})]
        public void ExtrapolateFromAboveShould(float[] input, float[] expectedResult)
        {
            float[] actualResult = ImpactAreaStageDamage.ExtrapolateFromAboveAtIndexLocation(input, 1, 5);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void ProduceReasonableResults()
        {
            Inventory inventory = CreateInventory();
            ImpactAreaStageDamage impactAreaStageDamage = new ImpactAreaStageDamage(impactAreaID,inventory, hydraulicDataset, convergenceCriteria, String.Empty,graphicalFrequency: stageFrequency);
            List<ImpactAreaStageDamage> impactAreaStageDamages = new List<ImpactAreaStageDamage>();
            impactAreaStageDamages.Add(impactAreaStageDamage);
            ScenarioStageDamage scenarioStageDamage = new ScenarioStageDamage(new List<ImpactAreaStageDamage>(impactAreaStageDamages));
            List<UncertainPairedData> results = scenarioStageDamage.Compute(new RandomProvider(1234), new ConvergenceCriteria());

            Assert.NotNull(results);

        }

        [Theory]
        [InlineData(new float[] { 500, 400, 300 }, new float[] { 455, 355, 255 })]
        public void ExtrapolateFromBelowShould(float[] input, float[] expectedResult)
        {
            float[] actualResult = ImpactAreaStageDamage.ExtrapolateFromBelowStagesAtIndexLocation(input, 1, 5,50);
            Assert.Equal(expectedResult, actualResult);

        }
    }
}
