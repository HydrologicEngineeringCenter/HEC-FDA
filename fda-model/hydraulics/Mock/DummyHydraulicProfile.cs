using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.Model.hydraulics.Interfaces;
using RasMapperLib;
using System;
using System.Xml.Linq;

namespace HEC.FDA.Model.hydraulics.Mock
{
    /// <summary>
    /// This class exists explicitly for testing purposes. Should never go throught he UI. 
    /// </summary>
    public class DummyHydraulicProfile : IHydraulicProfile
    {
        public float[] DummyWSEs { get; set; }
        public double Probability { get; set; } = 0.0;
        public string FileName { get; set; } = string.Empty;
        public string ProfileName { get; set; } = string.Empty;
        public DummyHydraulicProfile(float[] dummyWSEs, double prob)
        {
            DummyWSEs = dummyWSEs;
            Probability = prob;
        }
        public DummyHydraulicProfile()
        {

        }
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            IHydraulicProfile otherProfile = obj as IHydraulicProfile;
            if (otherProfile != null)
                return Probability.CompareTo(otherProfile.Probability);
            else
                throw new ArgumentException("Object is not a HydraulicProfile");
        }

        public bool Equals(IHydraulicProfile hydraulicProfileForComparison)
        {
            throw new System.NotImplementedException();
        }

        public string GetFilePath(string parentDirectory)
        {
            return "This Class is for Testing Only.";
        }

        public float[] GetWSE(PointMs pts, HydraulicDataSource dataSource, string parentDirectory)
        {
            return DummyWSEs;
        }

        public XElement ToXML()
        {
            throw new System.NotImplementedException();
        }
    }
}
