using GDALAssist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Inventory
{
    public class StructureCompute
    {


        public void Compute(string structureName, string impactAreaName, string terrainName)
        {
            //this code is from GeoFDA outputChildTreeNode. ComputeExisting

            //so we need the structure points. Geofda gets it from the shapefile, but we don't have that.
            //i think i need to get it from the geo tables in the database somehow.

            string tableConstant = Saving.PersistenceManagers.StructureInventoryPersistenceManager.STRUCTURE_INVENTORY_TABLE_CONSTANT;
            string structureTableName = tableConstant + structureName;

            //turn the structures into lifesim PointFeatures.
            LifeSimGIS.GeoPackageReader gpr = new LifeSimGIS.GeoPackageReader(Storage.Connection.Instance.Reader);
            LifeSimGIS.PointFeatures structPointFeatures = (LifeSimGIS.PointFeatures)gpr.ConvertToGisFeatures(structureTableName);

            //might need the projection
            Projection structProj = GetStructureProjection(gpr, structureTableName);// gpr.GetGisFeatureProjection(structureTableName);

            Projection structuresprj = null;
            //if (System.IO.File.Exists(System.IO.Path.ChangeExtension(_structureinventory.GetStructurePath, ".prj")))
            //{
            //    structuresprj = New GDALAssist.ESRIProjection(System.IO.Path.ChangeExtension(_structureinventory.GetStructurePath, ".prj"));
            //    structuresprj = New GDALAssist.WKTProjection(structuresprj.GetWKT);
            //    structuresprj = New GDALAssist.Proj4Projection(structuresprj.GetProj4);
            //}
            //else
            //{
            //    structuresprj = New GDALAssist.WKTProjection("");
            //}


            //read the damage reach polygons and then create a dictionary for the structures to area mapping
            LifeSimGIS.PolygonFeatures polyFeatures = GetImpactAreaPolygons(impactAreaName);

            string filePath = Storage.Connection.Instance.GetTerrainFile(terrainName);
            if (filePath == null) { return; }
            LifeSimGIS.RasterFeatures terrainRasters = new LifeSimGIS.RasterFeatures(filePath);
            float[] groundElevations = terrainRasters.GridReader.SampleValues(structPointFeatures.GetPointsArray());

            int rejectedCount = 0;
            //we don't actually have an object that is a single structure. Maybe we need one?
            //Dictionary<int, list<structure>>

            //these are the polygon indeces that correspond to each structure location?
            int[] indexes = polyFeatures.GetPolygonIndicesByPoints(structPointFeatures);

            for(int i = 0;i<indexes.Length;i++)
            {
                if(indexes[i] == -1)
                {
                    //this structure is not in a damage reach, reject it

                }
                else
                {
                    if(groundElevations[i] == terrainRasters.GridReader.NoData[0])
                    {
                        //this structure doesn't have a terrain value, reject it
                    }
                    else
                    {
                        //each occupancy type must live in only one damage category
                        //get the dam cat
                        //get the occtype

                    }
                }
            }

            //load the terrain file

            //check projection and reproject if needed
            //        Dim groundelevations() As Single = tgrid.GridReader.SampleValues(structureshp.Points.ToArray)


            //then there is some logic to keep track of rejected structures. These are structures not found inside of a polygon.
        }

        private LifeSimGIS.PolygonFeatures GetImpactAreaPolygons(string impactAreaName)
        {
            DatabaseManager.SQLiteManager sqr = new DatabaseManager.SQLiteManager(Storage.Connection.Instance.ProjectFile);
            LifeSimGIS.GeoPackageReader gpr = new LifeSimGIS.GeoPackageReader(sqr);
            LifeSimGIS.PolygonFeatures polyFeatures = (LifeSimGIS.PolygonFeatures)gpr.ConvertToGisFeatures("IndexPointTable -" + impactAreaName);
            return polyFeatures;
        }
        private Projection GetStructureProjection(LifeSimGIS.GeoPackageReader gpr, string tableName)
        {
            return  gpr.GetGisFeatureProjection(tableName);

        }

    }
}
