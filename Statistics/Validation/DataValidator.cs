using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Statistics.Histograms;
using Utilities;

namespace Statistics.Validation
{
    internal class DataValidator : IValidator<IData>
    {
        public bool IsValid(IData entity, out IEnumerable<IMessage> errors)
        {
            errors = ReportErrors(entity);
            return errors.Any();
        }

        public IEnumerable<IMessage> ReportErrors(IData entity)
        {
            var msgs = new List<IMessage>();
            if (entity.Elements.IsNullOrEmpty()) IMessageFactory.Factory(IMessageLevels.Error, $"The provided data is invalid because it contains 0 finite elements.");
            return msgs;
        }
    }
}
