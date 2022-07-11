using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.FrequencyRelationships
{
    internal class AnalyticalFlowFrequencyVM
    {
        private FitToFlowsLP3VM _myFitToFlowsLP3VM;
        private ParameterLP3VM _myParameterLP3VM;

        public FitToFlowsLP3VM MyFitToFlowsLP3VM
        {
            get { return _myFitToFlowsLP3VM; }
            set { _myFitToFlowsLP3VM = value; }
        }
        public ParameterLP3VM MyParameterLP3VM
        {
            get { return _myParameterLP3VM; }
            set { _myParameterLP3VM = value; }
        }


    }
}
