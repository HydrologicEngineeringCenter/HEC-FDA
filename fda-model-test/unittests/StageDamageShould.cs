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
        private static int[] structureIDs = new int[] { 0, 1, 2, 3 };
        private static PointM pointM = new PointM(); // These won't get used. We're gonna do some goofy stuff with fake hydraulics.
        private static double[] firstFloorElevations = new double[] { 5, 6, 7, 8 };
        private static double[] structureValues = new double[] { 500, 600, 700, 800 };
        private static string residentialDamageCategory = "Residential";
        private static string commercialDamageCategory = "Commercial";
        private static string residentialNormalDistOccupancyTypeName = "Residential_One_Story_No_Basement_Normal";
        private static string commercialOccupancyTypeName = "Commercial_Warehouse";
        private static string[] damageCategories = new string[] { residentialDamageCategory, residentialDamageCategory, commercialDamageCategory, commercialDamageCategory };
        private static string[] occupancyTypes = new string[] { residentialNormalDistOccupancyTypeName, residentialNormalDistOccupancyTypeName, commercialOccupancyTypeName, commercialOccupancyTypeName };
        private static int impactAreaID = 1;
        private static ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 20000, maxIterations: 50000);
        private static string contentAssetCategory = "Content";
        private static string structureAssetCategory = "Structure";

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

        public static OccupancyType residentialOccupancyTypeNormalDists = OccupancyType.builder()
            .withName(residentialNormalDistOccupancyTypeName)
            .withDamageCategory(residentialDamageCategory)
            .withStructureDepthPercentDamage(_ResidentialNormallyDistDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ResidentialNormallyDistDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationNormallyDistUncertainty)
            .withStructureValueUncertainty(_structureValueNormallyDistUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatioNormallyDist)
            .build();

        private static OccupancyType commercialOccupancyTypeNormalDists = OccupancyType.builder()
            .withName(commercialOccupancyTypeName)
            .withDamageCategory(commercialDamageCategory)
            .withStructureDepthPercentDamage(_CommercialNormallyDistDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_CommercialNormallyDistDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationNormallyDistUncertainty)
            .withStructureValueUncertainty(_structureValueNormallyDistUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatioNormallyDist)
            .build();
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

        private static OccupancyType triangularLeftSkewOccType = OccupancyType.builder()
            .withName(residentialTriLeftDistOccupancyTypeName)
            .withDamageCategory(residentialDamageCategory)
            .withStructureDepthPercentDamage(TriDistSkewLeftercentDamageFunction)
            .withContentDepthPercentDamage(TriDistSkewLeftercentDamageFunction)
            .withFirstFloorElevationUncertainty(TriDistSkewLeftFFE)
            .withStructureValueUncertainty(TriDistSkewLeftStValUncertainty)
            .withContentToStructureValueRatio(TriDistSkewLeftCSVR)
            .build();

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

        private static OccupancyType triangularRightSkewOccType = OccupancyType.builder()
            .withName(residentialTriRightDistOccupancyTypeName)
            .withDamageCategory(residentialDamageCategory)
            .withStructureDepthPercentDamage(TriDistSkewRightPercentDamageFunction)
            .withContentDepthPercentDamage(TriDistSkewRightPercentDamageFunction)
            .withFirstFloorElevationUncertainty(TriDistSkewRightFFE)
            .withStructureValueUncertainty(TriDistSkewRightStValUncertainty)
            .withContentToStructureValueRatio(TriDistSkewRightCSVR)
            .build();

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

        private static OccupancyType UniformOccType = OccupancyType.builder()
            .withName(residentialUniformDistOccupancyTypeName)
            .withDamageCategory(residentialDamageCategory)
            .withStructureDepthPercentDamage(UniformPercentDamageFunction)
            .withContentDepthPercentDamage(UniformPercentDamageFunction)
            .withFirstFloorElevationUncertainty(UniformFFE)
            .withStructureValueUncertainty(UniformStValUncertainty)
            .withContentToStructureValueRatio(UniformCSVR)
            .build();
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

        private static DummyHydraulicProfile filteredHydraulicProfile2 = new DummyHydraulicProfile(new float[] { 0, 0  }, 0.5);
        private static DummyHydraulicProfile filteredHydraulicProfile5 = new DummyHydraulicProfile(new float[] { 1, 1  }, 0.2);
        private static DummyHydraulicProfile filteredHydraulicProfile10 = new DummyHydraulicProfile(new float[] {  2, 2 }, 0.1);
        private static DummyHydraulicProfile filteredHydraulicProfile25 = new DummyHydraulicProfile(new float[] {  3, 3 }, 0.04);
        private static DummyHydraulicProfile filteredHydraulicProfile50 = new DummyHydraulicProfile(new float[] { 4, 4 }, .02);
        private static DummyHydraulicProfile filteredHydraulicProfile100 = new DummyHydraulicProfile(new float[] { 5, 5 }, .01);
        private static DummyHydraulicProfile filteredHydraulicProfile200 = new DummyHydraulicProfile(new float[] { 6, 6 }, .005);
        private static DummyHydraulicProfile filteredHydraulicProfile500 = new DummyHydraulicProfile(new float[] { 7, 7 }, .002);
        private static List<IHydraulicProfile> filteredHydraulicProfiles = new List<IHydraulicProfile>() { filteredHydraulicProfile2, filteredHydraulicProfile5, filteredHydraulicProfile10, filteredHydraulicProfile25, filteredHydraulicProfile50, filteredHydraulicProfile100, filteredHydraulicProfile200, filteredHydraulicProfile500 };
        private static HydraulicDataset filteredHydraulicDataset = new HydraulicDataset(filteredHydraulicProfiles, hydraulicDataSource);

        private static GraphicalUncertainPairedData stageFrequency = new GraphicalUncertainPairedData(new double[] { .5, .2, .1, .04, .02, .01, .005, .002 },
        new double[] { 0, 1, 2, 3, 4, 5, 6, 7 }, 50, new CurveMetaData("Probability", "Stage", "Graphical Stage Frequency"));
        #endregion

        private static RandomProvider randomProvider = new RandomProvider(seed: 1234);


        //Calculations for this test can be found here: https://docs.google.com/spreadsheets/d/1jeTPOIi20Bz-CWIxM9jIUQz6pxNjwKt1/edit?usp=share_link&ouid=105470256128470573157&rtpof=true&sd=true
        [Theory]
        [InlineData(560, 504, 690, 621)]
        public void ComputeDamageOneCoordinateShouldComputeCorrectly(double expectedResidentialStructureDamage, double expectedResidentialContentDamage, double expectedCommercialStructureDamage, double expectedCommercialContentDamage)
        {
            //Arrange
            Inventory inventory = CreateInventory();
            Inventory residentialInventory = inventory.GetInventoryTrimmedToDamageCategory(residentialDamageCategory);
            Inventory commercialInventory = inventory.GetInventoryTrimmedToDamageCategory(commercialDamageCategory);
            float[] residentialWSEs = new float[] { 7, 10 };
            float[] commercialWSEs = new float[] { 8, 12 };            

            //Act
            ConsequenceDistributionResults residentialConsequenceDistributionResults = ImpactAreaStageDamage.ComputeDamageOneCoordinate(medianRandomProvider, convergenceCriteria, residentialInventory, residentialWSEs, analysisYear: 9999, impactAreaID, residentialDamageCategory);
            double actualResidentialStructureDamage = residentialConsequenceDistributionResults.MeanDamage(residentialDamageCategory, structureAssetCategory);
            double actualResidentialContentDamage = residentialConsequenceDistributionResults.MeanDamage(residentialDamageCategory, contentAssetCategory);

            ConsequenceDistributionResults commercialConsequenceDistributionResults = ImpactAreaStageDamage.ComputeDamageOneCoordinate(medianRandomProvider, convergenceCriteria, commercialInventory, commercialWSEs, analysisYear: 9999, impactAreaID, commercialDamageCategory);
            double actualCommercialStructureDamage = commercialConsequenceDistributionResults.MeanDamage(commercialDamageCategory, structureAssetCategory);
            double actualCommercialContentDamage = commercialConsequenceDistributionResults.MeanDamage(commercialDamageCategory, contentAssetCategory);

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
            Dictionary<string, OccupancyType> occupancyTypesList = new Dictionary<string, OccupancyType>() 
            {
                {residentialNormalDistOccupancyTypeName, residentialOccupancyTypeNormalDists },
                {commercialOccupancyTypeName, commercialOccupancyTypeNormalDists } 
            };

            Inventory inventory = new Inventory(null, null, null, occupancyTypesList, null, false, null, structures);
            return inventory;
        }
        /// <summary>
        /// This test demonstrates that the stage-damage algorithm computes damage with uncertainty correctly for a given set of water surface elevations
        /// Calculations for this test can be found at the below locations, by distribution type:
        /// Normal: https://docs.google.com/spreadsheets/d/19yUY75wVNrS3PLIX-QV_uPIopH_pL1ie/edit?usp=share_link&ouid=105470256128470573157&rtpof=true&sd=true
        /// Left-skewed triangular: https://docs.google.com/spreadsheets/d/1ZzDdCIQE7L8trnnCzomyNx60A1X4488-/edit?usp=share_link&ouid=105470256128470573157&rtpof=true&sd=true
        /// Right-skewed triangular: https://docs.google.com/spreadsheets/d/1g4XosSW8f-ndpJmrgphZpgJsOLUGsY-0/edit?usp=share_link&ouid=105470256128470573157&rtpof=true&sd=true
        /// Uniform: https://docs.google.com/spreadsheets/d/13tZkfWPL-UsEFKYShauPKEVSaRYQp-AP/edit?usp=share_link&ouid=105470256128470573157&rtpof=true&sd=true
        /// TODO: The below test has test scenarios commented out to save time in unit testing. 
        /// </summary>
        [Theory]
        //[InlineData(3, 9.85, 8.77, 30.46, 23.28, 5.73, 4.35, 6.38, 5.1)]//Passes
        //[InlineData(5, 99.99, 89.73, 104.32, 79.82, 88.86, 68.01, 61.13, 48.02)]//Passes
        [InlineData(9, 300.28, 270.07, 267.64, 204.96, 310.3, 237.83, 262.84, 224.35)]//Passes
        //[InlineData(11, 400.38, 360.28, 363.03, 278.21, 418.34, 320.65, 367.01, 310.05)]//Passes
        //[InlineData(20, 500.98, 450.83, 450.89, 345.44, 551.18, 422.6, 576.4, 461.01)]//Passes

        public void ComputeDamageWithUncertaintyOneCoordinateShouldComputeCorrecly(float wse, double expectedNormalDistStructure, double expectedNormalDistContent, double expectedTriLeftDistStructure, double expectedTriLeftDistContent, double expectedTriRightStructure, double expectedTriRightContent, double expectedUniformStructure, double expectedUniformContent)
        {
            //Arrange ---------------------------------------------------------------------

            //Structures 
            Structure normalStructure = new Structure(structureIDs[0], pointM, firstFloorElevations[0], structureValues[0], damageCategories[0], residentialNormalDistOccupancyTypeName, impactAreaID);
            Structure triLeftStructure = new Structure(structureIDs[0], pointM, firstFloorElevations[0], structureValues[0], damageCategories[0], residentialTriLeftDistOccupancyTypeName, impactAreaID);
            Structure triRightStructure = new Structure(structureIDs[0], pointM, firstFloorElevations[0], structureValues[0], damageCategories[0], residentialTriRightDistOccupancyTypeName , impactAreaID);
            Structure uniformStructure = new Structure(structureIDs[0], pointM, firstFloorElevations[0], structureValues[0], damageCategories[0], residentialUniformDistOccupancyTypeName, impactAreaID);

            //Occ Types
            Dictionary<string, OccupancyType> occupancyTypesDictionary = new Dictionary<string, OccupancyType>() 
            {
                {residentialNormalDistOccupancyTypeName, residentialOccupancyTypeNormalDists },
                {residentialTriLeftDistOccupancyTypeName, triangularLeftSkewOccType },
                {residentialTriRightDistOccupancyTypeName, triangularRightSkewOccType },
                {residentialUniformDistOccupancyTypeName, UniformOccType }
            };
            List<Structure> normalStructures = new List<Structure>() { normalStructure };
            List<Structure> triLeftStructures = new List<Structure>() { triLeftStructure };
            List<Structure> triRightStructures = new List<Structure>() { triRightStructure };
            List<Structure> uniformStructures = new List<Structure>() { uniformStructure };

            //Inventories 
            Inventory normalInventory = new Inventory(null, null, null, occupancyTypesDictionary, null, false, null, normalStructures);
            Inventory triLeftInventory = new Inventory(null, null, null, occupancyTypesDictionary, null, false, null, triLeftStructures);
            Inventory triRightInventory = new Inventory(null, null, null, occupancyTypesDictionary, null, false, null, triRightStructures);
            Inventory uniformInventory = new Inventory(null, null, null, occupancyTypesDictionary, null, false, null, uniformStructures);

            //Water
            float[] WSEs = new float[] { wse };

            //Act
            ConsequenceDistributionResults normal = ImpactAreaStageDamage.ComputeDamageOneCoordinate(randomProvider, convergenceCriteria, normalInventory, WSEs, analysisYear: 9999, impactAreaID, damageCategories[0]);
            ConsequenceDistributionResults triLeft = ImpactAreaStageDamage.ComputeDamageOneCoordinate(randomProvider, convergenceCriteria, triLeftInventory, WSEs, analysisYear: 9999, impactAreaID, damageCategories[0]);
            ConsequenceDistributionResults triRight = ImpactAreaStageDamage.ComputeDamageOneCoordinate(randomProvider, convergenceCriteria, triRightInventory, WSEs, analysisYear: 9999, impactAreaID, damageCategories[0]);
            ConsequenceDistributionResults uniform = ImpactAreaStageDamage.ComputeDamageOneCoordinate(randomProvider, convergenceCriteria, uniformInventory, WSEs, analysisYear: 9999, impactAreaID, damageCategories[0]);

            //Normal 
            double actualNormalResidentialStructureDamage = normal.MeanDamage(residentialDamageCategory, structureAssetCategory);
            double normalStructureRelativeDifference = Math.Abs(actualNormalResidentialStructureDamage - expectedNormalDistStructure) / expectedNormalDistStructure;
            double actualNormalResidentialContentDamage = normal.MeanDamage(residentialDamageCategory, contentAssetCategory);
            double normalContentRelativeDifference = Math.Abs(actualNormalResidentialContentDamage - expectedNormalDistContent) / expectedNormalDistContent;

            ////Tri Left
            double actualTriLeftStructureDamage = triLeft.MeanDamage(residentialDamageCategory, structureAssetCategory);
            double triLeftStructureRelativeDifference = Math.Abs(actualTriLeftStructureDamage - expectedTriLeftDistStructure) / expectedTriLeftDistStructure;
            double actualTriLeftContentDamage = triLeft.MeanDamage(residentialDamageCategory, contentAssetCategory);
            double triLeftContentRelativeDifference = Math.Abs(actualTriLeftContentDamage - expectedTriLeftDistContent) / expectedTriLeftDistContent;

            //////Tri Right
            double actualTriRightStructureDamage = triRight.MeanDamage(residentialDamageCategory, structureAssetCategory);
            double triRightStructureRelativeDiff = Math.Abs(actualTriRightStructureDamage - expectedTriRightStructure) / expectedTriRightStructure;
            double actualTriRightContentDamage = triRight.MeanDamage(residentialDamageCategory, contentAssetCategory);
            double triRightContentRelativeDiff = Math.Abs(actualTriRightContentDamage - expectedTriRightContent) / expectedTriRightContent;

            //////Uniform 
            double actualUniformStructureDamage = uniform.MeanDamage(residentialDamageCategory, structureAssetCategory);
            double uniformStructureRelativeDiff = Math.Abs(actualUniformStructureDamage - expectedUniformStructure) / expectedUniformStructure;
            double actualUniformContentDamage = uniform.MeanDamage(residentialDamageCategory, contentAssetCategory);
            double uniformContentRelativeDiff = Math.Abs(actualUniformContentDamage - expectedUniformContent) / expectedUniformContent;

            //Assert 
            double tolerance = 0.05;
            Assert.True(normalStructureRelativeDifference < tolerance);
            Assert.True(normalContentRelativeDifference < tolerance);
            Assert.True(triLeftStructureRelativeDifference < tolerance);
            Assert.True(triLeftContentRelativeDifference < tolerance);
            Assert.True(triRightStructureRelativeDiff < tolerance);
            Assert.True(triRightContentRelativeDiff < tolerance);
            Assert.True(uniformStructureRelativeDiff < tolerance);
            Assert.True(uniformContentRelativeDiff < tolerance);
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
            ConvergenceCriteria convergenceCriteriaDeterministic = new ConvergenceCriteria(minIterations: 1, maxIterations: 1);
            Inventory inventory = CreateInventory();
            ImpactAreaStageDamage impactAreaStageDamage = new ImpactAreaStageDamage(impactAreaID,inventory, filteredHydraulicDataset, convergenceCriteriaDeterministic, String.Empty,graphicalFrequency: stageFrequency, usingMockData: true);
            List<ImpactAreaStageDamage> impactAreaStageDamages = new List<ImpactAreaStageDamage>();
            impactAreaStageDamages.Add(impactAreaStageDamage);
            ScenarioStageDamage scenarioStageDamage = new ScenarioStageDamage(new List<ImpactAreaStageDamage>(impactAreaStageDamages));
            List<UncertainPairedData> results = scenarioStageDamage.Compute(new MedianRandomProvider(), convergenceCriteriaDeterministic);

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
