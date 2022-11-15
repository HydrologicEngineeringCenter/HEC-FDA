using HEC.FDA.Model.hydraulics.enums;
using RasMapperLib;
using System.Xml.Linq;

namespace HEC.FDA.Model.hydraulics.Interfaces
{
    public interface IHydraulicProfile
    {
        public double Probability { get; set; }
        public float[] GetWSE(PointMs pts, HydraulicDataSource dataSource, string parentDirectory);
        public XElement ToXML();

    }
}
