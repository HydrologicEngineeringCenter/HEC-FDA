﻿using System.Collections.Generic;
using Statistics;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.interfaces;

namespace HEC.FDA.Model.stageDamage
{
    public class ScenarioStageDamage
    {
        #region Fields 
        private List<ImpactAreaStageDamage> _ImpactAreaStageDamage;
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
        public List<UncertainPairedData> Compute(IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria)
        {
            List<UncertainPairedData> scenarioStageDamageResults = new List<UncertainPairedData>();
            foreach (ImpactAreaStageDamage impactAreaStageDamage in _ImpactAreaStageDamage)
            {
                List<UncertainPairedData> impactAreaStageDamageResults = impactAreaStageDamage.Compute(randomProvider);
                foreach (UncertainPairedData uncertainPairedData in impactAreaStageDamageResults)
                {
                    scenarioStageDamageResults.Add(uncertainPairedData);
                }
            }
            return scenarioStageDamageResults;
        }


        public List<string> ProduceStructureDetails()
        {
            List<string> structureDetails = new List<string>();
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