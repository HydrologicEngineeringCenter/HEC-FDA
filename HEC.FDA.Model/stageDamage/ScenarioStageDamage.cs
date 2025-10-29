using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using HEC.FDA.Model.interfaces;
using HEC.FDA.Model.paireddata;
using RasMapperLib;
using Utility.Matrices;
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
        public (List<UncertainPairedData>, List<UncertainPairedData>) Compute( bool computeIsDeterministic = false, ProgressReporter reporter = null)
        {
            reporter ??= ProgressReporter.None();
            Stopwatch sw = new();
            sw.Start();
            reporter.ReportMessage("Beginning Scenario Stage Damage Compute");
            (List<UncertainPairedData>, List<UncertainPairedData>) scenarioStageDamageResults = new([], []);
            int countImpactAreas = _ImpactAreaStageDamage.Count;
            for(int i = 0; i< countImpactAreas; i++)
            {
                ImpactAreaStageDamage impactAreaStageDamage = _ImpactAreaStageDamage[i];
                reporter.ReportMessage($"Starting Impact Area ID:{impactAreaStageDamage.ImpactAreaID}, Elapsed time: {sw.Elapsed}");
                reporter.ReportMessage($"Structure Count: {impactAreaStageDamage.Inventory.Structures.Count}");
                (List<UncertainPairedData>, List<UncertainPairedData>) impactAreaStageDamageResults = impactAreaStageDamage.Compute(computeIsDeterministic);
                scenarioStageDamageResults.Item1.AddRange(impactAreaStageDamageResults.Item1);
                scenarioStageDamageResults.Item2.AddRange(impactAreaStageDamageResults.Item2);
                reporter.ReportProgressFraction(((float)i+1) / countImpactAreas);
            }
            reporter.ReportProgressFraction(1f);
            reporter.ReportTaskCompleted(sw.Elapsed);
            sw.Stop();
            return scenarioStageDamageResults;
        }


        public List<string> ProduceStructureDetails(Dictionary<int,string> impactAreaNames)
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
