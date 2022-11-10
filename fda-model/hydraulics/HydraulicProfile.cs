using Geospatial.IO;
using Geospatial.Rasters;
using HEC.FDA.Model.hydraulics.enums;
using RasMapperLib;
using RasMapperLib.Mapping;
using System;
using System.Xml.Linq;
using System.Collections.Generic;
using Geospatial.GDALAssist;

namespace HEC.FDA.Model.hydraulics
{
    public class HydraulicProfile : IComparable
    {
        public const string PROFILE = "HydraulicProfile";
        private const string PATH = "Path";
        private const string PROB = "Probability";
        private const string PROFILE_NAME = "ProfileName";

        public double Probability { get; set; }
        /// <summary>
        /// This is not the full path. This is just the file name with extension. You need to get the hydraulic element name to create the full path.
        /// </summary>
        public string FileName { get; set; }
        public string ProfileName { get; set; }

        public HydraulicProfile(double probability, string fileName, string profileName = "MAX" )
        {
            Probability = probability;
            FileName = fileName;
            ProfileName = profileName;
        }

        public HydraulicProfile(XElement elem)
        {
            Probability = Convert.ToDouble(elem.Attribute(PROB).Value);
            FileName = elem.Attribute(PATH).Value;
            ProfileName = elem.Attribute(PROFILE_NAME).Value;
        }

        public float[] GetWSE(PointMs pts, HydraulicDataSource dataSource, string parentDirectory)
        { 
            
            if (dataSource == HydraulicDataSource.WSEGrid)
            {
                return GetWSEFromGrids(pts, parentDirectory);
            }
            else
            {
                return GetWSEFromHDF(pts, dataSource, parentDirectory);
            }
        }

        private float[] GetWSEFromGrids(PointMs pts, string parentDirectory)
        {
            //THIS IS A HACK TO KEEP IMPORT FROM GRIDS WORKING FOR TESTERS
            var baseDs = TiffDataSource<float>.TryLoad(GetFilePath(parentDirectory));

            if (baseDs == null)
            {
                return new float[pts.Count];
            }
            RasterPyramid<float> baseRaster = baseDs.AsRasterizer();

            List<Geospatial.Vectors.Point> geospatialpts = RasMapperLib.Utilities.Converter.Convert(pts);
            Memory<Geospatial.Vectors.Point> points = new Memory<Geospatial.Vectors.Point>(geospatialpts.ToArray());
            float[] elevationData = new float[points.Length];

            baseRaster.SamplePoints(points, elevationData);
            return elevationData;
            //END OF HACK

            //string vrtFile = GetFilePath(parentDirectory);
            //GdalBandedRaster<float> resultsGrid = new GdalBandedRaster<float>(vrtFile);
            //float[] wses = new float[pts.Count];
            ////We're using a different peice of the RAS Code to handle this part, so we have to switch to a different definition of Point in the RAS Library. 
            //List<Geospatial.Vectors.Point> geospatialpts = RasMapperLib.Utilities.Converter.Convert(pts);
            //Memory<Geospatial.Vectors.Point> points = new Memory<Geospatial.Vectors.Point>(geospatialpts.ToArray());
            //resultsGrid.SamplePoints(points, 0, wses);
            //return wses;
        }

        private float[] GetWSEFromHDF(PointMs pts, HydraulicDataSource dataSource, string parentDirectory)
        {
            var rasResult = new RASResults(GetFilePath(parentDirectory));
            var rasGeometry = rasResult.Geometry;
            var rasWSMap = new RASResultsMap(rasResult, MapTypes.Elevation);

            // Sample the geometry for the given points loaded from the shapefile.
            RASGeometryMapPoints mapPixels = rasGeometry.MapPixels(pts);

            float[] WSE = null;

            int profileIndex;
            if (dataSource == HydraulicDataSource.UnsteadyHDF)
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
        public bool Equals(HydraulicProfile hydraulicProfileForComparison)
        {
            bool hydraulicProfilesAreEqual = true;
            if (!Probability.Equals(hydraulicProfileForComparison.Probability))
            {
                hydraulicProfilesAreEqual = false;
            }
            if (!FileName.Equals(hydraulicProfileForComparison.FileName))
            {
                hydraulicProfilesAreEqual = false;
            }
            return hydraulicProfilesAreEqual;
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

        public XElement ToXML()
        {
            XElement elem = new XElement(PROFILE);
            elem.SetAttributeValue(PATH, FileName);
            elem.SetAttributeValue(PROB, Probability);
            elem.SetAttributeValue(PROFILE_NAME, ProfileName);
            return elem;
        }

        public string GetFilePath(string parentDirectory)
        {
            return parentDirectory + "\\" + FileName;
        }

    }
}

