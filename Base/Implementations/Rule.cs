using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Interfaces;
using System;

namespace HEC.MVVMFramework.Base.Implementations
{
    public class Rule : IRule
    {
        private readonly Func<bool> _expression;
        private readonly string _message;
        private readonly ErrorLevel _errorLevel;
        public Func<bool> Expression
        {
            get
            {
                return _expression;
            }
        }
        public string Message
        {
            get
            {
                return _message;
            }
        }
        public ErrorLevel ErrorLevel
        {
            get { return _errorLevel; }
        }

        public IErrorMessage ErrorMessage => new ErrorMessage(_message, _errorLevel);

        public Rule(Func<bool> expr, string msg) : this(expr, msg, ErrorLevel.Info)
        {
        }
        public Rule(Func<bool> expr, string msg, ErrorLevel errorLevel)
        {
            _expression = expr;
            _message = msg;
            _errorLevel = errorLevel;
        }
    }
}
