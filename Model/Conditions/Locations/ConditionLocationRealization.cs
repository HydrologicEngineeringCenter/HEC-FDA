using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Conditions.Locations
{
    internal sealed class ConditionLocationRealization : IConditionLocationRealization<string>
    {
        public string LateralStructureFailureElevation => "No Lateral Structure";
        public IReadOnlyDictionary<IMetric, double> Metrics { get; }
        public IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> Functions { get; }

        internal ConditionLocationRealization(IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> fxs, IReadOnlyDictionary<IMetric, double> metrics)
        {
            //ToDo: Validation
            Functions = fxs;
            Metrics = metrics;
        }
    }
}
