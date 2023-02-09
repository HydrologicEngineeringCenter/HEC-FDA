using HEC.FDA.ViewModel.FrequencyRelationships;
using Importer;
using Xunit;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.TableWithPlot.Rows;

namespace HEC.FDA.ViewModelTest
{
    [Trait("Category", "Remote")]
    public class GraphicalVMShould
    {
        [Fact]
        public void ReadAndWriteXML()
        {
            GraphicalVM expected = new GraphicalVM("X","xlabel","ylabel");
            expected.XLabel = "X";
            expected.YLabel = "Y";
            expected.Description = "Description";
            expected.EquivalentRecordLength = 590;
            var el = expected.ToXML();

            GraphicalVM Actual = new GraphicalVM("Brennan", "xlabel", "ylabel");
            Actual.LoadFromXML(el);
            Assert.Equal(expected.Name, Actual.Name);
            Assert.Equal(expected.Description, Actual.Description);
            Assert.Equal(expected.EquivalentRecordLength, Actual.EquivalentRecordLength);
            Assert.Equal(expected.XLabel, Actual.XLabel);
            Assert.Equal(expected.YLabel, Actual.YLabel);
        }

        [Fact]
        public void LoadFromAnImportFile()
        {
            double[] stages = new double[] { 1, 2, 3 };
            double[] probs = new double[] { .99, .5, .01 };
            int ERL = 45;

            ProbabilityFunction pf = new ProbabilityFunction();
            pf.ProbabilityFunctionTypeId = ProbabilityFunction.FrequencyFunctionType.GRAPHICAL;
            pf.ProbabilityDataTypeId = ProbabilityFunction.ProbabilityDataType.STAGE_FREQUENCY;
            pf.NumberOfGraphicalPoints = stages.Length;
            pf.Stage = stages;
            pf.ExceedanceProbability = probs;
            pf.EquivalentLengthOfRecord = ERL;

            GraphicalVM expected = new GraphicalVM(StringConstants.GRAPHICAL_FREQUENCY, StringConstants.EXCEEDANCE_PROBABILITY, StringConstants.STAGE);
            expected.Name = StringConstants.GRAPHICAL_FREQUENCY;
            expected.EquivalentRecordLength = ERL;
            expected.UseFlow = false;
            expected.SelectedItem.Data.Clear();
            expected.SelectedItem.Data.Add(new GraphicalRow(.99, 1));
            expected.SelectedItem.Data.Add(new GraphicalRow(.5, 2));
            expected.SelectedItem.Data.Add(new GraphicalRow(.01, 3));

            GraphicalVM actual = new GraphicalVM(pf);

            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.XLabel, actual.XLabel);
            Assert.Equal(expected.YLabel, actual.YLabel);
            Assert.Equal(expected.UseFlow, actual.UseFlow);
            Assert.Equal(expected.EquivalentRecordLength, actual.EquivalentRecordLength);
        } 
    }
}