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
        public const string HYDRAULIC_DATA_SET = "HydraulicDataSet";
        private const string PROBABILITY = "Probability";
        private const string WSE = "wse";
        private const string HYDRAULIC_TYPE_XML_TAG = "HydroType";
        private const string PROFILES = "Profiles";

        public List<IHydraulicProfile> HydraulicProfiles { get; } = new List<IHydraulicProfile>();
        public HydraulicDataSource DataSource { get; set; }
        public List<UncertainPairedData> GetGraphicalStageFrequency(string pointShapefileFilePath, string parentDirectory)
        {
            List<UncertainPairedData> ret = new List<UncertainPairedData>();
            PointFeatureLayer indexPoint = new PointFeatureLayer("Structure_Inventory", pointShapefileFilePath);
            PointMs indexPoints = new PointMs(indexPoint.Points().Select(p => p.PointM()));
            for (int j = 0; j < indexPoints.Count; j++)
            {
                double[] probs = new double[HydraulicProfiles.Count];
                float[] wses = new float[HydraulicProfiles.Count];
                for (int i = 0; i < HydraulicProfiles.Count; i++)
                {
                    PointM pt = indexPoints[j];
                    PointMs converted = new PointMs();
                    converted.Add(pt);
                    float[] singleWSE = HydraulicProfiles[i].GetWSE(converted, DataSource, parentDirectory);
                    probs[i] = HydraulicProfiles[i].Probability;
                    wses[i] = singleWSE[0];
                }
                IDistribution[] distributions = new IDistribution[wses.Length];
                for( int i=0; i<wses.Length; i++)
                {
                    distributions[i] = new Deterministic(wses[i]);
                }
                UncertainPairedData pd = new(probs, distributions, new CurveMetaData(xlabel: PROBABILITY, ylabel: WSE, name: "dealWithThisLater"));
                ret.Add(pd);
            }
            return ret;
        }

        public HydraulicDataset(List<IHydraulicProfile> profiles, HydraulicDataSource dataSource)
        {
            profiles.Sort();
            profiles.Reverse();
            HydraulicProfiles = profiles;
            DataSource = dataSource;
        }

        public HydraulicDataset(XElement xElement)
        {
            string hydroType = xElement.Attribute(HYDRAULIC_TYPE_XML_TAG).Value;
            Enum.TryParse(hydroType, out HydraulicDataSource myHydroType);
            DataSource = myHydroType;

            IEnumerable<XElement> profiles = xElement.Elements(PROFILES);
            IEnumerable<XElement> profileElems = profiles.Elements();

            foreach (XElement elem in profileElems)
            {
                HydraulicProfiles.Add((IHydraulicProfile)new HydraulicProfile(elem));
            }
        }
        public XElement ToXML()
        {
            XElement elem = new XElement(HYDRAULIC_DATA_SET);
            elem.SetAttributeValue(HYDRAULIC_TYPE_XML_TAG, DataSource);
            //path and probs
            XElement profiles = new XElement(PROFILES);
            foreach (HydraulicProfile profile in HydraulicProfiles)
            {
                profiles.Add(profile.ToXML());
            }
            elem.Add(profiles);
            return elem;
        }
        public (List<double>, List<float[]>) GetHydraulicDatasetInFloatsWithProbabilities(Inventory inventory, string hydraulicParentDirectory)
        {
            List<float[]> waterData = new List<float[]>();
            List<double> profileProbabilities = new List<double>();
            foreach (IHydraulicProfile hydraulicProfile in HydraulicProfiles)
            {
                float[] wsesAtStructures = hydraulicProfile.GetWSE(inventory.GetPointMs(), DataSource, hydraulicParentDirectory);
                waterData.Add(wsesAtStructures);
                profileProbabilities.Add(hydraulicProfile.Probability);
            }
            for (int i = 0; i < waterData.Count - 1; i++)
            {
                waterData[i] = CorrectDryStructureWSEs(waterData[i], inventory.GroundElevations, waterData[i + 1]);
            }
            waterData[waterData.Count - 1] = CorrectDryStructureWSEs(waterData[waterData.Count - 1], inventory.GroundElevations);
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
    }
}