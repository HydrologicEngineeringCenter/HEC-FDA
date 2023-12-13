using System.Collections.Generic;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.interfaces;
using System;

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
        /// Scenario SD <--
        /// Impact Area SD
        /// Damage Catagory 
        /// Compute Chunk
        /// Iteration
        /// Structure
        /// W.S.Profile
        /// </summary>
        /// <param name="randomProvider"></param>
        /// <returns></returns>
        public (List<UncertainPairedData>, List<UncertainPairedData>) Compute(IProvideRandomNumbers randomProvider)
        {

            (List<UncertainPairedData>, List<UncertainPairedData>) scenarioStageDamageResults = new(new List<UncertainPairedData>(), new List<UncertainPairedData>());

            foreach (ImpactAreaStageDamage impactAreaStageDamage in _ImpactAreaStageDamage)
            {
                (List<UncertainPairedData>, List<UncertainPairedData>) impactAreaStageDamageResults = impactAreaStageDamage.Compute(randomProvider);
                scenarioStageDamageResults.Item1.AddRange(impactAreaStageDamageResults.Item1);
                scenarioStageDamageResults.Item2.AddRange(impactAreaStageDamageResults.Item2);
            }
            return scenarioStageDamageResults;
        }


        public List<string> ProduceStructureDetails()
        {
            List<string> structureDetails = new();
            string generalHeader = $"Structure Details for Stage Damage Compute at {DateTime.Now}";
            structureDetails.Add(generalHeader);
            foreach (ImpactAreaStageDamage impactAreaStageDamage in _ImpactAreaStageDamage)
            {
                List<string> temp = impactAreaStageDamage.ProduceImpactAreaStructureDetails();
                foreach (string s in temp)
                {
                    structureDetails.Add(s);
                }
            }
            return structureDetails;
        }
        #endregion
    }
}
