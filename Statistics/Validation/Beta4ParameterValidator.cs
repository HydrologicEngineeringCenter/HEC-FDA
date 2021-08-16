using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Statistics.Distributions;

using Utilities;

namespace Statistics.Validation
{
    internal class Beta4ParameterValidator: IValidator<Beta4Parameters>
    {
        public IMessageLevels IsValid(Beta4Parameters entity, out IEnumerable<IMessage> errors)
        {
            errors = ReportErrors(entity);
            return errors.Max();
        } 
        public IEnumerable<IMessage> ReportErrors(Beta4Parameters obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj), "The scaled beta distribution could not be validated because it is null.");
            if (!(obj.SampleSize > 0)) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"{Resources.InvalidParameterizationNotice(obj.Print())} {obj.Requirements(false)} {Resources.SampleSizeSuggestion()}"));
            return msgs;    
        }

        internal static bool IsConstructable(double alpha, double beta, double location, double scale, int n, out string error)
        {
            error = ReportFatalErrors(alpha, beta, location, scale, n);
            return !(error.Length == 0);
        }
        private static string ReportFatalErrors(double alpha, double beta, double location, double scale, int n)
        {
            string error = "";
            if (!alpha.IsOnRange(0, double.MaxValue) || !beta.IsOnRange(0, double.MaxValue) || !scale.IsOnRange(0, double.MaxValue, false))
            {
                error += $"{Resources.FatalParameterizationNotice(Beta4Parameters.Print(alpha, beta, location, scale, n))} {Beta4Parameters.RequiredParameterization(true)} {Resources.SampleSizeSuggestion()}";
            }
            return error;
        }
    }
}
