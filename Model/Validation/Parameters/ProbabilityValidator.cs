using System;
using System.Collections.Generic;
using System.Text;
using Model.Parameters.Probabilities;
using Utilities;

namespace Model.Validation
{
    internal class ProbabilityValidator : IValidator<Probability>
    {
        public IMessageLevels IsValid(Probability obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }
        
        public IEnumerable<IMessage> ReportErrors(Probability obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj));
            else
            {
                if (!obj.IsProbability()) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"A probability parameter was expected but a {obj.ParameterType.Print()} parameter was found causing an error."));
                if (!obj.Units.IsProbability()) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The specified units: {obj.Units.Print()} are not a valid measurement of probability. The default unit of measurement for this {obj.ParameterType.Print()} parameter are: {obj.ParameterType.DefaultUnits().Print()}."));
                if (obj.Range.Min < 0 || obj.Range.Max > 1) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The {obj.ParameterType.Print()} parameter contains probability values on the range: {obj.Range.Print(true)}, this is outside the allowable range of: [0, 1]."));
            }
            return msgs;
        }
    }
}
