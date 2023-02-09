using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    [Trait("Category", "Remote")]
    public class LeveeElementShould
    {

        [Fact]
        public void WriteAndReadXML()
        {
            int id = 9;

            CurveComponentVM compVM = new CurveComponentVM("someName", "xLabel", "yLabel");
            compVM.SetPairedData(UncertainPairedDataFactory.CreateDefaultNormalData("xlabel", "ylabel", "name"));

            double elevation = 99;

            LateralStructureElement elem1 = new LateralStructureElement("myName", "lastEditDate", "desc", elevation,false, compVM, id);
            XElement elemXML = elem1.ToXML();

            LateralStructureElement elem2 = new LateralStructureElement(elemXML, id);

            Assert.True(elem1.Equals(elem2));
        }

    }
}
