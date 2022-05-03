using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace paireddata
{
    public class CurveMetaData
    {
        public string XLabel { get; }
        public string YLabel { get; }
        public string Name { get; }
        public string DamageCategory { get; }
        public string AssetCategory { get; }
        public bool IsNull { get; }
        public CurveTypesEnum CurveType {get;}
        public CurveMetaData()
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = "xlabel";
            YLabel = "ylabel";
            Name = "unnamed";
            DamageCategory = "unassiged";
            IsNull = true;
        }
        public CurveMetaData(string damageCategory)
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = "xlabel";
            YLabel = "ylabel";
            Name = "unnamed";
            DamageCategory = damageCategory;
            IsNull = false;
        }
        public CurveMetaData(string damageCategory, CurveTypesEnum curvetype)
        {
            CurveType = curvetype;
            XLabel = "xlabel";
            YLabel = "ylabel";
            Name = "unnamed";
            DamageCategory = damageCategory;
            IsNull=false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name, string damageCategory)
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            DamageCategory = damageCategory;
            IsNull = false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name)
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            DamageCategory = "unassigned";
            IsNull = false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name, string damageCategory, CurveTypesEnum curveType)
        {
            CurveType = curveType;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            DamageCategory = damageCategory;
            IsNull = false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name, CurveTypesEnum curveType)
        {
            CurveType = curveType;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            DamageCategory = "unassigned";
            IsNull = false;
        }
    }
}
