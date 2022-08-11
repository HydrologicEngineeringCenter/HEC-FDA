using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.AlternativeComparisonReport;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.IndexPoints;
using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;

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
            {typeof(IASElementSet), "impact_area_scenarios" },
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
        
        //public static IElementManager GetElementManager(Type childElementType)
        //{
        //    return GetElementManager<TerrainElement>();

        //    IElementManager manager = null;

        //    if (childElementType == typeof(TerrainElement))
        //    {
        //        manager = new TerrainElementPersistenceManager(StudyCacheForSaving);
        //    }
        //    return manager;
        //}

        public static IElementManager GetElementManager(ChildElement elem)
        {
            Type genericType = elem.GetType();
            Type baseType = typeof(SavingBase<>);
            Type realType = baseType.MakeGenericType(genericType);
            string tableName = TypeToTableDict[genericType];

            return (IElementManager)Activator.CreateInstance(realType, StudyCacheForSaving, tableName);
        }

        //public static IElementManager GetElementManager(ChildElement element)
        //{
        //    IElementManager manager = null;
        //    if (element.GetType() == typeof(TerrainElement))
        //    {
        //        manager = new TerrainElementPersistenceManager(StudyCacheForSaving);
        //    }
        //    else if(element.GetType() == typeof(StudyPropertiesElement))
        //    {
        //        manager = new StudyPropertiesPersistenceManager(StudyCacheForSaving);
        //    }
        //    else if (element.GetType() == typeof(StageDischargeElement))
        //    {
        //        manager = new StageDischargePersistenceManager(StudyCacheForSaving);
        //    }
        //    else if (element.GetType() == typeof(ExteriorInteriorElement))
        //    {
        //        manager = new ExteriorInteriorPersistenceManager(StudyCacheForSaving);
        //    }
        //    else if (element.GetType() == typeof(ImpactAreaElement))
        //    {
        //        manager = new ImpactAreaPersistenceManager(StudyCacheForSaving);
        //    }
        //    else if (element.GetType() == typeof(HydraulicElement))
        //    {
        //        manager = new HydraulicPersistenceManager(StudyCacheForSaving);
        //    }
        //    else if (element.GetType() == typeof(AnalyticalFrequencyElement))
        //    {
        //        manager = new FlowFrequencyPersistenceManager(StudyCacheForSaving);
        //    }
        //    else if (element.GetType() == typeof(InflowOutflowElement))
        //    {
        //        manager = new InflowOutflowPersistenceManager(StudyCacheForSaving);
        //    }
        //    else if (element.GetType() == typeof(LeveeFeatureElement))
        //    {
        //        manager = new SavingBase<LeveeFeatureElement>(StudyCacheForSaving);
        //    }
        //    else if (element.GetType() == typeof(InventoryElement))
        //    {
        //        manager = new StructureInventoryPersistenceManager(StudyCacheForSaving);
        //    }
        //    else if (element.GetType() == typeof(AggregatedStageDamageElement))
        //    {
        //        manager = new StageDamagePersistenceManager(StudyCacheForSaving);
        //    }
        //    else if (element.GetType() == typeof(IASElementSet))
        //    {
        //        manager = new IASPersistenceManager(StudyCacheForSaving);
        //    }
        //    else if(element.GetType() == typeof(AlternativeElement))
        //    {
        //        manager = new AlternativePersistenceManager(StudyCacheForSaving);
        //    }
        //    else if(element is AlternativeComparisonReportElement)
        //    {
        //        manager = new AlternativeComparisonReportPersistenceManager(StudyCacheForSaving);
        //    }
        //    else if(element is IndexPointsElement)
        //    {
        //        manager = new IndexPointsPersistenceManager(StudyCacheForSaving);
        //    }

        //    return manager;
        //}

        //public static IndexPointsPersistenceManager GetIndexPointsPersistenceManager()
        //{
        //    return new IndexPointsPersistenceManager(StudyCacheForSaving);
        //}

        //public static StageDischargePersistenceManager GetRatingManager()
        //{
        //    StageDischargePersistenceManager manager = new StageDischargePersistenceManager(StudyCacheForSaving);
        //    return manager;
        //}
        public static OccTypePersistenceManager GetOccTypeManager()
        {
            return new OccTypePersistenceManager(StudyCacheForSaving, TypeToTableDict[typeof(OccupancyTypesElement)]);
        }
        //public static StudyPropertiesPersistenceManager GetStudyPropertiesManager()
        //{
        //    StudyPropertiesPersistenceManager manager = new StudyPropertiesPersistenceManager(StudyCacheForSaving);
        //    return manager;
        //}
        public static TerrainElementPersistenceManager GetTerrainManager()
        {
            return new TerrainElementPersistenceManager(StudyCacheForSaving, TypeToTableDict[typeof(TerrainElement)]);
        }
        //public static  ImpactAreaPersistenceManager GetImpactAreaManager( )
        //{
        //    return new ImpactAreaPersistenceManager(StudyCacheForSaving);
        //}
        public static HydraulicPersistenceManager GetWaterSurfaceManager()
        {
            return new HydraulicPersistenceManager(StudyCacheForSaving, TypeToTableDict[typeof(HydraulicElement)]);
        }
        //public static FlowFrequencyPersistenceManager GetFlowFrequencyManager( )
        //{
        //    return new FlowFrequencyPersistenceManager(StudyCacheForSaving);
        //}
        //public static InflowOutflowPersistenceManager GetInflowOutflowManager( )
        //{
        //    return new InflowOutflowPersistenceManager(StudyCacheForSaving);
        //}
        //public static ExteriorInteriorPersistenceManager GetExteriorInteriorManager( )
        //{
        //    return new ExteriorInteriorPersistenceManager(StudyCacheForSaving);
        //}
        //public static SavingBase<LeveeFeatureElement> GetLeveeManager( )
        //{
        //    return new SavingBase<LeveeFeatureElement>(StudyCacheForSaving);
        //}
        //public static StageDamagePersistenceManager GetStageDamageManager( )
        //{
        //    return new StageDamagePersistenceManager(StudyCacheForSaving);
        //}
        public static StructureInventoryPersistenceManager GetStructureInventoryManager()
        {
            return new StructureInventoryPersistenceManager(StudyCacheForSaving, TypeToTableDict[typeof(InventoryElement)]);
        }
        public static IASPersistenceManager GetIASManager()
        {
            return new IASPersistenceManager(StudyCacheForSaving, TypeToTableDict[typeof(IASElementSet)]);
        }
        //public static AlternativePersistenceManager GetAlternativeManager()
        //{
        //    return new AlternativePersistenceManager(StudyCacheForSaving);
        //}
        //public static AlternativeComparisonReportPersistenceManager GetAlternativeCompReportManager()
        //{
        //    return new AlternativeComparisonReportPersistenceManager(StudyCacheForSaving);
        //}
        //public static StudyPropertiesPersistenceManager GetStudyPropertiesPersistenceManager()
        //{
        //    return new StudyPropertiesPersistenceManager(StudyCacheForSaving);
        //}
    }
}
