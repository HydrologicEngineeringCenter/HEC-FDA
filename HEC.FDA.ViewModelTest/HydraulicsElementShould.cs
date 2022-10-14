using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.enums;
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
            List<HydraulicProfile> rows = new List<HydraulicProfile>();
            for (int i = 0; i < 10; i++)
            {
                rows.Add(new HydraulicProfile(i, "path" + i));
            }
            HydraulicElement elem1 = new HydraulicElement("test", "desc", rows, HydraulicDataSource.WSEGrid, id);
            XElement elemXML = elem1.ToXML();

            HydraulicElement elem2 = new HydraulicElement(elemXML, id);
            bool theThingsMatch = elem1.Equals(elem2);
            Assert.True(theThingsMatch);
        }
    }
}
