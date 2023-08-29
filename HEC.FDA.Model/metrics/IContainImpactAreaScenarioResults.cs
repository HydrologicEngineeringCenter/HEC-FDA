namespace HEC.FDA.Model.metrics
{
    public interface IContainImpactAreaScenarioResults
    {
        PerformanceByThresholds PerformanceByThresholds { get; }
        StudyAreaConsequencesBinned ConsequenceResults { get; }
        int ImpactAreaID { get; }
        bool Equals(ImpactAreaScenarioResults incomingIContainResults);
    }
}