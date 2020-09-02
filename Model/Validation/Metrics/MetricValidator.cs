using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Validation.Metrics
{
    internal class MetricValidator : IValidator<IMetric>
    {
        public IMessageLevels IsValid(IMetric obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }

        public IEnumerable<IMessage> ReportErrors(IMetric obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj));
            if (!obj.ParameterType.IsMetric()) msgs.Add(IMessageFactory.Factory(
                IMessageLevels.Error, 
                $"The specified parameter: {obj.ParameterType.Print(true)} is not a valid type of metric."));
            if (obj.ParameterType == IParameterEnum.EAD && obj.Ordinate.Value() != 0) msgs.Add(IMessageFactory.Factory(
                IMessageLevels.Message, 
                $"The {obj.ParameterType.Print(true)} parameter type was specified with a target value of: {obj.Ordinate.Value().Print()}.",
                $"{obj.ParameterType.Print(true)} is calculated at the area under the damage frequency curve, target values are not used in this calculation"));
            if (obj.ParameterType != IParameterEnum.EAD && obj.Ordinate.Value().IsOnRange(0, 1)) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error,
                $"The specified target annual exceedance probability: {obj.Ordinate.Value()} is invalid because it is not on the valid probability range: [0, 1]."));
            return msgs;
        }
    }
}
