using HEC.FDA.Model.hydraulics.enums;
using RasMapperLib;
using System.Xml.Linq;

namespace HEC.FDA.Model.hydraulics.Mock
{
    public class DummyHydraulicProfile : Interfaces.IHydraulicProfile
    {
        public float[] DummyDepths { get; set; }
        public double Probability { get; set; }

        public float[] GetWSE(PointMs pts, HydraulicDataSource dataSource, string parentDirectory)
        {
            return DummyDepths;
        }

        public XElement ToXML()
        {
            throw new System.NotImplementedException();
        }
    }
}
