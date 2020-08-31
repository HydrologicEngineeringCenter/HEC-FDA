using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.FrequencyRelationships
{
    public class FlowDoubleWrapper: BaseViewModel
    {
        public double Flow { get; set; }

        public FlowDoubleWrapper(double flow)
        {
            Flow = flow;
        }
    }
}
