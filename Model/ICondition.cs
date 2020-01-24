using System.Collections.Generic;

using Model.Inputs.Functions;
using Model.Outputs;

namespace Model
{
    public interface ICondition : IValidateData
    {
        #region Properties
        string Id { get; }
        string Name { get; }
        int Year { get; }
        IFrequencyFunction EntryPoint { get; }
        IList<ITransformFunction> TransformFunctions { get; }
        IList<IMetric> Metrics { get; }
        #endregion

        IDictionary<IMetric, double> Compute(List<double> randomNumbers);
    }
}
