using Xunit;
using System.Collections.Generic;
using HEC.FDA.Model.structures;
using RasMapperLib;
using static HEC.FDA.Model.structures.OccupancyType;
using HEC.FDA.Model.compute;
using Geospatial.GDALAssist;
using System.IO;
using System;
using HEC.FDA.ModelTest.Resources;
using System.Reflection;
using System.Diagnostics;
using HEC.FDA.Model.Spatial;

namespace HEC.FDA.ModelTest.unittests.structures
{
    [Trait("RunsOn", "Remote")]
    [Collection("Serial")]
    public class InventoryShould
    {
        private const string IANameColumnHeader = "Name";
        private const string pathToNSIShapefile = StringResourcePaths.pathToNSIShapefile;
        private const string pathToIAShapefile = StringResourcePaths.pathToIAShapefile;
        private const string pathToTerrainHDF = StringResourcePaths.TerrainPath;
        private const string pathToMuncieProjection = StringResourcePaths.pathToIAProjectionFile;
        private const string pathToAlternativeProjection = StringResourcePaths.PathToAltProjection;

        //NSI Headers
        private const string StructureIDCol = "fd_id";
        private const string OccTypeCol = "occtype";
        private const string FirstFloorElevCol = "";
        private const string StructureValueCol = "val_struct";
        private const string FoundationHeightCol = "found_ht";
        private const string GroundElevCol = "";
        private const string ContentValueCol = "val_cont" ;
        private const string OtherValueCol = "";
        private const string VehicleValueCol = "val_vehic" ;
        private const string BeginningDamageDepthCol = "begDamDep";
        private const string YearInConstructionCol = "yrbuilt";
        private const string NotesCol = "";
        private const string DescriptionCol = "";
        private const string NumberOfStructuresCol = "";

        private Inventory GetTestInventory(bool useTerrainFile)
        {
            StructureSelectionMapping map = new StructureSelectionMapping(false, useTerrainFile, StructureIDCol,OccTypeCol,FirstFloorElevCol,StructureValueCol,FoundationHeightCol,
                GroundElevCol,ContentValueCol,OtherValueCol,VehicleValueCol,BeginningDamageDepthCol,YearInConstructionCol,NotesCol,DescriptionCol,NumberOfStructuresCol);
            //Empty (default) occupancy types
            OccupancyType occupancyType = OccupancyType.Builder().Build();

            //This is a hack to make every occtype string match the same key.
            Dictionary<string, OccupancyType> occupancyTypes = new Dictionary<string, OccupancyType>(new CustomEqualityComparer())
            {
                { "EMPTY", occupancyType }
            };
            Inventory inv;
            if (useTerrainFile)
            {
                inv = new Inventory(pathToNSIShapefile, pathToIAShapefile, map, occupancyTypes, pathToTerrainHDF, 1);
            }
            else
            {
                inv = new Inventory(pathToNSIShapefile, pathToIAShapefile, map, occupancyTypes, 1, pathToMuncieProjection);
            }
            return inv;
        }

        [Fact]
        public void ConstructFromValidShapefile()
        {
            Inventory inventory = GetTestInventory(false);
            Assert.NotNull(inventory);
            Assert.Equal(682, inventory.Structures.Count);
        }

        [Fact]
        public void GetGroundElevationFromTerrain()
        {
            float[] groundelevs = RASHelper.SamplePointsFromRaster(pathToNSIShapefile, pathToTerrainHDF,Projection.FromFile(pathToMuncieProjection));
            Assert.Equal(682, groundelevs.Length);
            Assert.Equal(947.0004, groundelevs[0], 1);
        }
        [Fact]
        public void ConstructsWithTerrainGroundElevs()
        {
            Inventory inv = GetTestInventory(true);
            Assert.Equal(682, inv.Structures.Count);
            Assert.True(inv.Structures[0].FirstFloorElevation > 900);
        }
        [Fact]
        public void filterInventoryToIAPolygon()
        {
            Inventory inv = GetTestInventory(false);
            Inventory trimmedInv1 = inv.GetInventoryTrimmedToImpactArea(0);
            Inventory trimmedInv2 = inv.GetInventoryTrimmedToImpactArea(1);
            Inventory trimmedInv3 = inv.GetInventoryTrimmedToImpactArea(2);
            Inventory trimmedInv4 = inv.GetInventoryTrimmedToImpactArea(3);
            int countActual = inv.Structures.Count;
            int count1 = trimmedInv1.Structures.Count;
            int count2 = trimmedInv2.Structures.Count;
            int count3 = trimmedInv3.Structures.Count;
            int count4 = trimmedInv4.Structures.Count;
            Assert.Equal(countActual, count1 + count2 + count3 + count4);
        }

        [Fact]
        public void ReturnProjectionFromVector()
        {
            //Act
            Projection proj = RASHelper.GetVectorProjection(new PointFeatureLayer("unusedName",pathToNSIShapefile));
            //Assert
            Assert.NotNull(proj);
        }
        [Fact]
        public void ReprojectPoints()
        {
            //These projections are VERY Slightly different.
            //It's enough to show that reprojection changes coords though, and lets us not add another file to the repo. 
            //Arrange 
            Projection projPnt = RASHelper.GetVectorProjection(new PointFeatureLayer("unusedName", pathToNSIShapefile));
            Projection projTerr = Projection.FromFile(pathToAlternativeProjection);
            PointM pnt= new PointM(0,0);
            //Act
            PointM newPnt = RASHelper.ReprojectPoint(pnt, projTerr, projPnt);
            //Assert
            Assert.NotEqual(pnt.X, newPnt.X);
        }
    }

    /// <summary>
    /// Used to make all string match the same key in the dictionary above. Super Hacky. DO NOT USE ANYWHERE OUTSIDE THIS CLASS. 
    /// </summary>
    internal class CustomEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return true; // Always return true to make all keys equal
        }

        public int GetHashCode(string obj)
        {
            return 0; // Return a constant hash code for all keys
        }
    }
}
