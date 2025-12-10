using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.AlternativeComparisonReport;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.LifeLoss;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Watershed;
using System;
using System.Collections.Generic;


namespace HEC.FDA.ViewModel.Utilities
{
    public static class ImageSources
    {
        private static readonly string IMAGE_PREFIX = "pack://application:,,,/HEC.FDA.View;component/Resources/";

        private static readonly Dictionary<Type, string> _TypeToImageDict = new()
        {
            {typeof(TerrainElement),IMAGE_PREFIX + "Terrain.png" },
            {typeof(AlternativeComparisonReportElement), IMAGE_PREFIX + "AlternativeComparisonReport_20x20.png" },
            {typeof(AlternativeElement), IMAGE_PREFIX + "Alternatives_20x20.png" },
            {typeof(ExteriorInteriorElement), IMAGE_PREFIX + "ExteriorInteriorStage.png" },
            {typeof(FrequencyElement), IMAGE_PREFIX + "FrequencyCurve.png" },
            {typeof(HydraulicElement), IMAGE_PREFIX + "WaterSurfaceElevation.png" },
            {typeof(IASElement), IMAGE_PREFIX + "ImpactAreaScenario_20x20.png" },
            {typeof(ImpactAreaElement), IMAGE_PREFIX + "ImpactAreas.png" },
            {typeof(IndexPointsElement), IMAGE_PREFIX + "ImpactAreas.png" },
            {typeof(InflowOutflowElement), IMAGE_PREFIX + "InflowOutflowCircle.png" },
            {typeof(LateralStructureElement), IMAGE_PREFIX + "LeveeFeature.png" },
            {typeof(AggregatedStageDamageElement), IMAGE_PREFIX + "StageDamage.png" },
            {typeof(StageDischargeElement), IMAGE_PREFIX + "RatingCurve.png" },
            {typeof(InventoryElement), IMAGE_PREFIX + "StructureInventory.png"},
            {typeof(OccupancyTypesElement), IMAGE_PREFIX + "StructureInventory.png"},
            {typeof(StageLifeLossElement), IMAGE_PREFIX + "lifesim.png" }
        };

        public static readonly string FAILURE_IMAGE = IMAGE_PREFIX + "FailureFunction.png";
        //<a target="_blank" href="https://icons8.com/icon/PE5vY419cmPT/caution">Caution</a> icon by <a target="_blank" href="https://icons8.com">Icons8</a>
        public static readonly string GREEN_CHECKMARK_IMAGE = IMAGE_PREFIX + "greenCheckmark.png";
        public static readonly string ERROR_IMAGE = IMAGE_PREFIX + "errorMark.png";
        public static readonly string CAUTION_IMAGE = IMAGE_PREFIX + "cautionMark.png";

        public static string GetImage(Type childElementType)
        {
            string image = null;
            if (_TypeToImageDict.ContainsKey(childElementType))
            {
                image = _TypeToImageDict[childElementType];
            }
            return image;
        }


    }
}
