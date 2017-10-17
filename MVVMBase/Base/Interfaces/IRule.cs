using System;

namespace Base.Interfaces
{
    public interface IRule
    {
        Base.Enumerations.ErrorLevel ErrorLevel { get; }
        Func<bool> Expression { get; }
        string Message { get; }
    }
}
