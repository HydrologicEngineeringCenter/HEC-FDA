using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.TableWithPlot;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    [Trait("Category", "Remote")]
    public class FrequencyElementShould
    {
        [Fact]
        public void WriteAndReadXML()
        {
            int id = 9;
            List<double> flows = new List<double>() { 1.0, 2, 3, 4, 5, 6 };
            int por = 45;
            bool isAnalytical = false;
            bool isStandard = false;
            double mean = 1;
            double stDev = 2;
            double skew = 3;
            GraphicalVM graphicalVM = new GraphicalVM("graph", "xLabel", "yLabel");
            CurveComponentVM compVM = new CurveComponentVM();

            AnalyticalFrequencyElement elem1 = new AnalyticalFrequencyElement("test", "lastEdit", "desc", por, isAnalytical, isStandard, mean, stDev, skew, flows, graphicalVM, compVM, id);
            XElement elemXML = elem1.ToXML();

            AnalyticalFrequencyElement elem2 = new AnalyticalFrequencyElement(elemXML, id);

            Assert.True(elem1.Equals(elem2));
        }
    }
}
