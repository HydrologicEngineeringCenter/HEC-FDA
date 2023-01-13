using HEC.MVVMFramework.Base.Enumerations;
using System;

namespace HEC.MVVMFramework.Base.Interfaces
{
    public interface IRule
    {
        ErrorLevel ErrorLevel { get; }
        Func<bool> Expression { get; }
        string Message { get; }
        IErrorMessage ErrorMessage { get; }
    }
}
