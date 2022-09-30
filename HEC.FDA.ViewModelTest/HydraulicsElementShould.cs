using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    [Trait("Category", "Unit")]
    public class HydraulicsElementShould
    {
        [Fact]
        public void WriteAndReadXML()
        {
            int id = 9;
            List<PathAndProbability> rows = new List<PathAndProbability>();
            for (int i = 0; i < 10; i++)
            {
                rows.Add(new PathAndProbability("path" + i, i));
            }
            HydraulicElement elem1 = new HydraulicElement("test", "desc", rows, true, HydraulicType.Gridded, id);
            XElement elemXML = elem1.ToXML();

            HydraulicElement elem2 = new HydraulicElement(elemXML, id);

            Assert.True(elem1.Equals(elem2));
        }
    }
}
