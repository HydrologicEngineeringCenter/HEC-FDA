using RasMapperLib;
using RasMapperLib.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fda_model.hydraulics
{
    public class HydraulicDataset
    {
        public List<HydraulicProfile> HydraulicProfiles { get; set; }
        public HydraulicDataset(List<HydraulicProfile> profiles)
        {
            profiles.Sort();
            HydraulicProfiles = profiles;
        }
        
        //TODO: correct depths in profiles to be -2 and -9 for dry. 
    }
}