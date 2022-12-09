using Geospatial.GDALAssist;
using Geospatial.GDALAssist.Vectors;
using HEC.FDA.Model.interfaces;
using Microsoft.Toolkit.HighPerformance.Helpers;
using RasMapperLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using RasMapperLib.Utilities;


namespace HEC.FDA.Model.structures;

//TODO: Figure out how to set Occupany Type Set
public class Inventory
{
    #region Fields
    private string _structureInventoryShapefile;
    private string _impactAreaShapefile;
    private StructureInventoryColumnMap _map;
    private List<OccupancyType> _occtypes;
    private string _impactAreaUniqueColumnHeader;
    private bool _updateGroundElevsFromTerrain;
    private string _terrainPath;
    #endregion

    #region Properties
    public List<Structure> Structures { get; } = new List<Structure>();
    public List<int> ImpactAreas
    {
        get
        {
            List<int> impactAreas = new List<int>();
            foreach (var structure in Structures)
            {
                if (!impactAreas.Contains(structure.ImpactAreaID))
                {
                    impactAreas.Add(structure.ImpactAreaID);
                }
            }
            return impactAreas;
        }
    }
    public List<string> DamageCategories
    {
        get
        {
            List<string> damageCatagories = new List<string>();
            foreach (Structure structure in Structures)
            {
                if (damageCatagories.Contains(structure.DamageCatagory))
                {
                    continue;
                }
                else
                {
                    damageCatagories.Add(structure.DamageCatagory);
                }
            }
            return damageCatagories;
        }
    }
    public float[] GroundElevations
    {
        get
        {
            float[] result = new float[Structures.Count];
            for (int i = 0; i < Structures.Count; i++)
            {
                result[i] = (float)Structures[i].GroundElevation;
            }
            return result;
        }
    }
    #endregion

    #region Utilities
    public static T TryGet<T>(object value, T defaultValue = default)
        where T : struct
    {
        if (value == null)
            return defaultValue;
        else if (value == DBNull.Value)
            return defaultValue;
        else
        {
            var retn = value as T?;
            if (retn.HasValue)
                return retn.Value;
            else
                return defaultValue;
        }
    }
    public static T TryGetObj<T>(object value, T defaultValue = default)
        where T : class
    {
        if (value == null)
            return defaultValue;
        else if (value == DBNull.Value)
            return defaultValue;
        else
        {
            var retn = value as T;
            if (retn != null)
                return retn;
            else
                return defaultValue;
        }
    }
    #endregion

    #region Methods
    public Polygon GetImpactAreaPolygon(string impactAreaName)
    {
        PolygonFeatureLayer impactAreas = new PolygonFeatureLayer("ImpactAreas", _impactAreaShapefile);
        for (int i = 0; i < impactAreas.FeatureCount(); i++)
        {
            var row = impactAreas.FeatureRow(i);
            string thisImpactAreaName = TryGetObj<string>(row[_impactAreaUniqueColumnHeader]);
            //TODO: this line does not appear to work correctly. THe condition is being evaluated as true despite the strings being vastly different. 
            if (thisImpactAreaName.Equals(impactAreaName))

            {
                return impactAreas.Polygon(i);
            }
        }
        return null;
    }
    private void LoadStructuresFromSourceFiles()
    {
        PointFeatureLayer structureInventory = new PointFeatureLayer("Structure_Inventory", _structureInventoryShapefile);
        createColumnHeadersForMissingColumns(ref structureInventory, _map);
        PolygonFeatureLayer impactAreaSet = new PolygonFeatureLayer("Impact_Area_Set", _impactAreaShapefile);

        float[] groundelevs = Array.Empty<float>();
        if (_updateGroundElevsFromTerrain)
        {
            groundelevs = GetGroundElevationFromTerrain(_structureInventoryShapefile, _terrainPath);
        }

        PointMs pointMs = new PointMs(structureInventory.Points().Select(p => p.PointM()));
        for (int i = 0; i < structureInventory.FeatureCount(); i++)
        {
            //required parameters
            PointM point = pointMs[i];
            var row = structureInventory.FeatureRow(i);
            int fid = TryGet<int>(row[_map.StructureID], -999);
            double val_struct = TryGet<double>(row[_map.StructureValue], -999);
            string st_damcat = TryGetObj<string>(row[_map.DamageCatagory], "NA");
            string occtype = TryGetObj<string>(row[_map.OccupancyType], "NA");
            //semi-required. We'll either have ff_elev given to us, or both ground elev and found_ht
            double found_ht = TryGet<double>(row[_map.FoundationHeight], -999); //not gauranteed
            double ground_elv;
            if (_updateGroundElevsFromTerrain)
            {
                ground_elv = groundelevs[i];
            }
            else
            {
                ground_elv = TryGet<double>(row[_map.GroundElev], -999); //not gauranteed
            }
            double ff_elev = TryGet<double>(row[_map.FirstFloorElev], -999); // not gauranteed
            if (row[_map.FirstFloorElev] == DBNull.Value)
            {
                ff_elev = ground_elv + found_ht;
            }
            //optional parameters
            double val_cont = TryGet<double>(row[_map.ContentValue], 0);
            double val_vehic = TryGet<double>(row[_map.VehicalValue], 0);
            double val_other = TryGet<double>(row[_map.OtherValue], 0);
            string cbfips = TryGetObj<string>(row[_map.CBFips], "NA");
            double beginningDamage = TryGet<double>(row[_map.BeginningDamageDepth], 0);
            int numStructures = TryGet<int>(row[_map.NumberOfStructures], 1);
            int yearInService = TryGet<int>(row[_map.YearInConstruction], -999);
            //TODO: handle number 
            int impactAreaID = GetImpactAreaFID(point);
            Structures.Add(new Structure(fid, point, ff_elev, val_struct, st_damcat, occtype, impactAreaID, val_cont, val_vehic, val_other, cbfips, beginningDamage, ground_elv, found_ht, yearInService, numStructures));
        }
    }
    private void createColumnHeadersForMissingColumns(ref PointFeatureLayer layer, StructureInventoryColumnMap map)
    {
        List<string> layerColumnNames = new List<string>();
        var row = layer.FeatureRow(0);
        foreach (DataColumn c in row.Table.Columns)  //loop through the columns. 
        {
            layerColumnNames.Add(c.ColumnName);
        }

        foreach (Tuple<string, Type> nameTypePair in map.ColumnHeaders)
        {
            if (!layerColumnNames.Contains(nameTypePair.Item1))
            {
                layer.AddAttributeColumn(nameTypePair.Item1, nameTypePair.Item2);
            }
        }
    }
    public static float[] GetGroundElevationFromTerrain(string pointShapefilePath, string TerrainPath)
    {
        PointFeatureLayer structureInventory = new PointFeatureLayer("Structure_Inventory", pointShapefilePath);
        PointMs pointMs = new PointMs(structureInventory.Points().Select(p => p.PointM()));
        TerrainLayer terrain = new TerrainLayer("Terrain", TerrainPath);
        return terrain.ComputePointElevations(pointMs);
    }
    private PointM ReprojectPoint(PointM point, Projection newProjection, Projection currentProjection)
    {

        Geospatial.Vectors.Point p = Converter.Convert(point);
        VectorExtensions.Reproject(p, currentProjection, newProjection);
        return Converter.ConvertPtM(p);
    }
    private Projection GetTerrainProjection(string Pointsfilename, string terrainFilename)
    {
        //Check extension of terrain file
        string extension = System.IO.Path.GetExtension(terrainFilename);
        // If HDF, create RASTerrainLayer, then get source files. Create a GDAL Raster from any source.
        if (extension == "hdf")
        {
            TerrainLayer terrain = new TerrainLayer("Terrain", terrainFilename);
            terrainFilename = terrain.get_RasterFilename(0);
        }
        GDALRaster raster = new GDALRaster(terrainFilename);
        return raster.GetProjection();
    }
    private Projection GetVectorProjection(string vectorPath)
    {
        VectorDataset vector = new VectorDataset(vectorPath);
        VectorLayer vectorLayer = vector.GetLayer(0);
        return vectorLayer.GetProjection();
    }
    public Inventory GetInventoryTrimmmedToPolygon(int impactAreaFID)
    {
        PolygonFeatureLayer impactAreaSet = new PolygonFeatureLayer("ImpactAreas", _impactAreaShapefile);
        List<Structure> filteredStructureList = new List<Structure>();

        foreach (Structure structure in Structures)
        {
            if (impactAreaSet[impactAreaFID].Contains(structure.Point))
            {
                filteredStructureList.Add(structure);
            }
        }
        return new Inventory(_structureInventoryShapefile, _impactAreaShapefile, _map, _occtypes, _impactAreaUniqueColumnHeader, _updateGroundElevsFromTerrain, _terrainPath, filteredStructureList);
    }
    public PointMs GetPointMs()
    {
        PointMs points = new PointMs();
        foreach (Structure structure in Structures)
        {
            points.Add(structure.Point);
        }
        return points;
    }
    public int GetImpactAreaFID(PointM point)
    {
        PolygonFeatureLayer impactAreaSet = new PolygonFeatureLayer("ImpactAreas", _impactAreaShapefile);
        List<Polygon> polygons = impactAreaSet.Polygons().ToList();
        for (int i = 0; i < polygons.Count; i++)
        {
            if (polygons[i].Contains(point))
            {
                return i;
            }
        }
        return -9999;
    }
    #endregion

    #region Constructors
    public Inventory(string pointShapefilePath, string impactAreaShapefilePath, StructureInventoryColumnMap map, List<OccupancyType> occTypes,
        string impactAreaUniqueColumnHeader, bool updateGroundElevFromTerrain, string terrainPath)
    {
        _structureInventoryShapefile = pointShapefilePath;
        _impactAreaShapefile = impactAreaShapefilePath;
        _map = map;
        _occtypes = occTypes;
        _impactAreaUniqueColumnHeader = impactAreaUniqueColumnHeader;
        _updateGroundElevsFromTerrain = updateGroundElevFromTerrain;
        _terrainPath = terrainPath;
        LoadStructuresFromSourceFiles();
    }
    public Inventory(string pointShapefilePath, string impactAreaShapefilePath, StructureInventoryColumnMap map, List<OccupancyType> occTypes,
    string impactAreaUniqueColumnHeader, bool updateGroundElevFromTerrain, string terrainPath, List<Structure> structures)
    {
        _structureInventoryShapefile = pointShapefilePath;
        _impactAreaShapefile = impactAreaShapefilePath;
        _map = map;
        _occtypes = occTypes;
        _impactAreaUniqueColumnHeader = impactAreaUniqueColumnHeader;
        _updateGroundElevsFromTerrain = updateGroundElevFromTerrain;
        _terrainPath = terrainPath;
        Structures = structures;

    }
    #endregion





    public DeterministicInventory Sample(IProvideRandomNumbers randomProvider, bool computeIsDeterministic = false)
    {

        List<DeterministicStructure> inventorySample = new List<DeterministicStructure>();
        foreach (Structure structure in Structures)
        {
            foreach (OccupancyType occupancyType in _occtypes)
            {
                if (structure.DamageCatagory.Equals(occupancyType.DamageCategory))
                {
                    if (structure.OccTypeName.Equals(occupancyType.Name))
                    {
                        inventorySample.Add(structure.Sample(randomProvider, occupancyType, computeIsDeterministic));
                        break;
                    }
                }
            }
            //it is possible that if an occupancy type doesnt exist a structure wont get added...
        }
        return new DeterministicInventory(inventorySample, ImpactAreas, DamageCategories);
    }

    internal List<string> StructureDetails()
    {
        string header = "StructureID,YearInService,DamageCategory,OccupancyType,X_Coordinate,Y_Coordinate,StructureValueInDatabase,StructureValueInflated,ContentValue,ContentValueInflated,OtherValue,OtherValueInflated,VehicleValue,VehicleValueInflated,TotalValue,TotalValueInflated,NumberOfStructures,FirstFloorElevation,GroundElevation,FoundationHeight,DepthBeginningDamage,";
        List<string> structureDetails = new List<string>() { header };
        foreach (Structure structure in Structures)
        {
            structureDetails.Add(structure.ProduceDetails());
        }
        return structureDetails;
    }
}
