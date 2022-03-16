using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.TableWithPlot;
using Statistics;
using Statistics.Distributions;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    internal class OccupancyType : BaseViewModel,  IOccupancyType
    {
        public string Description { get; set; }
        public string DamageCategory { get; set; }
        public ContinuousDistribution ContentToStructureValueUncertainty { get; set; }
        public double ContentToStructureValue { get; set; }
        public ContinuousDistribution OtherToStructureValueUncertainty { get; set; }
        public double OtherToStructureValue { get; set; }

        //These booleans determine if the content/vehicle/other curves are a ratio of structure value or not
        public bool IsContentRatio { get; set; }
        public bool IsVehicleRatio { get; set; }
        public bool IsOtherRatio { get; set; }
        public OccTypeItem StructureItem { get; set; }
        public OccTypeItem ContentItem { get; set; }
        public OccTypeItem VehicleItem { get; set; }
        public OccTypeItem OtherItem { get; set; }
        public ContinuousDistribution FoundationHeightUncertainty { get; set; }

        public int GroupID { get; set; }
        public int ID { get; set; }

        public OccupancyType(string name, string damCatName, int groupId)
        {
            Name = name;
            DamageCategory = damCatName;
            Description = "";
            GroupID = groupId;
            StructureItem = CreateDefaultItem(true);
            ContentItem = CreateDefaultItem(true);
            VehicleItem = CreateDefaultItem(true);
            OtherItem = CreateDefaultItem(false);
            FoundationHeightUncertainty = new Deterministic(0);
            ContentToStructureValueUncertainty = new Deterministic(0);
            OtherToStructureValueUncertainty = new Deterministic(0);

            OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();
            ID = manager.GetNextAvailableId();
        }


        public OccupancyType(string name, string description, int groupID, string damageCategory, OccTypeItem structureItem,
            OccTypeItem contentItem, OccTypeItem vehicleItem, OccTypeItem otherItem, ContinuousDistribution foundationHtUncertainty,
            ContinuousDistribution contentToStructureValueUncertainty, ContinuousDistribution otherToStructureValueUncertainty,
            double contentToStructureValue, double otherToStructureValue, int id)
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
            ContentToStructureValueUncertainty = contentToStructureValueUncertainty;
            OtherToStructureValueUncertainty = otherToStructureValueUncertainty;
            ContentToStructureValue = contentToStructureValue;
            OtherToStructureValue = otherToStructureValue;
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
            ContentToStructureValueUncertainty = ot.ContentToStructureValueUncertainty;
            OtherToStructureValueUncertainty = ot.OtherToStructureValueUncertainty;
            ContentToStructureValue = ot.ContentToStructureValue;
            OtherToStructureValue = ot.OtherToStructureValue;
            IsContentRatio = ot.IsContentRatio;
            IsVehicleRatio = ot.IsVehicleRatio;
            IsOtherRatio = ot.IsOtherRatio;
            StructureItem = ot.StructureItem;
            ContentItem = ot.ContentItem;
            VehicleItem = ot.VehicleItem;
            OtherItem = ot.OtherItem;
            GroupID = ot.GroupID;
            ID = ot.ID;        
        }

        private OccTypeItem CreateDefaultItem(bool isSelected)
        {
            ComputeComponentVM structureCurve = new ComputeComponentVM("Stage-Damage", "Stage", "Damage");
            ContinuousDistribution structValueUncert = new Deterministic(0);
            return new OccTypeItem(isSelected, structureCurve, structValueUncert);
        }
    }
}
