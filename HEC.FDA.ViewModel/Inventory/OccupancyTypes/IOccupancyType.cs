﻿using Statistics;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public interface IOccupancyType
    {
        int GroupID { get; set; }
        int ID { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string DamageCategory { get; set; }

        OccTypeAsset StructureItem { get; set; }
        OccTypeAssetWithRatio ContentItem { get; set; }
        OccTypeAsset VehicleItem { get; set; }
        OccTypeAssetWithRatio OtherItem { get; set; }
        ContinuousDistribution FoundationHeightUncertainty { get; set; }

        XElement ToXML();
        
    }
}