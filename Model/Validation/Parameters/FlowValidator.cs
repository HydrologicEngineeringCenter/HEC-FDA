using System;
using System.Collections.Generic;
using System.Text;
using Model.Parameters.Flows;
using Utilities;

namespace Model.Validation.Parameters
{
    internal class FlowValidator : IValidator<Flow>
    {
        public IMessageLevels IsValid(Flow obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }

        public IEnumerable<IMessage> ReportErrors(Flow obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj));
            else
            {
                if (!obj.ParameterType.IsFlow()) 
                    msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, 
                        $"A flow parameter was expected but a {obj.ParameterType.Print()} parameter was found causing an error."));
                if (!obj.Units.IsFlow()) 
                    msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, 
                        $"The specified units: {obj.Units.Print()} are not a valid measurement of flow. " +
                        $"The default unit of measurement for this {obj.ParameterType.Print()} parameter are: {obj.ParameterType.UnitsDefault().Print()}."));
                if (obj._RangeDefaultUnits.Min < 0 || obj._RangeDefaultUnits.Max > 600000000) 
                    msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, 
                        $"The {obj.ParameterType.Print()} parameter contains flow values on the range: {obj.Range.Print(true)} {obj.Units.Print(true)}, " +
                        $"this is outside the allowable range of: [0, {UnitsUtilities.ConvertFlows(600000000, obj.ParameterType.UnitsDefault(), obj.Units).Print()}] {obj.Units.Print()}.",
                        $"The allowable range of: [0, {UnitsUtilities.ConvertFlows(600000000, obj.ParameterType.UnitsDefault(), obj.Units).Print()}] {obj.Units.Print()} corresponds with " +
                        $"the minimum possible river flow and the estimated peak flow during the Missoula Flood (triggered by the failure of an ice dam), which occurred in modern day Washington and Oregon states approximately 15 - 25 thousand years ago."));
            }
            return msgs;
        }
    }
}
