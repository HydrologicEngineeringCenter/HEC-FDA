using HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;
using Xunit;

namespace HEC.FDA.ViewModelTest.FrequencyRelationships.FrequencyEditor
{
    [Trait("RunsOn", "Remote")]
    public class AnalyticalVMShould
    {
        [Fact]
        public void ReadAndWriteXML()
        {
            var ogVM = new AnalyticalVM();
            ogVM.IsFitToFlows = true;

            var newVM = new AnalyticalVM(ogVM.ToXML());

            Assert.Equal(ogVM.IsFitToFlows, newVM.IsFitToFlows);
        }
    }
}
