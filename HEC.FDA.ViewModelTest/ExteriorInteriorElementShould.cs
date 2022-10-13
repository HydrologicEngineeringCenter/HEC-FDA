using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    [Trait("Category", "Unit")]
    public class ExteriorInteriorElementShould
    {
        [Fact]
        public void WriteAndReadXML()
        {
            int id = 9;

            CurveComponentVM compVM = new CurveComponentVM("someName", "xLabel", "yLabel");
            compVM.SetPairedData(UncertainPairedDataFactory.CreateDefaultNormalData("xlabel", "ylabel", "name"));

            ExteriorInteriorElement elem1 = new ExteriorInteriorElement("myName", "lastEditDate", "desc", compVM, id);
            XElement elemXML = elem1.ToXML();

            ExteriorInteriorElement elem2 = new ExteriorInteriorElement(elemXML, id);

            Assert.True(elem1.Equals(elem2));
        }

    }
}
