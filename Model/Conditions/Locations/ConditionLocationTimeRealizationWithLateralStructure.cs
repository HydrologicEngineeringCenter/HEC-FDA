using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Conditions.Locations
{
    class ConditionLocationTimeRealizationWithLateralStructure : IConditionLocationTimeRealization
    {

        public IReadOnlyDictionary<IMetric, double> Metrics { get; }
        public ISampledParameter<IParameterOrdinate> LateralStructureFailureElevation { get; }
        public IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> Functions { get; }
        public bool HadLateralStructure => true;

        internal ConditionLocationTimeRealizationWithLateralStructure(IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> fxs, ISampledParameter<IParameterOrdinate> failureElevation, IReadOnlyDictionary<IMetric, double> metrics)
        {
            //ToDo: Validation
            Functions = fxs;
            Metrics = metrics;
            LateralStructureFailureElevation = failureElevation;
        }
    }
}
