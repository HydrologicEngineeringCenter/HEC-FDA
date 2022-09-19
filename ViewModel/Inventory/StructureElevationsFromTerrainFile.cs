using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Watershed;
using System.IO;

namespace HEC.FDA.ViewModel.Inventory
{
    public class StructureElevationsFromTerrainFile:BaseViewModel
    {
        private string _Path;
        public StructureElevationsFromTerrainFile(string path)
        {
            _Path = path;
        }
        //private RasterFeatures GetTerrainRasterFeatures(string filePath)
        //{
        //    //RasterFeatures terrainRasterFeatures = null;
        //    //try
        //    //{
        //    //    return new RasterFeatures(filePath);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    //todo: do what?
        //    //    return terrainRasterFeatures;
        //    //}
        //}

        //private PointD[] GetStructurePoints()
        //{
        //    PointD[] pointDs = null;
        //    try
        //    {
        //        ShapefileReader myReader = new ShapefileReader(_Path);
        //        PointFeatures pointFeatures = (PointFeatures)myReader.ToFeatures();
        //        pointDs = pointFeatures.GetPointsArray();
        //    }
        //    catch (Exception ex)
        //    {
        //        //todo: do what?
        //    }
        //    return pointDs;
        //}

        public List<float> GetStructureElevationsFromTerrainFile(ref string errorMessage)
        {
            /*List<float> elevations = new List<float>();
            List<TerrainElement> terrainElements = StudyCache.GetChildElementsOfType<TerrainElement>();
            if (terrainElements.Count > 0)
            {
                //there can only be one terrain in the study
                TerrainElement elem = terrainElements[0];
                string filePath = Storage.Connection.Instance.TerrainDirectory + "\\" + elem.Name + "\\" + elem.FileName;
                if (File.Exists(filePath))
                {
                    //todo: is this going to work with the different file types? vrt, hdf, tif, flt
                    RasterFeatures terrainRasters = GetTerrainRasterFeatures(filePath);
                    if (terrainRasters != null)
                    {
                        PointD[] structPoints = GetStructurePoints();
                        if (structPoints != null)
                        {
                            //todo: i can pass in a default value for missing data
                            float[] elevs = terrainRasters.GridReader.SampleValues(structPoints);
                            if(elevs!= null && elevs.Length>0)
                            {
                                elevations.AddRange(elevs);
                            }
                            else
                            {
                                errorMessage = "No elevations were calculated for the structures.";
                            }
                        }
                        else
                        {
                            errorMessage = "Exception thrown when reading structure file and converting to points.";
                        }
                    }
                    else
                    {
                        errorMessage = "Exception thrown when reading terrain file and converting to raster features.";
                    }
                }
                else
                {
                    errorMessage = "A terrain file exists in the study but the file could not be found in the study directory with the name of: " + elem.Name;
                }
            }
            else
            {
                errorMessage = "You have selected to get structure elevations using a terrain file. A terrain file does not exist in this study. Import one and try again.";
            }
            return elevations;*/
            return null;
        }
    }
}
