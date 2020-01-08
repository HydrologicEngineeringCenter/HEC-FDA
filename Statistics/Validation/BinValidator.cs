using System;
using System.Collections.Generic;
using System.Linq;

using Utilities;

namespace Statistics.Validation
{
    public class BinValidator: IValidator<IBin>
    {
        public BinValidator()
        {
        }
        public bool IsValid(IBin obj, out IEnumerable<IMessage> errors)
        {
            errors = ReportErrors(obj);
            return !errors.Any();
        }
        public IEnumerable<IMessage> ReportErrors(IBin obj)
        {
            List<IMessage> errors = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj), "The Bin could not be validated because it is null.");
            if (!Validate.IsRange(obj.Range.Min, obj.Range.Max) || obj.Range.Min == obj.Range.Max) errors.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The Histogram Bin is invalid because its minimum: {obj.Range.Min} and maximum: {obj.Range.Max} do not provide a valid finite range."));
            if (!(obj.Range.Min.IsFinite() && obj.Range.Max.IsFinite())) errors.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The Histogram Bin is invalid because its range: [{obj.Range.Min}, {obj.Range.Max}) is not finite."));
            if (!(obj.Count.IsOnRange(min: 0, max: int.MaxValue))) errors.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The Histogram Bin is invalid because its count: {obj.Count} is not on the valid range [0, {int.MaxValue}]."));
            if (!obj.MidPoint.IsFinite()) errors.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The histogram bin is invalid because its midpoint value: {obj.MidPoint} defined by the equations: (BinMax: {obj.Range.Max} - BinMin: {obj.Range.Min}) / 2 + BinMin: {obj.Range.Min}, is not a finite numerical value."));
            return errors;
        }
    }
}
