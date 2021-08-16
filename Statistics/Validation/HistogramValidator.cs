using Statistics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Statistics.Validation
{
    public class HistogramValidator : IValidator<IHistogram>
    {
        public IMessageLevels IsValid(IHistogram obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }
        public IEnumerable<IMessage> ReportErrors(IHistogram obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new System.ArgumentNullException(nameof(obj), "The IHistogram can not be validated because it is null.");
            if (obj.SampleSize < 1 || obj.Bins.Length < 1) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"{Resources.InvalidParameterizationNotice(obj.Print(true))} It requires: {Parameters()}."));
            if (obj.Bins.Length == 1) msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The histogram only contains 1 bin. This is likely to lead to unexpected results."));
            msgs.AddRange(PrintBinErrors(obj.Bins));
            return msgs;
        }
        private IEnumerable<IMessage> PrintBinErrors(IBin[] bins)
        {
            List<IMessage> ret = new List<IMessage>();
            for (int i = 0; i < bins.Length; i++)
            {
                if (bins[i].Messages.Any())
                {
                    IMessage[] msgs = bins[i].Messages.ToArray();
                    IMessageLevels level = IMessageLevels.NoErrors;
                    StringBuilder binMsg = new StringBuilder($"Histogram bin {i + 1} of {bins.Length.Print()} contains the following {msgs.Length} messages: \r\n");
                    for (int j = 0; j < msgs.Length; j++)
                    {
                        if (msgs[j].Level < level) level = msgs[j].Level;
                        binMsg.Append($"\t{msgs[j].Notice}.\r\n");
                    }
                    ret.Add(IMessageFactory.Factory(level, binMsg.ToString()));
                }
            }
            return ret;
        }

        /// <summary>
        /// Ensures that the number of requested bins is valid. 
        /// Then delegates to other IsConstructable methods to check for valid <paramref name="min"/> and <paramref name="max"/> histogram parameters.
        /// </summary>
        /// <param name="min"> The requested histogram minimum value (inclusive). </param>
        /// <param name="max"> The requested histogram maximum value (exclusive). </param>
        /// <param name="nBins"> The requested number of histogram bins. </param>
        /// <param name="errors"> Error messages which are returned if the histogram is not constructible. </param>
        /// <returns> A boolean describing the constructibility of the requested histogram. </returns>
        public static bool IsConstructable(double min, double max, int nBins, out IList<string> errors)
        {
            errors = new List<string>();
            IList<string> errors2;
            if (nBins < 1) errors.Add($"The requested number of bins: {nBins.Print()} is invalid, because it is not a positive integer.");
            bool returnvalue2 = false ;
            if (nBins < 1)
            {
                returnvalue2 = IsConstructable(min, max, double.MinValue, out errors2 );
            }
            else
            {
                returnvalue2 = IsConstructable(min, max, (max - min) / nBins, out  errors2);
            }
            errors.Concat( errors2);
            return !errors.Any() || returnvalue2;
        }
        /// <summary>
        /// Ensures that the requested empty histogram is valid. 
        /// </summary>
        /// <param name="min"> The requested histogram minimum value (inclusive). </param>
        /// <param name="max"> The requested histogram maximum value (exclusive). </param>
        /// <param name="binwidths"> The requested width of the histogram bins. </param>
        /// <param name="errors"> Error messages which are returned if the histogram is not constructible. </param>
        /// <returns> A boolean describing the constructibility of the requested histogram. </returns>
        public static bool IsConstructable(double min, double max, double binwidths, out IList<string> errors)
        {
            errors = new List<string>();
            if (!binwidths.IsFinite() || !(binwidths > 0)) errors.Add($"The requested bin width: {binwidths} is not valid because it is not a positive finite value.");
            if (max - min < binwidths) errors.Add($"The requested bin width: {binwidths.Print()} is invalid because it is great than the requested {typeof(Histograms.Histogram)} range: [{min.Print()}, {max.Print()}).");
            if (!(min.IsFinite() && max.IsFinite() && ValidationExtensions.IsRange(min, max, true, true))) errors.Add($"The requested range: [{min.Print()}, {max.Print()}) is invalid.");
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
        private static string Parameters() => $"(1) one or more binned observations in (2) one or more valid bins on (3) a logical finite range.";
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
