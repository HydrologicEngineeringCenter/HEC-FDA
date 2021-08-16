using System;
using System.Collections.Generic;
using System.Text;
using Functions;
using Utilities;

namespace Model.Parameters.Elevations
{
    internal class ElevationOrdinate : ElevationBase, IParameterOrdinate, IValidate<ElevationBase>
    {
        public IOrdinate Ordinate { get; }

        public ElevationOrdinate(IOrdinate value, IParameterEnum type, UnitsEnum units = UnitsEnum.Foot, string label = "", bool abbreviatedLabel = true): base(value.Range, value.Type == IOrdinateEnum.Constant, type, units, label, abbreviatedLabel)
        {
            if (value.IsNull()) throw new ArgumentNullException(nameof(value));
            else
            {
                Ordinate = value;
            }
        }

        public override string Print(bool round = true, bool abbreviate = true)
        {
            return $"{ParameterType.Print(abbreviate)}({Ordinate.Print(round)})";
        }
    }
}
