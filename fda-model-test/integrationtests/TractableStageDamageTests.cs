using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.structures;
using RasMapperLib;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ModelTest.integrationtests
{
    public class TractableStageDamageTests
    {
        #region H&H Data 
        private static double[] probabilities = new double[] { .99, .5, .2, .1, .04, .02, .01, .004, .002 };
        private static double[] stages = new double[] { 10, 12, 13, 14, 15, 16, 17, 18, 19 };
        private static int equivalentRecordLength = 50;

        private static List<float[]> ComputeStagesAtStructures()
        {
            List<float[]> stages = new List<float[]>();
            float[] stagesFirstProfile = new float[] { 13, 14, 15, 16 };
            stages.Add(stagesFirstProfile);

            for (int i = 0; i < probabilities.Length - 1; i++)
            {
                float[] profileStages = new float[4];
                for (int j = 0; j < 4; j++)
                {
                    profileStages[j] = stages[i][j] + 1;
                }
                stages.Add(profileStages);
            }
            return stages;
        }
        #endregion

        #region Occupancy Type Data
        static double[] depths = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        static IDistribution[] residentialStructureDamage = new IDistribution[] { new Deterministic(0), new Deterministic(10), new Deterministic(20), new Deterministic(30), new Deterministic(40), new Deterministic(50), new Deterministic(60), new Deterministic(70), new Deterministic(80), new Deterministic(90), new Deterministic(100) };
        static IDistribution[] residentialContentAndCommercialStructureDamage = new IDistribution[] { new Deterministic(0), new Deterministic(5), new Deterministic(15), new Deterministic(25), new Deterministic(35), new Deterministic(45), new Deterministic(55), new Deterministic(65), new Deterministic(75), new Deterministic(85), new Deterministic(95) };
        static IDistribution[] commericalContentDamage = new IDistribution[] { new Deterministic(0), new Deterministic(10), new Deterministic(20), new Deterministic(30), new Deterministic(40), new Deterministic(50), new Deterministic(60), new Deterministic(70), new Deterministic(80), new Deterministic(90) };
        static ValueRatioWithUncertainty residentialCSVR = new ValueRatioWithUncertainty(0.5);
        static ValueRatioWithUncertainty commercialCSVR = new ValueRatioWithUncertainty(1.2);
        static string residentialDamAndOccType = "Residential";
        static string commercialDamAndOccType = "Commercial";
        static string contentAssetType = "Content";
        static string structureAssetType = "Structure";
        static CurveMetaData residentialStructure = new CurveMetaData(residentialDamAndOccType, structureAssetType);
        static CurveMetaData residentialContent = new CurveMetaData(residentialDamAndOccType, contentAssetType);
        static CurveMetaData commercialStructure  = new CurveMetaData(commercialDamAndOccType, structureAssetType);
        static CurveMetaData commercialContent = new CurveMetaData(commercialDamAndOccType, contentAssetType);

        static UncertainPairedData residentialStructureDepthPercent = new UncertainPairedData(depths, residentialStructureDamage, residentialStructure);
        static UncertainPairedData residentialContentDepthPercent = new UncertainPairedData(depths, residentialContentAndCommercialStructureDamage, residentialContent);
        static UncertainPairedData commercialStructureDepthPercent = new UncertainPairedData(depths, residentialContentAndCommercialStructureDamage, commercialStructure);
        static UncertainPairedData commercialContentDepthPercent = new UncertainPairedData(depths, commericalContentDamage, commercialContent);
        
        static OccupancyType residentialOccType = OccupancyType.builder()
            .withName(residentialDamAndOccType)
            .withDamageCategory(residentialDamAndOccType)
            .withStructureDepthPercentDamage(residentialStructureDepthPercent)
            .withContentDepthPercentDamage(residentialContentDepthPercent)
            .withContentToStructureValueRatio(residentialCSVR)
            .build();

        static OccupancyType commercialOccType = OccupancyType.builder()
            .withName(commercialDamAndOccType)
            .withDamageCategory(commercialDamAndOccType)
            .withStructureDepthPercentDamage(commercialStructureDepthPercent)
            .withContentDepthPercentDamage(commercialContentDepthPercent)
            .withContentToStructureValueRatio(commercialCSVR)
            .build();

        static List<OccupancyType> occupancyTypes = new List<OccupancyType>() { residentialOccType, commercialOccType };
        #endregion

        #region Structure Data
        private static PointM pointM = new PointM();
        private static int impactAreaID = 34;

        private static Structure structure1 = new Structure(fid: 1, point: pointM, firstFloorElevation: 14, val_struct: 100, st_damcat: residentialDamAndOccType, occtype: residentialDamAndOccType, impactAreaID: impactAreaID);
        private static Structure structure2 = new Structure(fid: 2, point: pointM, firstFloorElevation: 15, val_struct: 200, st_damcat: residentialDamAndOccType, occtype: residentialDamAndOccType, impactAreaID: impactAreaID);
        private static Structure structure3 = new Structure(fid: 3, point: pointM, firstFloorElevation: 17, val_struct: 300, st_damcat: commercialDamAndOccType, occtype: commercialDamAndOccType, impactAreaID: impactAreaID);
        private static Structure structure4 = new Structure(fid: 4, point: pointM, firstFloorElevation: 18, val_struct: 400, st_damcat: commercialDamAndOccType, occtype: commercialDamAndOccType, impactAreaID: impactAreaID);
        private static List<Structure> structureList = new List<Structure>() { structure1, structure2, structure3, structure4};

        private static string dummyPath = "dummy";
        private static StructureInventoryColumnMap map = new StructureInventoryColumnMap(null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        private static Inventory structureInventory = new Inventory(dummyPath, dummyPath, map, occupancyTypes, "header", false, dummyPath, structureList);
        #endregion 




    }
}
