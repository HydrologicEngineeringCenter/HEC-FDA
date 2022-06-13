using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.TableWithPlot;
using System.Xml.Linq;
using Xunit;

namespace fda_viewmodel_test
{
    
    public class TableWithPlotVMShould
    {
        [Fact]
        public void SaveAndLoadGraphicalToXML()
        {
            TableWithPlotVM expected = new TableWithPlotVM(new GraphicalVM("X", "xlabel", "ylabel"));
            ((GraphicalVM)(expected.ComputeComponentVM)).EquivalentRecordLength = 1;
            XElement ele = expected.ToXML();
            TableWithPlotVM actual =  new TableWithPlotVM(ele);
            GraphicalVM exp = ((GraphicalVM)(expected.ComputeComponentVM));
            GraphicalVM act = ((GraphicalVM) (actual.ComputeComponentVM));
            Assert.Equal(exp.EquivalentRecordLength, act.EquivalentRecordLength);
        }
    }
}
