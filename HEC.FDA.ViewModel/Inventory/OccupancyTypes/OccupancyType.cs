using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using Statistics;
using Statistics.Distributions;
using System;
using System.Linq;
using System.Xml.Linq;
using static HEC.FDA.ViewModel.Inventory.OccupancyTypes.OccTypeAsset;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class OccupancyType : BaseViewModel
    {

        public string Description { get; set; }
        public string DamageCategory { get; set; }
        public OccTypeAsset StructureItem { get; set; }
        public OccTypeAssetWithRatio ContentItem { get; set; }
        public OccTypeAsset VehicleItem { get; set; }
        public OccTypeAssetWithRatio OtherItem { get; set; }
        public ContinuousDistribution FoundationHeightUncertainty { get; set; }

        public int GroupID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }

        
        public OccupancyType(string name, string damCatName, int groupId, int id)
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
            ID = id;
        } 

        public OccupancyType(string name, string description, int groupID, string damageCategory, OccTypeAsset structureItem,
            OccTypeAssetWithRatio contentItem, OccTypeAsset vehicleItem, OccTypeAssetWithRatio otherItem, ContinuousDistribution foundationHtUncertainty, int id)
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

        public OccupancyType(XElement occtypeElem)
        {
            Name = occtypeElem.Attribute("Name").Value;
            Description = occtypeElem.Attribute("Description").Value;
            DamageCategory = occtypeElem.Attribute("DamCat").Value;
            GroupID = Convert.ToInt32(occtypeElem.Attribute("GroupID").Value);
            ID = Convert.ToInt32(occtypeElem.Attribute("ID").Value);

            XElement foundUncert = occtypeElem.Element("FoundationUncertainty");
            FoundationHeightUncertainty = (ContinuousDistribution)ContinuousDistribution.FromXML(foundUncert.Descendants().First());

            StructureItem = new OccTypeAsset(occtypeElem.Element("StructureAsset").Element("Asset"));
            ContentItem = new OccTypeAssetWithRatio(occtypeElem.Element("ContentAsset").Element("Asset"));
            VehicleItem = new OccTypeAsset(occtypeElem.Element("VehicleAsset").Element("Asset"));
            OtherItem = new OccTypeAssetWithRatio(occtypeElem.Element("OtherAsset").Element("Asset"));
        }

        private OccTypeAsset CreateDefaultAsset(OcctypeAssetType assetType, bool isSelected)
        {
            CurveComponentVM depthPercentDamageCurve = new CurveComponentVM(StringConstants.OCCTYPE_PLOT_TITLE, StringConstants.OCCTYPE_DEPTH, StringConstants.OCCTYPE_PERCENT_DAMAGE, isDepthPercentDamage: true);
            depthPercentDamageCurve.SetPairedData(DefaultData.DepthPercentDamageDefaultCurve());
            ContinuousDistribution structValueUncert = new Deterministic(0);
            return new OccTypeAsset(assetType,  isSelected, depthPercentDamageCurve, structValueUncert);
        }

        private OccTypeAssetWithRatio CreateDefaultItemWithRatio(OcctypeAssetType itemType, bool isSelected)
        {
            CurveComponentVM depthPercentDamageCurve = new CurveComponentVM(StringConstants.OCCTYPE_PLOT_TITLE, StringConstants.OCCTYPE_DEPTH, StringConstants.OCCTYPE_PERCENT_DAMAGE, isDepthPercentDamage: true);
            depthPercentDamageCurve.SetPairedData(DefaultData.DepthPercentDamageDefaultCurve());
            ContinuousDistribution structValueUncert = new Deterministic(0);
            ContinuousDistribution structValueUncertRatio = new Deterministic(0);
            bool isByVal = true;
            return new OccTypeAssetWithRatio(itemType, isSelected, depthPercentDamageCurve, structValueUncert, structValueUncertRatio, isByVal);
        }

        public XElement ToXML()
        {
            XElement occtypeElem = new XElement("OccType");
            occtypeElem.SetAttributeValue("Name", Name);
            occtypeElem.SetAttributeValue("Description", Description);
            occtypeElem.SetAttributeValue("DamCat", DamageCategory);
            occtypeElem.SetAttributeValue("GroupID", GroupID);
            occtypeElem.SetAttributeValue("ID", ID);

            XElement foundUncert = new XElement("FoundationUncertainty");
            foundUncert.Add(FoundationHeightUncertainty.ToXML());
            occtypeElem.Add(foundUncert);

            XElement structElem = new XElement("StructureAsset");
            structElem.Add(StructureItem.ToXML());
            occtypeElem.Add(structElem);

            XElement contElem = new XElement("ContentAsset");
            contElem.Add(ContentItem.ToXML());
            occtypeElem.Add(contElem);

            XElement vehicleElem = new XElement("VehicleAsset");
            vehicleElem.Add(VehicleItem.ToXML());
            occtypeElem.Add(vehicleElem);

            XElement otherElem = new XElement("OtherAsset");
            otherElem.Add(OtherItem.ToXML());
            occtypeElem.Add(otherElem);

            return occtypeElem;
        }

    }
}
