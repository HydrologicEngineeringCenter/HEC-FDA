using System;
using System.Collections.Generic;
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
using Xunit;

namespace HEC.FDA.ModelTest.integrationtests
{
    [Trait("Category", "Integration")]
    public class StageDamageTestsRelativeToOldFDA
    {
        #region Metadata 
        private static int impactAreaID = 34;
        #endregion
        #region Engineering Data 
        #region Discharge-Frequency Function 
        private static ContinuousDistribution LP3Distribution = new LogPearson3(3.7070, .240, -.4750, 99);
        #endregion
        #region Stage-Discharge Function  
        static string dischargeLabel = "Discharge";
        static string stageLabel = "Stage";
        static string ratingName = "Muncie White River Rating Curve";
        static CurveMetaData ratingMetaData = new CurveMetaData(dischargeLabel, stageLabel, ratingName);

        static double[] RatingCurveFlows = { 1166, 2000, 3000, 4000, 5320, 6000, 7000, 8175, 9000, 9995, 12175, 13706, 15157, 16962, 18278, 20000, 24000 };
        static IDistribution[] StageDistributions =
        {
            new Triangular(928.9, 929.21, 929.5),
            new Triangular(930.7, 931.4, 931.9),
            new Triangular(932, 932.6, 933.3),
            new Triangular(933, 933.6, 934.4),
            new Triangular(933.8, 934.5, 935.3),
            new Triangular(934.5, 935.2, 936.1),
            new Triangular(935, 935.8, 936.8),
            new Triangular(935.4, 936.5, 937.6),
            new Triangular(935.9, 937.1, 938.2),
            new Triangular(936.3, 937.6, 939),
            new Triangular(937.1, 938.6, 940.2),
            new Triangular(937.9, 939.7, 941.4),
            new Triangular(938.6, 940.7, 942.8),
            new Triangular(939.3, 941.6, 943.8),
            new Triangular(941.2, 942.1, 944.25),
            new Triangular(942.3, 943.2, 946.6),
            new Triangular(944.5, 944.6, 948.5)
        };

        static UncertainPairedData stageDischarge = new UncertainPairedData(RatingCurveFlows, StageDistributions, ratingMetaData);
        #endregion
        #region Hydraulic Dataset 
        private static float[] twoYearStages = new float[] { 929.1169F, 937.5078F, 929.05005F, 929.1169F, 928.1668F };
        private static double twoYearAEP = 0.5;
        private static DummyHydraulicProfile twoYearHydraulics = new DummyHydraulicProfile(twoYearStages, twoYearAEP);

        private static float[] fiveYearStages = new float[] { 932.1329F, 940.1999F, 933.8862F, 932.1329F, 931.8541F };
        private static double fiveYearAEP = 0.2;
        private static DummyHydraulicProfile fiveYearHydraulics = new DummyHydraulicProfile(fiveYearStages, fiveYearAEP);

        private static float[] tenYearStages = new float[] { 934.0325f, 941.5191f, 936.101f, 934.0325f, 933.5824f };
        private static double tenYearAEP = 0.1;
        private static DummyHydraulicProfile tenYearHydraulics = new DummyHydraulicProfile(tenYearStages, tenYearAEP);

        private static float[] twentyYearStages = new float[] { 935.3226f, 942.31323f, 937.36914f, 935.3226f, 934.6846f };
        private static double twentyYearAEP = 0.05;
        private static DummyHydraulicProfile twentyYearHydraulics = new DummyHydraulicProfile(twentyYearStages, twentyYearAEP);

        private static float[] fiftyYearStages = new float[] { 937.26263f, 943.48883f, 939.14996f, 937.26263f, 936.4082f };
        private static double fiftyYearAEP = 0.02;
        private static DummyHydraulicProfile fiftyYearHydraulics = new DummyHydraulicProfile(fiftyYearStages, fiftyYearAEP);

        private static float[] oneHundredYearStages = new float[] { 938.5266f, 944.268f, 940.27466f, 938.5266f, 937.5823f };
        private static double oneHundredYearAEP = 0.01;
        private static DummyHydraulicProfile oneHundredYearHydraulics = new DummyHydraulicProfile(oneHundredYearStages, oneHundredYearAEP);

        private static float[] twoHundredYearStages = new float[] { 939.4723f, 944.8663f, 941.0969f, 939.4723f, 938.4652f };
        private static double twoHundredYearAEP = 0.005;
        private static DummyHydraulicProfile twoHundredYearHydraulics = new DummyHydraulicProfile(twoHundredYearStages, twoHundredYearAEP);

        private static float[] fiveHundredYearStages = new float[] { 940.52136f, 945.49133f, 941.98926f, 940.52136f, 939.5152f };
        private static double fiveHundredAEP = 0.002;
        private static DummyHydraulicProfile fiveHundredYearHydraulics = new DummyHydraulicProfile(fiveHundredYearStages, fiveHundredAEP);

        private static List<IHydraulicProfile> dummyHydraulicProfiles = new List<IHydraulicProfile>() { twoYearHydraulics, fiveYearHydraulics, tenYearHydraulics, twentyYearHydraulics, fiftyYearHydraulics, oneHundredYearHydraulics, twoHundredYearHydraulics, fiveHundredYearHydraulics };
        private static HydraulicDataset hydraulicDataset = new HydraulicDataset(dummyHydraulicProfiles, Model.hydraulics.enums.HydraulicDataSource.SteadyHDF);
        #endregion
        #endregion
        #region Economic Data 
        #region Depth-Percent Damage Functions
        private static string depthLabel = "Depth Above First Floor Elevation";
        private static string percentDamageLabel = "Distribution Of Percent Damage";
        private static string depthPercentDamageName = "Depth Percent Damage Function";

        private static double[] resNoBasementDepths = new double[] { -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private static double[] resNoBasementStructureMeanPercentDamage = new double[] { 0, 2.5, 13.4, 23.3, 32.1, 40.1, 47.1, 53.2, 58.6, 63.2, 67.2, 70.5, 73.2, 75.4, 77.2, 78.5, 79.5, 80.2, 80.7 };
        private static double[] resNoBasementStructureStDevPercentDamage = new double[] { 0, 2.7, 2, 1.6, 1.6, 1.8, 1.9, 2, 2.1, 2.2, 2.3, 2.4, 2.7, 3, 3.3, 3.7, 4.1, 4.5, 4.9 };
        private static double[] resNoBasementContentMeanPercentDamage = new double[] { 0, 2.4, 8.1, 13.3, 17.9, 22, 25.7, 28.8, 31.5, 33.8, 35.7, 37.2, 38.4, 39.2, 39.7, 40, 40.1, 40.2, 40.3 };
        private static double[] resNoBasementContentStDevPercentDamage = new double[] { 0, 2.1, 1.5, 1.2, 1.2, 1.4, 1.5, 1.6, 1.6, 1.7, 1.8, 1.9, 2.1, 2.3, 2.6, 2.9, 3.2, 3.5, 3.8 };

        private static double[] resWithBasementDepths = new double[] { -8.1, -8, -7, -6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private static double[] resWithBasementStructureMeanPercentDamage = new double[] { 0, 1.6, 1.7, 1.9, 2.9, 4.7, 7.2, 10.2, 13.9, 17.9, 22.3, 27, 31.9, 36.9, 41.9, 46.9, 51.8, 56.4, 60.8, 64.8, 68.4, 71.4, 73.7, 75.4, 76.3, 76.4 };
        private static double[] resWithBasementStructureStDevPercentDamage = new double[] { 0, 2.7, 2.7, 2.11, 1.8, 1.66, 1.56, 1.47, 1.37, 1.32, 1.35, 1.5, 1.75, 2.04, 2.34, 2.63, 2.89, 3.13, 3.38, 3.71, 4.22, 5.02, 6.19, 7.79, 9.84, 12.36 };
        private static double[] resWithBasementContentMeanPercentDamage = new double[] {0, 0, 1, 2.3, 3.7, 5.2, 6.8, 8.4, 10.1, 11.9, 13.8, 15.7, 17.7, 19.8, 22, 24.3, 26.7, 29.1, 31.7, 34.4, 37.2, 40, 43, 46.1, 49.3, 52.6 };
        private static double[] resWithBasementContentStDevPercentDamage = new double[] {0, 0, 2.27, 1.76, 1.49, 1.37, 1.29, 1.21, 1.13, 1.09, 1.11, 1.23, 1.43, 1.67, 1.92, 2.15, 2.36, 2.56, 2.76, 3.04, 3.46, 4.12, 5.08, 6.39, 8.08, 10.15 };

        private static UncertainPairedData resNoBasementStructureDepthPercentDamage = NormallyDistDepthPercentDamage(resNoBasementDepths, resNoBasementStructureMeanPercentDamage, resNoBasementStructureStDevPercentDamage);
        private static UncertainPairedData resNoBasementContentDepthPercentDamage = NormallyDistDepthPercentDamage(resNoBasementDepths, resNoBasementContentMeanPercentDamage, resNoBasementContentStDevPercentDamage);
        private static UncertainPairedData resWithBasementStructureDepthPercentDamage = NormallyDistDepthPercentDamage(resWithBasementDepths, resWithBasementStructureMeanPercentDamage, resWithBasementStructureStDevPercentDamage);
        private static UncertainPairedData resWithBasementContentDepthPercentDamage = NormallyDistDepthPercentDamage(resWithBasementDepths, resWithBasementContentMeanPercentDamage, resWithBasementContentStDevPercentDamage);

        private static UncertainPairedData NormallyDistDepthPercentDamage(double[] depths, double[] meanPercentDamage, double[] stDevPercentDamage)
        {
            IDistribution[] normalDists = new IDistribution[depths.Length];
            for (int i = 0; i < meanPercentDamage.Length; i++)
            {
                Normal normal = new Normal(meanPercentDamage[i], stDevPercentDamage[i]);
                normalDists[i] = normal;
            }
            CurveMetaData curveMetaData = new CurveMetaData(depthLabel, percentDamageLabel, depthPercentDamageName);
            UncertainPairedData uncertainPairedData = new UncertainPairedData(depths, normalDists, curveMetaData);
            return uncertainPairedData;
        }

        private static double[] comStructureDepths = new double[] { -1.1, -1, -0.5, 0, 0.5, 1, 1.5, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static double[] comContentDepths = new double[] { 0, 0.5, 1, 1.5, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        private static double[] com1StructureMinPercentDamage = new double[] { 0, 0, 0, 0, 4.1, 5.6, 8.2, 12.4, 17.7, 22.9, 25.3, 31.8, 33.9, 39.3, 41.8, 42.9 };
        private static double[] com1StructureMostLikelyPercentDamage = new double[] { 0, 0.4, 0.5, 1.3, 7.6, 11.6, 15.7, 19.5, 25.9, 32.9, 35.3, 41.1, 44.6, 48.1, 51.4, 52.9 };
        private static double[] com1StructureMaxPercentDamage = new double[] { 0, 1.4, 1.4, 3.2, 13.7, 16.8, 23, 29.6, 35.9, 43.4, 45.9, 50.6, 55.6, 58.7, 61.5, 63.3 };

        private static double[] com1ContentMinPercentDamage = new double[] { 0, 17.9, 23.9, 30.7, 36.4, 45.3, 53.9, 67.1, 77.1, 92.3, 94.1, 95, 95 };
        private static double[] com1ContentMostLikelyPercentDamage = new double[] { 0, 24, 30.7, 36.8, 40.9, 52.9, 64, 75.4, 87.3, 98.9, 99.9, 99.9, 100 };
        private static double[] com1ContentMaxPercentDamage = new double[] { 0.2, 26.8, 35.9, 42.1, 48.6, 63.3, 74, 87.6, 94.7, 99.4, 100, 100, 100 };

        private static double[] com8StructureMinPercentDamage = new double[] { 0, 0, 0, 0, 6.1, 9.3, 13.9, 20.9, 26.9, 37.5, 44, 50.9, 54.6, 59.7, 64.2, 64.9 };
        private static double[] com8StructureMostLikelyPercentDamage = new double[] { 0, 0.2, 0.3, 0.6, 11.1, 15.9, 22.5, 28.7, 37.4, 47.3, 52.1, 58.3, 63.5, 67, 70.9, 72.2 };
        private static double[] com8StructureMaxPercentDamage = new double[] { 0, 1.1, 1.1, 2.1, 18.6, 23.9, 32, 42.4, 48.9, 58.1, 61.9, 67.4, 73.5, 76.6, 79, 79.6 };

        private static double[] com8ContentMinPercentDamage = new double[] { 0, 5, 15, 21.4, 28.7, 39.9, 49.4, 65.4, 72.9, 82.9, 88, 94.3, 94.3 };
        private static double[] com8ContentMostLikelyPercentDamage = new double[] { 0, 10.6, 21.3, 29.4, 38.6, 52.7, 62.6, 73, 79.3, 88.3, 94.9, 98.5, 98.6 };
        private static double[] com8ContentMaxPercentDamage = new double[] { 0, 16.4, 31.3, 38.4, 51.4, 62.3, 72.7, 79.6, 83.6, 95.7, 98, 98.6, 98.6 };

        private static UncertainPairedData com1StructureDepthPercent = TriangularDistDepthPercentDamage(comStructureDepths, com1StructureMinPercentDamage, com1StructureMostLikelyPercentDamage, com1StructureMaxPercentDamage);
        private static UncertainPairedData com1ContentDepthPercent = TriangularDistDepthPercentDamage(comContentDepths, com1ContentMinPercentDamage, com1ContentMostLikelyPercentDamage, com1ContentMaxPercentDamage);
        private static UncertainPairedData com8StructureDepthPercent = TriangularDistDepthPercentDamage(comStructureDepths, com8StructureMinPercentDamage, com8StructureMostLikelyPercentDamage, com8StructureMaxPercentDamage);
        private static UncertainPairedData com8ContentDepthPercent = TriangularDistDepthPercentDamage(comContentDepths, com8ContentMinPercentDamage, com8ContentMostLikelyPercentDamage, com8ContentMaxPercentDamage);

        private static UncertainPairedData TriangularDistDepthPercentDamage(double[] depths, double[] minPercentDamage, double[] mostLikelyPercentDamage, double[] maxPercentDamage)
        {
            IDistribution[] triangleDists = new IDistribution[depths.Length];
            for (int i = 0; i < minPercentDamage.Length; i++)
            {
                Triangular tri = new Triangular(minPercentDamage[i], mostLikelyPercentDamage[i], maxPercentDamage[i]);
                triangleDists[i] = tri;
            }
            CurveMetaData curveMetaData = new CurveMetaData(depthLabel, percentDamageLabel, depthPercentDamageName);
            UncertainPairedData uncertainPairedData = new UncertainPairedData(depths, triangleDists, curveMetaData);
            return uncertainPairedData;
        }
        #endregion
        #region Occupancy Types
        /// <summary>
        /// DIff Between 1.4.3 and 2.0 Occ Type Parameters There is a difference in HEC-FDA Version 2 when defining distributions about the content and other-to-value ratios. 
        /// 1. In HEC-FDA Version 1, the standard deviation, min, or max of the content (or other)-to-structure value ratio is given as a percent of the ratio, 
        /// whereas HEC-FDA Version 2 directly uses the standard deviation, min, or max. In HEC-FDA Version 1 for example, 
        /// for a content-to-structure value ratio of 50%, an entered standard deviation of 10% would mean that the plus/minus one standard deviation 
        /// range is 45% to 55%. In HEC-FDA Version 2, the standard deviation would be entered as 5% to obtain the same range. Importing data into 
        /// HEC-FDA Version 2 from HEC-FDA Version 1 corrects for this difference. 
        /// 2. Content- and other-to-structure value ratios are entered into HEC-FDA Version 2.0 as ratios, not in terms of percent.
        /// </summary>
        private static string residentialDamCat = "RES";
        private static string residentialNoBasementOccTypeName = "RES1-1SNB";
        private static string residentialWithBasementOccTypeName = "RES1-2SWB";
        private static string commercialDamCat = "COM";
        private static string com1OccTypeName = "COM1";
        private static string com8OccTypeName = "COM8";
        private static string structureAssetType = "Structure";
        private static string contentAssetType = "Content";

        private static ValueRatioWithUncertainty hundredCSVR = new ValueRatioWithUncertainty(IDistributionEnum.Normal, 10, 100);
        private static ValueRatioWithUncertainty eightySevenCSVR = new ValueRatioWithUncertainty(IDistributionEnum.Triangular, .85 * 87.5, 87.5, 1.07 * 87.5);
        private static ValueRatioWithUncertainty thirtyOneCSVR = new ValueRatioWithUncertainty(IDistributionEnum.Triangular, .939 * 31.1, 31.1, 1.054 * 31.1);
        private static FirstFloorElevationUncertainty halfFootFFEUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Normal, 0.5);
        private static ValueUncertainty triResStructureValUncertainty = new ValueUncertainty(IDistributionEnum.Triangular, 35.71, 128.57);
        private static ValueUncertainty triComStructureValueUncertainty = new ValueUncertainty(IDistributionEnum.Triangular, 61.54, 130.77);

        private static OccupancyType residentialNoBasementOccType = OccupancyType.builder()
            .withName(residentialNoBasementOccTypeName)
            .withDamageCategory(residentialDamCat)
            .withStructureDepthPercentDamage(resNoBasementStructureDepthPercentDamage)
            .withContentDepthPercentDamage(resNoBasementContentDepthPercentDamage)
            .withContentToStructureValueRatio(hundredCSVR)
            .withFirstFloorElevationUncertainty(halfFootFFEUncertainty)
            .withStructureValueUncertainty(triResStructureValUncertainty)
            .build();

        private static OccupancyType residentialWithBasementOccType = OccupancyType.builder()
            .withName(residentialWithBasementOccTypeName)
            .withDamageCategory(residentialDamCat)
            .withStructureDepthPercentDamage(resWithBasementStructureDepthPercentDamage)
            .withContentDepthPercentDamage(resWithBasementContentDepthPercentDamage)
            .withContentToStructureValueRatio(hundredCSVR)
            .withFirstFloorElevationUncertainty(halfFootFFEUncertainty)
            .withStructureValueUncertainty(triResStructureValUncertainty)
            .build();

        private static OccupancyType com1OccType = OccupancyType.builder()
            .withName(com1OccTypeName)
            .withDamageCategory(commercialDamCat)
            .withStructureDepthPercentDamage(com1StructureDepthPercent)
            .withContentDepthPercentDamage(com1ContentDepthPercent)
            .withContentToStructureValueRatio(eightySevenCSVR)
            .withFirstFloorElevationUncertainty(halfFootFFEUncertainty)
            .withStructureValueUncertainty(triComStructureValueUncertainty)
            .build();

        private static OccupancyType com8OccType = OccupancyType.builder()
            .withName(com8OccTypeName)
            .withDamageCategory(commercialDamCat)
            .withStructureDepthPercentDamage(com8StructureDepthPercent)
            .withContentDepthPercentDamage(com8ContentDepthPercent)
            .withContentToStructureValueRatio(thirtyOneCSVR)
            .withFirstFloorElevationUncertainty(halfFootFFEUncertainty)
            .withStructureValueUncertainty(triComStructureValueUncertainty)
            .build();

        private static List<OccupancyType> occupancyTypes = new List<OccupancyType>() { residentialNoBasementOccType, residentialWithBasementOccType, com1OccType, com8OccType };

        #endregion
        #region Structure Inventory 
        private static PointM pointM = new PointM();
        private static Structure structure1 = new Structure(fid: 26828269, point: pointM, firstFloorElevation: 938.5, val_struct: 170550.4, st_damcat: residentialDamCat, occtype: residentialWithBasementOccTypeName, impactAreaID: impactAreaID);
        private static Structure structure2 = new Structure(fid: 26829233, point: pointM, firstFloorElevation: 946.6839, val_struct: 104125.1, st_damcat: residentialDamCat, occtype: residentialNoBasementOccTypeName, impactAreaID: impactAreaID);
        private static Structure structure3 = new Structure(fid: 26831732, point: pointM, firstFloorElevation: 944.7132, val_struct: 312385.7, st_damcat: residentialDamCat, occtype: residentialNoBasementOccTypeName, impactAreaID: impactAreaID);
        private static Structure structure4 = new Structure(fid: 26855314, point: pointM, firstFloorElevation: 948.9375, val_struct: 514858.3, st_damcat: commercialDamCat, occtype: com1OccTypeName, impactAreaID: impactAreaID);
        private static Structure structure5 = new Structure(fid: 26860181, point: pointM, firstFloorElevation: 942.4375, val_struct: 125000, st_damcat: commercialDamCat, occtype: com8OccTypeName, impactAreaID: impactAreaID);
        private static List<Structure> structureList = new List<Structure>() { structure1, structure2, structure3, structure4, structure5 };

        private static string dummyPath = "dummy";
        private static StructureInventoryColumnMap map = new StructureInventoryColumnMap(null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        private static Inventory structureInventory = new Inventory(dummyPath, dummyPath, map, occupancyTypes, "header", false, dummyPath, structureList);
        #endregion
        #endregion
        #region Compute Objects
        private static ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 10, maxIterations: 50);
        private static ImpactAreaStageDamage impactAreaStageDamage = new ImpactAreaStageDamage(impactAreaID, structureInventory, hydraulicDataset, convergenceCriteria, dummyPath, analyticalFlowFrequency: LP3Distribution, dischargeStage: stageDischarge, usingMockData: true);

        #endregion
        #region Expected Values 
        private static double[] stageDamageStages = new double[] { 928.5, 929, 929.5, 930, 930.5, 931, 931.5, 932, 932.5, 933, 933.5, 934, 934.5, 935, 935.5, 936, 936.5, 937, 937.5, 938, 938.5, 939, 939.5, 940, 940.5, 941, 941.5, 942, 942.5, 943, 943.5, 944, 944.5, 945, 945.5, 946, 946.5, 947, 947.5, 948 };
        private static double[] structureMeanDamage = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 111.98186, 162.038841, 501.717253, 1188.46665, 1940.71634, 3262.01548, 4632.9304, 7684.02781, 9711.62137, 11992.6462, 14517.7268, 16738.7642, 19885.064, 21656.4401, 23476.6422, 25911.1439, 27445.1695, 29099.7589, 30612.6299, 31487.6761, 32406.983, 33420.1515, 34249.3219, 35149.4959, 35873.4693, 36662.6909, 37433.0798 };
        private static double[] contentMeanDamage = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 90.950323, 130.216348, 416.538758, 973.731958, 1611.29664, 2554.75195, 3691.44096, 5893.9883, 7428.50646, 8945.64473, 10719.954, 12371.4029, 14638.4233, 15876.4654, 17178.3933, 18816.6823, 19970.9823, 21011.3307, 21926.5472, 22463.9183, 23005.4741, 23532.3804, 23999.4502, 24471.074, 24812.7378, 25192.2645, 25583.4063 };
        private static string damageLabel = "Damage";
        private static string stageDamageName = "Aggregated Stage Damage Function";
        private static CurveMetaData stageDamageMetaData = new CurveMetaData(stageLabel, damageLabel, stageDamageName);
        private static PairedData structureStageDamage = new PairedData(stageDamageStages, structureMeanDamage, stageDamageMetaData);
        private static PairedData contentStageDamage = new PairedData(stageDamageStages, contentMeanDamage, stageDamageMetaData);

        #endregion
        #region Stage-Damage Integration Test 
        [Fact]
        public void StageDamageShouldMatchBetweenVersions()
        {
            int seed = 1234;
            RandomProvider randomProvider = new RandomProvider(seed);
            List<UncertainPairedData> stageDamageFunctions = impactAreaStageDamage.Compute(randomProvider);

            double relativeTolerance = 0.15;

            foreach (UncertainPairedData stageDamage in stageDamageFunctions)
            {
                IPairedData pairedData = stageDamage.SamplePairedData(0.5, true);
                if (stageDamage.CurveMetaData.DamageCategory == residentialDamCat)
                {
                    if (stageDamage.CurveMetaData.AssetCategory == structureAssetType)
                    {
                        for (int i = 0; i < structureStageDamage.Xvals.Length; i++)
                        {
                            double actualDamage = pairedData.f(structureStageDamage.Xvals[i]);
                            if (actualDamage > 0)
                            {
                                double relativeDifference = Math.Abs(actualDamage - structureStageDamage.Yvals[i])/structureStageDamage.Yvals[i];
                                Assert.True(relativeDifference < relativeTolerance);
                            }
                        }
                    }
                    if (stageDamage.CurveMetaData.AssetCategory == contentAssetType)
                    {
                        for (int i = 0; i < contentStageDamage.Xvals.Length; i++)
                        {
                            double actualDamage = pairedData.f(contentStageDamage.Xvals[i]);
                            if (actualDamage > 0)
                            {
                                double relativeDifference = Math.Abs(actualDamage - contentStageDamage.Yvals[i]) / contentStageDamage.Yvals[i];
                                Assert.True(relativeDifference < relativeTolerance);
                            }
                        }
                    }
                }

            }

        }
        #endregion
    }
}
