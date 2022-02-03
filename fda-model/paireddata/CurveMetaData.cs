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
        public string Description { get; }
        public string Category { get; }
        public bool IsNull { get; }
        public CurveTypesEnum CurveType {get;}
        public CurveMetaData()
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = "xlabel";
            YLabel = "ylabel";
            Name = "made up";
            Description = "TODO: implement me";
            Category = "residential";
            IsNull = true;
        }
        public CurveMetaData(string category)
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = "xlabel";
            YLabel = "ylabel";
            Name = "made up";
            Description = "TODO: implement me";
            Category = category;
            IsNull = false;
        }
        public CurveMetaData(string category, CurveTypesEnum curvetype)
        {
            CurveType = curvetype;
            XLabel = "xlabel";
            YLabel = "ylabel";
            Name = "made up";
            Description = "TODO: implement me";
            Category = category;
            IsNull=false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name, string description, string category)
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            Description = description;
            Category = category;
            IsNull = false;
        }
        public CurveMetaData(string xlabel, string ylabel, string name, string description)
        {
            CurveType = CurveTypesEnum.StrictlyMonotonicallyIncreasing;
            XLabel = xlabel;
            YLabel = ylabel;
            Name = name;
            Description = description;
            Category = "AGAIG";
            IsNull = false;
        }
    }
}
