using System;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Hydraulics.GriddedData
{
    //[Author(q0heccdm, 9 / 11 / 2017 8:48:18 AM)]
    public class PathAndProbability
    {
        public const string PATH_AND_PROB = "PathAndProbability";
        private const string PATH = "Path";
        private const string PROB = "Probability";

        #region Properties
        public string Path { get; set; }
        public double Probability { get; set;}
        #endregion
        #region Constructors
        public PathAndProbability(string path, double probability)
        {
            Path = path;
            Probability = probability;
        }

        public PathAndProbability(XElement elem)
        {
            Path = elem.Attribute(PATH).Value;
            Probability = Convert.ToDouble(elem.Attribute(PROB).Value);
        }
        #endregion

        public XElement ToXML()
        {
            XElement elem = new XElement(PATH_AND_PROB);
            elem.SetAttributeValue(PATH, Path);
            elem.SetAttributeValue(PROB, Probability);
            return elem;
        }

        public bool Equals(PathAndProbability pathAndProb)
        {
            bool isEqual = true;
            if(!Path.Equals(pathAndProb.Path))
            {
                isEqual = false;
            }
            if(Probability != pathAndProb.Probability)
            {
                isEqual = false;
            }
            return isEqual;
        }

    }
}
