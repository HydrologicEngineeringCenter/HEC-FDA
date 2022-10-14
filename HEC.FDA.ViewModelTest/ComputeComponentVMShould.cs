using HEC.FDA.ViewModel.TableWithPlot;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    [Trait("Category", "Unit")]
    public class CurveComponentVMShould
    {
        [Fact]
        public void ReadAndWriteXML()
        {
            CurveComponentVM expected = new CurveComponentVM("Brennan");
            expected.SelectedItem = expected.Options[2];
            var el = expected.ToXML();
            CurveComponentVM Actual = new CurveComponentVM(el);
            Assert.Equal(expected.SelectedItem.GetType(), Actual.SelectedItem.GetType());
        }
    }
}
