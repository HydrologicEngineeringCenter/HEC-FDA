using HEC.FDA.Model.utilities;
using System;
using System.Xml.Linq;
using Utilities;

namespace HEC.FDA.Model.paireddata
{
    [StoredProperty("CurveMetaData")]
    public class CurveMetaData
    {
        [StoredProperty("XLabel")]
        public string XLabel { get; }
        // I'm opening this property up for modification because we're inconsistent with setting it initially, and as
        // as stop-gap it's useful to be able to set it properly before it's needed. 
        [StoredProperty("YLabel")]
        public string YLabel { get; set; }
        [StoredProperty("Name")]
        public string Name { get; }
        [StoredProperty("DamCat")]
        public string DamageCategory { get; }
        [StoredProperty("AssetCat")]
        public string AssetCategory { get; }
        [StoredProperty("IsNull")]
        public bool IsNull { get; set; }
        [StoredProperty("ImpactAreaID")]
        public int ImpactAreaID { get; } = 0;
        public CurveMetaData()
        {
            XLabel = "xlabel";
            YLabel = "ylabel";
            Name = "unnamed";
            DamageCategory = "unassiged";
            AssetCategory = "unassigned";
            IsNull = true;
        }
        public CurveMetaData(string damageCategory, string assetCategory = "unassigned")
        {
            XLabel = "xlabel";
            YLabel = "ylabel";
            Name = "unnamed";
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            IsNull = false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name, string damageCategory, string assetCategory = "unassigned")
        {
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            IsNull = false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name)
        {
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            DamageCategory = "unassigned";
            AssetCategory = "unassigned";
            IsNull = false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name, string damageCategory, int impactAreaID, string assetCategory = "unassigned")
        {
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            IsNull = false;
            ImpactAreaID = impactAreaID;
        }
        public XElement WriteToXML()
        {
            StoredPropertyAttribute attribute = (StoredPropertyAttribute)Attribute.GetCustomAttribute(typeof(CurveMetaData), typeof(StoredPropertyAttribute));
            string thresholdMasterTag = attribute.SerializedName;

            XElement masterElement = new(thresholdMasterTag);

            string xLabelTag = Serialization.GetXMLTagFromProperty(GetType(), nameof(XLabel));
            masterElement.SetAttributeValue(xLabelTag, XLabel);

            string yLabelTag = Serialization.GetXMLTagFromProperty(GetType(),nameof(YLabel));
            masterElement.SetAttributeValue(yLabelTag, YLabel);

            string nameTag = Serialization.GetXMLTagFromProperty(GetType(),nameof(Name));
            masterElement.SetAttributeValue(nameTag, Name);

            string damCatTag = Serialization.GetXMLTagFromProperty(GetType(),nameof(DamageCategory));
            masterElement.SetAttributeValue(damCatTag, DamageCategory);

            string assetCatTag = Serialization.GetXMLTagFromProperty(GetType(),nameof(AssetCategory));
            masterElement.SetAttributeValue(assetCatTag, AssetCategory);

            string isNullTag = Serialization.GetXMLTagFromProperty(GetType(),nameof(IsNull));
            masterElement.SetAttributeValue(isNullTag, IsNull);
            return masterElement;
        }

        public static CurveMetaData ReadFromXML(XElement xElement)
        {
            Type metaDataType = typeof(CurveMetaData);

            string xLabelTag = Serialization.GetXMLTagFromProperty(metaDataType, nameof(XLabel));
            string xLabel = xElement.Attribute(xLabelTag)?.Value;

            string yLabelTag = Serialization.GetXMLTagFromProperty(metaDataType, nameof(YLabel));
            string yLabel = xElement.Attribute(yLabelTag)?.Value;

            string nameTag = Serialization.GetXMLTagFromProperty(metaDataType, nameof(Name));
            string name = xElement.Attribute(nameTag)?.Value;

            string damCatTag = Serialization.GetXMLTagFromProperty(metaDataType, nameof(DamageCategory));
            string damageCategory = xElement.Attribute(damCatTag)?.Value;

            string assetCatTag = Serialization.GetXMLTagFromProperty(metaDataType, nameof(AssetCategory));
            string assetCategory = xElement.Attribute(assetCatTag)?.Value;

            string isNullTag = Serialization.GetXMLTagFromProperty(metaDataType, nameof(IsNull));
            if (!bool.TryParse(xElement.Attribute(isNullTag)?.Value, out bool isNull))
                isNull = true;

            CurveMetaData curveMetaData = new(xLabel, yLabel, name, damageCategory, assetCategory)
            {
                IsNull = isNull
            };

            return curveMetaData;
        }
    }
}
