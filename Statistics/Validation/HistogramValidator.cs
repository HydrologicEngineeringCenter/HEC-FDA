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
            List<IMessage> errors = new List<IMessage>();
            if (entity.IsNull()) throw new System.ArgumentNullException(nameof(entity), "The required IHistogram parameter is null.");
            if (entity.SampleSize < 1) errors.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The histogram contains no observations (SampleSize: {entity.SampleSize}) and therefore cannot be used."));
            if (entity.Bins.Length < 1) errors.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The histogram contains no bins and therefore cannot be used."));
            if (entity.Bins.Length == 1) errors.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The histogram only contains 1 bin. This is likely to lead to unexpected results."));
            return errors;
        }

        /// <summary>
        /// Ensures that the number of requested bins is valid. 
        /// Then delegates to other IsConstructable methods to check for valid <paramref name="min"/> and <paramref name="max"/> histogram parameters.
        /// </summary>
        /// <param name="min"> The requested histogram minimum value (inclusive). </param>
        /// <param name="max"> The requested histogram maximum value (exclusive). </param>
        /// <param name="nBins"> The requested number of histogram bins. </param>
        /// <param name="errors"> Error messages which are returned if the histogram is not constructable. </param>
        /// <returns> A boolean describing the constructability of the requested histogram. </returns>
        public static bool IsConstructable(double min, double max, int nBins, out IList<string> errors)
        {
            errors = new List<string>();
            if (nBins < 1) errors.Add($"The requested number of bins: {nBins} is invalid, because it is not a positive integer.");           
            bool returnvalue2 = nBins < 1 ? IsConstructable(min, max, double.MinValue, out IList<string> errors2) : IsConstructable(min, max, (max - min) / nBins, out errors2);
            errors.Concat(errors2);
            return errors.Any() || !returnvalue2;
        }
        /// <summary>
        /// Ensures that the requested empty histogram is valid. 
        /// </summary>
        /// <param name="min"> The requested histogram minimum value (inclusive). </param>
        /// <param name="max"> The requested histogram maximum value (exclusive). </param>
        /// <param name="binwidths"> The requested width of the histogram bins. </param>
        /// <param name="errors"> Error messages which are returned if the histogram is not constructable. </param>
        /// <returns> A boolean describing the constructability of the requested histogram. </returns>
        public static bool IsConstructable(double min, double max, double binwidths, out IList<string> errors)
        {
            errors = new List<string>();
            if (!binwidths.IsFinite() || !(binwidths > 0)) errors.Add($"The requested bin width: {binwidths} is not valid because it is not a positive finite value.");
            if (max - min < binwidths) errors.Add($"The requested bin width: {binwidths} is invalid because it is great than the requested {typeof(Histograms.Histogram)} range: [{min}, {max}).");
            if (!(min.IsFinite() && max.IsFinite() && Validate.IsRange(min, max))) errors.Add($"The requested range: [{min}, {max}) is invalid.");
            return !errors.Any();
        }
        public static bool IsConstructable(IData data, double min, double max, double width, out IList<string> errors)
        {
            errors = new List<string>();
            if (data.IsNull()) errors.Add($"The required {nameof(data)} parameter is null.");
            IsConstructable(max, min, width, out IList<string> moreErrors);
            errors.ToList().AddRange(moreErrors);
            return !errors.Any();
        }
        public static bool IsConstructable(IData data, double width, out IList<string> errors)
        {
            errors = new List<string>();
            if (data.IsNull()) errors.Add($"The required {nameof(data)} parameter is null.");
            IsConstructable(data.Range.Min, data.Range.Max, width, out IList<string> moreErrors);
            errors.ToList().AddRange(moreErrors);
            return !errors.Any();
        }
        public static bool IsConstructable(IHistogram histogram, IData data, List<IConvergenceCriteria> criterias, out IList<string> errors)
        {
            errors = new List<string>();
            if (histogram.IsNull()) errors.Add($"The required {nameof(histogram)} parameter is null.");
            if (data.IsNull()) errors.Add($"The required {nameof(data)} parameter is null.");
            if (criterias.IsNullOrEmpty()) errors.Add($"The required {nameof(data)} parameter is null or empty.");
            return !errors.Any();
        }
        //public static bool IsConstructable(double min, double max, IEnumerable<double> data, double binwidths, out IList<string> errors)
        //{
        //    errors = new List<string>();
        //    if (!(min.IsFinite() && max.IsFinite() && Validate.IsRange(min, max))) errors.Add($"The requested range: [{min}, {max}) is invalid.");
        //    bool returnvalue2 = IsConstructable(data, binwidths, out IList<string> errors2);
        //    errors.Concat(errors2);
        //    return errors.Any() || !returnvalue2;
        //}
        ///// <summary>
        ///// Ensures that the data to be binned and bin widths are valid.
        ///// </summary>
        ///// <param name="data"> The data to be binned. </param>
        ///// <param name="binwidths"> The requested width of the histogram bins. </param>
        ///// <param name="errors"> Error messages which are returned if the histogram is not constructable. </param>
        ///// <returns> A boolean describing the constructability of the requested histogram. </returns>
        //public static bool IsConstructable(IEnumerable<double> data, double binwidths, out IList<string> errors)
        //{
        //    errors = new List<string>();
        //    if (data.IsNullOrEmpty()) errors.Add("The provided sample data cannot be used to set the histogram bounds because it is null or empty.");
        //    if (!binwidths.IsFinite()) errors.Add($"The requested histogram bin width: {binwidths} is invalid because it is not finite.");
        //    return !errors.Any();
        //}
    }
}
