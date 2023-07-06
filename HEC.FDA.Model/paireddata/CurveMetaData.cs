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
        [StoredProperty("YLabel")]
        public string YLabel { get; }
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
            XElement masterElement = new XElement("thresholdMasterTag");

            string xLabelTag = Serialization.GetXMLTagFromProperty(XLabel.GetType(),nameof(XLabel));
            masterElement.SetAttributeValue(xLabelTag, XLabel);

            string yLabelTag = Serialization.GetXMLTagFromProperty(YLabel.GetType(),nameof(YLabel));
            masterElement.SetAttributeValue(yLabelTag, YLabel);

            string nameTag = Serialization.GetXMLTagFromProperty(Name.GetType(),nameof(Name));
            masterElement.SetAttributeValue(nameTag, Name);

            string damCatTag = Serialization.GetXMLTagFromProperty(DamageCategory.GetType(),nameof(DamageCategory));
            masterElement.SetAttributeValue(damCatTag, DamageCategory);

            string assetCatTag = Serialization.GetXMLTagFromProperty(AssetCategory.GetType(),nameof(AssetCategory));
            masterElement.SetAttributeValue(assetCatTag, AssetCategory);

            string isNullTag = Serialization.GetXMLTagFromProperty(IsNull.GetType(),nameof(IsNull));
            masterElement.SetAttributeValue(isNullTag, IsNull);
            return masterElement;
        }

        public static CurveMetaData ReadFromXML(XElement xElement)
        {
            CurveMetaData metaDataForReflection = new();

            string xLabelTag = Serialization.GetXMLTagFromProperty(metaDataForReflection.XLabel.GetType(), nameof(XLabel));
            string xLabel = xElement.Attribute(xLabelTag)?.Value;

            string yLabelTag = Serialization.GetXMLTagFromProperty(metaDataForReflection.YLabel.GetType(), nameof(YLabel));
            string yLabel = xElement.Attribute(yLabelTag)?.Value;

            string nameTag = Serialization.GetXMLTagFromProperty(metaDataForReflection.Name.GetType(), nameof(Name));
            string name = xElement.Attribute(nameTag)?.Value;

            string damCatTag = Serialization.GetXMLTagFromProperty(metaDataForReflection.DamageCategory.GetType(), nameof(DamageCategory));
            string damageCategory = xElement.Attribute(damCatTag)?.Value;

            string assetCatTag = Serialization.GetXMLTagFromProperty(metaDataForReflection.AssetCategory.GetType(), nameof(AssetCategory));
            string assetCategory = xElement.Attribute(assetCatTag)?.Value;

            string isNullTag = Serialization.GetXMLTagFromProperty(metaDataForReflection.IsNull.GetType(), nameof(IsNull));
            if (!bool.TryParse(xElement.Attribute(isNullTag)?.Value, out bool isNull))
                return new CurveMetaData();
            
            CurveMetaData curveMetaData = new CurveMetaData(xLabel, yLabel, name, damageCategory, assetCategory);
            curveMetaData.IsNull = isNull;

            return curveMetaData;
        }
    }
}
