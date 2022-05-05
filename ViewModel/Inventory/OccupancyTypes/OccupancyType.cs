using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using Statistics;
using Statistics.Distributions;
using static HEC.FDA.ViewModel.Inventory.OccupancyTypes.OccTypeAsset;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    internal class OccupancyType : BaseViewModel,  IOccupancyType
    {

        public string Description { get; set; }
        public string DamageCategory { get; set; }

        public OccTypeAsset StructureItem { get; set; }
        public OccTypeItemWithRatio ContentItem { get; set; }
        public OccTypeAsset VehicleItem { get; set; }
        public OccTypeItemWithRatio OtherItem { get; set; }
        public ContinuousDistribution FoundationHeightUncertainty { get; set; }

        public int GroupID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
       

        public OccupancyType(string name, string damCatName, int groupId)
        {
            Name = name;
            DamageCategory = damCatName;
            Description = "";
            GroupID = groupId;
            StructureItem = CreateDefaultAsset(OcctypeAssetType.structure, true);
            ContentItem = CreateDefaultItemWithRatio(OcctypeAssetType.content, true);
            VehicleItem = CreateDefaultAsset(OcctypeAssetType.vehicle, true);
            OtherItem = CreateDefaultItemWithRatio(OcctypeAssetType.other, false);
            FoundationHeightUncertainty = new Deterministic(0);

            OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();
            ID = manager.GetNextAvailableId();
        }

        public OccupancyType(string name, string description, int groupID, string damageCategory, OccTypeAsset structureItem,
            OccTypeItemWithRatio contentItem, OccTypeAsset vehicleItem, OccTypeItemWithRatio otherItem, ContinuousDistribution foundationHtUncertainty, int id)
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
            StructureItem = new OccTypeAsset( ot.StructureItem);
            ContentItem = new OccTypeItemWithRatio( ot.ContentItem);
            VehicleItem = new OccTypeAsset( ot.VehicleItem);
            OtherItem = new OccTypeItemWithRatio( ot.OtherItem);
            GroupID = ot.GroupID;
            ID = ot.ID;        
        }

        private OccTypeAsset CreateDefaultAsset(OcctypeAssetType assetType, bool isSelected)
        {
            ComputeComponentVM structureCurve = new ComputeComponentVM(StringConstants.OCCTYPE_PLOT_TITLE, StringConstants.OCCTYPE_DEPTH, StringConstants.OCCTYPE_PERCENT_DAMAGE);
            ContinuousDistribution structValueUncert = new Deterministic(0);
            return new OccTypeAsset(assetType,  isSelected, structureCurve, structValueUncert);
        }

        private OccTypeItemWithRatio CreateDefaultItemWithRatio(OcctypeAssetType itemType, bool isSelected)
        {
            ComputeComponentVM structureCurve = new ComputeComponentVM(StringConstants.OCCTYPE_PLOT_TITLE, StringConstants.OCCTYPE_DEPTH, StringConstants.OCCTYPE_PERCENT_DAMAGE);
            ContinuousDistribution structValueUncert = new Deterministic(0);
            ContinuousDistribution structValueUncertRatio = new Deterministic(0);
            bool isByVal = true;
            return new OccTypeItemWithRatio(itemType, isSelected, structureCurve, structValueUncert, structValueUncertRatio, isByVal);
        }
    }
}
