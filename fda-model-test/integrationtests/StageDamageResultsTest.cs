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
using System.Linq;
using RasMapperLib.Utilities;
using HEC.FDA.Model.hydraulics.Interfaces;

namespace HEC.FDA.ModelTest.integrationtests
{
    [Trait("Category", "Disk")]
    public class StageDamageResultsTest
    {
        //TODO: This test class is incomplete 
        private static string IANameColumnHeader = "Name";
        private static string pathToNSIShapefile = @"..\..\..\fda-model-test\Resources\MuncieNSI\Muncie-SI_CRS2965.shp";
        private static string pathToIAShapefile = @"..\..\..\fda-model-test\Resources\MuncieImpactAreas\ImpactAreas.shp";
        private static string pathToTerrain = @"..\..\..\fda-model-test\Resources\MuncieTerrain\Terrain (1)_30ft_clip.hdf";//Not being used?


        //water data
        private const string ParentDirectoryToSteadyResult = @"..\..\..\fda-model-test\Resources\MuncieSteadyResult";
        private const string SteadyHDFFileName = @"Muncie.p10.hdf";
        private const string Name2 = "2";
        private const string Name5 = "5";
        private const string Name10 = "10";
        private const string Name25 = "25";
        private const string Name50 = "50";
        private const string Name100 = "100";
        private const string Name200 = "200";
        private const string Name500 = "500";
        private const HydraulicDataSource hydraulicDataSource = HydraulicDataSource.SteadyHDF;

        private static HydraulicProfile hydraulicProfile2 = new HydraulicProfile(0.5, SteadyHDFFileName, Name2);
        private static HydraulicProfile hydraulicProfile5 = new HydraulicProfile(0.2, SteadyHDFFileName, Name5);
        private static HydraulicProfile hydraulicProfile10 = new HydraulicProfile(0.1, SteadyHDFFileName, Name10);
        private static HydraulicProfile hydraulicProfile25 = new HydraulicProfile(0.04, SteadyHDFFileName, Name25);
        private static HydraulicProfile hydraulicProfile50 = new HydraulicProfile(.02, SteadyHDFFileName, Name50);
        private static HydraulicProfile hydraulicProfile100 = new HydraulicProfile(.01, SteadyHDFFileName, Name100);
        private static HydraulicProfile hydraulicProfile200 = new HydraulicProfile(.005, SteadyHDFFileName, Name200);
        private static HydraulicProfile hydraulicProfile500 = new HydraulicProfile(.002, SteadyHDFFileName, Name500);
        private static List<HydraulicProfile> hydraulicProfiles = new List<HydraulicProfile>() { hydraulicProfile2, hydraulicProfile5, hydraulicProfile10, hydraulicProfile25, hydraulicProfile50, hydraulicProfile100, hydraulicProfile200, hydraulicProfile500 };
        private static HydraulicDataset hydraulicDataset = new HydraulicDataset(hydraulicProfiles.Cast<IHydraulicProfile>().ToList(), hydraulicDataSource);

        private static StructureInventoryColumnMap map = new StructureInventoryColumnMap(null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        //Begin IND1 DDF - The FDA1 model does not include any IND damages, so this will be used for all nonPUB DDFs. Didn't want to delete this in case we use later
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
        private static string name = "IND1";
        private static string damageCategory = "IND";
        private static int seed = 1234;
        private static RandomProvider randomProvider = new RandomProvider(seed);
        //End IND1 and Begin PUB DDFs, which is the asset class used in this test 
        private static double[] EDU1StructDepths = new double[] { -1.1, -1, -.5, 0, .5, 1, 1.5, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static double[] EDU1ContDepths = new double[] { 0, .5, 1, 1.5, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static IDistribution[] EDU1StructPercentDamages = new IDistribution[]
        {
            new Triangular(0,0, 0),
            new Triangular(0, .2, .6),
            new Triangular(0, .3, 1),
            new Triangular(0, .6, 2.3),
            new Triangular(5, 9.9, 16.3),
            new Triangular(8.1, 14.2, 20),
            new Triangular(11.4, 18.1, 25.7),
            new Triangular(16.9, 23.9, 32.9),
            new Triangular(22.5, 30.2, 38.2),
            new Triangular(29.8, 36.9, 44.7),
            new Triangular(32.6, 39.4, 46),
            new Triangular(39, 43.9, 51.3),
            new Triangular(40.5, 45.8, 52.9),
            new Triangular(43.2, 48.4, 56.3),
            new Triangular(45, 51.1, 60),
            new Triangular(46.4, 52.8, 62.4)
        };

        private static IDistribution[] EDU1ContPercentDamages = new IDistribution[]
        {
            new Triangular(0,0, 0),
            new Triangular(9.4, 14.3, 19.6),
            new Triangular(16.7, 21.7, 29.3),
            new Triangular(19.7, 26.6, 34.3),
            new Triangular(23.4, 30.4, 37.7),
            new Triangular(31.4, 39, 45.4),
            new Triangular(38, 45, 50.7),
            new Triangular(43.3, 47.9, 52.7),
            new Triangular(48.6, 51.9, 55.4),
            new Triangular(52.1, 55.7, 60.1),
            new Triangular(55.7, 59.3, 61.7),
            new Triangular(57.3, 60.6, 64.9),
            new Triangular(58.6, 63.4, 67.7)
        };

        private static CurveMetaData EDU1metaData = new CurveMetaData("Depths", "Percent Damage", "Depth-Percent Damage Function");
        private static UncertainPairedData _EDU1StructureDepthPercentDamageFunction = new UncertainPairedData(EDU1StructDepths, EDU1StructPercentDamages, EDU1metaData);
        private static UncertainPairedData _EDU1ContentDepthPercentDamageFunction = new UncertainPairedData(EDU1ContDepths, EDU1ContPercentDamages, EDU1metaData);
        private static FirstFloorElevationUncertainty EDU1firstFloorElevationUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Normal, 0.5);
        private static ValueUncertainty _EDU1structureValueUncertainty = new ValueUncertainty(IDistributionEnum.Triangular, 30.77, 38.46);
        private static ValueRatioWithUncertainty _EDU1contentToStructureValueRatio = new ValueRatioWithUncertainty(IDistributionEnum.Triangular, 5, 7.3, 10.5);//T 7.3 2.3 3.2

        private static double[] GOV1StructDepths = new double[] { -1.1, -1, -.5, 0, .5, 1, 1.5, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static double[] GOV1ContDepths = new double[] { 0, .5, 1, 1.5, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static IDistribution[] GOV1StructPercentDamages = new IDistribution[]
        {
            new Triangular(0,0, 0),
            new Triangular(0, .5, 1.2),
            new Triangular(0, .6, 1.4),
            new Triangular(0, 1.1, 3.3),
            new Triangular(3.5, 7.1, 12.9),
            new Triangular(4.9, 10.5, 15.9),
            new Triangular(6.9, 14, 21),
            new Triangular(10.8, 17.3, 25.9),
            new Triangular(15.1, 22.3, 31),
            new Triangular(19.6, 28.2, 38.5),
            new Triangular(22, 32.1, 42.6),
            new Triangular(27.1, 35.5, 45.7),
            new Triangular(30.3, 38.9, 50.5),
            new Triangular(35.1, 42.7, 54.4),
            new Triangular(37, 45.7, 57.1),
            new Triangular(38.6, 47.1, 58.6)
        };

        private static IDistribution[] GOV1ContPercentDamages = new IDistribution[]
        {
            new Triangular(0,0, 0),
            new Triangular(5, 5, 10),
            new Triangular(7, 15, 25),
            new Triangular(10, 20, 30),
            new Triangular(15, 25, 35),
            new Triangular(30, 40, 50),
            new Triangular(44, 50, 60),
            new Triangular(50, 58, 72),
            new Triangular(50, 65, 80),
            new Triangular(70, 78, 90),
            new Triangular(80, 89.9, 100),
            new Triangular(80, 90, 100),
            new Triangular(86, 92, 100)
        };

        private static CurveMetaData GOV1metaData = new CurveMetaData("Depths", "Percent Damage", "Depth-Percent Damage Function");
        private static UncertainPairedData _GOV1StructureDepthPercentDamageFunction = new UncertainPairedData(GOV1StructDepths, GOV1StructPercentDamages, GOV1metaData);
        private static UncertainPairedData _GOV1ContentDepthPercentDamageFunction = new UncertainPairedData(GOV1ContDepths, GOV1ContPercentDamages, GOV1metaData);
        private static FirstFloorElevationUncertainty GOV1firstFloorElevationUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Normal, 0.5);
        private static ValueUncertainty _GOV1structureValueUncertainty = new ValueUncertainty(IDistributionEnum.Triangular, 30.77, 38.45);
        private static ValueRatioWithUncertainty _GOV1contentToStructureValueRatio = new ValueRatioWithUncertainty(IDistributionEnum.Triangular, 75, 88, 92.2);//T 88 13 4.2

        private static double[] REL1StructDepths = new double[] { -1.1, -1, -.5, 0, .5, 1, 1.5, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static double[] REL1ContDepths = new double[] { 0, .5, 1, 1.5, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static IDistribution[] REL1StructPercentDamages = new IDistribution[]
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

        private static IDistribution[] REL1ContPercentDamages = new IDistribution[]
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

        private static CurveMetaData REL1metaData = new CurveMetaData("Depths", "Percent Damage", "Depth-Percent Damage Function");
        private static UncertainPairedData _REL1StructureDepthPercentDamageFunction = new UncertainPairedData(REL1StructDepths, REL1StructPercentDamages, REL1metaData);
        private static UncertainPairedData _REL1ContentDepthPercentDamageFunction = new UncertainPairedData(REL1ContDepths, REL1ContPercentDamages, REL1metaData);
        private static FirstFloorElevationUncertainty REL1firstFloorElevationUncertainty = new FirstFloorElevationUncertainty(IDistributionEnum.Normal, 0.5);
        private static ValueUncertainty _REL1structureValueUncertainty = new ValueUncertainty(IDistributionEnum.Triangular, 30.77, 38.46);
        private static ValueRatioWithUncertainty _REL1contentToStructureValueRatio = new ValueRatioWithUncertainty(IDistributionEnum.Triangular, 36.2, 46.8, 53.5);//T 46.8 10.6 6.7

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
            .withStructureDepthPercentDamage(_EDU1StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_EDU1ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(EDU1firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_EDU1structureValueUncertainty)
            .withContentToStructureValueRatio(_EDU1contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeEDU2 = OccupancyType.builder()
            .withName("EDU2")
            .withDamageCategory("PUB")
            .withStructureDepthPercentDamage(_EDU1StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_EDU1ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(EDU1firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_EDU1structureValueUncertainty)
            .withContentToStructureValueRatio(_EDU1contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeGOV1 = OccupancyType.builder()
            .withName("GOV1")
            .withDamageCategory("PUB")
            .withStructureDepthPercentDamage(_GOV1StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_GOV1ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(GOV1firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_GOV1structureValueUncertainty)
            .withContentToStructureValueRatio(_GOV1contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeGOV2 = OccupancyType.builder()
            .withName("GOV2")
            .withDamageCategory("PUB")
            .withStructureDepthPercentDamage(_GOV1StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_GOV1ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(GOV1firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_GOV1structureValueUncertainty)
            .withContentToStructureValueRatio(_GOV1contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeIND1 = OccupancyType.builder()
            .withName("IND1")
            .withDamageCategory("IND")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeIND2 = OccupancyType.builder()
            .withName("IND2")
            .withDamageCategory("IND")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeIND3 = OccupancyType.builder()
            .withName("IND3")
            .withDamageCategory("IND")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeIND4 = OccupancyType.builder()
            .withName("IND4")
            .withDamageCategory("IND")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeIND5 = OccupancyType.builder()
            .withName("IND5")
            .withDamageCategory("IND")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeIND6 = OccupancyType.builder()
            .withName("IND6")
            .withDamageCategory("IND")
            .withStructureDepthPercentDamage(_StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_structureValueUncertainty)
            .withContentToStructureValueRatio(_contentToStructureValueRatio)
            .build();

        private static OccupancyType occupancyTypeREL1 = OccupancyType.builder()
            .withName("REL1")
            .withDamageCategory("PUB")
            .withStructureDepthPercentDamage(_REL1StructureDepthPercentDamageFunction)
            .withContentDepthPercentDamage(_REL1ContentDepthPercentDamageFunction)
            .withFirstFloorElevationUncertainty(REL1firstFloorElevationUncertainty)
            .withStructureValueUncertainty(_REL1structureValueUncertainty)
            .withContentToStructureValueRatio(_REL1contentToStructureValueRatio)
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
            occupancyTypeREL1,
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

        private static Inventory inventory = new Inventory(pathToNSIShapefile, pathToIAShapefile, map, occTypes, IANameColumnHeader, false, null);

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

        private static int stageImpactAreaID = 1;

        private static ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();

        [Theory]
        [InlineData(1234, 12704029.1)]//this is mean damages at stage of 940ft for PUB damcat
        public void StageDamageShould(int seed, double expectedDamage) 
        {

            ImpactAreaStageDamage stageDamageObject = new ImpactAreaStageDamage(stageImpactAreaID, inventory, hydraulicDataset, convergenceCriteria, ParentDirectoryToSteadyResult, LP3Distribution, dischargeStage:stageDischarge);
            List<ImpactAreaStageDamage> stageDamageObjectList = new List<ImpactAreaStageDamage>() { stageDamageObject };
            ScenarioStageDamage scenarioStageDamage = new ScenarioStageDamage(stageDamageObjectList);

            List<UncertainPairedData> stageDamageFunctions = scenarioStageDamage.Compute(randomProvider, convergenceCriteria);

            double actualDamageAtGivenStage = 0;
            double givenStage = 940;

            foreach (UncertainPairedData currentUncertainPairedData in stageDamageFunctions) 
            {
                if (currentUncertainPairedData.ImpactAreaID.Equals(impactAreaID))
                {
                    if (currentUncertainPairedData.DamageCategory.Equals("PUB"))
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

