using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class FlowFrequencyPersistenceManager : SavingBase<AnalyticalFrequencyElement>
    {
        /// <summary>
        /// The name of the parent table that will hold all elements of this type
        /// </summary>
        public override string TableName { get { return "analytical_frequency_relationships"; } }

        public FlowFrequencyPersistenceManager(Study.FDACache studyCache):base(studyCache)
        {
        }

    }
}
