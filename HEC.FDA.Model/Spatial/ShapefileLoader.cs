using Geospatial.GDALAssist;
using RasMapperLib;
using System;
using System.Collections.Generic;
using System.Linq;
using HEC.FDA.Model.utilities;
using HEC.FDA.Model.structures;

namespace HEC.FDA.Model.Spatial;
public class ShapefileLoader
{
    private const string DEFAULT_MISSING_STRING_VALUE = "EMPTY";
    private const int DEFAULT_MISSING_NUMBER_VALUE = IntegerGlobalConstants.DEFAULT_MISSING_VALUE;
    private const string UNUSED_STRING_VALUE = "";

    public static readonly Dictionary<string, Type> _expectedTypes = new()
            {
                { StructureSelectionMapping.STRUCTURE_ID, typeof(string)},
                { StructureSelectionMapping.OCCUPANCY_TYPE, typeof(string) },
                { StructureSelectionMapping.FIRST_FLOOR_ELEV, typeof(double) },
                { StructureSelectionMapping.STRUCTURE_VALUE, typeof(double) },
                { StructureSelectionMapping.FOUNDATION_HEIGHT, typeof(double) },
                { StructureSelectionMapping.GROUND_ELEV, typeof(double) },
                { StructureSelectionMapping.CONTENT_VALUE, typeof(double) },
                { StructureSelectionMapping.OTHER_VALUE, typeof(double) },
                { StructureSelectionMapping.VEHICLE_VALUE, typeof(double) },
                { StructureSelectionMapping.BEG_DAMAGE_DEPTH, typeof(double) },
                { StructureSelectionMapping.YEAR_IN_CONSTRUCTION, typeof(int) },
                { StructureSelectionMapping.NOTES, typeof(string) },
                { StructureSelectionMapping.DESCRIPTION, typeof(string) },
                { StructureSelectionMapping.NUMBER_OF_STRUCTURES, typeof(int) }
            };

    public static IReadOnlyDictionary<string, Type> ExpectedTypes
    {
        get { return _expectedTypes; }
    }
    public static List<Structure> LoadStructuresFromSourceFiles(string pointShapefilePath, StructureSelectionMapping map, string terrrainFilePath, bool updateGroundElevFromTerrain,
            string ImpactAreaShapefilePath, Projection studyProjection, Dictionary<string, OccupancyType> occTypes)
    {
        List<Structure> Structures = [];

        PolygonFeatureLayer impactAreaFeatureLayer = new(UNUSED_STRING_VALUE, ImpactAreaShapefilePath);
        List<Polygon> impactAreas = RASHelper.LoadImpactAreasFromSourceFiles(impactAreaFeatureLayer, studyProjection);
        float[] groundelevs = Array.Empty<float>();
        PointFeatureLayer structureFeatureLayer = new(UNUSED_STRING_VALUE, pointShapefilePath);
        PointMs pointMs = new(structureFeatureLayer.Points().Select(p => p.PointM()));
        if (updateGroundElevFromTerrain)
        {
            groundelevs = RASHelper.SamplePointsFromRaster(pointShapefilePath, terrrainFilePath, studyProjection);
        }

        for (int i = 0; i < structureFeatureLayer.FeatureCount(); i++)
        {
            //required parameters
            PointM point = pointMs[i];
            System.Data.DataRow row = structureFeatureLayer.FeatureRow(i);
            string fid = GetFID(map, row);
            double val_struct = RASHelper.GetRowValueForColumn<double>(row, map.StructureValueCol, DEFAULT_MISSING_NUMBER_VALUE);
            string occtype = RASHelper.GetRowValueForColumn(row, map.OccTypeCol, DEFAULT_MISSING_STRING_VALUE);
            string st_damcat = DEFAULT_MISSING_STRING_VALUE;
            if (occTypes.ContainsKey(occtype))
            {
                st_damcat = occTypes[occtype].DamageCategory;
                occtype = occTypes[occtype].Name;
            }
            //semi-required. We'll either have ff_elev given to us, or both ground elev and found_ht
            double found_ht = RASHelper.GetRowValueForColumn<double>(row, map.FoundationHeightCol, DEFAULT_MISSING_NUMBER_VALUE); //not gauranteed
            double ground_elv;
            if (updateGroundElevFromTerrain)
            {
                ground_elv = groundelevs[i];
            }
            else
            {
                ground_elv = RASHelper.GetRowValueForColumn<double>(row, map.GroundElevCol, DEFAULT_MISSING_NUMBER_VALUE); //not gauranteed
            }
            double ff_elev = RASHelper.GetRowValueForColumn<double>(row, map.FirstFloorElevCol, DEFAULT_MISSING_NUMBER_VALUE); // not gauranteed  
            if (ff_elev == DEFAULT_MISSING_NUMBER_VALUE)
            {
                ff_elev = ground_elv + found_ht;
            }
            //optional parameters
            double val_cont = RASHelper.GetRowValueForColumn<double>(row, map.ContentValueCol, 0);
            double val_vehic = RASHelper.GetRowValueForColumn<double>(row, map.VehicleValueCol, 0);
            double val_other = RASHelper.GetRowValueForColumn<double>(row, map.OtherValueCol, 0);
            string cbfips = DEFAULT_MISSING_STRING_VALUE;
            double beginningDamage = RASHelper.GetRowValueForColumn<double>(row, map.BeginningDamageDepthCol, DEFAULT_MISSING_NUMBER_VALUE);
            if (beginningDamage == DEFAULT_MISSING_NUMBER_VALUE)
            {
                if (found_ht != DEFAULT_MISSING_NUMBER_VALUE)
                {
                    beginningDamage = -found_ht;
                }
            }
            int numStructures = RASHelper.GetRowValueForColumn(row, map.NumberOfStructuresCol, 1);
            int yearInService = RASHelper.GetRowValueForColumn(row, map.YearInConstructionCol, DEFAULT_MISSING_NUMBER_VALUE);
            //TODO: handle number 
            int impactAreaID = GetImpactAreaFID(point, impactAreas);
            string notes = RASHelper.GetRowValueForColumn(row, map.NotesCol, DEFAULT_MISSING_STRING_VALUE);
            string description = RASHelper.GetRowValueForColumn(row, map.DescriptionCol, DEFAULT_MISSING_STRING_VALUE);
            Structures.Add(new Structure(fid, point, ff_elev, val_struct, st_damcat, occtype, impactAreaID, val_cont,
                val_vehic, val_other, cbfips, beginningDamage, ground_elv, found_ht, yearInService, numStructures, notes, description));
        }
        return Structures;
    }

    /// <summary>
    /// FID is commonly either string or int. This method will allow us to accept both. Tries to get string, if it fails, tries to get int. If it fails again, returns a default missing string value. 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private static string GetFID(StructureSelectionMapping map, System.Data.DataRow row)
    {
        string name = RASHelper.GetRowValueForColumn(row, map.StructureIDCol, DEFAULT_MISSING_STRING_VALUE);
        if (name == DEFAULT_MISSING_STRING_VALUE)
        {
            name = RASHelper.GetRowValueForColumn(row, map.StructureIDCol, DEFAULT_MISSING_NUMBER_VALUE).ToString();
        }
        if (name == DEFAULT_MISSING_NUMBER_VALUE.ToString())
        {
            name = DEFAULT_MISSING_STRING_VALUE;
        }
        return name;
    }

    public static int GetImpactAreaFID(PointM point, List<Polygon> ImpactAreas)
    {
        for (int i = 0; i < ImpactAreas.Count; i++)
        {
            if (ImpactAreas[i].Contains(point))
            {
                return i;
            }
        }
        return -9999;
    }
}
