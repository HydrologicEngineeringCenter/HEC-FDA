using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Parameters.Damages
{
    internal sealed class Damage : IParameter, IValidate<Damage>
    {
        public UnitsEnum Units { get; }
        public IParameterEnum ParameterType { get; }
        public string Label { get; }
        public IRange<double> Range { get; }
        public bool IsConstant { get; }

        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }

        internal Damage(IRange<double> range, bool isConstant, IParameterEnum type, UnitsEnum units = UnitsEnum.NotSet, string label = "")
        {
            if (range.IsNull()) throw new ArgumentException(nameof(range));
            else
            {
                Range = range;
                IsConstant = isConstant;
                ParameterType = type;
                Units = units == UnitsEnum.NotSet ? type.DefaultUnits() : units;
                Label = label == "" ? type.Print() : label;
            }
        }

        public IMessageLevels Validate(IValidator<Damage> validator, out IEnumerable<IMessage> msgs)
        {
            return validator.IsValid(this, out msgs);
        }
        
        public string Print(bool round = false, bool abbreviate = false)
        {
            return $"{ParameterType.Print(abbreviate)} in {Units.Print(abbreviate)}.";
        }
        public string PrintValue(bool round = false, bool abbreviate = false)
        {
            return $"{Print(round, abbreviate)} on range: {Range.Print(round)}.";
        }

       
    }
}
