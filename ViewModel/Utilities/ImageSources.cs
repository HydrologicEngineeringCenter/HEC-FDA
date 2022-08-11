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
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Watershed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Utilities
{
    public static class ImageSources
    {
        private static readonly string IMAGE_PREFIX = "pack://application:,,,/HEC.FDA.View;component/Resources/";
        
        public static readonly string ALTERNATIVE_IMAGE = IMAGE_PREFIX + "Alternatives_20x20.png";
        
        public static readonly string AGGREGATED_STAGE_DAMAGE_IMAGE = IMAGE_PREFIX + "StageDamage.png";

        public static readonly string ALTERNATIVE_COMPARISON_REPORT_IMAGE = IMAGE_PREFIX + "AlternativeComparisonReport_20x20.png";

        public static readonly string INFLOW_OUTFLOW_IMAGE = IMAGE_PREFIX + "InflowOutflowCircle.png";

        public static readonly string FREQUENCY_IMAGE = IMAGE_PREFIX + "FrequencyCurve.png";

        public static readonly string LEVEE_FEATURE_IMAGE = IMAGE_PREFIX + "LeveeFeature.png";

        public static readonly string FAILURE_IMAGE = IMAGE_PREFIX + "FailureFunction.png";

        public static readonly string IMPACT_AREAS_IMAGE = IMAGE_PREFIX + "ImpactAreas.png";

        public static readonly string SCENARIO_IMAGE = IMAGE_PREFIX + "ImpactAreaScenario_20x20.png";

        public static readonly string INVENTORY_ELEMENT_IMAGE = IMAGE_PREFIX + "StructureInventory.png";

        public static readonly string EXTERIOR_INTERIOR_IMAGE = IMAGE_PREFIX + "ExteriorInteriorStage.png";

        public static readonly string RATING_IMAGE = IMAGE_PREFIX + "RatingCurve.png";

        public static readonly string TERRAIN_IMAGE = IMAGE_PREFIX + "Terrain.png";

        public static readonly string WATER_SURFACE_ELEVATION_IMAGE = IMAGE_PREFIX + "WaterSurfaceElevation.png";

        public static readonly string ADD_IMAGE = IMAGE_PREFIX + "Add.png";

        public static string GetImage(ChildElement elem)
        {
            string image = null;

            if (elem.GetType() == typeof(TerrainElement))
            {
                image = TERRAIN_IMAGE;
            }
            else if (elem.GetType() == typeof(ImpactAreaElement))
            {
                image = IMPACT_AREAS_IMAGE;
            }
            else if (elem.GetType() == typeof(IndexPointsElement))
            {
                image = WATER_SURFACE_ELEVATION_IMAGE;
            }
            else if (elem.GetType() == typeof(HydraulicElement))
            {
                image = WATER_SURFACE_ELEVATION_IMAGE;
            }
            else if (elem.GetType() == typeof(AnalyticalFrequencyElement))
            {
                image = FREQUENCY_IMAGE;
            }
            else if (elem.GetType() == typeof(InflowOutflowElement))
            {
                image = INFLOW_OUTFLOW_IMAGE;
            }
            else if (elem.GetType() == typeof(RatingCurveElement))
            {
                image = RATING_IMAGE;
            }
            else if (elem.GetType() == typeof(ExteriorInteriorElement))
            {
                image = EXTERIOR_INTERIOR_IMAGE;
            }
            else if (elem.GetType() == typeof(LeveeFeatureElement))
            {
                image = LEVEE_FEATURE_IMAGE;
            }
            else if (elem.GetType() == typeof(InventoryElement))
            {
                image = INVENTORY_ELEMENT_IMAGE;
            }
            else if (elem.GetType() == typeof(AggregatedStageDamageElement))
            {
                image = AGGREGATED_STAGE_DAMAGE_IMAGE;
            }
            else if (elem.GetType() == typeof(IASElementSet))
            {
                image = SCENARIO_IMAGE;
            }
            else if (elem.GetType() == typeof(AlternativeElement))
            {
                image = ALTERNATIVE_IMAGE;
            }
            else if (elem is AlternativeComparisonReportElement)
            {
                image = ALTERNATIVE_COMPARISON_REPORT_IMAGE;
            }

            return image;
        }

    }
}
