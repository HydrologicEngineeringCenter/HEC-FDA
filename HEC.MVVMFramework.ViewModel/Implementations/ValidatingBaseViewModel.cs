﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.ViewModel.Enumerations;
using HEC.MVVMFramework.ViewModel.Events;
using HEC.MVVMFramework.ViewModel.Interfaces;

namespace HEC.MVVMFramework.ViewModel.Implementations
{
    public class ValidatingBaseViewModel : BaseViewModel, IValidate, INavigate
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public event NavigationEventHandler NavigationEvent;
        private Dictionary<string, IPropertyRule> _RuleMap = new Dictionary<string, IPropertyRule>();
        private List<IErrorMessage> _Errors;
        private NamedAction _ErrorsAction;
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
            set { _ErrorsAction = (NamedAction)value; }
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
        public ValidatingBaseViewModel()
        {
            ErrorsAction = new NamedAction() { Action = ShowErrors, IsEnabled = HasErrors };
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
        private void ShowErrors(object arg1, EventArgs arg2)
        {
            if (HasErrors)
            {
                ErrorReportViewModel ervm = new ErrorReportViewModel();
                Navigate(this, new NavigationEventArgs(ervm, NavigationOptionsEnum.AsNewWindow, "Errors Exist"));
                ervm.SetErrors(_RuleMap);
            }

        }
        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName == null)
            {
                return GetErrors();
            }
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
            if (_Errors == null)
            {
                Validate();
            }
            return _Errors;
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
                    NotifyPropertyChanged(nameof(HasErrors));
                    if (ErrorsAction is IDisplayToUI) { ((IDisplayToUI)ErrorsAction).IsEnabled = HasErrors; }
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
                    NotifyPropertyChanged(nameof(HasErrors));//errors no longer exist
                    if (ErrorsAction is IDisplayToUI) { ((IDisplayToUI)ErrorsAction).IsEnabled = HasErrors; }
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
                if ((currState ^ prevState) > 0) { NotifyPropertyChanged(nameof(HasErrors)); if (ErrorsAction is IDisplayToUI) { ((IDisplayToUI)ErrorsAction).IsEnabled = HasErrors; } }
            }
        }
        protected override void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (_RuleMap.ContainsKey(propertyName)) { Validate(propertyName); }
            base.NotifyPropertyChanged(propertyName);
        }

        public void Navigate(object sender, NavigationEventArgs e)
        {
            NavigationEvent?.Invoke(sender, e);
        }
    }
}
