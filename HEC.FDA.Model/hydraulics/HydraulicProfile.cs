using Geospatial.IO;
using Geospatial.Rasters;
using HEC.FDA.Model.hydraulics.enums;
using RasMapperLib;
using RasMapperLib.Mapping;
using System;
using System.Xml.Linq;
using System.Collections.Generic;
using HEC.FDA.Model.hydraulics.Interfaces;

namespace HEC.FDA.Model.hydraulics
{
    public class HydraulicProfile :  IHydraulicProfile
    {
        #region Fields
        private const string PROFILE = "HydraulicProfile";
        private const string PATH = "Path";
        private const string PROB = "Probability";
        private const string PROFILE_NAME = "ProfileName";
        #endregion
        #region Properties
        public double Probability { get; set; }
        /// <summary>
        /// This is not the full path. This is just the file name with extension. You need to get the hydraulic element name to create the full path.
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// This really only matters for steady data. For Unsteady, we're always going to use "MAX" and Gridded data has no concept of profile name. 
        /// </summary>
        public string ProfileName { get; set; }
        #endregion
        #region Constructors
        public HydraulicProfile(double probability, string fileName, string profileName )
        {
            Probability = probability;
            FileName = fileName;
            ProfileName = profileName;
        }
        public HydraulicProfile(XElement elem)
        {
            Probability = Convert.ToDouble(elem.Attribute(PROB).Value);
            FileName = elem.Attribute(PATH).Value;
            //Gridded Data doesn't need a profile name. 
            var profileNameAttr = elem.Attribute(PROFILE_NAME);
            if (profileNameAttr != null)
            {
                ProfileName = elem.Attribute(PROFILE_NAME).Value;
            }
        }
        #endregion
        #region Methods
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
            string filePath = GetFilePath(parentDirectory);
            var baseDs = TiffDataSource<float>.TryLoad(filePath);

            if (baseDs == null)
            {
                return new float[pts.Count];
            }
            RasterPyramid<float> baseRaster = (RasterPyramid<float>)baseDs.AsRasterizer();

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
            string fullPathToResult = GetFilePath(parentDirectory);
            var rasResult = new RASResults(fullPathToResult);
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
        /// <summary>
        /// allows for sorting based on probability of the profile.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool Equals(IHydraulicProfile hydraulicProfileForComparison)
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
        public XElement ToXML()
        {
            XElement elem = new(PROFILE);
            elem.SetAttributeValue(PATH, FileName);
            elem.SetAttributeValue(PROB, Probability);
            elem.SetAttributeValue(PROFILE_NAME, ProfileName);
            return elem;
        }
        public string GetFilePath(string parentDirectory)
        {
            return parentDirectory + "\\" + FileName;
        }
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is IHydraulicProfile otherProfile)
                return Probability.CompareTo(otherProfile.Probability);
            else
                throw new ArgumentException("Object is not a HydraulicProfile");
        }
        #endregion
    }
}

