using Xunit;
using System.Collections.Generic;
using HEC.FDA.Model.structures;
using RasMapperLib;
using static HEC.FDA.Model.structures.OccupancyType;
using HEC.FDA.Model.compute;
using Geospatial.GDALAssist;
using System.IO;
using System;

namespace HEC.FDA.ModelTest.unittests.structures
{
    [Trait("RunsOn", "Remote")]
    [Collection("Serial")]
    public class InventoryShould
    {
        private const string IANameColumnHeader = "Name";
        private const string pathToNSIShapefile = @"..\..\..\HEC.FDA.ModelTest\Resources\MuncieNSI\Muncie-SI_CRS2965.shp";
        private const string pathToIAShapefile = @"..\..\..\HEC.FDA.ModelTest\Resources\MuncieImpactAreas\ImpactAreas.shp";
        private const string pathToTerrainHDF = @"..\..\..\HEC.FDA.ModelTest\Resources\MuncieTerrain\Terrain (1)_30ft_clip.hdf";
        private const string pathToMuncieProjection = @"..\..\..\HEC.FDA.ModelTest\Resources\MuncieImpactAreas\ImpactAreas.prj";
        private const string pathToAlternativeProjection = @"..\..\..\HEC.FDA.ModelTest\Resources\Projections\26844.prj";

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

        public InventoryShould()
        {
            InitializeGDAL();
        }
        

        private void InitializeGDAL()
        {
            string gdalPath = @"GDAL\";
            if (!Directory.Exists(gdalPath))
            {
                Console.WriteLine("GDAL directory not found: " + gdalPath);
                return;
            }
            GDALSetup.InitializeMultiplatform(gdalPath);
        }

        private Inventory GetTestInventory(bool useTerrainFile)
        {
            StructureSelectionMapping map = new StructureSelectionMapping(false, useTerrainFile, StructureIDCol,OccTypeCol,FirstFloorElevCol,StructureValueCol,FoundationHeightCol,
                GroundElevCol,ContentValueCol,OtherValueCol,VehicleValueCol,BeginningDamageDepthCol,YearInConstructionCol,NotesCol,DescriptionCol,NumberOfStructuresCol);
            //Empty (default) occupancy types
            OccupancyType occupancyType = OccupancyType.Builder().Build();
            Dictionary<string, OccupancyType> occupancyTypes = new Dictionary<string, OccupancyType>() { { "occtype", occupancyType } };
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
            float[] groundelevs = Model.structures.RASHelper.SamplePointsFromRaster(pathToNSIShapefile, pathToTerrainHDF,Projection.FromFile(pathToMuncieProjection));
            Assert.Equal(682, groundelevs.Length);
            Assert.Equal(946.5, groundelevs[0], 1);
        }
        [Fact]
        public void ConstructsWithTerrainGroundElevs()
        {
            Inventory inv = GetTestInventory(true);
            Assert.Equal(682, inv.Structures.Count);//Was 696
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
            Projection proj = Model.structures.RASHelper.GetVectorProjection(new PointFeatureLayer("unusedName",pathToNSIShapefile));
            //Assert
            Assert.NotNull(proj);
        }
        [Fact]
        public void ReprojectPoints()
        {
            //These projections are VERY Slightly different.
            //It's enough to show that reprojection changes coords though, and lets us not add another file to the repo. 
            //Arrange 
            Projection projPnt = Model.structures.RASHelper.GetVectorProjection(new PointFeatureLayer("unusedName", pathToNSIShapefile));
            Projection projTerr = Projection.FromFile(pathToAlternativeProjection);
            PointM pnt= new PointM(0,0);
            //Act
            PointM newPnt = Model.structures.RASHelper.ReprojectPoint(pnt, projTerr, projPnt);
            //Assert
            Assert.NotEqual(pnt.X, newPnt.X);
        }
    }
}
