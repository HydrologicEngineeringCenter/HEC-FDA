using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model
{
    internal static class IParameterUtilities
    {
        private static IRange<int> _ElevationRange => IRangeFactory.Factory(21, 29);
        internal static bool IsElevation(this IParameterEnum parameter) => _ElevationRange.IsOnRange((int)parameter);
        internal static bool IsNullOrEmpty(this IParameterSeries parameter)
        {
            if (parameter.IsNull() ||
                parameter.Parameter.IsNull()) return true;
            else return false;
        }
    }
}
