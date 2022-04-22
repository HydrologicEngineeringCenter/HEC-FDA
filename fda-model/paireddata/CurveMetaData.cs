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
        public string Category { get; }
        public bool IsNull { get; }
        public CurveTypesEnum CurveType {get;}
        public CurveMetaData()
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = "xlabel";
            YLabel = "ylabel";
            Name = "made up";
            Category = "residential";
            IsNull = true;
        }
        public CurveMetaData(string category)
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = "xlabel";
            YLabel = "ylabel";
            Name = "made up";
            Category = category;
            IsNull = false;
        }
        public CurveMetaData(string category, CurveTypesEnum curvetype)
        {
            CurveType = curvetype;
            XLabel = "xlabel";
            YLabel = "ylabel";
            Name = "made up";
            Category = category;
            IsNull=false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name, string category)
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            Category = category;
            IsNull = false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name)
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            Category = "AGAIG";
            IsNull = false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name, string category, CurveTypesEnum curveType)
        {
            CurveType = curveType;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            Category = category;
            IsNull = false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name, CurveTypesEnum curveType)
        {
            CurveType = curveType;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            Category = "AGAIG";
            IsNull = false;
        }
    }
}
