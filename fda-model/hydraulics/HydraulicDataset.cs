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
    }
}