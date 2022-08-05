using HEC.FDA.ViewModel.Watershed;
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

    }
}
