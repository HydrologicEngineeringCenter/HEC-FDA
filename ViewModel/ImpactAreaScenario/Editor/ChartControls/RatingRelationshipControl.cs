using HEC.Plotting.Core.DataModel;
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
            : base("Rating", "Stage", "Flow", "Rating-Curve",false, true)
        {
            YAxisAlignment = AxisAlignment.Left;
            XAxisAlignment = AxisAlignment.Top;
        }

    }
}
