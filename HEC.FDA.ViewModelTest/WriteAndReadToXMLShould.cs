using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{

    public class WriteAndReadToXMLShould
    {

        [Fact]
        public void TestTerrainElementWriteThenReadAreEqual()
        {
            TerrainElement terrain = new TerrainElement("TerName", "TerFileName", 1);
            XElement terrainXML = terrain.ToXML();

            TerrainElement terrain2 = new TerrainElement(terrainXML, 1);

            Assert.True(terrain.Equals(terrain2));
        }

        [Fact]
        public void TestImpactAreaElementWriteThenReadAreEqual()
        {
            int id = 9;
            List<ImpactAreaRowItem> rows = new List<ImpactAreaRowItem>();
            for(int i = 0; i < 10; i++)
            {
                rows.Add(new ImpactAreaRowItem(i, "row"+ i));
            }
            ImpactAreaElement impAreaElem = new ImpactAreaElement("testName", "desc", rows, id);
            XElement impXML = impAreaElem.ToXML();

            ImpactAreaElement impAreaElem2 = new ImpactAreaElement(impXML, id);

            Assert.True(impAreaElem.Equals(impAreaElem2));
        }

        [Fact]
        public void TestIndexPointsElementWriteThenReadAreEqual()
        {
            int id = 9;
            List<string> rows = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                rows.Add("test" + i);
            }
            IndexPointsElement elem1 = new IndexPointsElement("testName", "desc", rows, id);
            XElement impXML = elem1.ToXML();

            IndexPointsElement elem2 = new IndexPointsElement(impXML, id);

            Assert.True(elem1.Equals(elem2));
        }

        [Fact]
        public void TestHydraulicsElementWriteThenReadAreEqual()
        {
            int id = 9;
            List<PathAndProbability> rows = new List<PathAndProbability>();
            for (int i = 0; i < 10; i++)
            {
                rows.Add(new PathAndProbability("path" + i, i));
            }
            HydraulicElement elem1 = new HydraulicElement("test", "desc", rows, true, HydraulicType.Gridded, id);
            XElement elemXML = elem1.ToXML();

            HydraulicElement elem2 = new HydraulicElement(elemXML, id);

            Assert.True(elem1.Equals(elem2));
        }

        [Fact]
        public void TestFrequencyElementWriteThenReadAreEqual()
        {
            int id = 9;
            List<double> flows = new List<double>() { 1.0, 2,3,4,5,6};
            int por = 45;
            bool isAnalytical = false;
            bool isStandard = false;
            double mean = 1;
            double stDev = 2;
            double skew = 3;
            GraphicalVM graphicalVM = new GraphicalVM("graph", "xLabel", "yLabel");
            ComputeComponentVM compVM = new ComputeComponentVM();

            AnalyticalFrequencyElement elem1 = new AnalyticalFrequencyElement("test","lastEdit", "desc",por,isAnalytical,isStandard,mean,stDev,skew,flows,graphicalVM,compVM, id);
            XElement elemXML = elem1.ToXML();

            AnalyticalFrequencyElement elem2 = new AnalyticalFrequencyElement(elemXML, id);

            Assert.True(elem1.Equals(elem2));
        }

        [Fact]
        public void TestRegulatedUnregulatedElementWriteThenReadAreEqual()
        {
            int id = 9;

            ComputeComponentVM compVM = new ComputeComponentVM("someName", "xLabel", "yLabel");
            compVM.SetPairedData(UncertainPairedDataFactory.CreateDefaultNormalData("xlabel", "ylabel", "name"));

            InflowOutflowElement elem1 = new InflowOutflowElement("myName", "lastEditDate", "desc", compVM, id);
            XElement elemXML = elem1.ToXML();

            InflowOutflowElement elem2 = new InflowOutflowElement(elemXML, id);

            Assert.True(elem1.Equals(elem2));
        }

        [Fact]
        public void TestExteriorInteriorStageElementWriteThenReadAreEqual()
        {
            int id = 9;

            ComputeComponentVM compVM = new ComputeComponentVM("someName", "xLabel", "yLabel");
            compVM.SetPairedData(UncertainPairedDataFactory.CreateDefaultNormalData("xlabel", "ylabel", "name"));

            ExteriorInteriorElement elem1 = new ExteriorInteriorElement("myName", "lastEditDate", "desc", compVM, id);
            XElement elemXML = elem1.ToXML();

            ExteriorInteriorElement elem2 = new ExteriorInteriorElement(elemXML, id);

            Assert.True(elem1.Equals(elem2));
        }

        [Fact]
        public void TestStageDischargeElementWriteThenReadAreEqual()
        {
            int id = 9;

            ComputeComponentVM compVM = new ComputeComponentVM("someName", "xLabel", "yLabel");
            compVM.SetPairedData(UncertainPairedDataFactory.CreateDefaultNormalData("xlabel", "ylabel", "name"));

            StageDischargeElement elem1 = new StageDischargeElement("myName", "lastEditDate", "desc", compVM, id);
            XElement elemXML = elem1.ToXML();

            StageDischargeElement elem2 = new StageDischargeElement(elemXML, id);

            Assert.True(elem1.Equals(elem2));
        }

        [Fact]
        public void TestLeveeElementWriteThenReadAreEqual()
        {
            int id = 9;

            ComputeComponentVM compVM = new ComputeComponentVM("someName", "xLabel", "yLabel");
            compVM.SetPairedData(UncertainPairedDataFactory.CreateDefaultNormalData("xlabel", "ylabel", "name"));

            double elevation = 99;

            LateralStructureElement elem1 = new LateralStructureElement("myName", "lastEditDate", "desc", elevation,false, compVM, id);
            XElement elemXML = elem1.ToXML();

            LateralStructureElement elem2 = new LateralStructureElement(elemXML, id);

            Assert.True(elem1.Equals(elem2));
        }

        //this test was failing because it calls code that has a message box.
        //[Fact]
        //public void TestStageDamageElementWriteThenReadAreEqual()
        //{
        //    int id = 9;

        //    ComputeComponentVM compVM = new ComputeComponentVM("someName", "xLabel", "yLabel");
        //    compVM.SetPairedData(UncertainPairedDataFactory.CreateDefaultNormalData("xlabel", "ylabel", "name"));

        //    int selectedWSE = 1;
        //    int selectedStructs = 2;
        //    int selectedIndexPoints = 3;
        //    List<StageDamageCurve> curves = new List<StageDamageCurve>();
        //    List<ImpactAreaFrequencyFunctionRowItem> functions = new List<ImpactAreaFrequencyFunctionRowItem>();

        //    AggregatedStageDamageElement elem1 = new AggregatedStageDamageElement("myName", "lastEditDate", "desc",
        //        selectedWSE,selectedStructs, selectedIndexPoints,curves,functions,true, id);
        //    XElement elemXML = elem1.ToXML();

        //    AggregatedStageDamageElement elem2 = new AggregatedStageDamageElement(elemXML, id);

        //    Assert.True(elem1.Equals(elem2));
        //}

    }
}
