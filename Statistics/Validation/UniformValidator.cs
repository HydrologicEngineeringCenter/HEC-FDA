using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Statistics.Distributions;

namespace Statistics.Validation
{
    internal class UniformValidator: IValidator<Uniform>
    {
        public UniformValidator()
        {
        }
        
        public IMessageLevels IsValid(Uniform obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }
        public IEnumerable<IMessage> ReportErrors(Uniform obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj), "The uniform distribution cannot be validated because it is null.");
            if (obj.SampleSize < 1) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"{Resources.InvalidParameterizationNotice(obj.Print(true))} {obj.Requirements(false)} {Resources.SampleSizeSuggestion()}"));
            return msgs;
        }

        internal static bool IsConstructable(IRange<double> range, out string msg)
        {
            msg = ReportFatalError(range);
            return !(msg.Length == 0);
        }
        private static string ReportFatalError(IRange<double> range)
        {
            string msg = "";
            if (range.IsNull()) msg += "The uniform distribution cannot be constructed because it range is null.";
            if (range.Messages.Max() > IMessageLevels.Message) msg += $"{Resources.FatalParameterizationNotice(Uniform.Print(range))} {Uniform.RequiredParameterization(true)} {Resources.SampleSizeSuggestion()}";
            return msg;
        }

    }
}
