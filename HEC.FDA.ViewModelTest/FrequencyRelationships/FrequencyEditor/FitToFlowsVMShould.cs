using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest.FrequencyRelationships.FrequencyEditor
{
    [Trait("RunsOn", "Remote")]
    public class FitToFlowsVMShould
    {
        [Fact]
        public void ReadAndWriteToXML()
        {
            //Arrange
            var ogVM = new FitToFlowVM();
            ogVM.Data.Add(new FlowDoubleWrapper(11000));
            ogVM.ComputeAction(null, null);
            XElement ele = ogVM.ToXML();

            //Act
            var newVM = new FitToFlowVM(ele);

            //Assert
            Assert.Equal(newVM.Mean, ogVM.Mean);
        }
    }
}
