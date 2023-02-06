using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Interfaces;
using System;

namespace HEC.MVVMFramework.Base.Implementations
{
    public class ErrorMessage : IErrorMessage
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
            _message = DateTime.Now + " " + message;
            _errorLevel = errorLevel;
        }
        public override string ToString()
        {
            return string.Format("Error Level: {0} Error: {1}\n", ErrorLevel, Message);
        }
    }
}
