using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor.ChartControls
{
    public class DamageFrequencyControl:ChartControlBase
    {

        public DamageFrequencyControl()
            : base("DamageFrequency", "Frequency", "Damage", "Damage-Frequency", useProbabilityX: true, yAxisAlignment: HEC.Plotting.Core.DataModel.AxisAlignment.Right)
        {

        }

    }
}
