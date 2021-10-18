using HEC.Plotting.Core.DataModel;
using HEC.Plotting.SciChart2D.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario.Editor.ChartControls
{
    public class FrequencyRelationshipControl : ChartControlBase
    {


        public FrequencyRelationshipControl()
            :base("FrequencyRelationship","Frequency", "Flow", "Flow-Frequency", false, false)
        {
            XAxisAlignment = AxisAlignment.Top;
        }

        

    }
}
