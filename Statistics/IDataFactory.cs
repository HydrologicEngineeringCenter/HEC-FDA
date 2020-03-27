using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{
    /// <summary>
    /// Provides methods for the creation of objects implementing the <see cref="IData"/> interface.
    /// Objects implementing the <see cref="IData"/> interface separate <see cref="double"/> precision datasets into containers of:
    /// <ul>
    /// <li> Usable finite numeric values, and </li>
    /// <li> Unusable non-numeric (<see cref="double.NaN"/>) or non-finite (<see cref="double.NegativeInfinity"/>, <see cref="double.PositiveInfinity"/>)</li>
    /// </ul> 
    /// The usable data container is returned as an <see cref="IData.Elements"/> property. Information about the unusable data is provided in "/> 
    /// </summary>
    public static class IDataFactory
    {
        /// <summary>
        /// Creates a new <see cref="IData"/> object instance. 
        /// </summary>
        /// <param name="data"> The dataset to be separated into usable (finite numeric values) and unusable data (<see cref="double.NaN"/>, <see cref="double.NegativeInfinity"/>, <see cref="double.PositiveInfinity"/>) containers. </param>
        /// <param name="usableDataRequirement"> <see langword="true"/> if an <see cref="Utilities.InvalidConstructorArgumentsException"/> should be thrown if no usable data elements are found in the <paramref name="data"/> dataset. Set to <see langword="false"/> by default. </param>
        /// <returns> An <see cref="IData"/> object containing a usable <see cref="IData.Elements"/> and unusable data containers. </returns>
        public static IData Factory(IEnumerable<double> data, bool usableDataRequirement = false) => new Data(data);
    }
}
