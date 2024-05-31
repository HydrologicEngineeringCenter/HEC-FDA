using Geospatial.GDALAssist;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.Model.hydraulics.Interfaces;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.structures;
using RasMapperLib;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HEC.FDA.Model.hydraulics
{
    public class HydraulicDataset
    {
        #region Fields
        public const string HYDRAULIC_DATA_SET = "HydraulicDataSet";
        private const string HYDRAULIC_TYPE_XML_TAG = "HydroType";
        private const string PROFILES = "Profiles";
        #endregion
        #region Properties
        public List<IHydraulicProfile> HydraulicProfiles { get; } = new List<IHydraulicProfile>();
        public HydraulicDataSource DataSource { get; set; }
        #endregion
        #region Constructor
        public HydraulicDataset(List<IHydraulicProfile> profiles, HydraulicDataSource dataSource)
        {
            profiles.Sort();
            profiles.Reverse();
            HydraulicProfiles = profiles;
            DataSource = dataSource;
        }
        /// <summary>
        /// Empty constructor for loading from XML. Allows the load form XML to fail, but still have an object to add to the tree to delete.
        /// if we want to remove the dataset.
        /// </summary>
        public HydraulicDataset()
        {
        }
        public bool LoadFromXML(XElement xElement)
        {
            try
            {
                string hydroType = xElement.Attribute(HYDRAULIC_TYPE_XML_TAG).Value;
                Enum.TryParse(hydroType, out HydraulicDataSource myHydroType);
                DataSource = myHydroType;

                IEnumerable<XElement> profiles = xElement.Elements(PROFILES);
                IEnumerable<XElement> profileElems = profiles.Elements();

                foreach (XElement elem in profileElems)
                {
                    HydraulicProfile newProfile = new(elem);
                    HydraulicProfiles.Add(newProfile);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion
        #region Methods
        public List<UncertainPairedData> GetGraphicalStageFrequency(string pointShapefileFilePath, string parentDirectory, Projection studyProjection)
        {
            List<UncertainPairedData> ret = new();
            PointFeatureLayer indexPoint = new("ThisNameIsNotUsedForAnythingHere", pointShapefileFilePath);
            if (!indexPoint.SourceFileExists)
            {
                return null;
            }
            PointMs indexPoints = new(indexPoint.Points().Select(p => p.PointM()));
            
            //reproject the point to the study projection. For GetWSE, all the reprojection is done in the Inventory class. We aren't using an inventory here, so we need to do it manually.
            Projection pointProj = RASHelper.GetVectorProjection(indexPoint);
            if (!pointProj.IsEqual(studyProjection))
            {
                indexPoints.Project(pointProj,studyProjection);
            }
            
            for (int j = 0; j < indexPoints.Count; j++)
            {
                double[] probs = new double[HydraulicProfiles.Count];
                float[] wses = new float[HydraulicProfiles.Count];
                for (int i = 0; i < HydraulicProfiles.Count; i++)
                {
                    //Get WSE expects a PointMs, so we need to convert the PointM to a PointMs
                    PointM pt = indexPoints[j];
                    PointMs converted = new PointMs();
                    converted.Add(pt);
                    float[] singleWSE = HydraulicProfiles[i].GetWSE(converted, DataSource, parentDirectory);
                    probs[i] = HydraulicProfiles[i].Probability;
                    wses[i] = singleWSE[0];
                }
                IDistribution[] distributions = new IDistribution[wses.Length];
                for (int i = 0; i < wses.Length; i++)
                {
                    distributions[i] = new Deterministic(wses[i]);
                }
                UncertainPairedData pd = new(probs, distributions, new CurveMetaData());
                ret.Add(pd);
            }
            return ret;
        }
        public XElement ToXML()
        {
            XElement elem = new(HYDRAULIC_DATA_SET);
            elem.SetAttributeValue(HYDRAULIC_TYPE_XML_TAG, DataSource);
            //path and probs
            XElement profiles = new(PROFILES);
            foreach (HydraulicProfile profile in HydraulicProfiles)
            {
                profiles.Add(profile.ToXML());
            }
            elem.Add(profiles);
            return elem;
        }
        /// <summary>
        /// Gets hydraulic data for every structure in the inventory and corrects no data values (-9999s) to realistic elevations based on the logic in cref="SomeOtherClass 
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="hydraulicParentDirectory"></param>
        /// <returns> A tuple of list of double (the exceedence probabilities releated to the WSPs) and a list of float arrays (The water surface elevations for every structure in the inventory</returns>
        public (List<double>, List<float[]>) GetHydraulicDatasetInFloatsWithProbabilities(Inventory inventory, string hydraulicParentDirectory)
        {
            List<float[]> waterData = new();
            List<double> profileProbabilities = new();
            foreach (IHydraulicProfile hydraulicProfile in HydraulicProfiles)
            {
                float[] wsesAtStructures = hydraulicProfile.GetWSE(inventory.GetPointMs(), DataSource, hydraulicParentDirectory);
                waterData.Add(wsesAtStructures);
                profileProbabilities.Add(hydraulicProfile.Probability);
            }
            for (int i = 0; i < waterData.Count - 1; i++)
            {
                waterData[i] = CorrectDryStructureWSEs(waterData[i], inventory.GetGroundElevations(), waterData[i + 1]);
            }
            waterData[waterData.Count - 1] = CorrectDryStructureWSEs(waterData[waterData.Count - 1], inventory.GetGroundElevations());
            return (profileProbabilities, waterData);

        }
        public static float[] CorrectDryStructureWSEs(float[] wsesToCorrect, float[] groundElevs, float[] nextProfileWses = null)
        {
            float offsetForDryStructures = 9;
            float offsetForBarelyDryStructures = 2;
            if (nextProfileWses == null)
            {
                for (int i = 0; i < wsesToCorrect.Length; i++)
                {
                    bool dryInCurrentProfile = wsesToCorrect[i] < groundElevs[i];
                    //The case where the largest profile has dry structures
                    if (dryInCurrentProfile)
                    {
                        wsesToCorrect[i] = (groundElevs[i] - offsetForDryStructures);
                    }
                }
            }
            else
            {
                for (int i = 0; i < wsesToCorrect.Length; i++)
                {
                    bool dryInNextProfile = nextProfileWses[i] < groundElevs[i];
                    bool dryInCurrentProfile = wsesToCorrect[i] < groundElevs[i];
                    if (dryInCurrentProfile)
                    {
                        //The case where the next largest profile is also dry
                        if (dryInNextProfile)
                        {
                            wsesToCorrect[i] = (groundElevs[i] - offsetForDryStructures);
                        }
                        //The case where the next largest profile is not dry
                        else
                        {
                            wsesToCorrect[i] = (groundElevs[i] - offsetForBarelyDryStructures);
                        }
                    }
                }
            }
            return wsesToCorrect;
        }
        #endregion
    }
}