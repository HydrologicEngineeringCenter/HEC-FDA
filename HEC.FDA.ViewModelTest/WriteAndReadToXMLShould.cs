using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Watershed;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{

    public class WriteAndReadToXMLShould
    {

        [Fact]
        public void TestTerrainElementWriteThenReadAreEqual()
        {
            TerrainElement terrain = new TerrainElement("TerName", "TerFileName", 1);
            XElement terrainXML = terrain.ToXML();

            TerrainElement terrain2 = new TerrainElement(terrainXML, 1);

            Assert.True(terrain.Equals(terrain2));
        }

        [Fact]
        public void TestImpactAreaElementWriteThenReadAreEqual()
        {
            int id = 9;
            List<ImpactAreaRowItem> rows = new List<ImpactAreaRowItem>();
            for(int i = 0; i < 10; i++)
            {
                rows.Add(new ImpactAreaRowItem(i, "row"+ i));
            }
            ImpactAreaElement impAreaElem = new ImpactAreaElement("testName", "desc", rows, id);
            XElement impXML = impAreaElem.ToXML();

            ImpactAreaElement impAreaElem2 = new ImpactAreaElement(impXML, id);

            Assert.True(impAreaElem.Equals(impAreaElem2));
        }

        [Fact]
        public void TestIndexPointsElementWriteThenReadAreEqual()
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
