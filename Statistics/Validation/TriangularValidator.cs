using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Statistics.Distributions;
using System.Linq;

namespace Statistics.Validation
{
    internal class TriangularValidator: IValidator<Triangular>
    {
        internal TriangularValidator()
        {
        }

        public IMessageLevels IsValid(Triangular obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }
        public IEnumerable<IMessage> ReportErrors(Triangular obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj), "The triangular distribution cannot be validated because it is null.");
            if (obj.Range.Min == obj.Range.Max) msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The {obj.Print(true)} minimum and maximum values are equal. This is allowed but reduces the distribution to a constant."));
            if (obj.SampleSize < 1) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"{Resources.InvalidParameterizationNotice(obj.Print(true))} {obj.Requirements(false)} {Resources.SampleSizeSuggestion()}"));
            return msgs;
        }

        internal static bool IsConstructable(double mode, IRange<double> range, out string msg)
        {
            msg = ReportFatalError(mode, range);
            return msg.Length == 0;
        }
        private static string ReportFatalError(double mode, IRange<double> range)
        {
            string msg = "";
            if (range.IsNull()) msg += "The triangular distribution cannot be constructed because it range is null.";

            bool isModeInfinite = !mode.IsFinite();
            bool doesRangeHaveErrorMessages = range.Messages.Max() > IMessageLevels.Message;
            bool isNotOnRange = !ValidationExtensions.IsOnRange(mode, range.Min, range.Max);

            if (isModeInfinite || doesRangeHaveErrorMessages || isNotOnRange)
            {
                msg += $"{Resources.FatalParameterizationNotice(Triangular.Print(mode, range))} {Triangular.RequiredParameterization(true)} {Resources.SampleSizeSuggestion()}";
            }
            return msg;
        }
    }
}
