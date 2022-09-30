﻿using fda_model.hydraulics.enums;
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

        public HydraulicProfile(double probability, string filepath,  HydraulicDataSource dataSource, string profileName, string terrainFile = null)
        {
            Probability = probability;
            FilePath = filepath;
            TerrainPath = terrainFile;
            DataSourceFormat = dataSource;
            ProfileName = profileName;
        }
        public float[] GetWSE(PointMs pts)
        {
            
            TerrainLayer terrain = new TerrainLayer("Terrain", TerrainPath);
            float[] terrainElevs = terrain.ComputePointElevations(pts);

            if (DataSourceFormat == HydraulicDataSource.WSEGrid)
            {
                return GetWSEFromGrids(pts, terrainElevs);
            }
            else
            {
                return GetWSEFromHDF(pts, terrainElevs);
            }
        }

        private float[] GetWSEFromGrids(PointMs pts, float[] terrainElevs)
        {
            //TODO Sample off grids
            return null;
        }

        private float[] GetWSEFromHDF(PointMs pts, float[] terrainElevs)
        {
            var rasResult = new RASResults(FilePath);
            var rasGeometry = rasResult.Geometry;
            var rasWSMap = new RASResultsMap(rasResult, MapTypes.Elevation);

            // Sample the geometry for the given points loaded from the shapefile.
            RASGeometryMapPoints mapPixels = rasGeometry.MapPixels(pts);

            float[] WSE = null;

            int profileIndex;
            if (DataSourceFormat == HydraulicDataSource.UnsteadyHDF)
            {
                profileIndex = RASResultsMap.MaxProfileIndex;
            }
            else
            {
                profileIndex = rasResult.ProfileIndex(ProfileName);
            }
            // This will produce -9999 for NoData values.
            rasResult.ComputeSwitch(rasWSMap, mapPixels, profileIndex, terrainElevs, null, ref WSE);
            return WSE;
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

