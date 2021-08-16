using Functions;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;


namespace Model.Parameters.Elevations
{
    internal sealed class ElevationRange : ElevationBase
    {
        internal ElevationRange(IRange<double> range, bool isConstant, IParameterEnum parameterType, UnitsEnum units = UnitsEnum.Foot, string label = "", bool abbreviatedLabel = true) : base(range, isConstant, parameterType, units, label, abbreviatedLabel)
        {
        }
    }
}
