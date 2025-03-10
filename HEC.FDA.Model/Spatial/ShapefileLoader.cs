using Geospatial.GDALAssist;
using RasMapperLib;
using System;
using System.Collections.Generic;
using System.Linq;
using HEC.FDA.Model.utilities;
using HEC.FDA.Model.structures;
using Geospatial.Features;
using Geospatial.IO;
using Geospatial.Vectors;
using Utility.Logging;
using Geospatial.GDALAssist.Vectors;
using Utility.Memory;

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

    //[Obsolete("This method is deprecated. Use UpdatedLoadStructuresFromSourceFiles instead.")]
    //public static List<Structure> LoadStructuresFromSourceFiles(string pointShapefilePath, StructureSelectionMapping map, string terrrainFilePath, bool updateGroundElevFromTerrain,
    //        string impactAreaShapefilePath, Projection studyProjection, Dictionary<string, OccupancyType> occTypes)
    //{
    //    List<Structure> Structures = [];
    //    PolygonFeatureLayer impactAreaFeatureLayer = new(UNUSED_STRING_VALUE, impactAreaShapefilePath);
    //    List<Polygon> impactAreas = RASHelper.LoadImpactAreasFromSourceFiles(impactAreaFeatureLayer, studyProjection);
    //    float[] groundelevs = Array.Empty<float>();
    //    PointFeatureLayer structureFeatureLayer = new(UNUSED_STRING_VALUE, pointShapefilePath);
    //    PointMs pointMs = new(structureFeatureLayer.Points().Select(p => p.PointM()));

    //    //reproject right here. SI gets put into study projection immediately. 
    //    Projection siProjection = RASHelper.GetVectorProjection(pointShapefilePath);
    //    pointMs = RASHelper.ReprojectPoints(studyProjection, siProjection, pointMs);

    //    if (updateGroundElevFromTerrain)
    //    {
    //        groundelevs = RASHelper.SamplePointsFromRaster(pointMs, terrrainFilePath);
    //    }

    //    for (int i = 0; i < structureFeatureLayer.FeatureCount(); i++)
    //    {
    //        //required parameters
    //        PointM point = pointMs[i];
    //        System.Data.DataRow row = structureFeatureLayer.FeatureRow(i);
    //        string fid = GetFID(map, row);
    //        double val_struct = RASHelper.GetRowValueForColumn<double>(row, map.StructureValueCol, DEFAULT_MISSING_NUMBER_VALUE);
    //        string occtype = RASHelper.GetRowValueForColumn(row, map.OccTypeCol, DEFAULT_MISSING_STRING_VALUE);
    //        string st_damcat = DEFAULT_MISSING_STRING_VALUE;
    //        if (occTypes.ContainsKey(occtype))
    //        {
    //            st_damcat = occTypes[occtype].DamageCategory;
    //            occtype = occTypes[occtype].Name;
    //        }
    //        //semi-required. We'll either have ff_elev given to us, or both ground elev and found_ht
    //        double found_ht = RASHelper.GetRowValueForColumn<double>(row, map.FoundationHeightCol, DEFAULT_MISSING_NUMBER_VALUE); //not gauranteed
    //        double ground_elv;
    //        if (updateGroundElevFromTerrain)
    //        {
    //            ground_elv = groundelevs[i];
    //        }
    //        else
    //        {
    //            ground_elv = RASHelper.GetRowValueForColumn<double>(row, map.GroundElevCol, DEFAULT_MISSING_NUMBER_VALUE); //not gauranteed
    //        }
    //        double ff_elev = RASHelper.GetRowValueForColumn<double>(row, map.FirstFloorElevCol, DEFAULT_MISSING_NUMBER_VALUE); // not gauranteed  
    //        if (ff_elev == DEFAULT_MISSING_NUMBER_VALUE)
    //        {
    //            ff_elev = ground_elv + found_ht;
    //        }
    //        //optional parameters
    //        double val_cont = RASHelper.GetRowValueForColumn<double>(row, map.ContentValueCol, 0);
    //        double val_vehic = RASHelper.GetRowValueForColumn<double>(row, map.VehicleValueCol, 0);
    //        double val_other = RASHelper.GetRowValueForColumn<double>(row, map.OtherValueCol, 0);
    //        string cbfips = DEFAULT_MISSING_STRING_VALUE;
    //        double beginningDamage = RASHelper.GetRowValueForColumn<double>(row, map.BeginningDamageDepthCol, DEFAULT_MISSING_NUMBER_VALUE);
    //        if (beginningDamage == DEFAULT_MISSING_NUMBER_VALUE)
    //        {
    //            if (found_ht != DEFAULT_MISSING_NUMBER_VALUE)
    //            {
    //                beginningDamage = -found_ht;
    //            }
    //        }
    //        int numStructures = RASHelper.GetRowValueForColumn(row, map.NumberOfStructuresCol, 1);
    //        int yearInService = RASHelper.GetRowValueForColumn(row, map.YearInConstructionCol, DEFAULT_MISSING_NUMBER_VALUE);
    //        //TODO: handle number 
    //        int impactAreaID = GetImpactAreaFID(point, impactAreas);
    //        string notes = RASHelper.GetRowValueForColumn(row, map.NotesCol, DEFAULT_MISSING_STRING_VALUE);
    //        string description = RASHelper.GetRowValueForColumn(row, map.DescriptionCol, DEFAULT_MISSING_STRING_VALUE);
    //        Structures.Add(new Structure(fid, point, ff_elev, val_struct, st_damcat, occtype, impactAreaID, val_cont,
    //            val_vehic, val_other, cbfips, beginningDamage, ground_elv, found_ht, yearInService, numStructures, notes, description));
    //    }
    //    return Structures;
    //}



    public static OperationResult LoadStructuresFromSourceFiles(string pointShapefilePath, StructureSelectionMapping map, string terrrainFilePath, bool updateGroundElevFromTerrain,
            string impactAreaShapefilePath, Projection studyProjection, Dictionary<string, OccupancyType> occTypes, out List<Structure> structures)
    {
        structures = [];

        // 1. Read the Impact Area shapefile (polygons) 
        OperationResult impactResult = ShapefileWriter.TryReadShapefile(impactAreaShapefilePath, out PolygonFeatureCollection impactAreaCollection, studyProjection);
        if (!impactResult.Result)
        {
            return impactResult;
        }

        // 2. Read the structure shapefile (points)
        OperationResult pointResult = ShapefileWriter.TryReadShapefile(pointShapefilePath, out PointFeatureCollection structurePoints, studyProjection);
        if (!pointResult.Result)
        {
            return impactResult;
        }

        // 3. If we have to update the ground elevations from the terrain, then do so now.
        float[] groundelevs = []; //initialized so we can ref it later even if we don't update it.
        if (updateGroundElevFromTerrain)
        {
            groundelevs = RASHelper.SamplePointsFromRaster(structurePoints.Features, terrrainFilePath);
        }

        // 4. Now loop through each feature in the point collection.
        for (int i = 0; i < structurePoints.Count; i++)
        {
            // Obtain the attribute row for this feature.
            var row = structurePoints.AttributeTable.Rows[i];

            // Get the feature’s ID.
            string fid = GetFID(map, row).Trim();

            double val_struct = row.ValueAs<double>(map.StructureValueCol, DEFAULT_MISSING_NUMBER_VALUE);
            string occtype = row.ValueAs<string>(map.OccTypeCol, DEFAULT_MISSING_STRING_VALUE).Trim();
            if (!occTypes.TryGetValue(occtype, out OccupancyType occ))
            {
                return OperationResult.Fail($"Occupancy type {occtype} not found in the list of occupancy types.");
            }
            string st_damcat = occ.DamageCategory;

            double found_ht = row.ValueAs<double>(map.FoundationHeightCol, DEFAULT_MISSING_NUMBER_VALUE);
            double ground_elv = updateGroundElevFromTerrain ? groundelevs[i] : row.ValueAs<double>(map.GroundElevCol, DEFAULT_MISSING_NUMBER_VALUE);
            double ff_elev = row.ValueAs<double>(map.FirstFloorElevCol, DEFAULT_MISSING_NUMBER_VALUE);
            if (ff_elev == DEFAULT_MISSING_NUMBER_VALUE)
            {
                ff_elev = ground_elv + found_ht;
            }

            // Optional parameters:
            double val_cont = row.ValueAs<double>(map.ContentValueCol, 0);
            double val_vehic = row.ValueAs<double>(map.VehicleValueCol, 0);
            double val_other = row.ValueAs<double>(map.OtherValueCol, 0);
            string cbfips = DEFAULT_MISSING_STRING_VALUE;

            double beginningDamage = row.ValueAs<double>(map.BeginningDamageDepthCol, DEFAULT_MISSING_NUMBER_VALUE);
            if (beginningDamage == DEFAULT_MISSING_NUMBER_VALUE)
            {
                if (found_ht != DEFAULT_MISSING_NUMBER_VALUE)
                    beginningDamage = -found_ht;
            }

            int numStructures = row.ValueAs<int>(map.NumberOfStructuresCol, 1);
            int yearInService = row.ValueAs<int>(map.YearInConstructionCol, DEFAULT_MISSING_NUMBER_VALUE);

            // Get the point geometry from the feature.
            Geospatial.Vectors.Point point = structurePoints[i];
            int impactAreaID = GetImpactAreaFID(point, impactAreaCollection);

            string notes = row.ValueAs<string>(map.NotesCol, DEFAULT_MISSING_STRING_VALUE).Trim();
            string description = row.ValueAs<string>(map.DescriptionCol, DEFAULT_MISSING_STRING_VALUE).Trim();

            // Create and add the new Structure.
            structures.Add(new Structure(
                fid, point, ff_elev, val_struct, st_damcat, occtype,
                impactAreaID, val_cont, val_vehic, val_other,
                cbfips, beginningDamage, ground_elv, found_ht,
                yearInService, numStructures, notes, description));
        }

        return OperationResult.Success();
    }





    /// <summary>
    /// FID is commonly either string or int. This method will allow us to accept both. Tries to get string, if it fails, tries to get int. If it fails again, returns a default missing string value. 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private static string GetFID(StructureSelectionMapping map, TableRow row)
    {
        string name = row.ValueAs(map.StructureIDCol, DEFAULT_MISSING_STRING_VALUE);
        if (name == DEFAULT_MISSING_STRING_VALUE){
            {
                name = row.ValueAs(map.StructureIDCol, DEFAULT_MISSING_NUMBER_VALUE).ToString();
            }
        }
        if (name == DEFAULT_MISSING_NUMBER_VALUE.ToString())
        {
            name = DEFAULT_MISSING_STRING_VALUE;
        }
        return name;
    }

    public static int GetImpactAreaFID(Geospatial.Vectors.Point point, PolygonFeatureCollection ImpactAreas)
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
