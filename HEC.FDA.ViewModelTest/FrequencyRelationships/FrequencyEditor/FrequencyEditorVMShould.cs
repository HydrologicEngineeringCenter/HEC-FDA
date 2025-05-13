using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using System.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest.FrequencyRelationships.FrequencyEditor
{
    [Trait("RunsOn", "Remote")]
    public class FrequencyEditorVMShould
    {
        [Fact]
        public void ReadAndWriteXML()
        {
            //Assemble
            var ogVM = new FrequencyEditorVM();
            ogVM.IsGraphical = true;
            ogVM.MyGraphicalVM.InputDataProvider.Data.Add(new DeterministicRow(.001, 10000,true));
            ogVM.ParameterEntryVM.Mean = 4;

            //Act
            var newVM = new FrequencyEditorVM(ogVM.ToXML());

            //Assert
            double expectedGraphFlow = ((DeterministicRow)ogVM.MyGraphicalVM.InputDataProvider.Data.Last()).Value;
            double actualGraphFlow = ((DeterministicRow)newVM.MyGraphicalVM.InputDataProvider.Data.Last()).Value;
            Assert.Equal(expectedGraphFlow, actualGraphFlow);

            Assert.Equal(ogVM.IsGraphical, newVM.IsGraphical);

            double expectedMean = ogVM.ParameterEntryVM.Mean;
            double actualMean = newVM.ParameterEntryVM.Mean;
            Assert.Equal(expectedMean,actualMean);
        }
    }
}
