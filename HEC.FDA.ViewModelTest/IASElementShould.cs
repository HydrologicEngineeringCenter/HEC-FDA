using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.ImpactAreaScenario.Editor;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    [Trait("RunsOn", "Remote")]
    public class IASElementShould // : IDisposable
    {
        #region Setup
        //On deserialization, we're currently wired to fire off some validation that makes this testing
        //more complex than it ought to be. This set up is ai generated to establish a study cache we can use to get thes test done we want
        // typically this type of testing ought to be done at the model level, but because of some legacy decisions about viewmodel objectsbeing
        // largely responsible for serialization, here we are. 

        private readonly IStudyCache _originalCache;

        public IASElementShould()
        {
            _originalCache = BaseViewModel.StudyCache;
            BaseViewModel.StudyCache = new FakeStudyCache();
        }

        public void Dispose()
        {
            BaseViewModel.StudyCache = _originalCache;
        }

        /// <summary>
        /// Minimal IStudyCache implementation for testing. Returns null/empty for all lookups.
        /// </summary>
        private class FakeStudyCache : IStudyCache
        {
            private static ImpactAreaElement CreateDummyImpactAreaElement()
            {
                List<ImpactAreaRowItem> rows = new()
                {
                    new ImpactAreaRowItem(100, "ImpactArea100"),
                    new ImpactAreaRowItem(101, "ImpactArea101"),
                };
                return new ImpactAreaElement("TestImpactAreas", "desc", rows, 1, "Name");
            }

            public StudyPropertiesElement GetStudyPropertiesElement() => null;
            public List<ChildElement> GetChildElementsOfType(Type elementType)
            {
                if (elementType == typeof(ImpactAreaElement))
                {
                    return new List<ChildElement> { CreateDummyImpactAreaElement() };
                }
                return new();
            }
            public List<ChildElement> GetChildrenOfParent(ParentElement element) => new();
            public List<T> GetChildElementsOfType<T>() where T : ChildElement => new();
            public T GetParentElementOfType<T>() where T : ParentElement => default;
            public ChildElement GetChildElementOfType(Type type, int ID) => null;

            public event FDACache.AddElementEventHandler AlternativeCompReportAdded;
            public event FDACache.AddElementEventHandler AlternativeAdded;
            public event FDACache.AddElementEventHandler RatingAdded;
            public event FDACache.AddElementEventHandler TerrainAdded;
            public event FDACache.AddElementEventHandler ImpactAreaAdded;
            public event FDACache.AddElementEventHandler IndexPointsAdded;
            public event FDACache.AddElementEventHandler WaterSurfaceElevationAdded;
            public event FDACache.AddElementEventHandler FlowFrequencyAdded;
            public event FDACache.AddElementEventHandler InflowOutflowAdded;
            public event FDACache.AddElementEventHandler ExteriorInteriorAdded;
            public event FDACache.AddElementEventHandler LeveeAdded;
            public event FDACache.AddElementEventHandler StageDamageAdded;
            public event FDACache.AddElementEventHandler StructureInventoryAdded;
            public event FDACache.AddElementEventHandler StageLifeLossAdded;
            public event FDACache.AddElementEventHandler IASElementAdded;
            public event FDACache.AddElementEventHandler OccTypeElementAdded;

            public event FDACache.AddElementEventHandler AlternativeCompReportRemoved;
            public event FDACache.AddElementEventHandler AlternativeRemoved;
            public event FDACache.AddElementEventHandler RatingRemoved;
            public event FDACache.AddElementEventHandler TerrainRemoved;
            public event FDACache.AddElementEventHandler ImpactAreaRemoved;
            public event FDACache.AddElementEventHandler IndexPointsRemoved;
            public event FDACache.AddElementEventHandler WaterSurfaceElevationRemoved;
            public event FDACache.AddElementEventHandler FlowFrequencyRemoved;
            public event FDACache.AddElementEventHandler InflowOutflowRemoved;
            public event FDACache.AddElementEventHandler ExteriorInteriorRemoved;
            public event FDACache.AddElementEventHandler LeveeRemoved;
            public event FDACache.AddElementEventHandler StageDamageRemoved;
            public event FDACache.AddElementEventHandler StructureInventoryRemoved;
            public event FDACache.AddElementEventHandler StageLifeLossRemoved;
            public event FDACache.AddElementEventHandler IASElementRemoved;
            public event FDACache.AddElementEventHandler OccTypeElementRemoved;

            public event FDACache.UpdateElementEventHandler AlternativeCompReportUpdated;
            public event FDACache.UpdateElementEventHandler AlternativeUpdated;
            public event FDACache.UpdateElementEventHandler RatingUpdated;
            public event FDACache.UpdateElementEventHandler TerrainUpdated;
            public event FDACache.UpdateElementEventHandler ImpactAreaUpdated;
            public event FDACache.UpdateElementEventHandler IndexPointsUpdated;
            public event FDACache.UpdateElementEventHandler WaterSurfaceElevationUpdated;
            public event FDACache.UpdateElementEventHandler FlowFrequencyUpdated;
            public event FDACache.UpdateElementEventHandler InflowOutflowUpdated;
            public event FDACache.UpdateElementEventHandler ExteriorInteriorUpdated;
            public event FDACache.UpdateElementEventHandler LeveeUpdated;
            public event FDACache.UpdateElementEventHandler StageDamageUpdated;
            public event FDACache.UpdateElementEventHandler StructureInventoryUpdated;
            public event FDACache.UpdateElementEventHandler StageLifeLossUpdated;
            public event FDACache.UpdateElementEventHandler IASElementUpdated;
            public event FDACache.UpdateElementEventHandler OccTypeElementUpdated;
        }


        #endregion


        private static ConsequencesConfig CreateFullConfig()
        {
            return new ConsequencesConfig
            {
                ConfigHasFailureStageDamage = true,
                ConfigFailureStageDamageID = 10,
                ConfigHasNonFailureStageDamage = true,
                ConfigNonFailureStageDamageID = 20,
                ConfigHasFailureStageLifeLoss = true,
                ConfigFailureStageLifeLossID = 30,
                ConfigHasNonFailureStageLifeLoss = true,
                ConfigNonFailureStageLifeLossID = 40
            };
        }

        private static ConsequencesConfig CreateMinimalConfig()
        {
            return new ConsequencesConfig
            {
                ConfigHasFailureStageDamage = true,
                ConfigFailureStageDamageID = 10,
                ConfigHasNonFailureStageDamage = false,
                ConfigNonFailureStageDamageID = -1,
                ConfigHasFailureStageLifeLoss = false,
                ConfigFailureStageLifeLossID = -1,
                ConfigHasNonFailureStageLifeLoss = false,
                ConfigNonFailureStageLifeLossID = -1
            };
        }

        private static List<SpecificIAS> CreateSpecificIASList(ConsequencesConfig config, int count = 2)
        {
            List<SpecificIAS> list = new();
            for (int i = 0; i < count; i++)
            {
                var thresholds = new List<ThresholdRowItem>();
                var specificIAS = new SpecificIAS(
                    impactAreaID: 100 + i,
                    flowFreqID: 1,
                    inflowOutflowID: 2,
                    ratingID: 3,
                    extIntID: 4,
                    leveeFailureID: 5,
                    thresholds: thresholds,
                    calculateDefaultThreshold: true,
                    defaultStage: 10.0,
                    config: config);
                list.Add(specificIAS);
            }
            return list;
        }

        [Fact]
        public void RoundTripIASElementWithAllConsequenceConfigs()
        {
            // Arrange
            int id = 1;
            ConsequencesConfig config = CreateFullConfig();
            List<SpecificIAS> specificIASList = CreateSpecificIASList(config);

            IASElement original = new("TestScenario", "description", "2024-01-01", "2024",
                stageDamageElementID: 10, nonFailureStageDamageID: 20,
                failureStageLifeLossID: 30, nonFailureStageLifeLossID: 40,
                hasNonFailureStageDamage: true, elems: specificIASList, id: id);

            // Act
            XElement xml = original.ToXML();
            IASElement roundTripped = new(xml, id);

            // Assert - IASElement level
            Assert.Equal(original.FailureStageDamageID, roundTripped.FailureStageDamageID);
            Assert.Equal(original.NonFailureStageDamageID, roundTripped.NonFailureStageDamageID);
            Assert.Equal(original.FailureStageLifeLossID, roundTripped.FailureStageLifeLossID);
            Assert.Equal(original.NonFailureStageLifeLossID, roundTripped.NonFailureStageLifeLossID);
            Assert.Equal(original.HasNonFailureStageDamage, roundTripped.HasNonFailureStageDamage);
            Assert.Equal(original.AnalysisYear, roundTripped.AnalysisYear);
            Assert.Equal(original.SpecificIASElements.Count, roundTripped.SpecificIASElements.Count);
        }

        [Fact]
        public void RoundTripIASElementWithNoOptionalConsequences()
        {
            // Arrange
            int id = 2;
            ConsequencesConfig config = CreateMinimalConfig();
            List<SpecificIAS> specificIASList = CreateSpecificIASList(config);

            IASElement original = new("MinimalScenario", "description", "2024-01-01", "2024",
                stageDamageElementID: 10, nonFailureStageDamageID: -1,
                failureStageLifeLossID: -1, nonFailureStageLifeLossID: -1,
                hasNonFailureStageDamage: false, elems: specificIASList, id: id);

            // Act
            XElement xml = original.ToXML();
            IASElement roundTripped = new(xml, id);

            // Assert
            Assert.Equal(10, roundTripped.FailureStageDamageID);
            Assert.Equal(-1, roundTripped.NonFailureStageDamageID);
            Assert.Equal(-1, roundTripped.FailureStageLifeLossID);
            Assert.Equal(-1, roundTripped.NonFailureStageLifeLossID);
            Assert.False(roundTripped.HasNonFailureStageDamage);
        }

        [Fact]
        public void LoadOldXmlWithoutLifeLossAttributes()
        {
            // Arrange - construct XML that mimics old format (no life loss attributes)
            int id = 3;
            XElement oldXml = new(IASElement.IAS_SET);
            oldXml.SetAttributeValue(IASElement.YEAR, "2024");
            oldXml.SetAttributeValue(IASElement.FAILURE_STAGE_DAMAGE_ID, 10);
            // Old format: no NON_FAILURE_STAGE_DAMAGE_ID, no life loss attributes, no HAS_NON_FAILURE_STAGE_DAMAGE

            // Add a Header element (required by base ChildElement constructor)
            XElement header = new("Header");
            header.SetAttributeValue("ID", id);
            header.SetAttributeValue("Name", "OldScenario");
            header.SetAttributeValue("Description", "old format");
            header.SetAttributeValue("LastEditDate", "2024-01-01");
            oldXml.Add(header);

            // Add a child IAS element (old format with StageDamage element)
            XElement iasChild = new("IAS");
            iasChild.SetAttributeValue("ImpactArea", 100);
            iasChild.Add(new XElement("FrequencyRelationship", new XAttribute("ID", 1)));
            iasChild.Add(new XElement("InflowOutflow", new XAttribute("ID", 2)));
            iasChild.Add(new XElement("Rating", new XAttribute("ID", 3)));
            iasChild.Add(new XElement("LateralStructure", new XAttribute("ID", 5)));
            iasChild.Add(new XElement("ExteriorInterior", new XAttribute("ID", 4)));
            iasChild.Add(new XElement("StageDamage", new XAttribute("ID", 10)));
            iasChild.Add(new XElement("Thresholds"));
            oldXml.Add(iasChild);

            // Act
            IASElement loaded = new(oldXml, id);

            // Assert - life loss IDs default to -1
            Assert.Equal(10, loaded.FailureStageDamageID);
            Assert.Equal(IASElement.NONE_SELECTED_ID, loaded.NonFailureStageDamageID);
            Assert.Equal(IASElement.NONE_SELECTED_ID, loaded.FailureStageLifeLossID);
            Assert.Equal(IASElement.NONE_SELECTED_ID, loaded.NonFailureStageLifeLossID);
            Assert.False(loaded.HasNonFailureStageDamage);
        }

        [Fact]
        public void SpecificIASConsequencePropertiesPopulatedFromParent()
        {
            // Arrange
            int id = 4;
            ConsequencesConfig config = CreateFullConfig();
            List<SpecificIAS> specificIASList = CreateSpecificIASList(config);

            IASElement original = new("TestScenario", "description", "2024-01-01", "2024",
                stageDamageElementID: 10, nonFailureStageDamageID: 20,
                failureStageLifeLossID: 30, nonFailureStageLifeLossID: 40,
                hasNonFailureStageDamage: true, elems: specificIASList, id: id);

            // Act
            XElement xml = original.ToXML();
            IASElement roundTripped = new(xml, id);

            // Assert - each child SpecificIAS has all 8 consequence properties populated
            Assert.Equal(2, roundTripped.SpecificIASElements.Count);
            foreach (SpecificIAS child in roundTripped.SpecificIASElements)
            {
                Assert.True(child.HasFailureStageDamage);
                Assert.Equal(10, child.FailureStageDamageID);
                Assert.True(child.HasNonFailureStageDamage);
                Assert.Equal(20, child.NonFailureStageDamageID);
                Assert.True(child.HasFailureStageLifeLoss);
                Assert.Equal(30, child.FailureStageLifeLossID);
                Assert.True(child.HasNonFailureStageLifeLoss);
                Assert.Equal(40, child.NonFailureStageLifeLossID);
            }
        }


    }
}
