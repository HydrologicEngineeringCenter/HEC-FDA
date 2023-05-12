using HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;
using Xunit;

namespace HEC.FDA.ViewModelTest.FrequencyRelationships.FrequencyEditor
{
    [Trait("RunsOn", "Remote")]
    public class ParameterEntryVMShould
    {
        [Fact]
        public void ReadAndWriteXML()
        {
            //Arrange
            var ogVM = new ParameterEntryVM
            {
                Mean = 4
            };

            //Act
            var newVM = new ParameterEntryVM(ogVM.ToXML());

            //Assert
            Assert.Equal(ogVM.Mean, newVM.Mean);
        }
    }
}
