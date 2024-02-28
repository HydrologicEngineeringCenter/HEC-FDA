using HEC.FDA.Model.compute;
using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.Interfaces;
using HEC.FDA.Model.hydraulics.Mock;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.stageDamage;
using HEC.FDA.Model.structures;
using RasMapperLib;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using Xunit;

namespace HEC.FDA.ModelTest.integrationtests
{
    [Trait("RunsOn", "Remote")]

    public class UncertainStageDamageTests
    {
        #region H&H Data
        private static int quantityProfilesAndStructuresPerImpactArea = 4;

        //First impact area
        private static ContinuousDistribution lp3 = new LogPearson3(2.8, .438, .075, 50);
        private static double[] RatingCurveFlows = { 0, lp3.InverseCDF(1-0.5), lp3.InverseCDF(1-0.2), lp3.InverseCDF(1-0.1), lp3.InverseCDF(1-0.04), lp3.InverseCDF(1-0.02), lp3.InverseCDF(1-0.01), lp3.InverseCDF(1-0.004), lp3.InverseCDF(1-0.002) };
        private static IDistribution[] StageDistributions =
        {
            new Normal(458,0.00001),
            new Normal(468,.312),
            new Normal(469,.362),
            new Normal(470,.422),
            new Normal(471,.456),
            new Normal(473,.474),
            new Normal(475,.5),
            new Normal(477,.5),
            new Normal(479,.5)
        };
        private static UncertainPairedData dischargeStage = new UncertainPairedData(RatingCurveFlows, StageDistributions, new CurveMetaData("flows", "stages", "rating curve"));

        //Second impact area 
        private static double[] probabilities = new double[] { .5, .2, .1, .04, .02, .01, .004, .002 };
        private static double[] stages = new double[] { 468, 469, 470, 471, 473, 475, 477, 479 };
        private static int equivalentRecordLength = 50;
        private static CurveMetaData stageFreqMetaData = new CurveMetaData("probability", "stages", "graphical stage frequency");
        private static GraphicalUncertainPairedData frequencyStage = new GraphicalUncertainPairedData(probabilities, stages, equivalentRecordLength, stageFreqMetaData, true);

        //Hydraulics for a given impact area 
        private static HydraulicDataset ComputeHydraulicDataset()
        {
            List<IHydraulicProfile> dummyHydraulicProfiles = new List<IHydraulicProfile>();
            for (int i = 0; i < stages.Length; i++)
            {
                float[] profileStages = new float[quantityProfilesAndStructuresPerImpactArea];
                for (int j = 0; j < quantityProfilesAndStructuresPerImpactArea; j++)
                {
                    profileStages[j] = (float)stages[i];
                }
                DummyHydraulicProfile dummyHydraulicProfile = new DummyHydraulicProfile(profileStages, probabilities[i]);
                dummyHydraulicProfiles.Add(dummyHydraulicProfile);
            }
            HydraulicDataset hydraulicDataset = new HydraulicDataset(dummyHydraulicProfiles, Model.hydraulics.enums.HydraulicDataSource.WSEGrid);
            return hydraulicDataset;
        }
        private static HydraulicDataset hydraulicDataset_A = ComputeHydraulicDataset();
        private static HydraulicDataset hydraulicDataset_B = ComputeHydraulicDataset();


        #endregion

        #region Occupancy Type Data
        private static double[] depths = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static IDistribution[] res1StructureDamage = new IDistribution[] { new Normal(0, 0), new Normal(10, 1), new Normal(20, 2), new Normal(30, 3), new Normal(40, 4), new Normal(50, 5), new Normal(60, 6), new Normal(70, 7), new Normal(80, 9), new Normal(90, 9), new Normal(100, 10) };
        private static IDistribution[] res1cont_res2StructDamage = new IDistribution[] { new Normal(0, 0), new Normal(5, .5), new Normal(15, 1.5), new Normal(25, 2.5), new Normal(35, 3.5), new Normal(45, 4.5), new Normal(55, 5.5), new Normal(65, 6.5), new Normal(75, 7.5), new Normal(85, 8.5), new Normal(95, 9.5) };
        private static IDistribution[] res2ContentDamage = new IDistribution[] { new Normal(0, 0), new Normal(0, 0), new Normal(10, 1), new Normal(20, 2), new Normal(30, 3), new Normal(40, 4), new Normal(50, 5), new Normal(60, 6), new Normal(70, 7), new Normal(80, 8), new Normal(90, 9) };
        private static ValueRatioWithUncertainty res1CSVR = new ValueRatioWithUncertainty(IDistributionEnum.Normal, 10, 50);
        private static ValueRatioWithUncertainty res2CSVR = new ValueRatioWithUncertainty(IDistributionEnum.Normal, 20, 120);
        private static FirstFloorElevationUncertainty elevationUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Normal, 1);
        private static string residentialDamCat = "Residential";
        private static string res1OccType = "RES-1";
        private static string res2OccType = "RES-2";
        private static string contentAssetType = "Content";
        private static string structureAssetType = "Structure";
        private static CurveMetaData residentialStructure = new CurveMetaData(residentialDamCat, structureAssetType);
        private static CurveMetaData residentialContent = new CurveMetaData(residentialDamCat, contentAssetType);

        private static UncertainPairedData res1StructureDepthPercent = new UncertainPairedData(depths, res1StructureDamage, residentialStructure);
        private static UncertainPairedData res1ContentDepthPercent = new UncertainPairedData(depths, res1cont_res2StructDamage, residentialContent);
        private static UncertainPairedData res2StructureDepthPercent = new UncertainPairedData(depths, res1cont_res2StructDamage, residentialStructure);
        private static UncertainPairedData res2ContentDepthPercent = new UncertainPairedData(depths, res2ContentDamage, residentialStructure);

        private static OccupancyType res1 = OccupancyType.Builder()
            .WithName(res1OccType)
            .WithDamageCategory(residentialDamCat)
            .WithStructureDepthPercentDamage(res1StructureDepthPercent)
            .WithContentDepthPercentDamage(res1ContentDepthPercent)
            .WithContentToStructureValueRatio(res1CSVR)
            .WithFirstFloorElevationUncertainty(elevationUncertainty)
            .Build();

        private static OccupancyType res2 = OccupancyType.Builder()
            .WithName(res2OccType)
            .WithDamageCategory(residentialDamCat)
            .WithStructureDepthPercentDamage(res2StructureDepthPercent)
            .WithContentDepthPercentDamage(res2ContentDepthPercent)
            .WithContentToStructureValueRatio(res2CSVR)
            .WithFirstFloorElevationUncertainty(elevationUncertainty)
            .Build();

        private static Dictionary<string, OccupancyType> occupancyTypes = new Dictionary<string, OccupancyType>()
        {
            {res1OccType, res1},
            {res2OccType, res2}
        };
        #endregion

        #region Structure Data
        private static PointM pointM = new PointM();

        //First impact area structures
        private static int impactAreaID_1 = 1;
        private static Structure structure1 = new Structure(fid: "1", point: pointM, firstFloorElevation: 473, val_struct: 100, st_damcat: residentialDamCat, occtype: res2OccType, impactAreaID: impactAreaID_1);
        private static Structure structure2 = new Structure(fid: "2", point: pointM, firstFloorElevation: 474, val_struct: 200, st_damcat: residentialDamCat, occtype: res2OccType, impactAreaID: impactAreaID_1);
        private static Structure structure3 = new Structure(fid: "3", point: pointM, firstFloorElevation: 473.5, val_struct: 300, st_damcat: residentialDamCat, occtype: res1OccType, impactAreaID: impactAreaID_1);
        private static Structure structure4 = new Structure(fid: "4", point: pointM, firstFloorElevation: 474.5, val_struct: 400, st_damcat: residentialDamCat, occtype: res1OccType, impactAreaID: impactAreaID_1);

        //Second impact area structures 
        private static int impactAreaID_2 = 2;
        private static Structure structure5 = new Structure(fid: "5", point: pointM, firstFloorElevation: 473, val_struct: 100, st_damcat: residentialDamCat, occtype: res2OccType, impactAreaID: impactAreaID_2);
        private static Structure structure6 = new Structure(fid: "6", point: pointM, firstFloorElevation: 474, val_struct: 200, st_damcat: residentialDamCat, occtype: res2OccType, impactAreaID: impactAreaID_2);
        private static Structure structure7 = new Structure(fid: "7", point: pointM, firstFloorElevation: 473.5, val_struct: 300, st_damcat: residentialDamCat, occtype: res1OccType, impactAreaID: impactAreaID_2);
        private static Structure structure8 = new Structure(fid: "8", point: pointM, firstFloorElevation: 474.5, val_struct: 400, st_damcat: residentialDamCat, occtype: res1OccType, impactAreaID: impactAreaID_2);

        private static StructureSelectionMapping map = new StructureSelectionMapping(false, false, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

        private static Inventory impactArea_A_StructureInventory = new Inventory(occupancyTypes, new List<Structure>() { structure1, structure2, structure3, structure4 });
        private static Inventory impactArea_B_StructureInventory = new Inventory(occupancyTypes, new List<Structure>() { structure5, structure6, structure7, structure8 });

        #endregion

        #region Other objects 
        RandomProvider randomProvider = new RandomProvider(seed: 1234);
        #endregion

        #region Expected Values
        //These damage values were obtained from the computational engine on Feb 8 2024
        private static double[] stageAtWhichToCheckForDamage = new double[] { 472, 473, 477, 478, 479 };

        private static double[] expected_mean_residentialDamage_A = new double[] { 2.058882, 13.687099, 460.336736, 630.7556179, 801.78568839 }; 
        private static double[] expected_conf95_damageDists_A = new double[] { 11.9896858, 63.75562, 691.87106, 877.92861, 1066.1249756 };
        private static double[] expected_mean_residentialDamage_B = new double[] { 1.9554089, 13.667534, 459.677534, 629.9959, 800.94675 };
        private static double[] expected_conf95_damageDists_B = new double[] { 11.348461, 63.35985, 691.037619, 877.09907, 1065.306728 };

        #endregion

        
        [Fact]
        public void UncertainStageDamageTest()
        {
            //Arrange
            ImpactAreaStageDamage impactAreaStageDamage_A = new ImpactAreaStageDamage(impactAreaID_1, impactArea_A_StructureInventory, hydraulicDataset_A, "fakeHydroDir", analyticalFlowFrequency: lp3, dischargeStage: dischargeStage, usingMockData: true);
            ImpactAreaStageDamage impactAreaStageDamage_B = new ImpactAreaStageDamage(impactAreaID_2, impactArea_B_StructureInventory, hydraulicDataset_B, "fakeHydroDir", graphicalFrequency: frequencyStage, usingMockData: true);
            List<ImpactAreaStageDamage> impactAreas = new List<ImpactAreaStageDamage>() { impactAreaStageDamage_A, impactAreaStageDamage_B };
            ScenarioStageDamage scenarioStageDamage = new ScenarioStageDamage(impactAreas);

            //Act
            List<UncertainPairedData> stageDamageFunctions = scenarioStageDamage.Compute(randomProvider).Item1;

            List<IPairedData> meanDamageFunctions = new List<IPairedData>();
            double meanProb = 0.5;
            List<IPairedData> conf95DamageFunctions = new List<IPairedData>();
            double conf95Prob = 0.95;

            foreach (UncertainPairedData stageDamageFunction in stageDamageFunctions)
            {
                IPairedData meanDamageFunction = stageDamageFunction.SamplePairedData(meanProb, true);
                meanDamageFunctions.Add(meanDamageFunction);
                IPairedData conf95Function = stageDamageFunction.SamplePairedData(conf95Prob);
                conf95DamageFunctions.Add(conf95Function);
            }

            double[] actual_meanDamages_A = new double[stageAtWhichToCheckForDamage.Length];
            double[] actual_Conf95Damages_A = new double[stageAtWhichToCheckForDamage.Length];
            double[] actual_meanDamages_B = new double[stageAtWhichToCheckForDamage.Length];
            double[] actual_Conf95Damages_B = new double[stageAtWhichToCheckForDamage.Length];

            for (int i = 0; i < stageAtWhichToCheckForDamage.Length; i++)
            {
                double meanDamageA = 0;
                double conf95DamageA = 0;

                double meanDamageB = 0;
                double conf95DamageB = 0;

                foreach (PairedData stageDamageFunction in meanDamageFunctions)
                {
                    if (stageDamageFunction.MetaData.ImpactAreaID == impactAreaID_1)
                    {
                        meanDamageA += stageDamageFunction.f(stageAtWhichToCheckForDamage[i]);
                    }
                    else
                    {
                        meanDamageB += stageDamageFunction.f(stageAtWhichToCheckForDamage[i]);
                    }
                }
                actual_meanDamages_A[i] = meanDamageA;
                actual_meanDamages_B[i] = meanDamageB;
                foreach (PairedData stageDamageFunction in conf95DamageFunctions)
                {
                    if (stageDamageFunction.MetaData.ImpactAreaID == impactAreaID_1)
                    {
                        conf95DamageA += stageDamageFunction.f(stageAtWhichToCheckForDamage[i]);
                    }
                    else
                    {
                        conf95DamageB += stageDamageFunction.f(stageAtWhichToCheckForDamage[i]);
                    }
                }
                actual_Conf95Damages_A[i] = conf95DamageA;
                actual_Conf95Damages_B[i] = conf95DamageB;
            }

            //Assert
            for (int i = 0; i < stageAtWhichToCheckForDamage.Length; i++)
            {

                //Impact Area A Assertion
                Assert.Equal(actual_meanDamages_A[i], expected_mean_residentialDamage_A[i], 1);
                Assert.Equal(actual_Conf95Damages_A[i], expected_conf95_damageDists_A[i],1);

                //Impact Area B Assertion 
                Assert.Equal(expected_mean_residentialDamage_B[i],actual_meanDamages_B[i], 1);
                Assert.Equal(expected_conf95_damageDists_B[i],actual_Conf95Damages_B[i], 1);

            }
        }
       
    }
}
