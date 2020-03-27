using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Statistics.Histograms;
using Utilities;

namespace Statistics.Validation
{
    internal class DataValidator : IValidator<Data>
    {
        public IMessageLevels IsValid(Data entity, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(entity);
            return msgs.Max();
        }
        public IEnumerable<IMessage> ReportErrors(Data entity)
        {
            var msgs = new List<IMessage>();
            if (entity.IsNull()) throw new ArgumentNullException(nameof(entity), "The data object could not be validated because it is null.");
            if (entity.Elements.IsNullOrEmpty()) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The provided data is invalid because it contains 0 finite numeric elements."));
            if (!entity.InvalidElements.IsNullOrEmpty())
            {
                int nan = 0, negInfinity = 0, posInfinity = 0;
                foreach(double x in entity.InvalidElements)
                {
                    if (double.IsNaN(x)) nan++;
                    if (double.IsNegativeInfinity(x)) negInfinity++;
                    if (double.IsPositiveInfinity(x)) posInfinity++; 
                }
                msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The provided data contained: {nan} double.NaN, {negInfinity} double.NegativeInfinity and {posInfinity} double.PositiveInfinity discarded data elements."));
            }
            return msgs;
        }
        public static bool IsConstructable(IEnumerable<double> data, out string msg)
        {
            msg = "";
            if (data.IsNullOrEmpty()) msg += $"The provided data is null or empty.";
            return msg.Length == 0;
        }
    }
}
