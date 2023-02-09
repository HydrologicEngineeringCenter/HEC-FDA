using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.TableWithPlot.Data;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    [Trait("RunsOn", "Remote")]
    public class GraphicalDataProviderShould
    {
        [Fact]
        public void ReadAndWriteXML()
        {
            GraphicalDataProvider gdp = new GraphicalDataProvider();
            UncertainPairedData upd = gdp.ToUncertainPairedData("x","y","name","dest","cat","asset");
            XElement ele = upd.WriteToXML();
            UncertainPairedData readUPD = UncertainPairedData.ReadFromXML(ele);
            Assert.NotNull(readUPD);
        }
    }
}
