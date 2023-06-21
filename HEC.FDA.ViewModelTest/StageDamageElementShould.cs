using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FlowTransforms;
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
    [Trait("RunsOn", "Remote")]
    public class StageDamageElementShould
    {
    
        [Fact]
        public void WriteAndReadXML()
        {
            int id = 9;

            CurveComponentVM compVM = new CurveComponentVM("someName", "xLabel", "yLabel");
            compVM.SetPairedData(UncertainPairedDataFactory.CreateDefaultNormalData("xlabel", "ylabel", "name"));

            int selectedWSE = 1;
            int selectedStructs = 2;
            List<StageDamageCurve> curves = new List<StageDamageCurve>();
            ImpactAreaRowItem ri = new ImpactAreaRowItem(1, "myImpactArea");

            curves.Add(new StageDamageCurve(ri, "damCat", compVM, "assetCat", StageDamageConstructionType.USER));
            List<ImpactAreaFrequencyFunctionRowItem> functions = new List<ImpactAreaFrequencyFunctionRowItem>();
            functions.Add(new ImpactAreaFrequencyFunctionRowItem(ri, new List<FrequencyElement>(), new List<StageDischargeElement>(), new List<InflowOutflowElement>()));

            AggregatedStageDamageElement elem1 = new AggregatedStageDamageElement("myName", "lastEditDate", "desc", 1900,
                selectedWSE, selectedStructs, curves, functions, true,false, id);
            XElement elemXML = elem1.ToXML();

            AggregatedStageDamageElement elem2 = new AggregatedStageDamageElement(elemXML, id);

            Assert.True(elem1.Equals(elem2));
        }

    }
}
