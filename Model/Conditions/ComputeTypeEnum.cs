using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Inputs.Conditions
{
    public enum ComputeTypeEnum
    {
        PerformanceOnly,
        EadOnly,
        PerformancePlusEad,
        PerformanceOnlyWithFragility,
        EadOnlyWithFragility,
        PerformancePlusEadWithFragility,
    }
}
