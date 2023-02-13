using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.TableWithPlot;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    [Trait("RunsOn", "Remote")]
    public class TableWithPlotVMShould
    {
        [Fact]
        public void SaveAndLoadGraphicalToXML()
        {
            TableWithPlotVM expected = new TableWithPlotVM(new GraphicalVM("X", "xlabel", "ylabel"));
            ((GraphicalVM)(expected.CurveComponentVM)).EquivalentRecordLength = 1;
            XElement ele = expected.ToXML();
            TableWithPlotVM actual =  new TableWithPlotVM(ele);
            GraphicalVM exp = ((GraphicalVM)(expected.CurveComponentVM));
            GraphicalVM act = ((GraphicalVM) (actual.CurveComponentVM));
            Assert.Equal(exp.EquivalentRecordLength, act.EquivalentRecordLength);
        }
    }
}
