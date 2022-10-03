using HEC.FDA.Model.hydraulics.enums;
using RasMapperLib;
using RasMapperLib.Mapping;
using System;

namespace HEC.FDA.Model.hydraulics
{
    public class HydraulicProfile : IComparable
    {
        public double Probability { get; set; }
        public string HydraulicsFilePath { get; set; }
        public HydraulicDataSource DataSourceFormat { get; set; }
        public string ProfileName { get; set; }

        public HydraulicProfile(double probability, string hydraulicsFilepath, HydraulicDataSource dataSource, string profileName)
        {
            Probability = probability;
            HydraulicsFilePath = hydraulicsFilepath;
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
            var rasResult = new RASResults(HydraulicsFilePath);
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
            //ComputeSwitch requires terrian elevs, but they wont be used for WSE, so we give a dummy array
            float[] dummyTerrainElevs = new float[pts.Count];

            // This will produce -9999 for NoData values.
            rasResult.ComputeSwitch(rasWSMap, mapPixels, profileIndex, dummyTerrainElevs, null, ref WSE);
            return WSE;
        }

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

