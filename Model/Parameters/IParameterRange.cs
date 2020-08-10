using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model
{
    /// <summary>
    /// Builds on the <see cref="IParameter"/> interface by providing access to the range of values.
    /// </summary>
    public interface IParameterRange: IParameter
    {
        /// <summary>
        /// The units of measurement.
        /// </summary>
        UnitsEnum Units { get; }
        /// <summary>
        /// The range of parameter values.
        /// </summary>
        IRange<double> Range { get; }
    }
}
