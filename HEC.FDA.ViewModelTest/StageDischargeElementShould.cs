using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    public class StageDischargeElementShould
    {

        [Fact]
        public void WriteAndReadXML()
        {
            int id = 9;

            ComputeComponentVM compVM = new ComputeComponentVM("someName", "xLabel", "yLabel");
            compVM.SetPairedData(UncertainPairedDataFactory.CreateDefaultNormalData("xlabel", "ylabel", "name"));

            StageDischargeElement elem1 = new StageDischargeElement("myName", "lastEditDate", "desc", compVM, id);
            XElement elemXML = elem1.ToXML();

            StageDischargeElement elem2 = new StageDischargeElement(elemXML, id);

            Assert.True(elem1.Equals(elem2));
        }
    }
}
