using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{

    public class StageDamageElementShould
    {
    
        [Fact]
        public void WriteAndReadXML()
        {
            int id = 9;

            ComputeComponentVM compVM = new ComputeComponentVM("someName", "xLabel", "yLabel");
            compVM.SetPairedData(UncertainPairedDataFactory.CreateDefaultNormalData("xlabel", "ylabel", "name"));

            int selectedWSE = 1;
            int selectedStructs = 2;
            List<StageDamageCurve> curves = new List<StageDamageCurve>();
            ImpactAreaRowItem ri = new ImpactAreaRowItem(1, "myImpactArea");

            curves.Add(new StageDamageCurve(ri, "damCat", compVM, "assetCat", StageDamageConstructionType.USER));
            List<ImpactAreaFrequencyFunctionRowItem> functions = new List<ImpactAreaFrequencyFunctionRowItem>();
            functions.Add(new ImpactAreaFrequencyFunctionRowItem(ri, new List<AnalyticalFrequencyElement>(), new List<StageDischargeElement>()));

            AggregatedStageDamageElement elem1 = new AggregatedStageDamageElement("myName", "lastEditDate", "desc",
                selectedWSE, selectedStructs, curves, functions, true, id);
            XElement elemXML = elem1.ToXML();

            AggregatedStageDamageElement elem2 = new AggregatedStageDamageElement(elemXML, id);

            Assert.True(elem1.Equals(elem2));
        }

    }
}
