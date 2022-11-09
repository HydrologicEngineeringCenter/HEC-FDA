using HEC.FDA.Model.interfaces;
using Microsoft.Toolkit.HighPerformance.Helpers;
using RasMapperLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace HEC.FDA.Model.structures;

//TODO: Figure out how to set Occupany Type Set
public class Inventory
{
    private Dictionary<string, int> _impactAreaNameToID;
    private PolygonFeatureLayer _impactAreaSet;
    private List<OccupancyType> _Occtypes;
    private List<string> _damageCategories;
    private List<int> _impactAreaIDs;
    private string _impactAreaUniqueColumnHeader;
    public List<Structure> Structures { get; }
    public List<int> ImpactAreas
    {
        get { return _impactAreaIDs; }
    }
    public List<string> DamageCategories
    {
        get { return _damageCategories; }
    }

    public float[] GroundElevations
    {
        get
        {
            float[] result = new float[Structures.Count];
            for(int i = 0; i < Structures.Count; i++)
            {
                result[i] = (float)Structures[i].GroundElevation;
            }
            return result;
        }
    }

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

    public Polygon GetImpactAreaPolygon(string impactAreaName)
    {
        for (int i = 0; i < _impactAreaSet.FeatureCount(); i++)
        {
            var row = _impactAreaSet.FeatureRow(i);
            string thisImpactAreaName = TryGetObj<string>(row[_impactAreaUniqueColumnHeader]);
            if (thisImpactAreaName.Equals(impactAreaName)) ;
            {
                return _impactAreaSet.Polygon(i);
            }
        }
        return null;
    }

    private PointFeatureLayer createColumnHeadersForMissingColumns(PointFeatureLayer layer, StructureInventoryColumnMap map)
    {
        List<string> layerColumnNames = new List<string>();
        var row = layer.FeatureRow(0);
        foreach (DataColumn c in row.Table.Columns)  //loop through the columns. 
        {
            layerColumnNames.Add(c.ColumnName);
        }

        foreach (string columnName in map.ColumnHeaders.Keys)
        {
            if (!layerColumnNames.Contains(columnName))
            {
                layer.AddAttributeColumn(columnName, map.ColumnHeaders[columnName]);
            }
        }
        return layer;
    }

    public Inventory(string pointShapefilePath, string impactAreaShapefilePath, StructureInventoryColumnMap map, List<OccupancyType> occTypes, 
        string impactAreaUniqueColumnHeader, bool updateGroundElevFromTerrain, string terrainPath = null)
    {
        //TODO: I think we need "default" values like -999 for the "missing" attributes or some other way to evaluate what
        //is missing to avoid null reference exceptions in the compute 
        PointFeatureLayer structureInventory = new PointFeatureLayer("Structure_Inventory", pointShapefilePath);
        structureInventory = createColumnHeadersForMissingColumns(structureInventory, map);

        _impactAreaSet = new PolygonFeatureLayer("Impact_Area_Set", impactAreaShapefilePath);
        _impactAreaUniqueColumnHeader = impactAreaUniqueColumnHeader;

        float[] groundelevs = Array.Empty<float>();
        if (updateGroundElevFromTerrain)
        {
            groundelevs = GetGroundElevationFromTerrain(pointShapefilePath, terrainPath);
        }

        PointMs pointMs = new PointMs(structureInventory.Points().Select(p => p.PointM()));
        Structures = new List<Structure>();
        try
        {
            for (int i = 0; i < structureInventory.FeatureCount(); i++)
            {
                //required parameters
                PointM point = pointMs[i];
                var row = structureInventory.FeatureRow(i);
                int fid = TryGet<int>(row[map.StructureID], -999);
                double val_struct = TryGet<double>(row[map.StructureValue], -999);
                string st_damcat = TryGetObj<string>(row[map.DamageCatagory], "NA");
                string occtype = TryGetObj<string>(row[map.OccupancyType], "NA");

                //semi-required. We'll either have ff_elev given to us, or both ground elev and found_ht
                double found_ht = TryGet<double>(row[map.FoundationHeight], -999); //not gauranteed
                double ground_elv;
                if (updateGroundElevFromTerrain)
                {
                    ground_elv = groundelevs[i];
                }
                else
                {
                    ground_elv = TryGet<double>(row[map.GroundElev], -999); //not gauranteed
                }
                double ff_elev = TryGet<double>(row[map.FirstFloorElev], -999); // not gauranteed
                if (row[map.FirstFloorElev] == DBNull.Value)
                {
                    ff_elev = ground_elv + found_ht;
                }


                //optional parameters
                double val_cont = TryGet<double>(row[map.ContentValue], -999);
                double val_vehic = TryGet<double>(row[map.VehicalValue], -999);
                double val_other = TryGet<double>(row[map.OtherValue], -999);
                string cbfips = TryGetObj<string>(row[map.CBFips], "NA");


                int impactAreaID = GetImpactAreaFID(point, impactAreaShapefilePath);
                Structures.Add(new Structure(fid, point, ff_elev, ground_elv, val_struct, st_damcat, occtype, impactAreaID, val_cont, val_vehic, val_other, cbfips));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        _Occtypes = occTypes;
        GetUniqueImpactAreas();
        GetUniqueDamageCatagories();
    }

    public Inventory(List<Structure> structures, List<OccupancyType> occTypes)
    {
        Structures = structures;
        _Occtypes = occTypes;
        GetUniqueImpactAreas();
        GetUniqueDamageCatagories();
    }
    public static float[] GetGroundElevationFromTerrain(string pointShapefilePath, string TerrainPath)
    {
        PointFeatureLayer structureInventory = new PointFeatureLayer("Structure_Inventory", pointShapefilePath);
        PointMs pointMs = new PointMs(structureInventory.Points().Select(p => p.PointM()));
        TerrainLayer terrain = new TerrainLayer("Terrain", TerrainPath);
        return terrain.ComputePointElevations(pointMs);
    }

    private void GetUniqueImpactAreas()
    {
        List<int> impactAreas = new List<int>();
        foreach (var structure in Structures)
        {
            if (!impactAreas.Contains(structure.ImpactAreaID))
            {
                impactAreas.Add(structure.ImpactAreaID);
            }
        }
        _impactAreaIDs = impactAreas;
    }
    /// <summary>
    /// Loops through entire inventory and reports back a list of all the unique damage catagories associated with the structures
    /// </summary>
    /// <returns></returns>
    internal void GetUniqueDamageCatagories()
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
        _damageCategories = damageCatagories;
    }

    private Inventory GetInventoryTrimmmedToPolygon(Polygon impactArea)
    {
        List<Structure> filteredStructureList = new List<Structure>();

        foreach (Structure structure in Structures)
        {
            if (impactArea.Contains(structure.Point))
            {
                filteredStructureList.Add(structure);
            }
        }
        return new Inventory(filteredStructureList, _Occtypes);
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
    public static int GetImpactAreaFID(PointM point, string polygonShapefilePath)
    {
        PolygonFeatureLayer polygonFeatureLayer = new PolygonFeatureLayer("impactAreas", polygonShapefilePath);
        List<Polygon> polygons = polygonFeatureLayer.Polygons().ToList();
        for (int i = 0; i < polygons.Count; i++)
        {
            if (polygons[i].Contains(point))
            {
                return i;
            }
        }
        return -9999;
    }
    public DeterministicInventory Sample(IProvideRandomNumbers randomProvider)
    {

        List<DeterministicStructure> inventorySample = new List<DeterministicStructure>();
        foreach (Structure structure in Structures)
        {
            foreach (OccupancyType occupancyType in _Occtypes)
            {
                if (structure.DamageCatagory.Equals(occupancyType.DamageCategory))
                {
                    if (structure.OccTypeName.Equals(occupancyType.Name))
                    {
                        inventorySample.Add(structure.Sample(randomProvider, occupancyType));
                        break;
                    }
                }
            }
            //it is possible that if an occupancy type doesnt exist a structure wont get added...
        }
        return new DeterministicInventory(inventorySample, _impactAreaIDs, _damageCategories);
    }
}
