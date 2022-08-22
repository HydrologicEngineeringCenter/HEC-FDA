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
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Saving
{
    /// <summary>
    /// Used to get persistence managers for each element. Holds the cache that contains all the elements in memory
    /// </summary>
    public static class PersistenceFactory
    {

        /// <summary>
        /// Cache that holds all the elements in memory
        /// </summary>
        public static FDACache StudyCacheForSaving { get; set; }

        private static Dictionary<Type, string> TypeToTableDict = new Dictionary<Type, string>()
        {
            {typeof(TerrainElement), "terrains" },
            {typeof(AlternativeComparisonReportElement), "alternative_comparison_reports" },
            {typeof(AlternativeElement), "alternatives" },
            {typeof(ExteriorInteriorElement), "exterior_interior_relationships" },
            {typeof(AnalyticalFrequencyElement), "analytical_frequency_relationships" },
            {typeof(HydraulicElement), "hydraulics" },
            {typeof(IASElement), "impact_area_scenarios" },
            {typeof(ImpactAreaElement), "impact_area_set" },
            {typeof(IndexPointsElement), "index_points" },
            {typeof(InflowOutflowElement), "regulated_unregulated_relationships" },
            {typeof(LateralStructureElement), "lateral_structures" },
            {typeof(OccupancyTypesElement), "occupancy_types" },
            {typeof(AggregatedStageDamageElement), "stage_damage_relationships" },
            {typeof(StageDischargeElement), "stage_discharge_relationships" },
            {typeof(InventoryElement), "structure_inventories"},
            {typeof(StudyPropertiesElement), "study_properties" },
        };

        public static IElementManager GetElementManager<T>()
            where T : ChildElement
        {
            string table = TypeToTableDict[typeof(T)];
            return new SavingBase<T>(StudyCacheForSaving, table);
        }
        

        public static IElementManager GetElementManager(ChildElement elem)
        {
            Type genericType = elem.GetType();
            Type baseType = typeof(SavingBase<>);
            Type realType = baseType.MakeGenericType(genericType);
            string tableName = TypeToTableDict[genericType];

            return (IElementManager)Activator.CreateInstance(realType, StudyCacheForSaving, tableName);
        }

        public static TerrainElementPersistenceManager GetTerrainManager()
        {
            return new TerrainElementPersistenceManager(StudyCacheForSaving, TypeToTableDict[typeof(TerrainElement)]);
        }
        public static HydraulicPersistenceManager GetWaterSurfaceManager()
        {
            return new HydraulicPersistenceManager(StudyCacheForSaving, TypeToTableDict[typeof(HydraulicElement)]);
        }
        
        public static StructureInventoryPersistenceManager GetStructureInventoryManager()
        {
            return new StructureInventoryPersistenceManager(StudyCacheForSaving, TypeToTableDict[typeof(InventoryElement)]);
        }
        public static IASPersistenceManager GetIASManager()
        {
            return new IASPersistenceManager(StudyCacheForSaving, TypeToTableDict[typeof(IASElement)]);
        }

    }
}
