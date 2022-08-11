using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Saving.PersistenceManagers
{
    public class InflowOutflowPersistenceManager : SavingBase<InflowOutflowElement>
    {
        public override string TableName { get { return "regulated_unregulated_relationships"; } }

        public InflowOutflowPersistenceManager(Study.FDACache studyCache):base(studyCache)
        {
        }

    }
}
