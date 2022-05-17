using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace paireddata
{
    public class CurveMetaData
    {
        public string XLabel { get; }
        public string YLabel { get; }
        public string Name { get; }
        public string DamageCategory { get; }
        public string AssetCategory { get; }
        public bool IsNull { get; set; }
        public CurveTypesEnum CurveType {get;}
        public CurveMetaData()
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = "xlabel";
            YLabel = "ylabel";
            Name = "unnamed";
            DamageCategory = "unassiged";
            AssetCategory = "unassigned";
            IsNull = true;
        }
        public CurveMetaData(string damageCategory, string assetCategory = "unassigned")
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = "xlabel";
            YLabel = "ylabel";
            Name = "unnamed";
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            IsNull = false;
        }
        public CurveMetaData(string damageCategory, CurveTypesEnum curvetype, string assetCategory = "unassigned")
        {
            CurveType = curvetype;
            XLabel = "xlabel";
            YLabel = "ylabel";
            Name = "unnamed";
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;

            IsNull = false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name, string damageCategory, string assetCategory = "unassigned")
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            IsNull = false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name)
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            DamageCategory = "unassigned";
            AssetCategory = "unassigned";
            IsNull = false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name, string damageCategory, CurveTypesEnum curveType, string assetCategory = "unassigned")
        {
            CurveType = curveType;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            IsNull = false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name, CurveTypesEnum curveType)
        {
            CurveType = curveType;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            DamageCategory = "unassigned";
            AssetCategory = "unassigned";
            IsNull = false;
        }

        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("Curve_Metadata");
            masterElement.SetAttributeValue("CurveType", Convert.ToString(CurveType));
            masterElement.SetAttributeValue("XLabel", XLabel);
            masterElement.SetAttributeValue("YLabel", YLabel);
            masterElement.SetAttributeValue("Name", Name);
            masterElement.SetAttributeValue("DamageCategory", DamageCategory);
            masterElement.SetAttributeValue("AssetCategory", AssetCategory);
            masterElement.SetAttributeValue("IsNull", IsNull);
            return masterElement;
        }

        public static CurveMetaData ReadFromXML(XElement xElement)
        {
            CurveTypesEnum curveType = (CurveTypesEnum)Enum.Parse(typeof(CurveTypesEnum), xElement.Attribute("CurveType").Value);
            string xLabel = xElement.Attribute("XLabel").Value;
            string yLabel = xElement.Attribute("YLabel").Value;
            string name = xElement.Attribute("Name").Value;
            string damageCategory = xElement.Attribute("DamageCategory").Value;
            string assetCategory = xElement.Attribute("AssetCategory").Value;
            CurveMetaData curveMetaData = new CurveMetaData(xLabel, yLabel, name, damageCategory, curveType, assetCategory);
            bool isNull = Convert.ToBoolean(xElement.Attribute("IsNull").Value);
            curveMetaData.IsNull = isNull;
            return curveMetaData;
        }
    }
}
