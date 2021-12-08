using LifeSimGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Watershed;

namespace ViewModel.Inventory
{
    public class StructureElevationsFromTerrainFile:BaseViewModel
    {
        private string _Path;
        public StructureElevationsFromTerrainFile(string path)
        {
            _Path = path;
        }
        private RasterFeatures GetTerrainRasterFeatures(string filePath)
        {
            RasterFeatures terrainRasterFeatures = null;
            try
            {
                terrainRasterFeatures = new RasterFeatures(filePath);
                return terrainRasterFeatures;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Compute Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                //isValid = false;
                //errorMessage = "Exception thrown when reading terrain file and converting to raster features.";
                //return isValid;
                return terrainRasterFeatures;
            }

        }

        private PointD[] GetStructurePoints()
        {
            PointD[] pointDs = null;
            try
            {
                ShapefileReader myReader = new ShapefileReader(_Path);
                PointFeatures pointFeatures = (PointFeatures)myReader.ToFeatures();
                pointDs = pointFeatures.GetPointsArray();
            }
            catch (Exception ex)
            {
                //isValid = false;
                //errorMessage = "Exception thrown when reading structure file and converting to points.";
                //return isValid;
                //return pointDs;
            }
            return pointDs;
        }

        public float[] GetStructureElevationsFromTerrainFile(ref string errorMessage)
        {
            float[] elevations = null;
            //todo: should i just do a try catch around the whole thing to reduce all the if statements?
            bool isValid = true;
            List<TerrainElement> terrainElements = StudyCache.GetChildElementsOfType<TerrainElement>();
            if (terrainElements.Count > 0)
            {
                string firstTerrainName = terrainElements[0].Name;
                string filePath = Storage.Connection.Instance.GetTerrainFile(firstTerrainName);
                if (filePath != null)
                {
                    RasterFeatures terrainRasters = GetTerrainRasterFeatures(filePath);
                    if (terrainRasters != null)
                    {
                        PointD[] structPoints = GetStructurePoints();
                        if (structPoints != null)
                        {
                            //todo: i can pass in a default value for missing data
                            elevations = terrainRasters.GridReader.SampleValues(structPoints);
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
                    errorMessage = "A terrain file exists in the study but the file could not be found in the study directory with the name of: " + firstTerrainName;
                }
            }
            else
            {
                errorMessage = "You have selected to get structure elevations using a terrain file. A terrain file does not exist in this study. Import one and try again.";
            }
            return elevations;
        }


    }
}
