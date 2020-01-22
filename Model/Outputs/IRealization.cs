using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Inputs.Functions;
using Model.Inputs.Conditions;

namespace Model.Outputs
{
    public interface IRealization
    { 
        ICondition Condition { get; }
        //todo: maybe get rid of this 
        double[] SampleProbabilities { get; }
        //IFrequencyFunction[] FrequencyFunctions { get; }
        //ITransformFunction[] TransformFunctions { get; }
        IDictionary<IMetric, double> Metrics { get; }
    }
}
