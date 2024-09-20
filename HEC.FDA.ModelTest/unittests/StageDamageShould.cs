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
    [Trait("RunsOn", "Local")]
    public class StageDamageShould
    {

        //structure data
        private static string[] structureIDs = new string[] { "0", "1", "2", "3" };
        private static PointM pointM = new PointM(); // These won't get used. We're gonna do some goofy stuff with fake hydraulics.
        private static double[] firstFloorElevations = new double[] { 5, 6, 7, 8 };
        private static double[] structureValues = new double[] { 500, 600, 700, 800 };
        private static string residentialDamageCategory = "Residential";
        private static string commercialDamageCategory = "Commercial";
        private static string residentialNormalDistOccupancyTypeName = "Residential_One_Story_No_Basement_Normal";
        private static string commercialOccupancyTypeName = "Commercial_Warehouse";
        private static string[] damageCategories = new string[] { residentialDamageCategory, residentialDamageCategory, residentialDamageCategory, residentialDamageCategory };
        private static string[] occupancyTypes = new string[] { residentialNormalDistOccupancyTypeName, residentialNormalDistOccupancyTypeName, residentialNormalDistOccupancyTypeName, residentialNormalDistOccupancyTypeName };
        private static int impactAreaID = 1;

        #region Normally Distributed Occ Type Data
        //occupancy type data
        private static double[] depths = new double[] { -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static IDistribution[] residentialPercentDamageNormallyDist = new IDistribution[]
        {
            new Normal(0,0),
            new Normal(10,4),
            new Normal(20,8),
            new Normal(30,12),
            new Normal(40,16),
            new Normal(50,12),
            new Normal(60,8),
            new Normal(70, 10),
            new Normal(80, 8),
            new Normal(90, 4),
            new Normal(100,0),
            new Normal(100,0),
            new Normal(100,0)
        };
        private static IDistribution[] commercialPercentDamageNormallyDist = new IDistribution[]
        {
            new Normal(0,0),
            new Normal(10,4),
            new Normal(20,8),
            new Normal(30,12),
            new Normal(40,16),
            new Normal(50,12),
            new Normal(60,8),
            new Normal(70, 10),
            new Normal(80, 8),
            new Normal(90, 4),
            new Normal(100,0),
            new Normal(100,0),
            new Normal(100,0)
        };
        private static CurveMetaData metaData = new CurveMetaData("Depths", "Percent Damage", "Depth-Percent Damage Function");
        private static UncertainPairedData _ResidentialNormallyDistDepthPercentDamageFunction = new UncertainPairedData(depths, residentialPercentDamageNormallyDist, metaData);
        private static UncertainPairedData _CommercialNormallyDistDepthPercentDamageFunction = new UncertainPairedData(depths, commercialPercentDamageNormallyDist, metaData);
        private static FirstFloorElevationUncertainty firstFloorElevationNormallyDistUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Normal, 0.5);
        private static ValueUncertainty _structureValueNormallyDistUncertainty = new ValueUncertainty(IDistributionEnum.Normal, .1);
        private static ValueRatioWithUncertainty _contentToStructureValueRatioNormallyDist = new ValueRatioWithUncertainty(IDistributionEnum.Normal, 10, 90);
        private static MedianRandomProvider medianRandomProvider = new MedianRandomProvider();

        public static OccupancyType residentialOccupancyTypeNormalDists = OccupancyType.Builder()
            .WithName(residentialNormalDistOccupancyTypeName)
            .WithDamageCategory(residentialDamageCategory)
            .WithStructureDepthPercentDamage(_ResidentialNormallyDistDepthPercentDamageFunction)
            .WithContentDepthPercentDamage(_ResidentialNormallyDistDepthPercentDamageFunction)
            .WithFirstFloorElevationUncertainty(firstFloorElevationNormallyDistUncertainty)
            .WithStructureValueUncertainty(_structureValueNormallyDistUncertainty)
            .WithContentToStructureValueRatio(_contentToStructureValueRatioNormallyDist)
            .Build();

        private static OccupancyType commercialOccupancyTypeNormalDists = OccupancyType.Builder()
            .WithName(commercialOccupancyTypeName)
            .WithDamageCategory(commercialDamageCategory)
            .WithStructureDepthPercentDamage(_CommercialNormallyDistDepthPercentDamageFunction)
            .WithContentDepthPercentDamage(_CommercialNormallyDistDepthPercentDamageFunction)
            .WithFirstFloorElevationUncertainty(firstFloorElevationNormallyDistUncertainty)
            .WithStructureValueUncertainty(_structureValueNormallyDistUncertainty)
            .WithContentToStructureValueRatio(_contentToStructureValueRatioNormallyDist)
            .Build();
        #endregion

        #region Triangular Skewed-Left Occupancy Type Data
        private static string residentialTriLeftDistOccupancyTypeName = "Residential_One_Story_No_Basement_TriLeft";
        private static IDistribution[] percentDamageTriSkewLeft = new IDistribution[]
        {
            new Triangular(0,0,0),
            new Triangular(8,10,11),
            new Triangular(10,20,22),
            new Triangular(16,30,33),
            new Triangular(18,40,46),
            new Triangular(20,50,52),
            new Triangular(30,60,68),
            new Triangular(40,70,80),
            new Triangular(52,80,88),
            new Triangular(63,90,94),
            new Triangular(100,100,100),
            new Triangular(100,100,100),
            new Triangular(100,100,100)
        };

        private static UncertainPairedData TriDistSkewLeftercentDamageFunction = new UncertainPairedData(depths, percentDamageTriSkewLeft, metaData);
        private static FirstFloorElevationUncertainty TriDistSkewLeftFFE = new FirstFloorElevationUncertainty(IDistributionEnum.Triangular, 3, 1);
        private static ValueUncertainty TriDistSkewLeftStValUncertainty = new ValueUncertainty(IDistributionEnum.Triangular, 60, 110);
        private static ValueRatioWithUncertainty TriDistSkewLeftCSVR = new ValueRatioWithUncertainty(IDistributionEnum.Triangular, 40, 90, 100);

        private static OccupancyType triangularLeftSkewOccType = OccupancyType.Builder()
            .WithName(residentialTriLeftDistOccupancyTypeName)
            .WithDamageCategory(residentialDamageCategory)
            .WithStructureDepthPercentDamage(TriDistSkewLeftercentDamageFunction)
            .WithContentDepthPercentDamage(TriDistSkewLeftercentDamageFunction)
            .WithFirstFloorElevationUncertainty(TriDistSkewLeftFFE)
            .WithStructureValueUncertainty(TriDistSkewLeftStValUncertainty)
            .WithContentToStructureValueRatio(TriDistSkewLeftCSVR)
            .Build();

        #endregion

        #region Triangular Skewed Right Occupancy Type Data
        private static string residentialTriRightDistOccupancyTypeName = "Residential_One_Story_No_Basement_TriRight";

        private static IDistribution[] percentDamageTriSkewRight = new IDistribution[]
{
            new Triangular(0,0,0),
            new Triangular(8,10,20),
            new Triangular(19,20,32),
            new Triangular(28,30,43),
            new Triangular(38,40,56),
            new Triangular(45,50,62),
            new Triangular(52,60,78),
            new Triangular(64,70,80),
            new Triangular(74,80,98),
            new Triangular(82,90,100),
            new Triangular(100,100,100),
            new Triangular(100,100,100),
            new Triangular(100,100,100)
};

        private static UncertainPairedData TriDistSkewRightPercentDamageFunction = new UncertainPairedData(depths, percentDamageTriSkewRight, metaData);
        private static FirstFloorElevationUncertainty TriDistSkewRightFFE = new FirstFloorElevationUncertainty(IDistributionEnum.Triangular, 1, 3);
        private static ValueUncertainty TriDistSkewRightStValUncertainty = new ValueUncertainty(IDistributionEnum.Triangular, 90, 140);
        private static ValueRatioWithUncertainty TriDistSkewRightCSVR = new ValueRatioWithUncertainty(IDistributionEnum.Triangular, 60, 70, 100);

        private static OccupancyType triangularRightSkewOccType = OccupancyType.Builder()
            .WithName(residentialTriRightDistOccupancyTypeName)
            .WithDamageCategory(residentialDamageCategory)
            .WithStructureDepthPercentDamage(TriDistSkewRightPercentDamageFunction)
            .WithContentDepthPercentDamage(TriDistSkewRightPercentDamageFunction)
            .WithFirstFloorElevationUncertainty(TriDistSkewRightFFE)
            .WithStructureValueUncertainty(TriDistSkewRightStValUncertainty)
            .WithContentToStructureValueRatio(TriDistSkewRightCSVR)
            .Build();

        #endregion

        #region Uniform Occupancy Type Data
        private static string residentialUniformDistOccupancyTypeName = "Residential_One_Story_No_Basement_Uniform";

        private static IDistribution[] percentDamageUniform = new IDistribution[]
{
            new Uniform(0,0),
            new Uniform(8,10),
            new Uniform(19,20),
            new Uniform(28,30),
            new Uniform(38,40),
            new Uniform(45,50),
            new Uniform(52,60),
            new Uniform(64,70),
            new Uniform(74,80),
            new Uniform(82,90),
            new Uniform(100,100),
            new Uniform(100,100),
            new Uniform(100,100)
};

        private static UncertainPairedData UniformPercentDamageFunction = new UncertainPairedData(depths, percentDamageUniform, metaData);
        private static FirstFloorElevationUncertainty UniformFFE = new FirstFloorElevationUncertainty(IDistributionEnum.Uniform, 1, 3);
        private static ValueUncertainty UniformStValUncertainty = new ValueUncertainty(IDistributionEnum.Uniform, 90, 140);
        private static ValueRatioWithUncertainty UniformCSVR = new ValueRatioWithUncertainty(IDistributionEnum.Uniform, 60, 500, 100);

        private static OccupancyType UniformOccType = OccupancyType.Builder()
            .WithName(residentialUniformDistOccupancyTypeName)
            .WithDamageCategory(residentialDamageCategory)
            .WithStructureDepthPercentDamage(UniformPercentDamageFunction)
            .WithContentDepthPercentDamage(UniformPercentDamageFunction)
            .WithFirstFloorElevationUncertainty(UniformFFE)
            .WithStructureValueUncertainty(UniformStValUncertainty)
            .WithContentToStructureValueRatio(UniformCSVR)
            .Build();
        #endregion 

        #region Water Data
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

        private static DummyHydraulicProfile filteredHydraulicProfile2 = new DummyHydraulicProfile(new float[] { 0, 0 }, 0.5);
        private static DummyHydraulicProfile filteredHydraulicProfile5 = new DummyHydraulicProfile(new float[] { 1, 1 }, 0.2);
        private static DummyHydraulicProfile filteredHydraulicProfile10 = new DummyHydraulicProfile(new float[] { 2, 2 }, 0.1);
        private static DummyHydraulicProfile filteredHydraulicProfile25 = new DummyHydraulicProfile(new float[] { 3, 3 }, 0.04);
        private static DummyHydraulicProfile filteredHydraulicProfile50 = new DummyHydraulicProfile(new float[] { 4, 4 }, .02);
        private static DummyHydraulicProfile filteredHydraulicProfile100 = new DummyHydraulicProfile(new float[] { 5, 5 }, .01);
        private static DummyHydraulicProfile filteredHydraulicProfile200 = new DummyHydraulicProfile(new float[] { 6, 6 }, .005);
        private static DummyHydraulicProfile filteredHydraulicProfile500 = new DummyHydraulicProfile(new float[] { 7, 7 }, .002);
        private static List<IHydraulicProfile> filteredHydraulicProfiles = new List<IHydraulicProfile>() { filteredHydraulicProfile2, filteredHydraulicProfile5, filteredHydraulicProfile10, filteredHydraulicProfile25, filteredHydraulicProfile50, filteredHydraulicProfile100, filteredHydraulicProfile200, filteredHydraulicProfile500 };
        private static HydraulicDataset filteredHydraulicDataset = new HydraulicDataset(filteredHydraulicProfiles, hydraulicDataSource);

        private static GraphicalUncertainPairedData stageFrequency = new GraphicalUncertainPairedData(new double[] { .5, .2, .1, .04, .02, .01, .005, .002 },
        new double[] { 0, 1, 2, 3, 4, 5, 6, 7 }, 50, new CurveMetaData("Probability", "Stage", "Graphical Stage Frequency"), true);
        #endregion

     


        private static RandomProvider randomProvider = new RandomProvider(seed: 1234);

        private Inventory CreateInventory()
        {
            List<Structure> structures = new List<Structure>();
            for (int i = 0; i < structureIDs.Length; i++)
            {
                Structure structure = new Structure((string)structureIDs[i], pointM, firstFloorElevations[i], structureValues[i], damageCategories[i], occupancyTypes[i], impactAreaID);
                structures.Add(structure);
            }
            Dictionary<string, OccupancyType> occupancyTypesList = new Dictionary<string, OccupancyType>()
            {
                {residentialNormalDistOccupancyTypeName, residentialOccupancyTypeNormalDists },
                {commercialOccupancyTypeName, commercialOccupancyTypeNormalDists }
            };

            Inventory inventory = new Inventory(occupancyTypesList,structures);
            return inventory;
        }


        [Fact]
        public void ErrorsShouldStopCompute()
        {
            //Arrange
            Inventory inventory = CreateInventory();
            ImpactAreaStageDamage impactAreaStageDamage = new ImpactAreaStageDamage(impactAreaID, inventory, hydraulicDataset, String.Empty, usingMockData: true);
            List<ImpactAreaStageDamage> impactAreaStageDamageList = new List<ImpactAreaStageDamage>() { impactAreaStageDamage };
            ScenarioStageDamage scenarioStageDamage = new ScenarioStageDamage(impactAreaStageDamageList);

            //Act
            //This compute should return a list with count of 0 stage-damage functions - we didnt provide any H&H summary relationships 
            //so cannot calculate stage frequency, and the compute should check for that 
            List<UncertainPairedData> nullStageDamage = impactAreaStageDamage.Compute(randomProvider).Item1;

            //Assert
            Assert.Equal(0, nullStageDamage.Count);
        }

        [Theory]
        [InlineData(6)]
        public void StructureDetailsShould(double expectedLength)
        {
            //Arrange
            Inventory inventory = CreateInventory();
            ImpactAreaStageDamage impactAreaStageDamage = new ImpactAreaStageDamage(impactAreaID, inventory, hydraulicDataset, String.Empty, usingMockData: true);
            List<ImpactAreaStageDamage> impactAreaStageDamageList = new List<ImpactAreaStageDamage>() { impactAreaStageDamage };
            ScenarioStageDamage scenarioStageDamage = new ScenarioStageDamage(impactAreaStageDamageList);
            Dictionary<int, string> iaNames = new();
            iaNames.Add(1, "Test Impact Area");

            //Act
            List<string> structureDetails = scenarioStageDamage.ProduceStructureDetails(iaNames);

            //Assert
            Assert.Equal(expectedLength, structureDetails.Count);
        }



        [Theory]
        [InlineData(new float[] { 5, 4, 3 }, new float[] { 10, 9, 8 })]
        public void ExtrapolateFromAboveShould(float[] input, float[] expectedResult)
        {
            float[] actualResult = ImpactAreaStageDamage.ExtrapolateFromAboveAtIndexLocation(input, 1, 5);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void ProduceNonNullResults()
        {
            ConvergenceCriteria convergenceCriteriaDeterministic = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            Inventory inventory = CreateInventory();
            ImpactAreaStageDamage impactAreaStageDamage = new ImpactAreaStageDamage(impactAreaID, inventory, hydraulicDataset, String.Empty, graphicalFrequency: stageFrequency, usingMockData: true);
            List<ImpactAreaStageDamage> impactAreaStageDamages = new List<ImpactAreaStageDamage>();
            impactAreaStageDamages.Add(impactAreaStageDamage);
            ScenarioStageDamage scenarioStageDamage = new ScenarioStageDamage(new List<ImpactAreaStageDamage>(impactAreaStageDamages));
            List<UncertainPairedData> results = scenarioStageDamage.Compute(new MedianRandomProvider()).Item1;

            Assert.NotNull(results);

        }

        [Theory]
        [InlineData(new float[] { 500, 400, 300 }, new float[] { 455, 355, 255 })]
        public void ExtrapolateFromBelowShould(float[] input, float[] expectedResult)
        {
            float[] actualResult = ImpactAreaStageDamage.ExtrapolateFromBelowStagesAtIndexLocation(input, 1, 5, 50);
            Assert.Equal(expectedResult, actualResult);

        }
    }
}
