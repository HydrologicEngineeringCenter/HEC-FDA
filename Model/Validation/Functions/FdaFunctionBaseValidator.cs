using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Validation.Functions
{
    internal sealed class FdaFunctionBaseValidator: IValidator<IFdaFunction>
    {
        public IMessageLevels IsValid(IFdaFunction obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }
        public IEnumerable<IMessage> ReportErrors(IFdaFunction obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.XSeries.State > IMessageLevels.NoErrors) msgs.Add(IMessageFactory.Factory(obj.XSeries.State,
                $"The function domain contains {obj.XSeries.State.ToString()}s. Check details for more information.",
                $"The {obj.XSeries.Label} contains the following validation messages: \r\n{obj.XSeries.Messages.PrintTabbedListOfMessages()}"));
            if (obj.YSeries.State > IMessageLevels.NoErrors) msgs.Add(IMessageFactory.Factory(obj.YSeries.State,
                $"The function range contains {obj.YSeries.State.ToString()}s. Check  details for more information.",
                $"The {obj.YSeries.Label} contains the following validation messages: \r\n{obj.YSeries.Messages.PrintTabbedListOfMessages()}"));
            return msgs;
        }
    }
}
