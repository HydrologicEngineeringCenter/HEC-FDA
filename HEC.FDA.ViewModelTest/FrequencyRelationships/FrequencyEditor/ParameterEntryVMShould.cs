using HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest.FrequencyRelationships.FrequencyEditor
{
    public class ParameterEntryVMShould
    {
        [Fact]
        public void ReadAndWriteXML()
        {
            //Arrange
            var ogVM = new ParameterEntryVM();
            ogVM.Mean = 4;

            //Act
            var newVM = new ParameterEntryVM(ogVM.ToXML());

            //Assert
            Assert.Equal(ogVM.Mean, newVM.Mean);
        }
    }
}
