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
    public class TractableStageDamageTests
    {
        #region H&H Data 



        private static readonly double[] probabilities = new double[] {.5, .2, .1, .04, .02, .01, .004, .002 };
        private static readonly double[] graphicalStages = new double[] {12, 13, 14, 15, 16, 17, 18, 19 };

        private static readonly int equivalentRecordLength = 50;
        private static readonly GraphicalUncertainPairedData stageFrequency = new(probabilities, graphicalStages, equivalentRecordLength, new CurveMetaData("probability", "stages", "graphical stage frequency"), true);

        private static readonly double[] inflows = new double[] { 1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900 };
        private static readonly GraphicalUncertainPairedData flowFrequency = new(probabilities, inflows, equivalentRecordLength, new CurveMetaData("probability", "discharge", "graphical flow frequency"), false);

        private static readonly IDistribution[] outflows = new IDistribution[]
        {
            new Deterministic(120),
            new Deterministic(130),
            new Deterministic(140),
            new Deterministic(150),
            new Deterministic(160),
            new Deterministic(170),
            new Deterministic(180),
            new Deterministic(190),
        };
        private static readonly UncertainPairedData unregReg = new(inflows, outflows, new CurveMetaData("unregulated", "regulated", "reg unreg function"));

        private static readonly double[] flows = new double[] { 120, 130, 140, 150, 160, 170, 180, 190 };
        private static readonly IDistribution[] stages = new IDistribution[]
        {
            new Deterministic(12),
            new Deterministic(13),
            new Deterministic(14),
            new Deterministic(15),
            new Deterministic(16),
            new Deterministic(17),
            new Deterministic(18),
            new Deterministic(19)
        };
        private static readonly UncertainPairedData dischargeStage = new(flows, stages, new CurveMetaData("discharge", "stage", "stage discharge function"));



        private static List<float[]> ComputeStagesAtStructures(float stage1, float stage2)
        {
            List<float[]> stages = new();
            float[] stagesFirstProfile = new float[] { stage1, stage2 };
            stages.Add(stagesFirstProfile);

            for (int i = 0; i < probabilities.Length - 1; i++)
            {
                float[] profileStages = new float[2];
                for (int j = 0; j < profileStages.Length; j++)
                {
                    profileStages[j] = stages[i][j] + 1;
                }
                stages.Add(profileStages);
            }
            return stages;
        }

        private static HydraulicDataset ComputeHydraulicDataset(float stage1, float stage2)
        {
            List<IHydraulicProfile> dummyHydraulicProfiles = new();
            List<float[]> stages = ComputeStagesAtStructures(stage1, stage2);
            int i = 0;
            foreach (float[] stage in stages)
            {
                DummyHydraulicProfile dummyHydraulicProfile = new(stage, probabilities[i]);
                dummyHydraulicProfiles.Add(dummyHydraulicProfile);
                i++;
            }
            HydraulicDataset hydraulicDataset = new(dummyHydraulicProfiles, Model.hydraulics.enums.HydraulicDataSource.WSEGrid);
            return hydraulicDataset;
        }

     
        #endregion

        #region Occupancy Type Data
        static readonly double[] depths = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        static readonly IDistribution[] residentialStructureDamage = new IDistribution[] { new Deterministic(0), new Deterministic(10), new Deterministic(20), new Deterministic(30), new Deterministic(40), new Deterministic(50), new Deterministic(60), new Deterministic(70), new Deterministic(80), new Deterministic(90), new Deterministic(100) };
        static readonly IDistribution[] residentialContentAndCommercialStructureDamage = new IDistribution[] { new Deterministic(0), new Deterministic(5), new Deterministic(15), new Deterministic(25), new Deterministic(35), new Deterministic(45), new Deterministic(55), new Deterministic(65), new Deterministic(75), new Deterministic(85), new Deterministic(95) };
        static readonly IDistribution[] commericalContentDamage = new IDistribution[] { new Deterministic(0), new Deterministic(0), new Deterministic(10), new Deterministic(20), new Deterministic(30), new Deterministic(40), new Deterministic(50), new Deterministic(60), new Deterministic(70), new Deterministic(80), new Deterministic(90) };
        static readonly ValueRatioWithUncertainty residentialCSVR = new(50);
        static readonly ValueRatioWithUncertainty commercialCSVR = new(120);
        static readonly string residentialDamAndOccType = "Residential";
        static readonly string commercialDamAndOccType = "Commercial";
        static readonly string contentAssetType = "Content";
        static readonly string structureAssetType = "Structure";
        static readonly CurveMetaData residentialStructure = new(residentialDamAndOccType, structureAssetType);
        static readonly CurveMetaData residentialContent = new(residentialDamAndOccType, contentAssetType);
        static readonly CurveMetaData commercialStructure  = new(commercialDamAndOccType, structureAssetType);
        static readonly CurveMetaData commercialContent = new(commercialDamAndOccType, contentAssetType);

        static readonly UncertainPairedData residentialStructureDepthPercent = new(depths, residentialStructureDamage, residentialStructure);
        static readonly UncertainPairedData residentialContentDepthPercent = new(depths, residentialContentAndCommercialStructureDamage, residentialContent);
        static readonly UncertainPairedData commercialStructureDepthPercent = new(depths, residentialContentAndCommercialStructureDamage, commercialStructure);
        static readonly UncertainPairedData commercialContentDepthPercent = new(depths, commericalContentDamage, commercialContent);
        
        static readonly OccupancyType residentialOccType = OccupancyType.Builder()
            .WithName(residentialDamAndOccType)
            .WithDamageCategory(residentialDamAndOccType)
            .WithStructureDepthPercentDamage(residentialStructureDepthPercent)
            .WithContentDepthPercentDamage(residentialContentDepthPercent)
            .WithContentToStructureValueRatio(residentialCSVR)
            .Build();

        static readonly OccupancyType commercialOccType = OccupancyType.Builder()
            .WithName(commercialDamAndOccType)
            .WithDamageCategory(commercialDamAndOccType)
            .WithStructureDepthPercentDamage(commercialStructureDepthPercent)
            .WithContentDepthPercentDamage(commercialContentDepthPercent)
            .WithContentToStructureValueRatio(commercialCSVR)
            .Build();

        static readonly Dictionary<string, OccupancyType> occupancyTypes = new() 
        { 
            { residentialDamAndOccType, residentialOccType },
            { commercialDamAndOccType, commercialOccType}
        };
        
        #endregion

        #region Structure Data
        private static readonly PointM pointM = new();
        private static readonly int impactAreaID = 34;
        private static readonly double groundElevation = 12;
        private static readonly Structure structure1 = new(fid: 1, point: pointM, firstFloorElevation: 14, val_struct: 100, st_damcat: residentialDamAndOccType, occtype: residentialDamAndOccType, impactAreaID: impactAreaID, groundElevation: groundElevation);
        private static readonly Structure structure2 = new(fid: 2, point: pointM, firstFloorElevation: 15, val_struct: 200, st_damcat: residentialDamAndOccType, occtype: residentialDamAndOccType, impactAreaID: impactAreaID, groundElevation: groundElevation);
        private static readonly Structure structure3 = new(fid: 3, point: pointM, firstFloorElevation: 17, val_struct: 300, st_damcat: commercialDamAndOccType, occtype: commercialDamAndOccType, impactAreaID: impactAreaID, groundElevation: groundElevation);
        private static readonly Structure structure4 = new(fid: 4, point: pointM, firstFloorElevation: 18, val_struct: 400, st_damcat: commercialDamAndOccType, occtype: commercialDamAndOccType, impactAreaID: impactAreaID, groundElevation: groundElevation);
        private static readonly List<Structure> residentialStructureList = new() { structure1, structure2 };
        private static readonly List<Structure> commercialStructureList = new() { structure3, structure4 };
        private static readonly Inventory residentialStructureInventory = new( occupancyTypes, residentialStructureList);
        private static readonly Inventory commercialStructureInventory = new(occupancyTypes, commercialStructureList);

        #endregion

        #region Other objects 
        private static readonly ConvergenceCriteria convergenceCriteria = new(minIterations: 100, maxIterations: 200);
        #endregion

        /// <summary>
        /// The solution for the below test is available at https://docs.google.com/spreadsheets/d/1QTjZ6BzGMBmxB-xWurNz08wnQx7HrmOm/edit?usp=share_link&ouid=105470256128470573157&rtpof=true&sd=true
        /// </summary>
        [Theory]
        [InlineData(new double[] {0, 0, 30, 60, 90, 120, 150, 180}, new double[] {0, 0, 0, 0, 84, 168, 252, 336}, "Residential", 13, 14, false)]
        [InlineData(new double[] { 0, 0, 30, 60, 90, 120, 150, 180 }, new double[] { 0, 0, 0, 0, 84, 168, 252, 336 }, "Commercial", 15, 16, false)]
        [InlineData(new double[] { 0, 0, 30, 60, 90, 120, 150, 180 }, new double[] { 0, 0, 0, 0, 84, 168, 252, 336 }, "Residential", 13, 14, true)]
        [InlineData(new double[] { 0, 0, 30, 60, 90, 120, 150, 180 }, new double[] { 0, 0, 0, 0, 84, 168, 252, 336 }, "Commercial", 15, 16, true)]
        public void TrackStageDamageTest(double[] expectedResDamage, double[] expectedComDamage, string damageCategory, float stage1, float stage2, bool useRegUnreg)
        {
            HydraulicDataset hydraulicDataset = ComputeHydraulicDataset(stage1, stage2);
            List<UncertainPairedData> stageDamageFunctions;

            if (useRegUnreg)
            {
                if (damageCategory == residentialDamAndOccType)
                {
                    ImpactAreaStageDamage impactAreaStageDamage = new(impactAreaID, residentialStructureInventory, hydraulicDataset, "fakeHydroDir", graphicalFrequency: flowFrequency, dischargeStage: dischargeStage, unregulatedRegulated: unregReg, usingMockData: true);
                    stageDamageFunctions = impactAreaStageDamage.Compute(new MedianRandomProvider());
                }
                else
                {
                    ImpactAreaStageDamage impactAreaStageDamage = new(impactAreaID, commercialStructureInventory, hydraulicDataset, "fakeHydroDir", graphicalFrequency: flowFrequency, dischargeStage: dischargeStage, unregulatedRegulated: unregReg, usingMockData: true);
                    stageDamageFunctions = impactAreaStageDamage.Compute(new MedianRandomProvider());
                }
            } 
            else
            {
                if (damageCategory == residentialDamAndOccType)
                {
                    ImpactAreaStageDamage impactAreaStageDamage = new(impactAreaID, residentialStructureInventory, hydraulicDataset, "fakeHydroDir", graphicalFrequency: stageFrequency, usingMockData: true);
                    stageDamageFunctions = impactAreaStageDamage.Compute(new MedianRandomProvider());
                }
                else
                {
                    ImpactAreaStageDamage impactAreaStageDamage = new(impactAreaID, commercialStructureInventory, hydraulicDataset, "fakeHydroDir", graphicalFrequency: stageFrequency, usingMockData: true);
                    stageDamageFunctions = impactAreaStageDamage.Compute(new MedianRandomProvider());
                }
            }
                
            double absoluteTolerance = 3;
            double relativeTolerance = 0.05;
            foreach (UncertainPairedData stageDamageFunction in stageDamageFunctions)
            {
                IPairedData pairedData = stageDamageFunction.SamplePairedData(0.5, true);
                if (stageDamageFunction.CurveMetaData.DamageCategory == residentialDamAndOccType)
                {
                    if (stageDamageFunction.CurveMetaData.AssetCategory == structureAssetType)
                    {
                        for (int i = 0; i < graphicalStages.Length; i++)
                        {
                            //TODO: f(stage) not f(probability) 
                            double actualDamage = pairedData.f(graphicalStages[i]);
                            if (expectedResDamage[i] == 0)
                            {
                                double difference = Math.Abs(actualDamage - expectedResDamage[i]);
                                Assert.True(difference < absoluteTolerance);
                            } 
                            else
                            {
                                double relativeDifference = Math.Abs(actualDamage - expectedResDamage[i]) / expectedResDamage[i];
                                Assert.True(relativeDifference < relativeTolerance);
                            }
                        }
                    }
                } 
                else
                {
                    if (stageDamageFunction.CurveMetaData.AssetCategory == contentAssetType)
                    {
                        for (int i = 0; i < graphicalStages.Length; i++)
                        {
                            double actualDamage = pairedData.f(graphicalStages[i]);
                            if (expectedComDamage[i] == 0)
                            {
                                double diff = Math.Abs(actualDamage - expectedComDamage[i]);
                                Assert.True(diff < absoluteTolerance);
                            }
                            else
                            {
                                double relativeDifference = Math.Abs(actualDamage - expectedComDamage[i]) / expectedComDamage[i];
                                Assert.True(relativeDifference < relativeTolerance);
                            }
                        }
                    }
                }
            }
        }


    }
}
