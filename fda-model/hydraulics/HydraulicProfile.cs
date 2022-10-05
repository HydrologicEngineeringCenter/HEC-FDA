using HEC.FDA.Model.hydraulics.enums;
using RasMapperLib;
using RasMapperLib.Mapping;
using System;

namespace HEC.FDA.Model.hydraulics
{
    public class HydraulicProfile : IComparable
    {
        public double Probability { get; set; }
        public string FilePath { get; set; }
        public HydraulicDataSource DataSourceFormat { get; set; }
        public string ProfileName { get; set; }

        public HydraulicProfile(double probability, string filepath, HydraulicDataSource dataSource, string profileName)
        {
            Probability = probability;
            FilePath = filepath;
            DataSourceFormat = dataSource;
            ProfileName = profileName;
        }
        public float[] GetWSE(PointMs pts)
        { 
            
            if (DataSourceFormat == HydraulicDataSource.WSEGrid)
            {
                return GetWSEFromGrids(pts);
            }
            else
            {
                return GetWSEFromHDF(pts);
            }
        }

        private float[] GetWSEFromGrids(PointMs pts)
        {
            //TODO Sample off grids
            return null;
        }

        private float[] GetWSEFromHDF(PointMs pts)
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
            // Compute Switch requires an array of terrain elevations, but since we're using WSE they're not necessary. Mock array is just an array of propper size with values of 0. 
            float[] mockTerrainElevs = new float[pts.Count];
            rasResult.ComputeSwitch(rasWSMap, mapPixels, profileIndex, mockTerrainElevs, null, ref WSE);
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
                return Probability.CompareTo(otherProfile.Probability);
            else
                throw new ArgumentException("Object is not a HydraulicProfile");
        }
    }
}

