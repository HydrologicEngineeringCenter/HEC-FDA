using System;
using System.Collections.Generic;
using System.Text;
using Functions;
using Utilities;

namespace Model
{
    /// <summary>
    /// A container for <see cref="IParameter{T}"/> containing an <see cref="IEnumerable{IOrdinate}"/>.
    /// Mostly for use in holding function variables.
    /// </summary>
    public interface IParameterSeries: IParameter<IEnumerable<IOrdinate>>
    {
    }
}
