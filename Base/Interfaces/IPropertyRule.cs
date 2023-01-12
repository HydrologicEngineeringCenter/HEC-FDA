using HEC.MVVMFramework.Base.Enumerations;
using System.Collections.Generic;

namespace HEC.MVVMFramework.Base.Interfaces
{
    public interface IPropertyRule
    {
        List<IRule> Rules { get; }
        IEnumerable<string> Errors { get; }
        List<IErrorMessage> ErrorMessages { get; }
        ErrorLevel ErrorLevel { get; }
        void AddRule(IRule rule);
        void Update();
    }
}
