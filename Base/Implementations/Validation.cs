using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HEC.MVVMFramework.Base.Implementations
{
    public class Validation : IValidate
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private Dictionary<string, IPropertyRule> _RuleMap = new Dictionary<string, IPropertyRule>();
        private List<IErrorMessage> _Errors = new List<IErrorMessage>();
        private INamedAction _ErrorsAction;
        private ErrorLevel _errorLevel;
        public bool HasErrors
        {
            get
            {
                return _errorLevel > ErrorLevel.Unassigned;
            }
        }
        public INamedAction ErrorsAction
        {
            get { return _ErrorsAction; }
            set { _ErrorsAction = value; }
        }
        public Dictionary<string, IPropertyRule> RuleMap
        {
            get
            {
                return _RuleMap;
            }
        }
        public ErrorLevel ErrorLevel
        {
            get { return _errorLevel; }
        }

        public void AddMultiPropertyRule(List<string> properties, IRule rule)
        {
            foreach (string prop in properties)
            {
                if (prop.Equals(nameof(HasErrors))) continue;
                if (prop.Equals(nameof(ErrorLevel))) continue;
                if (_RuleMap.ContainsKey(prop))
                {
                    _RuleMap[prop].AddRule(rule);
                }
                else
                {
                    PropertyRule r = new PropertyRule(rule);
                    _RuleMap.Add(prop, r);
                }
            }
        }
        public void AddSinglePropertyRule(string property, IRule rule)
        {
            if (property.Equals(nameof(HasErrors))) return;
            if (property.Equals(nameof(ErrorLevel))) return;
            if (_RuleMap.ContainsKey(property))
            {
                _RuleMap[property].AddRule(rule);
            }
            else
            {
                PropertyRule r = new PropertyRule(rule);
                _RuleMap.Add(property, r);
            }
        }
        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName == null) return null;
            if (_RuleMap.ContainsKey(propertyName))
            {
                if (_RuleMap[propertyName].ErrorLevel > ErrorLevel.Unassigned)
                {
                    return _RuleMap[propertyName].Errors;
                }
            }
            return null;
        }
        public IEnumerable GetErrors()
        {
            Validate();
            return _Errors;
        }

        /// <summary>
        /// To be used in property rules where we want a single string message that contains all the errors.
        /// </summary>
        /// <returns></returns>
        public String GetErrorMessages()
        {
            StringBuilder errorsBuilder = new StringBuilder();
            foreach (IErrorMessage err in _Errors)
            {
                errorsBuilder.AppendLine(string.Format("Error Level: {0} Error: {1}", ErrorLevel, err.Message));
            }
            return errorsBuilder.ToString();
        }

        public void Validate()
        {
            _Errors = new List<IErrorMessage>();
            ErrorLevel prevErrorState = _errorLevel;
            _errorLevel = ErrorLevel.Unassigned;
            foreach (string s in _RuleMap.Keys)
            {
                _RuleMap[s].Update();
                if (_RuleMap[s].ErrorLevel > ErrorLevel.Unassigned)
                {
                    if (_errorLevel > ErrorLevel.Unassigned)
                    {
                        _errorLevel = _RuleMap[s].ErrorLevel;
                    }
                    else
                    {
                        _errorLevel = _errorLevel | _RuleMap[s].ErrorLevel;
                    }
                    _Errors.AddRange(_RuleMap[s].Errors);
                }
            }
            if (_errorLevel > ErrorLevel.Unassigned)
            {
                //errors exist
                if (_errorLevel != prevErrorState)
                {
                    ErrorLevel changed = _errorLevel ^ prevErrorState;
                    //NotifyPropertyChanged(nameof(HasErrors));
                    foreach (ErrorLevel e in Enum.GetValues(typeof(ErrorLevel)))
                    {
                        if ((changed & e) > 0)
                        {
                            if ((_errorLevel & e) > 0)
                            {
                                //turned on
                            }
                            else
                            {
                                //turned off
                            }
                        }
                    }
                }
            }
            else
            {
                //no errors exist
                if (_errorLevel != prevErrorState)
                {
                    //NotifyPropertyChanged(nameof(HasErrors));//errors no longer exist
                }
            }
        }
        public void Validate(string propertyName)
        {
            ErrorLevel prevState = _RuleMap[propertyName].ErrorLevel;
            _RuleMap[propertyName].Update();
            if (prevState != _RuleMap[propertyName].ErrorLevel)
            {
                //a change occured.

                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                ErrorLevel currState = _RuleMap[propertyName].ErrorLevel;

                //check if it is the biggest one.
                bool biggerErrorsExist = false;
                foreach (string s in _RuleMap.Keys)
                {
                    if (_RuleMap[s].ErrorLevel > currState)
                    {
                        biggerErrorsExist = true;
                    }
                }
                if (!biggerErrorsExist)
                {
                    if (currState == ErrorLevel.Unassigned) _errorLevel = currState;
                    if (_errorLevel > ErrorLevel.Unassigned)
                    {
                        _errorLevel = _errorLevel | currState;
                    }
                    else
                    {
                        _errorLevel = currState;
                    }
                }
                if ((currState ^ prevState) > 0)
                {
                    //NotifyPropertyChanged(nameof(HasErrors)); 
                }
            }
        }
    }
}
