using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.TableWithPlot;
using Statistics;
using Statistics.Distributions;
using static HEC.FDA.ViewModel.Inventory.OccupancyTypes.OccTypeItem;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    internal class OccupancyType : BaseViewModel,  IOccupancyType
    {
        public string Description { get; set; }
        public string DamageCategory { get; set; }

        public OccTypeItem StructureItem { get; set; }
        public OccTypeItemWithRatio ContentItem { get; set; }
        public OccTypeItem VehicleItem { get; set; }
        public OccTypeItemWithRatio OtherItem { get; set; }
        public ContinuousDistribution FoundationHeightUncertainty { get; set; }

        public int GroupID { get; set; }
        public int ID { get; set; }

        public OccupancyType(string name, string damCatName, int groupId)
        {
            Name = name;
            DamageCategory = damCatName;
            Description = "";
            GroupID = groupId;
            StructureItem = CreateDefaultItem(OcctypeItemType.structure, true);
            ContentItem = CreateDefaultItemWithRatio(OcctypeItemType.content, true);
            VehicleItem = CreateDefaultItem(OcctypeItemType.vehicle, true);
            OtherItem = CreateDefaultItemWithRatio(OcctypeItemType.other, false);
            FoundationHeightUncertainty = new Deterministic(0);

            OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();
            ID = manager.GetNextAvailableId();
        }

        public OccupancyType(string name, string description, int groupID, string damageCategory, OccTypeItem structureItem,
            OccTypeItemWithRatio contentItem, OccTypeItem vehicleItem, OccTypeItemWithRatio otherItem, ContinuousDistribution foundationHtUncertainty, int id)
        {
            Name = name;
            Description = description;
            GroupID = groupID;
            DamageCategory = damageCategory;
            StructureItem = structureItem;
            ContentItem = contentItem;
            VehicleItem = vehicleItem;
            OtherItem = otherItem;
            FoundationHeightUncertainty = foundationHtUncertainty;
            ID = id;
        }
        public OccupancyType(string name, string damageCategoryName)
        {
            Name = name;
            DamageCategory = damageCategoryName;
        }

        public OccupancyType(IOccupancyType ot)
        {
            Name = ot.Name;
            Description = ot.Description;
            DamageCategory = ot.DamageCategory;
            FoundationHeightUncertainty = ot.FoundationHeightUncertainty;
            StructureItem = new OccTypeItem( ot.StructureItem);
            ContentItem = new OccTypeItemWithRatio( ot.ContentItem);
            VehicleItem = new OccTypeItem( ot.VehicleItem);
            OtherItem = new OccTypeItemWithRatio( ot.OtherItem);
            GroupID = ot.GroupID;
            ID = ot.ID;        
        }

        private OccTypeItem CreateDefaultItem(OcctypeItemType itemType, bool isSelected)
        {
            ComputeComponentVM structureCurve = new ComputeComponentVM("Stage-Damage", "Stage", "Damage");
            ContinuousDistribution structValueUncert = new Deterministic(0);
            return new OccTypeItem(itemType,  isSelected, structureCurve, structValueUncert);
        }

        private OccTypeItemWithRatio CreateDefaultItemWithRatio(OcctypeItemType itemType, bool isSelected)
        {
            ComputeComponentVM structureCurve = new ComputeComponentVM("Stage-Damage", "Stage", "Damage");
            ContinuousDistribution structValueUncert = new Deterministic(0);
            ContinuousDistribution structValueUncertRatio = new Deterministic(0);
            bool isByVal = true;
            return new OccTypeItemWithRatio(itemType, isSelected, structureCurve, structValueUncert, structValueUncertRatio, isByVal);
        }
    }
}
