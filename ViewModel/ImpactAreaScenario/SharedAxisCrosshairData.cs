using HEC.Plotting.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario
{
    public class SharedAxisCrosshairData
    {
        public CrosshairData OtherCrosshairData { get; set; }
        public Axis OtherAxis { get; set; }
        public Axis CurrentAxis { get; set; }

        public SharedAxisCrosshairData(CrosshairData otherCrosshairData, Axis otherAxis, Axis currentAxis)
        {
            OtherCrosshairData = otherCrosshairData;
            OtherAxis = otherAxis;
            CurrentAxis = currentAxis;
        }
    }
}
