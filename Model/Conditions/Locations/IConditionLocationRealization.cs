using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{

    public interface IConditionLocationRealization<T>
    {
        T LateralStructureFailureElevation { get; }
        IReadOnlyDictionary<IMetric, double> Metrics { get; }
        IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> Functions { get; }
    }
}
