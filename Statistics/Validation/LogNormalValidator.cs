using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Statistics.Validation
{
    internal class LogNormalValidator: IValidator<Distributions.LogNormal>
    {
        internal LogNormalValidator()
        {
        }

        public IMessageLevels IsValid(Distributions.LogNormal entity, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(entity);
            return msgs.Max();
        }
        public IEnumerable<IMessage> ReportErrors(Distributions.LogNormal obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj), "The normal distribution could not be validated because it is null.");
            if (!(obj.SampleSize > 0)) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"{Resources.InvalidParameterizationNotice(obj.Print(true))} {obj.Requirements(false)} {Resources.SampleSizeSuggestion()}."));
            if (!obj.Range.IsFinite()) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"{Resources.NonFiniteRangeNotice(obj)}"));
            else msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The normal distribution has been restricted to the finite range: {obj.Range.Print(true)} which spans the probability range: {obj._ProbabilityRange.Print(true)}."));
            return msgs;
        }

        internal static bool IsConstructable(double mean, double sd, int n, out string msg)
        {
            msg = ReportFatalErrors(mean, sd, n);
            return !msg.Any();
        }
        private static string ReportFatalErrors(double mean, double sd, int n)
        {
            string msg = "";
            if (!mean.IsFinite() || !sd.IsFinite() || (sd < 0)) msg += $"{Resources.FatalParameterizationNotice(LogNormal.Print(mean, sd, n))} {LogNormal.RequiredParameterization(true)} {Resources.SampleSizeSuggestion()}.";
            return msg;
        }
    }
}
