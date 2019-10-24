using System.Collections.Generic;
using System.Linq;

using Utilities;

namespace Statistics.Validation
{
    public class HistogramValidator : IValidator<IHistogram>
    {
        public bool IsValid(IHistogram entity, out IEnumerable<IMessage> errors)
        {
            errors = ReportErrors(entity);
            return !errors.Any();
        }

        public IEnumerable<IMessage> ReportErrors(IHistogram entity)
        {
            return new List<IMessage>();
        }
    }
}
