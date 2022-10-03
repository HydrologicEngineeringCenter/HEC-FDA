using HEC.FDA.ViewModel.TableWithPlot;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    [Trait("Category", "Unit")]
    public class ComputeComponentVMShould
    {
        [Fact]
        public void ReadAndWriteXML()
        {
            ComputeComponentVM expected = new ComputeComponentVM("Brennan");
            expected.SelectedItem = expected.Options[2];
            var el = expected.ToXML();
            ComputeComponentVM Actual = new ComputeComponentVM(el);
            Assert.Equal(expected.SelectedItem.GetType(), Actual.SelectedItem.GetType());
        }
    }
}
