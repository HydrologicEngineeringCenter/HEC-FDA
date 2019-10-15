using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Statistics.Histograms;

using Utilities.Validation;

namespace Statistics.Validation
{
    public class HistogramValidator : IValidator<IHistogram>
    {
        public bool IsValid(IHistogram entity, out IEnumerable<string> errors)
        {
            errors = ReportErrors(entity);
            return !errors.Any();
        }

        public IEnumerable<string> ReportErrors(IHistogram entity)
        {
            return new List<string>();
        }
    }
}
