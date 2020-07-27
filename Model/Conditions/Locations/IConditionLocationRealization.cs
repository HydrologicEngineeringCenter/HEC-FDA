using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    //todo: This seems like it should be the result. the realization should store lateral structures frequency functions etc. 
    public interface IConditionLocationRealization
    {
        IReadOnlyDictionary<IParameterEnum, ISampleRecord> Samples { get; }
        IReadOnlyDictionary<IMetric, double> Metrics { get; }
    }
    internal sealed class ConditionLocationRealization: IConditionLocationRealization
    {
        public IReadOnlyDictionary<IParameterEnum, ISampleRecord> Samples { get; }
        public IReadOnlyDictionary<IMetric, double> Metrics { get; }

        internal ConditionLocationRealization(IDictionary<IMetric, double> metrics, IDictionary<IParameterEnum, ISampleRecord> samples)
        {
            //ToDo: Validation
            Metrics = (IReadOnlyDictionary<IMetric, double>)metrics;
            Samples = (IReadOnlyDictionary<IParameterEnum, ISampleRecord>)samples;
        }
    }

}
