using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary> Describes the order of a parameter series. </summary>
    public enum IFunctionOrder
    {
        /// <summary> The order of the Y values with respect to the X values is indeterminate because one or more of the coordinates has a distribute X or Y parameter </summary>
        NotSet = -1,
        /// <summary> Y value are neither increasing nor decreasing with respect to X values. </summary>
        NonMonotonic = 0,
        /// <summary> Y values are increasing with respect to X values, e.g. f(x(i)) is less than f(x(i + 1)) for all x(i) with i = {0.. n} and x(i) less than x(i + 1).  </summary>
        StrictlyIncreasing = 1,
        /// <summary> Y values increasing or remain constant with respect to increasing X values, e.g. f(x<sub>i</sub>) is less than or equal to f(x(i + 1)) for all x(i) with i = {0.. n} and x(i) less than x(i + 1).  </summary>
        WeaklyIncreasing = 2,
        /// <summary> Y values are decreasing with respect to X values, e.g. f(x(i)) is greater than f(x(i + 1)) for all x(i) with i = {0.. n} and x(i) less than x(i + 1).  </summary>
        StrictlyDecreasing = 3,
        /// <summary> Y values are decreasing or remain constant with respect to X values, e.g. f(x(i)) is greater than or equal to f(x(i + 1)) for all x(i) with i = {0.. n} and x(i) less than x(i + 1).  </summary>
        WeaklyDecreasing = 4,
    };
}
