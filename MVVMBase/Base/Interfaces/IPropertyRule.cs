

using System.Collections.Generic;

namespace Base.Interfaces
{
    public interface IPropertyRule
    {
        List<IRule> Rules { get; }
        IEnumerable<string> Errors { get; }
        Enumerations.ErrorLevel ErrorLevel { get; }
        void AddRule(IRule rule);
        void Update();
    }
}
