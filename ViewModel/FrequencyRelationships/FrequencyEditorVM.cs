using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    internal class FrequencyEditorVM
    {
        private AnalyticalFlowFrequencyVM _myAnalyticalFlowFrequencyVM;
        private GraphicalVM _myGraphicalVM;

        public AnalyticalFlowFrequencyVM MyAnalyticalFlowFrequencyVM
        {
            get { return _myAnalyticalFlowFrequencyVM; }
            set { _myAnalyticalFlowFrequencyVM = value; }
        }
        public GraphicalVM MyGraphicalVM
        {
            get { return _myGraphicalVM; }
            set { _myGraphicalVM = value; }
        }
    }
}
