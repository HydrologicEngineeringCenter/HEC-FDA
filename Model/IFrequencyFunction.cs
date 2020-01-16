using Model.Inputs.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public interface IFrequencyFunction : IFdaFunction
    {

        IFrequencyFunction Compose(ITransformFunction transformFunction, double probability1, double probability2);

    }
}
