using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Utilities;

namespace Statistics
{
    /// <summary>
    /// Provides an container organizing <see cref="IEnumerable{double}"/> datasets into: 
    /// <ul>
    /// <li> Usable, ordered finite date elements </li>
    /// <li> Unusable, discarded invalid non-numeric data values </li>
    /// </ul>
    /// An option is provided specifying if an <see cref="Utilities.InvalidConstructorArgumentsException"/> should be thrown if the specified dataset contains 0 finite numeric values. 
    /// </summary>
    public interface IData: IMessagePublisher
    {
        /// <summary>
        /// The fully inclusive range of data values (e.g. [min, max]). <seealso cref="IRange{double}"/>
        /// </summary>
        IRange<double> Range { get; }
        /// <summary>
        /// The set of finite, numeric data elements in acceding order.
        /// </summary>
        IOrderedEnumerable<double> Elements { get; }
        /// <summary>
        /// The number of finite numeric data elements.
        /// </summary>
        int SampleSize { get; }
    }
}
