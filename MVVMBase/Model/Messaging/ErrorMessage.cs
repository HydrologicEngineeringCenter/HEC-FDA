using System;
using Base.Enumerations;

namespace Model.Messaging
{
    public class ErrorMessage : Base.Interfaces.IErrorMessage
    {
        private readonly ErrorLevel _errorLevel;
        private readonly string _message;
        public ErrorLevel ErrorLevel
        {
            get
            {
                return _errorLevel;
            }
        }
        public string Message
        {
            get
            {
                return _message;
            }
        }
        public ErrorMessage(string message, ErrorLevel errorLevel)
        {
            _message = message;
            _errorLevel = errorLevel;
        }
        public override string ToString()
        {
            return string.Format("Error Level: {0} Error: {1}\n", ErrorLevel, Message);
        }
    }
}
