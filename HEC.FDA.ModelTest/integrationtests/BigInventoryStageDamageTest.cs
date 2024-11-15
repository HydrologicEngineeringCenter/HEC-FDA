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
using System.Linq;
using System.IO;

namespace HEC.FDA.ModelTest.integrationtests;

[Trait("RunsOn", "Local")]
[Collection("Serial")]
public class BigInventoryStageDamageTest
{
    #region Structure Data 

    private static int multiplier = 10000;
    private static PointM pointM = new PointM(); // These won't get used. We're gonna do some goofy stuff with fake hydraulics.
    private static List<double> firstFloorElevations = ExpandArray(new List<double> { 5.01, 6.01, 7.01, 8.01, 9.01});
    private static List<double> structureValues = ExpandArray(new List<double> { 500.01, 600.01, 700.01, 800.01, 900.01 });
    private static string residentialDamageCategory = "Residential";
    private static string commercialDamageCategory = "Commercial";
    private static string residentialNormalDistOccupancyTypeName = "Residential_One_Story_No_Basement_Normal";
    private static string commercialOccupancyTypeName = "Commercial_Warehouse";
    private static List<string> damageCategories = ExpandArray(new List<string> { residentialDamageCategory, residentialDamageCategory, residentialDamageCategory, residentialDamageCategory, residentialDamageCategory });
    private static List<string> occupancyTypes = ExpandArray(new List<string> { residentialNormalDistOccupancyTypeName, residentialNormalDistOccupancyTypeName, residentialNormalDistOccupancyTypeName, residentialNormalDistOccupancyTypeName, residentialNormalDistOccupancyTypeName });
    private static int impactAreaID = 1;
    private static ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria(minIterations: 20000, maxIterations: 50000);

    private static List<object> structureIDs = StructureIDs();

    private Inventory CreateInventory()
    {
        int numberOfStructures = structureValues.Count;
        List<Structure> structures = new List<Structure>();
        for (int i = 0; i < numberOfStructures; i++)
        {
            Structure structure = new Structure(structureIDs[i].ToString(), pointM, (double)firstFloorElevations[i], (double)structureValues[i], (string)damageCategories[i], (string)occupancyTypes[i], impactAreaID);
            structures.Add(structure);
        }
        Dictionary<string, OccupancyType> occupancyTypesList = new Dictionary<string, OccupancyType>()
            {
                {residentialNormalDistOccupancyTypeName, residentialOccupancyTypeNormalDists },
                {commercialOccupancyTypeName, commercialOccupancyTypeNormalDists }
            };

        Inventory inventory = new Inventory(occupancyTypesList, structures);
        return inventory;
    }

    private static List<object> StructureIDs()
    {
        List<object> ints = new List<object>();
        for (int i = 0; i < structureValues.Count; i++)
        {
            ints.Add(i);
        }
        return ints;
    }

    #endregion

    private static List<int> ExpandArray(List<int> inputList)
    {
        int newLength = inputList.Count * multiplier;
        int i = 0;

        List<int> result = new List<int>();

        while (i < newLength)
        {
            foreach (int item in inputList)
            {
                result.Add(item);
                i++;
            }
        }
        return result;

    }

    private static List<float> ExpandArray(List<float> inputList)
    {
        int newLength = inputList.Count * multiplier;
        int i = 0;

        List<float> result = new List<float>();

        while (i < newLength)
        {
            foreach (float item in inputList)
            {
                result.Add(item);
                i++;
            }
        }
        return result;

    }

    private static List<string> ExpandArray(List<string> inputList)
    {
        int newLength = inputList.Count * multiplier;
        int i = 0;

        List<string> result = new List<string>();

        while (i < newLength)
        {
            foreach (string item in inputList)
            {
                result.Add(item);
                i++;
            }
        }
        return result;

    }

    private static List<double> ExpandArray(List<double> inputList)
    {
        int newLength = inputList.Count * multiplier;
        int i = 0;

        List<double> result = new List<double>();

        while (i < newLength)
        {
            foreach (double item in inputList)
            {
                result.Add(item);
                i++;
            }
        }
        return result;

    }

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

    #region Water Data

    private const HydraulicDataSource hydraulicDataSource = HydraulicDataSource.SteadyHDF;
    private static DummyHydraulicProfile hydraulicProfile2 = new DummyHydraulicProfile(ExpandArray(new List<float> { 1, 1, 1, 1, 1 }).ToArray(), 0.5);
    private static DummyHydraulicProfile hydraulicProfile5 = new DummyHydraulicProfile((ExpandArray(new List<float> { 1, 1, 1, 1, 1 })).ToArray(), 0.2);
    private static DummyHydraulicProfile hydraulicProfile10 = new DummyHydraulicProfile((ExpandArray(new List<float> { 2, 2, 2, 2, 2 })).ToArray(), 0.1);
    private static DummyHydraulicProfile hydraulicProfile25 = new DummyHydraulicProfile((ExpandArray(new List<float> { 3, 3, 3, 3, 3 })).ToArray(), 0.04);
    private static DummyHydraulicProfile hydraulicProfile50 = new DummyHydraulicProfile((ExpandArray(new List<float> { 4, 4, 4, 4, 4 })).ToArray(), .02);
    private static DummyHydraulicProfile hydraulicProfile100 = new DummyHydraulicProfile((ExpandArray(new List<float> { 5, 5, 5, 5, 5 })).ToArray(), .01);
    private static DummyHydraulicProfile hydraulicProfile200 = new DummyHydraulicProfile((ExpandArray(new List<float> { 6, 6, 6, 6, 6 })).ToArray(), .005);
    private static DummyHydraulicProfile hydraulicProfile500 = new DummyHydraulicProfile((ExpandArray(new List<float> { 7, 7, 7, 7, 7 })).ToArray(), .002);
    private static List<IHydraulicProfile> hydraulicProfiles = new List<IHydraulicProfile>() { hydraulicProfile2, hydraulicProfile5, hydraulicProfile10, hydraulicProfile25, hydraulicProfile50, hydraulicProfile100, hydraulicProfile200, hydraulicProfile500 };
    private static HydraulicDataset hydraulicDataset = new HydraulicDataset(hydraulicProfiles, hydraulicDataSource);

    private static GraphicalUncertainPairedData stageFrequency = new GraphicalUncertainPairedData(new double[] { .5, .2, .1, .04, .02, .01, .005, .002 },
new double[] { 0, 1, 2, 3, 4, 5, 6, 7 }, 50, new CurveMetaData("Probability", "Stage", "Graphical Stage Frequency"), true);

    private static string filePath = @"C:\Temp\HEC-FDA\times.csv";

    #endregion

    [Fact]
    public void ComputeTime()
    {
        //Arrange
        Inventory inventory = CreateInventory();
        ImpactAreaStageDamage impactAreaStageDamage = new ImpactAreaStageDamage(impactAreaID, inventory, hydraulicDataset, String.Empty, graphicalFrequency: stageFrequency, usingMockData: true);
        List<ImpactAreaStageDamage> impactAreaStageDamageList = new List<ImpactAreaStageDamage>() { impactAreaStageDamage };
        ScenarioStageDamage scenarioStageDamage = new ScenarioStageDamage(impactAreaStageDamageList);

        string time = "the big inventory stage damage compute was started at: " + DateTime.Now.ToString();
        //Act

        
        List<UncertainPairedData> stageDamageFunctions = scenarioStageDamage.Compute().Item1;
        time += " and the compute was completed at: " + DateTime.Now.ToString();

        File.AppendAllText(filePath, time);

        //Assert
        //I choose to assert nothing. 
    }
}
