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
using HEC.FDA.Model.Spatial.Extensions;

namespace HEC.FDA.Model.Spatial;
public class StructureFactory
{
    private const string DEFAULT_MISSING_STRING_VALUE = "EMPTY";
    private const int DEFAULT_MISSING_NUMBER_VALUE = IntegerGlobalConstants.DEFAULT_MISSING_VALUE;
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

    public static OperationResult LoadStructuresFromSourceFiles(string pointShapefilePath, StructureSelectionMapping map, string terrrainFilePath, bool updateGroundElevFromTerrain,
            string impactAreaShapefilePath, Projection studyProjection, Dictionary<string, OccupancyType> occTypes, out List<Structure> structures)
    {
        structures = [];

        // 1. Read the Impact Area shapefile (polygons) 
        OperationResult impactResult = ShapefileIO.TryRead(impactAreaShapefilePath, out PolygonFeatureCollection impactAreaCollection, studyProjection);
        if (!impactResult.Result)
        {
            return impactResult;
        }

        // 2. Read the structure shapefile (points)
        OperationResult pointResult = ShapefileIO.TryRead(pointShapefilePath, out PointFeatureCollection structurePoints, studyProjection);
        if (!pointResult.Result)
        {
            return impactResult;
        }

        // 3. If we have to update the ground elevations from the terrain, then do so now.
        float[] groundelevs = []; //initialized so we can ref it later even if we don't update it.
        if (updateGroundElevFromTerrain)
        {
            groundelevs = RASHelper.SamplePointsFromTerrain(structurePoints.Features, terrrainFilePath);
        }

        // 4. Now loop through each feature in the point collection.
        for (int i = 0; i < structurePoints.Count; i++)
        {
            // Obtain the attribute row for this feature.
            var row = structurePoints.AttributeTable.Rows[i];

            // Get the feature’s ID.
            string fid = GetFID(map.StructureIDCol, row).Trim();

            double val_struct = row.TryGetValueAs<double>(map.StructureValueCol, DEFAULT_MISSING_NUMBER_VALUE);
            string occtypeInputName = row.TryGetValueAs<string>(map.OccTypeCol, DEFAULT_MISSING_STRING_VALUE).Trim();
            if (!occTypes.TryGetValue(occtypeInputName, out OccupancyType occ))
            {
                return OperationResult.Fail($"Occupancy type {occtypeInputName} not found in the list of occupancy types.");
            }
            string st_damcat = occ.DamageCategory;
            string occtypeActualName = occ.Name;

            double found_ht = row.TryGetValueAs<double>(map.FoundationHeightCol, DEFAULT_MISSING_NUMBER_VALUE);
            double ground_elv = updateGroundElevFromTerrain ? groundelevs[i] : row.TryGetValueAs<double>(map.GroundElevCol, DEFAULT_MISSING_NUMBER_VALUE);
            double ff_elev = row.TryGetValueAs<double>(map.FirstFloorElevCol, ground_elv + found_ht);

            // Optional parameters:
            double val_cont = row.TryGetValueAs<double>(map.ContentValueCol, 0);
            double val_vehic = row.TryGetValueAs<double>(map.VehicleValueCol, 0);
            double val_other = row.TryGetValueAs<double>(map.OtherValueCol, 0);
            string cbfips = DEFAULT_MISSING_STRING_VALUE;
            double bdd_default = DEFAULT_MISSING_NUMBER_VALUE;
            if (found_ht != DEFAULT_MISSING_NUMBER_VALUE)
            {
                bdd_default = -found_ht;
            }
            double beginningDamage = row.TryGetValueAs<double>(map.BeginningDamageDepthCol, bdd_default);
            int numStructures = row.TryGetValueAs<int>(map.NumberOfStructuresCol, 1);
            int yearInService = row.TryGetValueAs<int>(map.YearInConstructionCol, DEFAULT_MISSING_NUMBER_VALUE);

            // Get the point geometry from the feature.
            Geospatial.Vectors.Point point = structurePoints[i];
            int impactAreaID = GetImpactAreaFID(point, impactAreaCollection);
            string notes = row.TryGetValueAs<string>(map.NotesCol, DEFAULT_MISSING_STRING_VALUE).Trim();
            string description = row.TryGetValueAs<string>(map.DescriptionCol, DEFAULT_MISSING_STRING_VALUE).Trim();

            // Create and add the new Structure.
            structures.Add(new Structure(
                fid, point, ff_elev, val_struct, st_damcat, occtypeActualName,
                impactAreaID, val_cont, val_vehic, val_other,
                cbfips, beginningDamage, ground_elv, found_ht,
                yearInService, numStructures, notes, description));
        }

        return OperationResult.Success();
    }





    /// <summary>  
    /// Retrieves the Feature Identifier (FID) from a specified column in a table row.  
    /// The FID can be of type string, int, or double.  
    /// Attempts to extract the FID as a string first; if unsuccessful, tries to extract it as an int,  
    /// and finally as a double. If all attempts fail, returns a default missing string value.  
    /// Throws an exception if the column type is unsupported.  
    /// </summary>  
    /// <param name="columnName">The name of the column containing the FID.</param>  
    /// <param name="row">The table row from which to extract the FID.</param>  
    /// <returns>The FID as a string.</returns>  
    /// <exception cref="Exception">Thrown if the column type is not string, int, or double.</exception>
    public static string GetFID(string columnName, TableRow row)
    {
        Type fidType = row.Table.GetColumn(columnName).Type;
        if (fidType == typeof(string))
        {
            return row.ValueAs<string>(columnName, DEFAULT_MISSING_NUMBER_VALUE.ToString());
        }
        else if (fidType == typeof(int))
        {
            return row.ValueAs<int>(columnName, DEFAULT_MISSING_NUMBER_VALUE).ToString();
        }
        else if (fidType == typeof(double))
        {
            return row.ValueAs<double>(columnName, DEFAULT_MISSING_NUMBER_VALUE).ToString();
        }
        else
        {
            throw new Exception("FID Must be int, string, or double");
        }
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
