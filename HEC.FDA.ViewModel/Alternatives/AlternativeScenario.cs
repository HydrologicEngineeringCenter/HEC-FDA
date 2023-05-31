using HEC.FDA.ViewModel.ImpactAreaScenario;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Alternatives
{
    public class AlternativeScenario : BaseViewModel
    {
        public static string ALTERNATIVE_SCENARIO = "AlternativeScenario";
        private const string ID_STRING = "ID";
        private const string YEAR = "Year";

        public int ScenarioID { get; }
        public int Year { get; }
        public AlternativeScenario(int scenarioID, int year)
        {
            ScenarioID = scenarioID;
            Year = year;
        }

        public AlternativeScenario(XElement elem)
        {
            ScenarioID = int.Parse(elem.Attribute(ID_STRING).Value);
            Year = int.Parse(elem.Attribute(YEAR).Value);
        }

        public XElement ToXML()
        {
            XElement elem = new XElement(ALTERNATIVE_SCENARIO);
            elem.SetAttributeValue(ID_STRING, ScenarioID);
            elem.SetAttributeValue(YEAR, Year);
            return elem;
        }

        public IASElement GetElement()
        {
            IASElement elem = null;
            List<IASElement> currentElementSets = StudyCache.GetChildElementsOfType<IASElement>();
            foreach (IASElement set in currentElementSets)
            {
                int setID = set.ID;
                if (setID == ScenarioID)
                {
                    elem = set;
                }
            }
            return elem;
        }
    }
}
