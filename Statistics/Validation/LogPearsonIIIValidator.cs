using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Statistics.Distributions;

namespace Statistics.Validation
{
    internal class LogPearsonIIIValidator: IValidator<LogPearsonIII>
    {
        internal LogPearsonIIIValidator()
        {
        }

        public bool IsValid(LogPearsonIII obj, out IEnumerable<Utilities.IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max() < IMessageLevels.Error;
        }
        public IEnumerable<IMessage> ReportErrors(LogPearsonIII obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj), "The log Pearson III distribution could not be validated because it is null.");
            if (!(obj.SampleSize > 0)) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"{Resources.InvalidParameterizationNotice(obj.Print(true))} {obj.Requirements(false)} {Resources.SampleSizeSuggestion()}."));
            if (!obj.Range.IsFinite()) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"{Resources.NonFiniteRangeNotice(obj)}"));
            return msgs;
        }
        internal static bool IsConstructable(double mean, double sd, double skew, int n, out string error)
        {
            error = ReportFatalError(mean, sd, skew, n);
            return !(error.Length == 0);
        }
        internal static string ReportFatalError(double mean, double sd, double skew, int n)
        {
            string msg = "";
            if (!mean.IsFinite() || !sd.IsFinite() || !skew.IsFinite()) msg += $"{Resources.FatalParameterizationNotice(LogPearsonIII.Print(mean, sd, skew, n))} {LogPearsonIII.RequiredParameterization(true)} {Resources.SampleSizeSuggestion()}";
            return msg;
        }
    }
}
