using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    interface IFrequencyFunction : IFdaFunction
    {

        IFrequencyFunction Compose(ITransformFunction transformFunction, double probability1, double probability2);

    }
}
