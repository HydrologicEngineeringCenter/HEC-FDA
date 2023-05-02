using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    [Trait("RunsOn", "Remote")]
    public class FrequencyElementShould
    {
        [Fact]
        public void WriteAndReadXML()
        {
            //arrange
            int id = 9;
            FrequencyEditorVM ogVM = new FrequencyEditorVM();
            ogVM.IsGraphical = true;
            AnalyticalFrequencyElement ogElem = new AnalyticalFrequencyElement("test", "lastEdit", "desc", id, ogVM);
            XElement elemXML = ogElem.ToXML();

            //act
            AnalyticalFrequencyElement elem2 = new AnalyticalFrequencyElement(elemXML, id);

            //assert
            Assert.True(ogElem.IsAnalytical == elem2.IsAnalytical);
        }
    }
}
