using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario.Editor.ChartControls
{
    public class RatingRelationshipControl : ChartControlBase
    {

        public RatingRelationshipControl()
            : base("Rating", "Stage", "Flow", "Rating-Curve", flipXY: true, xAxisAlignment: HEC.Plotting.Core.DataModel.AxisAlignment.Top)
        {

        }

    }
}
