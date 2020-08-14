using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Conditions.Locations
{
    internal sealed class ConditionLocationTimeRealizationNoLateralStructure: IConditionLocationTimeRealization
    {
        public ISampledParameter<IParameterOrdinate> LateralStructureFailureElevation { get; }
        public IReadOnlyDictionary<IMetric, double> Metrics { get; }
        public IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> Functions { get; }
        public bool HadLateralStructure => false;

        internal ConditionLocationTimeRealizationNoLateralStructure(IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> fxs, IReadOnlyDictionary<IMetric, double> metrics)
        {
            //ToDo: Validation
            Functions = fxs;
            Metrics = metrics;
            LateralStructureFailureElevation = new Samples.SampledOrdinate(IParameterFactory.Factory(double.NaN, IParameterEnum.ExteriorElevation), new Samples.Sample());
        }
    }
}
