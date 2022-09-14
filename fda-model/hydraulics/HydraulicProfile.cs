using fda_model.hydraulics.enums;
using RasMapperLib;
using RasMapperLib.Mapping;
using System;

namespace fda_model.hydraulics
{
    public class HydraulicProfile:IComparable
    {
        public double Probability { get; set; }
        public string FilePath { get; set; }
        public string TerrainPath { get; set; }
        public HydraulicDataSource DataSourceFormat { get; set; }
        public string ProfileName { get; set; }

        public HydraulicProfile(double probability, string filepath, string terrainFile, HydraulicDataSource dataSource, string profileName)
        {
            Probability = probability;
            FilePath = filepath;
            TerrainPath = terrainFile;
            DataSourceFormat = dataSource;
            ProfileName = profileName;
        }
        public float[] GetDepths(PointMs pts)
        {
            
            TerrainLayer terrain = new TerrainLayer("Terrain", TerrainPath);
            float[] terrainElevs = terrain.ComputePointElevations(pts);
            // Construct a result from the given filename.

            if (DataSourceFormat == HydraulicDataSource.UnsteadyHDF || DataSourceFormat == HydraulicDataSource.SteadyHDF)
            {
                return GetDepthsFromHDF(pts, terrainElevs);
            }
            else
            {
                return GetDepthsFromGrid(pts, terrainElevs);
            }
        }

        private float[] GetDepthsFromGrid(PointMs pts, float[] terrainElevs)
        {
            //TODO Sample off grids
            return null;
        }

        private float[] GetDepthsFromHDF(PointMs pts, float[] terrainElevs)
        {
            var rasResult = new RASResults(FilePath);
            var rasGeometry = rasResult.Geometry;
            var rasWSMap = new RASResultsMap(rasResult, MapTypes.Elevation);

            // Sample the geometry for the given points loaded from the shapefile.
            // If the geometry is the same for all of the results, we can actually reuse this object.
            // (It's pretty fast to recompute though, so I wouldn't bother)
            RASGeometryMapPoints mapPixels = rasGeometry.MapPixels(pts);
            // This will produce -9999 for NoData values.
            float[] depthVals = null;

            int profileIndex;
            if (DataSourceFormat == HydraulicDataSource.UnsteadyHDF)
            {
                profileIndex = RASResultsMap.MaxProfileIndex;
            }
            else
            {
                profileIndex = rasResult.ProfileIndex(ProfileName);
            }
            rasResult.ComputeSwitch(rasWSMap, mapPixels, profileIndex, terrainElevs, null, ref depthVals);
            return depthVals;
        }


        /// <summary>
        /// allows for sorting based on probability of the profile.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            HydraulicProfile otherProfile = obj as HydraulicProfile;
            if (otherProfile != null)
                return this.Probability.CompareTo(otherProfile.Probability);
            else
                throw new ArgumentException("Object is not a Temperature");
        }
    }
}

