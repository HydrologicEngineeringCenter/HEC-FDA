using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Conditions.Locations.Years.Realizations
{
    class ConditionLocationYearRealizationWithLateralStructure : IConditionLocationYearRealization
    {

        public int ID { get; }
        public IReadOnlyDictionary<IMetric, double> Metrics { get; }
        public ISampledParameter<IParameterOrdinate> LateralStructureFailureElevation { get; }
        public IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> Functions { get; }
        public bool HadLateralStructure => true;

        internal ConditionLocationYearRealizationWithLateralStructure(IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> fxs, ISampledParameter<IParameterOrdinate> failureElevation, IReadOnlyDictionary<IMetric, double> metrics, int id)
        {
            //ToDo: Validation
            ID = id;
            Functions = fxs;
            Metrics = metrics;
            LateralStructureFailureElevation = failureElevation;
        }
    }
}
