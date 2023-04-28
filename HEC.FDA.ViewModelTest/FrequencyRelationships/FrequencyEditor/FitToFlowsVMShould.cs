using HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest.FrequencyRelationships.FrequencyEditor
{
    public class FitToFlowsVMShould
    {
        [Fact]
        public void ReadAndWriteToXML()
        {
            //Arrange
            var ogVM = new FitToFlowVM();
            ogVM.Mean = 4;
            XElement ele = ogVM.ToXML();

            //Act
            var newVM = new FitToFlowVM(ele);

            //Assert
            Assert.Equal(newVM.Mean, ogVM.Mean);
        }
    }
}
