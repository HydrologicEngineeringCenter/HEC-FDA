using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Parameters.Elevations
{
    internal abstract class ElevationBase : IParameter
    {
        public string Label { get; }
        public UnitsEnum Units { get; }
        public IParameterEnum ParameterType { get; }
        public abstract IRange<double> Range { get; }
        public abstract bool IsConstant { get; }

        public ElevationBase(IParameterEnum parameterType, string label = "", UnitsEnum units = UnitsEnum.Foot)
        {
            Units = units;
            ParameterType = parameterType;
            Label = label == "" ? $"{ parameterType.Print()} ({units.Print()})" : label;
        }

        public string Print(bool round = false, bool abbreviate = false)
        {
            throw new NotImplementedException();
        }

        public string PrintValue(bool round = false, bool abbreviate = false)
        {
            throw new NotImplementedException();
        }
    }
}
