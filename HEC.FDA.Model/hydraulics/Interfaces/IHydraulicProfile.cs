using HEC.FDA.Model.hydraulics.enums;
using RasMapperLib;
using System;
using System.Xml.Linq;

namespace HEC.FDA.Model.hydraulics.Interfaces
{
    public interface IHydraulicProfile: IComparable
    {
        public double Probability { get; set; }
        public string FileName { get; set; }
        public string ProfileName { get; set; }
        public float[] GetWSE(PointMs pts, HydraulicDataSource dataSource, string parentDirectory);
        public XElement ToXML();
        public string GetFilePath(string parentDirectory);
        public bool Equals(IHydraulicProfile hydraulicProfileForComparison);
    }
}
