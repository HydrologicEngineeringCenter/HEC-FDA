using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Base.Interfaces;

namespace ViewModel.Implementations
{
    public class ValidatingBaseViewModel: Implementations.BaseViewModel, Base.Interfaces.IValidate
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private Dictionary<string, IPropertyRule> _RuleMap = new Dictionary<string, IPropertyRule>();
        private List<string> _Errors;
        private NamedAction _ErrorsAction;
        private Base.Enumerations.ErrorLevel _errorLevel;
        public bool HasErrors
        {
            get
            {
                return _errorLevel > Base.Enumerations.ErrorLevel.Unassigned;
            }
        }
        public Base.Interfaces.INamedAction ErrorsAction
        {
            get { return _ErrorsAction; }
            set { _ErrorsAction = (NamedAction)value; }
        }
        public Dictionary<string, IPropertyRule> RuleMap
        {
            get
            {
                return _RuleMap;
            }
        }
        public Base.Enumerations.ErrorLevel ErrorLevel
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
                    Validation.PropertyRule r = new Validation.PropertyRule(rule);
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
                Validation.PropertyRule r = new Validation.PropertyRule(rule);
                _RuleMap.Add(property, r);
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (_RuleMap.ContainsKey(propertyName))
            {
                if (_RuleMap[propertyName].ErrorLevel > Base.Enumerations.ErrorLevel.Unassigned)
                {
                    return _RuleMap[propertyName].Errors;
                }
            }
            return null;
        }
        public IEnumerable GetErrors()
        {
            if (_Errors == null)
            {
                if (HasErrors)
                {//validate hasnt been called, but there are errors (due to single property validation), update and validate all properties, and return the master list of errors.
                    Validate();
                }
                return new List<string>();
            }
            return _Errors;
        }
        public void Validate()
        {
            _Errors = new List<string>();
            Base.Enumerations.ErrorLevel prevErrorState = _errorLevel;
            _errorLevel = Base.Enumerations.ErrorLevel.Unassigned;
            foreach (string s in _RuleMap.Keys)
            {
                _RuleMap[s].Update();
                if (_RuleMap[s].ErrorLevel > Base.Enumerations.ErrorLevel.Unassigned)
                {
                    if (_errorLevel > Base.Enumerations.ErrorLevel.Unassigned)
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
            if (_errorLevel > Base.Enumerations.ErrorLevel.Unassigned)
            {
                //errors exist
                if (_errorLevel != prevErrorState)
                {
                    Base.Enumerations.ErrorLevel changed = _errorLevel ^ prevErrorState;
                    foreach (Base.Enumerations.ErrorLevel e in Enum.GetValues(typeof(Base.Enumerations.ErrorLevel)))
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
                    NotifyPropertyChanged(nameof(HasErrors));//errors no longer exist
                }
            }
        }
        public void Validate(string propertyName)
        {
            Base.Enumerations.ErrorLevel prevState = _RuleMap[propertyName].ErrorLevel;
            _RuleMap[propertyName].Update();
            if (prevState != _RuleMap[propertyName].ErrorLevel)
            {
                //a change occured.

                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                Base.Enumerations.ErrorLevel currState = _RuleMap[propertyName].ErrorLevel;

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
                    if (currState == Base.Enumerations.ErrorLevel.Unassigned) _errorLevel = currState;
                    if (_errorLevel > Base.Enumerations.ErrorLevel.Unassigned)
                    {
                        _errorLevel = _errorLevel | currState;
                    }
                    else
                    {
                        _errorLevel = currState;
                    }
                }
                if ((currState ^ prevState) > 0) { NotifyPropertyChanged(nameof(HasErrors)); }
            }
        }
        protected override void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName]string propertyName = "")
        {
            if (_RuleMap.ContainsKey(propertyName)) { Validate(propertyName); }
            base.NotifyPropertyChanged(propertyName);
        }
    }
}
