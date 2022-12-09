using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEC.FDA.Model.utilities;

namespace HEC.FDA.Model.metrics
{
    internal class ProcessedConsequenceResultsList
    {

        #region Fields 
        private List<ProcessedConsequenceResults> _results = new List<ProcessedConsequenceResults>();
        #endregion

        #region Properties
        internal List<ProcessedConsequenceResults> Results { get { return _results; } }
        #endregion

        #region Constructor 
        internal ProcessedConsequenceResultsList(List<ConsequenceResults> consequenceResults)
        {
            _results = Process(consequenceResults);
        }
        #endregion

        #region Methods 
        private List<ProcessedConsequenceResults> Process(List<ConsequenceResults> consequenceResultsList)
        {
            List<ProcessedConsequenceResults> results = new List<ProcessedConsequenceResults>();

            foreach (var consequenceResults in consequenceResultsList)
            {
                foreach (var consequenceResult in consequenceResults.ConsequenceResultList)
                {
                    ProcessEachConsequenceResult(consequenceResult);
                }
            }

            return results;
        }

        private void ProcessEachConsequenceResult(ConsequenceResult consequenceResult)
        {
            ProcessEachConsequence(StringConstants.STRUCTURE_ASSET_CATEGORY, consequenceResult.DamageCategory, consequenceResult.StructureDamage);
            ProcessEachConsequence(StringConstants.CONTENT_ASSET_CATEGORY, consequenceResult.DamageCategory, consequenceResult.ContentDamage);
            ProcessEachConsequence(StringConstants.VEHICLE_ASSET_CATEGORY, consequenceResult.DamageCategory, consequenceResult.VehicleDamage);
            ProcessEachConsequence(StringConstants.OTHER_ASSET_CATEGORY, consequenceResult.DamageCategory, consequenceResult.OtherDamage);
        }

        private void ProcessEachConsequence(string assetCategory, string damageCategory, double damageRealization)
        {
            ProcessedConsequenceResults structureProcessedConsequenceResults = GetProcessedConsequenceResults(assetCategory, damageCategory);
            if (structureProcessedConsequenceResults.IsNull)
            {
                ProcessedConsequenceResults newProcessedConsequenceResults = new ProcessedConsequenceResults(damageRealization, assetCategory, damageCategory);
                _results.Add(newProcessedConsequenceResults);
            }
            else
            {
                structureProcessedConsequenceResults.AddDamageRealization(damageRealization);
            }
        }

        private ProcessedConsequenceResults GetProcessedConsequenceResults(string assetCategory, string damageCategory)
        {
            if (_results.Count != 0)
            {
                foreach (ProcessedConsequenceResults processedConsequenceResults in _results)
                {
                    if (processedConsequenceResults.AssetCategory.Equals(assetCategory))
                    {
                        if (processedConsequenceResults.DamageCategory.Equals(damageCategory))
                        {
                            return processedConsequenceResults;
                        }
                    }
                }
            }
            return new ProcessedConsequenceResults(isNull: true);
        }

        #endregion
    }
}
