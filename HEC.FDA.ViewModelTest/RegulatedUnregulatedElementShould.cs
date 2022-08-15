using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    public class RegulatedUnregulatedElementShould
    {
        [Fact]
        public void WriteAndReadXML()
        {
            int id = 9;

            ComputeComponentVM compVM = new ComputeComponentVM("someName", "xLabel", "yLabel");
            compVM.SetPairedData(UncertainPairedDataFactory.CreateDefaultNormalData("xlabel", "ylabel", "name"));

            InflowOutflowElement elem1 = new InflowOutflowElement("myName", "lastEditDate", "desc", compVM, id);
            XElement elemXML = elem1.ToXML();

            InflowOutflowElement elem2 = new InflowOutflowElement(elemXML, id);

            Assert.True(elem1.Equals(elem2));
        }
    }
}
