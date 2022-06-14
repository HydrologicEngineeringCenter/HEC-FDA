using HEC.FDA.ViewModel.FrequencyRelationships;
using Xunit;

namespace fda_viewmodel_test
{
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
    }
}