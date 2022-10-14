using HEC.FDA.Model.hydraulics.enums;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.Model.hydraulics
{
    public class HydraulicDataset
    {
        public const string HYDRAULIC_DATA_SET = "HydraulicDataSet";
        private const string HYDRAULIC_TYPE_XML_TAG = "HydroType";
        private const string PROFILES = "Profiles";

        public List<HydraulicProfile> HydraulicProfiles { get; } = new List<HydraulicProfile>();
        public HydraulicDataSource DataSource { get; set; }

        public HydraulicDataset(List<HydraulicProfile> profiles, HydraulicDataSource dataSource)
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
                HydraulicProfiles.Add(new HydraulicProfile(elem));
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

        public static void CorrectDryStructureDepths(ref float[] wsesToCorrect, float[] firstFloorElevs, float[] nextProfileWses = null)
        {
            float offsetForDryStructures = 9;
            float offsetForBarelyDryStructures = 2;
            float dryCellValue = -9999;
            if (nextProfileWses == null)
            {
                for (int i = 0; i < wsesToCorrect.Length; i++)
                {
                    //The case where the largest profile has dry structures
                    if (wsesToCorrect[i] == dryCellValue)
                    {
                        wsesToCorrect[i] = (firstFloorElevs[i] - offsetForDryStructures);
                    }
                }
            }
            for (int i = 0; i < wsesToCorrect.Length; i++)
            {
                if (wsesToCorrect[i] == dryCellValue)
                {
                    //The case where the next largest profile is also dry
                    if (nextProfileWses[i] == dryCellValue)
                    {
                        wsesToCorrect[i] = (firstFloorElevs[i] - offsetForDryStructures);
                    }
                    //The case where the next largest profile is not dry
                    else
                    {
                        wsesToCorrect[i] = (firstFloorElevs[i] - offsetForBarelyDryStructures);
                    }
                }
            }
        }
    }
}