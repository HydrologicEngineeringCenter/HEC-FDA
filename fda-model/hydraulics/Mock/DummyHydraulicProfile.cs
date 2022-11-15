using HEC.FDA.Model.hydraulics.enums;
using RasMapperLib;
using System.Xml.Linq;

namespace HEC.FDA.Model.hydraulics.Mock
{
    /// <summary>
    /// This class exists explicitly for testing purposes. Should never go throught he UI. 
    /// </summary>
    public class DummyHydraulicProfile : Interfaces.IHydraulicProfile
    {
        public float[] DummyDepths { get; set; }
        public double Probability { get; set; }
        public string FileName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string ProfileName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public int CompareTo(object obj)
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(HydraulicProfile hydraulicProfileForComparison)
        {
            throw new System.NotImplementedException();
        }

        public string GetFilePath(string parentDirectory)
        {
            throw new System.NotImplementedException();
        }

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
