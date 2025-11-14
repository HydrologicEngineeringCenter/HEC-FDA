using HEC.FDA.Model.paireddata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Utility.Progress;

namespace HEC.FDA.Model.stageDamage
{
    public class ScenarioStageDamage
    {
        #region Fields 
        private readonly List<ImpactAreaStageDamage> _ImpactAreaStageDamage;
        #endregion
        public List<ImpactAreaStageDamage> ImpactAreaStageDamages
        {
            get { return _ImpactAreaStageDamage; }
        }
        #region Constructor 
        public ScenarioStageDamage(List<ImpactAreaStageDamage> impactAreaStageDamages)
        {
            _ImpactAreaStageDamage = impactAreaStageDamages;
        }
        #endregion

        #region Methods 
        /// <summary>
        /// Begins the outermost loop of the Scenario Stage Damage Compute. 
        /// At this time, the compute engine can run a deterministic stage-damage compute, but the user interface does not avail that option.
        /// Scenario SD <--
        /// Impact Area SD
        /// Damage Catagory 
        /// Compute Chunk
        /// Iteration
        /// Structure
        /// W.S.Profile
        /// </summary>
        /// <returns></returns>
        public (List<UncertainPairedData>, List<UncertainPairedData>) Compute(bool computeIsDeterministic = false, ProgressReporter reporter = null)
        {
            reporter ??= ProgressReporter.None();
            Stopwatch sw = new();
            sw.Start();
            var computeTask = reporter.SubTask("Stage Damage Compute", 0, 1);
            computeTask.ReportMessage("Beginning Scenario Stage Damage Compute");
            (List<UncertainPairedData>, List<UncertainPairedData>) scenarioStageDamageResults = new([], []);
            int countImpactAreas = _ImpactAreaStageDamage.Count;
            for (int i = 0; i < countImpactAreas; i++)
            {
                ImpactAreaStageDamage impactAreaStageDamage = _ImpactAreaStageDamage[i];
                var subPr = computeTask.SubTask($"Impact Area {impactAreaStageDamage.ImpactAreaID} Compute", (float)i / countImpactAreas, 1f / countImpactAreas);
                subPr.ReportMessage($"{sw?.Elapsed:hh\\:mm\\:ss\\.fff}      Starting Impact Area ID: {impactAreaStageDamage.ImpactAreaID}, Structure Count: {impactAreaStageDamage.Inventory.Structures.Count}");
                (List<UncertainPairedData>, List<UncertainPairedData>) impactAreaStageDamageResults = impactAreaStageDamage.Compute(computeIsDeterministic, subPr, sw);
                scenarioStageDamageResults.Item1.AddRange(impactAreaStageDamageResults.Item1);
                scenarioStageDamageResults.Item2.AddRange(impactAreaStageDamageResults.Item2);
                subPr.ReportProgress(100);
            }
            computeTask.ReportProgressFraction(1f);
            computeTask.ReportTaskCompleted(sw.Elapsed);
            sw.Stop();
            return scenarioStageDamageResults;
        }


        public List<string> ProduceStructureDetails(Dictionary<int, string> impactAreaNames)
        {
            List<string> structureDetails = new();
            string generalHeader = $"Structure Details for Stage Damage Compute at {DateTime.Now}";
            structureDetails.Add(generalHeader);
            foreach (ImpactAreaStageDamage impactAreaStageDamage in _ImpactAreaStageDamage)
            {
                List<string> temp = impactAreaStageDamage.ProduceImpactAreaStructureDetails(impactAreaNames);
                foreach (string s in temp)
                {
                    structureDetails.Add(s);
                }
            }
            return structureDetails;
        }

        public List<string> GetErrorMessages()
        {
            List<string> errorMessages = new();
            foreach (ImpactAreaStageDamage impactAreaStageDamage in (_ImpactAreaStageDamage))
            {
                if (impactAreaStageDamage.HasErrors)
                {
                    errorMessages.Add(impactAreaStageDamage.GetErrorsFromProperties());
                }
            }
            return errorMessages;
        }
        #endregion
    }
}
