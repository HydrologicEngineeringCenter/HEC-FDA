using System;
using System.Collections.Generic;
using System.Linq;
using Statistics.Histograms;
using Utilities.Validation;

namespace Statistics.Validation
{
    public class BinValidator: IValidator<IBin>
    {
        public BinValidator()
        {
        }
        public bool IsValid(IBin obj, out IEnumerable<string> errors)
        {
            errors = ReportErrors(obj);
            return !errors.Any();
        }
        public IEnumerable<string> ReportErrors(IBin obj)
        {
            List<string> errors = new List<string>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj), "The Bin could not be validated because it is null.");
            if (!Validate.IsRange(obj.Minimum, obj.Maximum) || obj.Minimum == obj.Maximum) errors.Add($"The Histogram Bin is invalid because its minimum: {obj.Minimum} and maximum: {obj.Maximum} do not provide a valid finite range.");
            if (!(obj.Minimum.IsFinite() && obj.Maximum.IsFinite())) errors.Add($"The Histogram Bin is invalid because its range: [{obj.Minimum}, {obj.Maximum}) is not finite.");
            if (!(obj.Count.IsOnRange(min: 0, max: int.MaxValue))) errors.Add($"The Histogram Bin is invalid because its count: {obj.Count} is not on the valid range [0, {int.MaxValue}].");
            if (!obj.MidPoint.IsFinite()) errors.Add($"The histogram bin is invalid because its midpoint value: {obj.MidPoint} defined by the equations: ( BinMax: {obj.Maximum} - BinMin: {obj.Minimum} ) / 2, is not a finite numerical value.");
            return errors;
        }
    }
}
