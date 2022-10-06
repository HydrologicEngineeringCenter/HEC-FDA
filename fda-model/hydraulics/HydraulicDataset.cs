using HEC.FDA.Model.structures;
using System.Collections.Generic;

namespace HEC.FDA.Model.hydraulics
{
    public class HydraulicDataset
    {
        public List<HydraulicProfile> HydraulicProfiles { get; set; }
        public HydraulicDataset(List<HydraulicProfile> profiles)
        {
            profiles.Sort();
            profiles.Reverse();
            HydraulicProfiles = profiles;
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