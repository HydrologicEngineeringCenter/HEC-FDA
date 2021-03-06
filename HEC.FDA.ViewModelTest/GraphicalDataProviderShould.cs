using HEC.FDA.ViewModel.TableWithPlot.Data;
using paireddata;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    public class GraphicalDataProviderShould
    {
        [Fact]
        public void ReadAndWriteXML()
        {
            GraphicalDataProvider gdp = new GraphicalDataProvider();
            UncertainPairedData upd = gdp.ToUncertainPairedData("x","y","name","dest","cat");
            XElement ele = upd.WriteToXML();
            UncertainPairedData readUPD = UncertainPairedData.ReadFromXML(ele);
            Assert.NotNull(readUPD);
        }
    }
}
