using System;
using System.Collections.Generic;
using System.Text;
using Model.Parameters.Damages;
using Utilities;

namespace Model.Validation
{
    internal sealed class DamageValidator : IValidator<Damage>
    {
        public IMessageLevels IsValid(Damage obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }

        public IEnumerable<IMessage> ReportErrors(Damage obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj));
            else
            {
                if (!obj.ParameterType.IsDamage()) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"A flood damage parameter was expected but a {obj.ParameterType.Print()} parameter was found causing an error."));
                if (!(obj.Units == UnitsEnum.Dollars)) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The specified units: {obj.Units.Print()} are not a valid measurement of flood damage. At this time the only supported unit of measurement for this {obj.ParameterType.Print()} parameter is: {obj.ParameterType.UnitsDefault().Print()}."));
                if (obj.Range.Min < 0) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The {obj.ParameterType.Print()} parameter contains damage values on the range: {obj.Range.Print(true)}, this is outside the allowable range of: [0, {double.PositiveInfinity}]."));
            }
            return msgs;
        }
    }
}
