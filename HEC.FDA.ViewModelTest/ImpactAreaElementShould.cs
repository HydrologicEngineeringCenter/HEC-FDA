using HEC.FDA.ViewModel.ImpactArea;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    [Trait("Category", "Unit")]
    public class ImpactAreaElementShould
    {
        [Fact]
        public void WriteAndReadXML()
        {
            int id = 9;
            List<ImpactAreaRowItem> rows = new List<ImpactAreaRowItem>();
            for (int i = 0; i < 10; i++)
            {
                rows.Add(new ImpactAreaRowItem(i, "row" + i));
            }
            ImpactAreaElement impAreaElem = new ImpactAreaElement("testName", "desc", rows, id, "header");
            XElement impXML = impAreaElem.ToXML();
            ImpactAreaElement impAreaElem2 = new ImpactAreaElement(impXML, id);
            Assert.True(impAreaElem.Equals(impAreaElem2));
        }
    }
}
