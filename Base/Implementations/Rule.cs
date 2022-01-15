using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Implementations
{
    public class Rule : Base.Interfaces.IRule
    {
        private readonly Func<bool> _expression;
        private readonly string _message;
        private readonly Base.Enumerations.ErrorLevel _errorLevel;
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
        public Base.Enumerations.ErrorLevel ErrorLevel
        {
            get { return _errorLevel; }
        }
        public Rule(Func<bool> expr, string msg) : this(expr, msg, Base.Enumerations.ErrorLevel.Info)
        {
        }
        public Rule(Func<bool> expr, string msg, Base.Enumerations.ErrorLevel errorLevel)
        {
            _expression = expr;
            _message = msg;
            _errorLevel = errorLevel;
        }
    }
}
