using System.Collections.Generic;
using HEC.FDA.Model.compute;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.stageDamage;
using HEC.FDA.Model.structures;
using Statistics;
using Statistics.Distributions;
using Xunit;
using RasMapperLib.Utilities;

namespace HEC.FDA.ModelTest.integrationtests
{   
    public class StageDamageResultsTest
    {
        private static string IANameColumnHeader = "Name";
        private static string pathToNSIShapefile = @"..\..\..\Resources\MuncieNSI\MuncieNSI.shp";
        private static string pathToIAShapefile = @"..\..\..\Resources\MuncieImpactAreas\ImpactAreas.shp";
        private static string pathToResult1 = @"Z:\Documents\Work\FDA2\FDA2.0\Studies\Muncie\ExampleStudyDataForQSG\ExampleStudyDataForQSG\Muncie\Hydraulics\Outputs\Native Output Files\Muncie.p01.hdf";
        private static string pathToResult2 = @"Z:\Documents\Work\FDA2\FDA2.0\Studies\Muncie\ExampleStudyDataForQSG\ExampleStudyDataForQSG\Muncie\Hydraulics\Outputs\Native Output Files\Muncie.p02.hdf";
        private static string pathToResult3 = @"Z:\Documents\Work\FDA2\FDA2.0\Studies\Muncie\ExampleStudyDataForQSG\ExampleStudyDataForQSG\Muncie\Hydraulics\Outputs\Native Output Files\Muncie.p03.hdf";
        private static string pathToResult4 = @"Z:\Documents\Work\FDA2\FDA2.0\Studies\Muncie\ExampleStudyDataForQSG\ExampleStudyDataForQSG\Muncie\Hydraulics\Outputs\Native Output Files\Muncie.p04.hdf";
        private static string pathToResult5 = @"Z:\Documents\Work\FDA2\FDA2.0\Studies\Muncie\ExampleStudyDataForQSG\ExampleStudyDataForQSG\Muncie\Hydraulics\Outputs\Native Output Files\Muncie.p05.hdf";
        private static string pathToResult6 = @"Z:\Documents\Work\FDA2\FDA2.0\Studies\Muncie\ExampleStudyDataForQSG\ExampleStudyDataForQSG\Muncie\Hydraulics\Outputs\Native Output Files\Muncie.p06.hdf";
        private static string pathToResult7 = @"Z:\Documents\Work\FDA2\FDA2.0\Studies\Muncie\ExampleStudyDataForQSG\ExampleStudyDataForQSG\Muncie\Hydraulics\Outputs\Native Output Files\Muncie.p07.hdf";
        private static string pathToResult8 = @"Z:\Documents\Work\FDA2\FDA2.0\Studies\Muncie\ExampleStudyDataForQSG\ExampleStudyDataForQSG\Muncie\Hydraulics\Outputs\Native Output Files\Muncie.p08.hdf";
        private static string pathToTerrain = @"..\..\..\Resources\MuncieTerrain\Terrain (1)_30ft_clip.hdf";//Not being used?
        private static string hydroParentDirectory = @"..\..\..\Resources\MuncieResult";

        private static StructureInventoryColumnMap map = new StructureInventoryColumnMap();

        private static double[] IND1StructDepths = new double[] { -1.1, -1, -.5, 0, .5, 1, 1.5, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static double[] IND1ContDepths = new double[] { 0, .5, 1, 1.5, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static IDistribution[] IND1StructPercentDamages = new IDistribution[]
        {
            new Triangular(0,0, 0),
            new Triangular(0, .4, 1.2),
            new Triangular(0, .5, 1.4),
            new Triangular(0, 1.1, 3.3),
            new Triangular(3.5, 7.6, 14),
            new Triangular(5.1, 11.8, 17.4),
            new Triangular(7.6, 16.1, 23.6),
            new Triangular(11.7, 19.9, 28.8),
            new Triangular(16.4, 25.4, 34.2),
            new Triangular(21.2, 31.4, 42.5),
            new Triangular(22.3, 34.2, 44.7),
            new Triangular(28.3, 39, 48.9),
            new Triangular(29.9, 41.8, 52.7),
            new Triangular(34.5, 45.7, 56.9),
            new Triangular(37.6, 50.4, 60.6),
            new Triangular(38.7, 51.7, 62.2)
        };

        private static IDistribution[] IND1ContPercentDamages = new IDistribution[]
        {
            new Triangular(0,0, 0),
            new Triangular(7.1, 13.4, 21.1),
            new Triangular(12.3, 20.7, 28),
            new Triangular(19.3, 27.6, 35.6),
            new Triangular(25.4, 33.7, 45.6),
            new Triangular(35.7, 47.4, 57),
            new Triangular(48.3, 56.9, 67.7),
            new Triangular(57.3, 65.6, 76),
            new Triangular(65.9, 73.6, 82.4),
            new Triangular(74.9, 81.3, 89.7),
            new Triangular(81.4, 88.4, 94.1),
            new Triangular(84.1, 91.6, 98.3),
            new Triangular(88.1, 93.6, 99.3)
        };

        private static CurveMetaData metaData = new CurveMetaData("Depths", "Percent Damage", "Depth-Percent Damage Function");
        private static UncertainPairedData _StructureDepthPercentDamageFunction = new UncertainPairedData(IND1StructDepths, IND1StructPercentDamages, metaData);
        private static UncertainPairedData _ContentDepthPercentDamageFunction = new UncertainPairedData(IND1ContDepths, IND1ContPercentDamages, metaData);
        private static FirstFloorElevationUncertainty firstFloorElevationUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Normal, 0.5);
        private static ValueUncertainty _structureValueUncertainty = new ValueUncertainty(IDistributionEnum.Triangular, 30.77, 38.45);
        private static ValueRatioWithUncertainty _contentToStructureValueRatio = new ValueRatioWithUncertainty(IDistributionEnum.Triangular, 36.2, 46.8, 53.5);//T 46.8 10.6 6.7
        private static int seed = 1234;
        private static RandomProvider randomProvider = new RandomProvider(seed);
        private static string name = "IND1";
        private static string damageCategory = "IND";

        private static OccupancyType occupancyTypeAuto = OccupancyType.builder()
            .withName("Auto")
            .withDamageCategory("Auto")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeCOM1 = OccupancyType.builder()
            .withName("COM1")
            .withDamageCategory("COM")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeCOM10 = OccupancyType.builder()
            .withName("COM10")
            .withDamageCategory("COM")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeCOM2 = OccupancyType.builder()
            .withName("COM2")
            .withDamageCategory("COM")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeCOM3 = OccupancyType.builder()
            .withName("COM3")
            .withDamageCategory("COM")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeCOM4 = OccupancyType.builder()
            .withName("COM4")
            .withDamageCategory("COM")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeCOM5 = OccupancyType.builder()
            .withName("COM5")
            .withDamageCategory("COM")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeCOM6 = OccupancyType.builder()
            .withName("COM6")
            .withDamageCategory("COM")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeCOM7 = OccupancyType.builder()
            .withName("COM7")
            .withDamageCategory("COM")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeCOM8 = OccupancyType.builder()
            .withName("COM8")
            .withDamageCategory("COM")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeCOM9 = OccupancyType.builder()
            .withName("COM9")
            .withDamageCategory("COM")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeEDU1 = OccupancyType.builder()
            .withName("EDU1")
            .withDamageCategory("PUB")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeEDU2 = OccupancyType.builder()
            .withName("EDU2")
            .withDamageCategory("PUB")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeGOV1 = OccupancyType.builder()
            .withName("GOV1")
            .withDamageCategory("GOV")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeGOV2 = OccupancyType.builder()
            .withName("GOV2")
            .withDamageCategory("GOV")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeIND1 = OccupancyType.builder()
            .withName(name)
            .withDamageCategory(damageCategory)
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeIND2 = OccupancyType.builder()
            .withName("IND2")
            .withDamageCategory(damageCategory)
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeIND3 = OccupancyType.builder()
            .withName("IND3")
            .withDamageCategory(damageCategory)
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeIND4 = OccupancyType.builder()
            .withName("IND4")
            .withDamageCategory(damageCategory)
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeIND5 = OccupancyType.builder()
            .withName("IND5")
            .withDamageCategory(damageCategory)
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeIND6 = OccupancyType.builder()
            .withName("IND6")
            .withDamageCategory(damageCategory)
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeREL1 = OccupancyType.builder()
            .withName("REL1")
            .withDamageCategory("PUB")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES11SNB = OccupancyType.builder()
            .withName("RES1-1SNB")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES11SWB = OccupancyType.builder()
            .withName("RES1-1SWB")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES12SNB = OccupancyType.builder()
            .withName("RES1-2SNB")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES12SWB = OccupancyType.builder()
            .withName("RES1-2SWB")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES13SNB = OccupancyType.builder()
            .withName("RES1-3SNB")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES13SWB = OccupancyType.builder()
            .withName("RES1-3SWB")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES1SLNB = OccupancyType.builder()
            .withName("RES1-SLNB")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES1SLWB = OccupancyType.builder()
            .withName("RES1-SLWB")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES2 = OccupancyType.builder()
            .withName("RES2")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES3 = OccupancyType.builder()
            .withName("RES3")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES3A = OccupancyType.builder()
            .withName("RES3A")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES3B = OccupancyType.builder()
            .withName("RES3B")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES3C = OccupancyType.builder()
            .withName("RES3C")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES3D = OccupancyType.builder()
            .withName("RES3D")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES3E = OccupancyType.builder()
            .withName("RES3E")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES3F = OccupancyType.builder()
            .withName("RES3F")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES4 = OccupancyType.builder()
            .withName("RES4")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES5 = OccupancyType.builder()
            .withName("RES5")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeRES6 = OccupancyType.builder()
            .withName("RES6")
            .withDamageCategory("RES")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static List<OccupancyType> occTypes = new List<OccupancyType>() 
        {
            occupancyTypeAuto,
            occupancyTypeIND1, 
            occupancyTypeIND2, 
            occupancyTypeIND3, 
            occupancyTypeIND4, 
            occupancyTypeIND5, 
            occupancyTypeIND6, 
            occupancyTypeCOM1, 
            occupancyTypeCOM2, 
            occupancyTypeCOM3, 
            occupancyTypeCOM4, 
            occupancyTypeCOM5,
            occupancyTypeCOM6,
            occupancyTypeCOM7,
            occupancyTypeCOM8,
            occupancyTypeCOM9,
            occupancyTypeCOM10,
            occupancyTypeEDU1,
            occupancyTypeEDU2,
            occupancyTypeGOV1,
            occupancyTypeGOV2,
            occupancyTypeRES11SNB,
            occupancyTypeRES11SWB,
            occupancyTypeRES12SNB,
            occupancyTypeRES12SWB,
            occupancyTypeRES13SNB,
            occupancyTypeRES13SWB,
            occupancyTypeRES1SLNB,
            occupancyTypeRES1SLWB,
            occupancyTypeRES2,
            occupancyTypeRES3,
            occupancyTypeRES3A,
            occupancyTypeRES3B,
            occupancyTypeRES3C,
            occupancyTypeRES3D,
            occupancyTypeRES3E,
            occupancyTypeRES3F,
            occupancyTypeRES4,
            occupancyTypeRES5,
            occupancyTypeRES6
        };

        private static Inventory inventory = new Inventory(pathToNSIShapefile, pathToIAShapefile, map, occTypes, IANameColumnHeader);

        static ContinuousDistribution LP3Distribution = new LogPearson3(3.7070, .240, -.4750, 99);
        static double[] RatingCurveFlows = { 1166, 2000, 3000, 4000, 5320, 6000, 7000, 8175, 9000, 9995, 12175, 13706, 15157, 16962, 18278, 20000, 24000 };

        static string xLabel = "Discharge";
        static string yLabel = "Stage";
        static string name1 = "Muncie White River";
        static int impactAreaID = 1;
        static CurveMetaData metaData1 = new CurveMetaData(xLabel, yLabel, name1 );


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

        static UncertainPairedData stageDischarge = new UncertainPairedData(RatingCurveFlows, StageDistributions, metaData1);

        private static HydraulicProfile profileFiftyPercent = new HydraulicProfile(.2, pathToResult1, "Max");
        private static HydraulicProfile profileTwentyPercent = new HydraulicProfile(.1, pathToResult2, "Max");
        private static HydraulicProfile profileTenPercent = new HydraulicProfile(.05, pathToResult3, "Max");
        private static HydraulicProfile profileFivePercent = new HydraulicProfile(.5, pathToResult4, "Max");
        private static HydraulicProfile profileTwoPercent = new HydraulicProfile(.02, pathToResult5, "Max");
        private static HydraulicProfile profileOnePercent = new HydraulicProfile(.01, pathToResult6, "Max");
        private static HydraulicProfile profilePointFivePercent = new HydraulicProfile(.005, pathToResult7, "Max");
        private static HydraulicProfile profilePointTwoPercent = new HydraulicProfile(.002, pathToResult8, "Max");

        private static List<HydraulicProfile> hydraulicDataSetList = new List<HydraulicProfile>() 
        { 
            profileFiftyPercent, 
            profileTwentyPercent, 
            profileTenPercent, 
            profileFivePercent, 
            profileTwoPercent, 
            profileOnePercent, 
            profilePointFivePercent, 
            profilePointTwoPercent 
        };

        private static HydraulicDataset hydraulicDataset = new HydraulicDataset(hydraulicDataSetList, HydraulicDataSource.UnsteadyHDF);

        private static int stageImpactAreaID = 1;

        private static ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();

        //[Fact] based on what I've read, think I need Theory here and not Fact
        [Theory]
        [InlineData(1234, 3452605.18)]//this is mean damages at stage of 940ft for IND damcat
        public void StageDamageShould(int seed, double expectedDamage) 
        {

            ImpactAreaStageDamage stageDamageObject = new ImpactAreaStageDamage(stageImpactAreaID, inventory, hydraulicDataset, convergenceCriteria, hydroParentDirectory, LP3Distribution, dischargeStage:stageDischarge);
            List<ImpactAreaStageDamage> stageDamageObjectList = new List<ImpactAreaStageDamage>() { stageDamageObject };
            ScenarioStageDamage scenarioStageDamage = new ScenarioStageDamage(stageDamageObjectList);

            List<UncertainPairedData> stageDamageFunctions = scenarioStageDamage.Compute(randomProvider, convergenceCriteria);

            double actualDamageAtGivenStage = 0;
            double givenStage = 940;

            foreach (UncertainPairedData currentUncertainPairedData in stageDamageFunctions) 
            {
                if (currentUncertainPairedData.ImpactAreaID.Equals(impactAreaID))
                {
                    if (currentUncertainPairedData.DamageCategory.Equals(damageCategory))
                    {
                        IPairedData stageDamagePairedData = UncertainPairedData.ConvertToPairedDataAtMeans(currentUncertainPairedData);
                        actualDamageAtGivenStage += stageDamagePairedData.f(givenStage);
                    }
                }
            }

            double tolerance = .05;
            double difference = actualDamageAtGivenStage - expectedDamage;
            double relDiff = System.Math.Abs(difference/expectedDamage);
            Assert.True(relDiff < tolerance);

        }
    }
}

