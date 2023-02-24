using HEC.MVVMFramework.Base.Enumerations;
using System.Linq;

namespace HEC.MVVMFramework.Model.Messaging
{
    public class DebuggingErrorMessage : TimeStampedErrorMessage
    {
        private readonly string _callingMethod;
        private readonly string _callingClass;
        private readonly int _callingLine;
        public DebuggingErrorMessage(string message, ErrorLevel errorLevel, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string filePath = "", [System.Runtime.CompilerServices.CallerLineNumber] int lineNo = 0) : base(message, errorLevel)
        {
            _callingMethod = memberName;
            _callingClass = filePath.Split(new char[] { '\\' }).Last();
            _callingLine = lineNo;
        }
        public override string ToString()
        {
            return string.Format("Error Level: {0} at {1} Error: {2}\n\tCalling Class: {3}\n\tCalling Method: {4}\n\tError Line: {5}\n", ErrorLevel, _DateTime, Message, _callingClass, _callingMethod, _callingLine);
        }

    }
}
