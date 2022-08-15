using HEC.FDA.ViewModel.IndexPoints;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    public class IndexPointsElementShould
    {
        [Fact]
        public void WriteAndReadXML()
        {
            int id = 9;
            List<string> rows = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                rows.Add("test" + i);
            }
            IndexPointsElement elem1 = new IndexPointsElement("testName", "desc", rows, id);
            XElement impXML = elem1.ToXML();

            IndexPointsElement elem2 = new IndexPointsElement(impXML, id);

            Assert.True(elem1.Equals(elem2));
        }
    }
}
