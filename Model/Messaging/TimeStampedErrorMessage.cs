using HEC.MVVMFramework.Base.Enumerations;
using System;

namespace Model.Messaging
{
    public class TimeStampedErrorMessage: ErrorMessage
    {
        protected readonly DateTime _DateTime;
        public TimeStampedErrorMessage(string message, ErrorLevel errorLevel): base(message,errorLevel)
        {
            _DateTime = DateTime.Now;
        }
        public override string ToString()
        {
            return string.Format("Error Level: {0} at {1} Error: {2}\n", ErrorLevel, _DateTime.ToString(), Message);
        }
    }
}
