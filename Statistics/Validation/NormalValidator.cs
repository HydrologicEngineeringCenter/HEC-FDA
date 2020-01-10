using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Utilities;

namespace Statistics.Validation
{
    internal class NormalValidator: IValidator<Distributions.Normal>
    {
        internal NormalValidator()
        {
        }

        public bool IsValid(Distributions.Normal entity, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(entity);
            return !msgs.Any();
        }
        public IEnumerable<IMessage> ReportErrors(Distributions.Normal entity)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (!entity.Range.Min.IsFinite() || !entity.Range.Max.IsFinite()) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The specified distribution, {entity.Print()} is invalid because it generates an infinite or non-numeric range of values: {entity.Range.Print()}. To repair this error re-paramaterize the distribution or truncate the infinitate values as the maximum and minimum finite values: [{double.MinValue}, {double.MaxValue}]."));
            return msgs;
        }
        
        internal static bool IsConstructable(double mean, double sd, int n, out string msg)
        {
            msg = ReportFatalErrors(mean, sd, n);
            return !msg.Any();
        }
        private static string ReportFatalErrors(double mean, double sd, int n)
        {
            if (!mean.IsFinite() || !sd.IsFinite()) return $"The requested distribution, {PrintSpecifiedParameterization(mean, sd, n)} could not be constructed because one or more of its parameters are invalid. The specified mean: {mean} and standard deviation: {sd} must be represented by a finite, numeric values and the sample size: {n} must be represented by a positive integer. If the distribution does not describe a sample the sample size parameter can be left blank and the maximum integer value {int.MaxValue} will be assigned by default.";
            else return "";                       
        }
        private static string PrintSpecifiedParameterization(double mean, double sd, int n)
        {
            return $"Normal(mean: {mean}, sd: {sd}, sample size: {n})";
        }
    }
}
