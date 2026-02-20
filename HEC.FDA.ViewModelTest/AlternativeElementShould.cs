using HEC.FDA.ViewModel.Alternatives;
using System.Xml.Linq;
using Xunit;

namespace HEC.FDA.ViewModelTest
{
    [Trait("RunsOn", "Remote")]
    public class AlternativeElementShould
    {
        [Fact]
        public void SerializeAndDeserializeWithBothScenarios()
        {
            int id = 1;
            string name = "TestAlt";
            string description = "Test description";
            string creationDate = "2026-01-01";
            AlternativeScenario baseScenario = new(100, 2023);
            AlternativeScenario futureScenario = new(200, 2072);

            AlternativeElement original = new(name, description, creationDate, baseScenario, futureScenario, id);
            XElement xml = original.ToXML();

            AlternativeElement deserialized = new(xml, id);

            Assert.Equal(name, deserialized.Name);
            Assert.Equal(description, deserialized.Description);
            Assert.Equal(baseScenario.ScenarioID, deserialized.BaseScenario.ScenarioID);
            Assert.Equal(baseScenario.Year, deserialized.BaseScenario.Year);
            Assert.NotNull(deserialized.FutureScenario);
            Assert.Equal(futureScenario.ScenarioID, deserialized.FutureScenario.ScenarioID);
            Assert.Equal(futureScenario.Year, deserialized.FutureScenario.Year);
        }

        [Fact]
        public void SerializeAndDeserializeWithBaseScenarioOnly()
        {
            int id = 2;
            string name = "BaseOnlyAlt";
            string description = "Base only description";
            string creationDate = "2026-01-01";
            AlternativeScenario baseScenario = new(100, 2023);

            AlternativeElement original = new(name, description, creationDate, baseScenario, null, id);
            XElement xml = original.ToXML();

            AlternativeElement deserialized = new(xml, id);

            Assert.Equal(name, deserialized.Name);
            Assert.Equal(description, deserialized.Description);
            Assert.Equal(baseScenario.ScenarioID, deserialized.BaseScenario.ScenarioID);
            Assert.Equal(baseScenario.Year, deserialized.BaseScenario.Year);
            Assert.Null(deserialized.FutureScenario);
        }
    }
}
