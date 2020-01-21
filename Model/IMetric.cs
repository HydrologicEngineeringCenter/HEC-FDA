using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Inputs.Conditions;
using Model.Inputs.Functions;
using Model.Inputs.Functions.ImpactAreaFunctions;

namespace Model
{
    public interface IMetric
    {
        #region Properties
        MetricEnum Type { get; }
        double ExceedanceTarget { get; }
        ImpactAreaFunctionEnum TargetFunction();
        #endregion

        double Compute(IFrequencyFunction frequencyFunction, double probability);
    }
}
