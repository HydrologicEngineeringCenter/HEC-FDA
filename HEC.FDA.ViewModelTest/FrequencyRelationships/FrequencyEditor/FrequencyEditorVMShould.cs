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
            ogVM.GraphicalVM.CurveComponentVM.SelectedItem.Data.Add(new GraphicalRow(.001, 10000));
            ogVM.ParameterEntryVM.Mean = 4;

            //Act
            var newVM = new FrequencyEditorVM(ogVM.ToXML());

            //Assert
            double expectedGraphFlow = ((GraphicalRow)ogVM.GraphicalVM.CurveComponentVM.SelectedItem.Data.Last()).Value;
            double actualGraphFlow = ((GraphicalRow)newVM.GraphicalVM.CurveComponentVM.SelectedItem.Data.Last()).Value;
            Assert.Equal(expectedGraphFlow, actualGraphFlow);

            Assert.Equal(ogVM.IsGraphical, newVM.IsGraphical);

            double expectedMean = ogVM.ParameterEntryVM.Mean;
            double actualMean = newVM.ParameterEntryVM.Mean;
            Assert.Equal(expectedMean,actualMean);
        }
    }
}
