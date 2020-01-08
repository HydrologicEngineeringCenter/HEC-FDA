using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Utilities;

namespace Statistics
{
    /// <summary>
    /// Provides an container for ordered, finite input data.
    /// </summary>
    public interface IData: IValidate<IData>
    {
        IRange<double> Range { get; }
        ///// <summary>
        ///// The minimum of the provided data minimum or the smallest data element.
        ///// </summary>
        //double Minimum { get; }
        ///// <summary>
        ///// The maximum of the provided data maximum or the largest data element.
        ///// </summary>
        //double Maximum { get; }
        /// <summary>
        /// Finite data elements in accending order.
        /// </summary>
        IOrderedEnumerable<double> Elements { get; }
        /// <summary>
        /// The number of finite data elements.
        /// </summary>
        int SampleSize { get; }
    }
}
