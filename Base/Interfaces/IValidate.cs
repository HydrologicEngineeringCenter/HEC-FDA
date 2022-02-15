using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Interfaces;
using System.Collections.Generic;

namespace Base.Interfaces
{
    public interface IValidate : System.ComponentModel.INotifyDataErrorInfo
    {
        Dictionary<string,IPropertyRule> RuleMap { get; }
        ErrorLevel ErrorLevel { get; }
        INamedAction ErrorsAction { get; }
        void AddMultiPropertyRule(List<string> properties, IRule rule);
        void AddSinglePropertyRule(string property, IRule rule);
        /// <summary>
        /// Validate a single property
        /// </summary>
        /// <param name="propertyName">the property wished to be validated</param>
        void Validate(string propertyName);
        /// <summary>
        /// Validate all properties at once
        /// </summary>
        void Validate();
    }
}
