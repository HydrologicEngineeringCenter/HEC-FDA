using System;
using System.Collections.Generic;
using System.Text;

using Functions;

using Utilities;

namespace Model
{
    /// <summary>
    /// A container for parameters with a single ordinate.
    /// </summary>
    public interface IParameterOrdinate: IParameter
    {
        UnitsEnum Units { get; }
        /// <summary>
        /// The elevation parameter value.
        /// </summary>
        IOrdinate Ordinate { get; }
    }
}
