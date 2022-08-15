using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HEC.FDA.ViewModel
{
    public delegate void RequestNavigationHandler( IDynamicTab tab, bool newWindow, bool asDialog);

    /// <summary>
    /// The base class for all view model classes. Contains methods that are common among all view model classes
    /// such as validation, navigation, adding rules.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Notes
        #endregion
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event RequestNavigationHandler RequestNavigation;

        #endregion
        #region Fields
        private List<BaseViewModel> _Children = new List<BaseViewModel>();

        /// <summary>
        /// This is a dictionary of property names, and any rules that go with that property.
        /// </summary>
        private Dictionary<string, PropertyRule> ruleMap = new Dictionary<string, PropertyRule>();
        private bool _HasChanges;
        private string _Error;

        #endregion
        #region Properties
        /// <summary>
        /// The StudyCache holds all the elements used in FDA. You can use this to get any of them 
        /// as well as listen for events where elements are added, removed, or updated
        /// </summary>
        public static IStudyCache StudyCache { get; set; }

        /// <summary>
        /// Required to implement IDataErrorInfo interface
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get
            {
                if (ruleMap.ContainsKey(columnName))
                {
                    ruleMap[columnName].Update();
                    return ruleMap[columnName].Error;
                }
                return null;
            }
        }
        /// <summary>
        /// WPF seems to not use the Error call, theoretically it is used to invalidate an object.
        /// This is required to implement IDataErrorInfo interface.
        /// 
        /// This is used to display a tooltip message on the "Save" button in the SaveCloseControl.
        /// </summary>
        public string Error
        {
            get { return _Error; }
            set { _Error = value; NotifyPropertyChanged(); }
        }

        public string ValidationErrorMessage { get; set; }

        private bool _HasFatalError;
        public bool HasFatalError
        {
            get { return _HasFatalError; }
            set { _HasFatalError = value; NotifyPropertyChanged(); }
        }
        /// <summary>
        /// Primarily used to determine if a class needs to save. Gets set when the notify property change fires.
        /// It is up to the save method to turn this back to false. 
        /// </summary>
        public bool HasChanges
        {
            get { return _HasChanges; }
            set 
            { 
                _HasChanges = value;
                if (!_HasChanges)
                {
                    foreach(BaseViewModel vm in _Children)
                    {
                        if(vm.HasChanges)
                        {
                            vm.HasChanges = false;
                        }
                    }
                }
                NotifyPropertyChanged(); 
            }
        }
        public bool WasCanceled { get; set; }

        #endregion
        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public BaseViewModel()
        {
            AddValidationRules();
        }
        #endregion
        #region Voids     


        /// <summary>
        /// Loops over and evaluates the property rules.
        /// </summary>
        public void Validate()
        {
            HasFatalError = false;
            List<string> errors = new List<string>();
            Error = "";
            foreach (PropertyRule pr in ruleMap.Values)
            {
                pr.Update();
                if (pr.HasError)
                {
                    if (pr.HasFatalError == true)
                    {
                        HasFatalError = true;
                    }
                    errors.Add(pr.Error);
                }
            }
            //If this VM doesn't have a fatal error, it is possible that a child VM has fatal errors. We want to 
            //bubble that up to this VM.
            if (!HasFatalError)
            {
                bool hasFatalError = false;
                foreach (BaseViewModel baseVM in _Children)
                {
                    if (baseVM.HasFatalError)
                    {
                        hasFatalError = true;
                        break;
                    }
                }
                HasFatalError = hasFatalError;
            }

            //handle the errors tooltip
            //we have already added the errors for this vm but there might be errors
            //to report from the child vms
            foreach (BaseViewModel baseVM in _Children)
            {
                if (baseVM.Error != null && baseVM.Error.Length > 0)
                {
                    errors.Add(baseVM.Error);
                }
            }

            //this is used to display the tooltip on the OK and SAVE buttons. Removing the last newline chars
            Error = string.Join(Environment.NewLine, errors);
        }

        protected void RegisterChildViewModel(BaseViewModel vm)
        {
            if (!_Children.Contains(vm))
            {
                vm.PropertyChanged += ChildChanged;
                _Children.Add(vm);
            }
        }

        private void ChildChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is BaseViewModel vm)
            {
                if (e.PropertyName.Equals(nameof(HasChanges)))
                {
                    HasChanges |= vm.HasChanges;
                }
                else if (e.PropertyName.Equals(nameof(HasFatalError)))
                {             
                    if (vm.HasFatalError)
                    {
                        HasFatalError = true;
                        
                    }
                    else
                    {
                        Validate();
                    }
                }
                else if(e.PropertyName.Equals(nameof(Error)))
                {
                    if(vm.Error != "")
                    {
                        Error += Environment.NewLine + vm.Error;
                        Error = Error.Trim();
                    }
                }
            }
        }

        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {

            //todo: I don't like excluding properties like this, but if the validate is going to update
            //properties, then you will get an infinite loop if you don't exclude them.
            if (propertyName.Equals(nameof(HasFatalError))
                || propertyName.Equals(nameof(Error))
                || propertyName.Equals(nameof(HasChanges))
                || propertyName.Equals("MessageRows")
                || propertyName.Equals("MessageCount")
                || propertyName.Equals("SaveStatusLevel")
                || propertyName.Equals("IsExpanded")
                || propertyName.Equals("SaveStatusVisible"))
            {
                //don't validate
            }
            else
            {
                HasChanges = true;
                Validate();
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Recursively goes up the tree structure to WindowVM.CurrentView_RequestNavigation(). Takes the vm and finds its matching view and displays it.
        /// Launches either a new window or a new tab in the main window.
        /// </summary>
        /// <param name="vm">The view model that you want to display</param>
        /// <param name="newWindow">True: displays the view in a new window. False: displays the view as a new tab in the main window.</param>
        /// <param name="asDialog">If newWindow is true, this determines if it is modal or not</param>
        /// <param name="title">The title for the tab or window.</param>
        public void Navigate( IDynamicTab tab, bool newWindow = true, bool asDialog = true)
        {
            if (RequestNavigation != null)
            {
                tab.BaseVM.WasCanceled = true;
                RequestNavigation( tab, newWindow, asDialog);
            }     
        }

        /// <summary>
        /// Adds a rule to a property in this VM. If that property changes, all rules attached to that property will get
        /// analyzed. 
        /// </summary>
        /// <param name="PropertyName">Name of the property to which this rule applies</param>
        /// <param name="ruleDelegate">The rule for the property</param>
        /// <param name="errorMessage">The tooltip message that will get displayed if in error</param>
        /// <param name="isFatalRule">True: Will disable the OK button and not let the user proceed. False: Will show as an error but will let the user click OK</param>
        public void AddRule(string PropertyName, Func<bool> ruleDelegate, string errorMessage, bool isFatalRule = true)
        {
            if (ruleMap.ContainsKey(PropertyName))
            {
                ruleMap[PropertyName].AddRule(ruleDelegate, errorMessage, isFatalRule);
            }
            else
            {
                ruleMap.Add(PropertyName, new PropertyRule(ruleDelegate, errorMessage, isFatalRule));
            }
        }


        #endregion

        public virtual void AddValidationRules() { }

        /// <summary>
        /// When a tab or a window is closing it will first check to see if it is ok to close
        /// by calling this method. This is a chance to warn the user that they have unsaved data
        /// or anything like that before actually closing the form.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsOkToClose()
        {
            return true;
        }

        /// <summary>
        /// Each view model class can have "rules" that are used to validate the class.
        /// The rules are attached to properties. When the property changes then each rule
        /// is evaluated for that property.
        /// </summary>
        protected class PropertyRule
        {
            private List<Rule> _rules = new List<Rule>();
            /// <summary>
            /// If a rule is in error then there will be some visual indication (turn red) and
            /// a tooltip message will be displayed over the control
            /// </summary>
            internal bool HasError { get; set; }
            /// <summary>
            /// If a rule is broken and it is a fatal error then the user will not be allowed to 
            /// continue by clicking the OK button.
            /// </summary>
            internal bool HasFatalError { get; set; }
            //internal bool IsDirty { get; set; }
            internal string Error { get; set; }
            internal PropertyRule(Func<bool> rule, string errormessage, bool isFatalRule = true)
            {
                _rules.Add(new Rule(rule, errormessage, isFatalRule));
            }
            internal void AddRule(Func<bool> rule, string errormessage, bool isFatalRule = true)
            {
                _rules.Add(new Rule(rule, errormessage, isFatalRule));
            }
            internal void Update()
            {
                Error = null;
                HasError = false;
                HasFatalError = false;
                try
                {
                    foreach (Rule r in _rules)
                    {
                        if (!r.expression())
                        {
                            Error += r.message + "\n";
                            HasError = true;
                            if (r.FatalIfInvalid == true)
                            {
                                HasFatalError = true;
                            }
                        }
                    }
                    if (HasError)
                    {
                        Error = Error.TrimEnd(new Char[] { '\n' });
                    }
                }
                catch (Exception e)
                {
                    Error = e.Message;
                    HasError = true;
                }
            }
            private class Rule
            {
                public readonly Func<bool> expression;
                public readonly string message;
                public readonly bool _FatalIfInvalid;

                public bool FatalIfInvalid
                {
                    get { return _FatalIfInvalid; }
                }


                internal Rule(Func<bool> expr, string msg, bool fatalIfInvalid = true)
                {
                    _FatalIfInvalid = fatalIfInvalid;
                    expression = expr;
                    message = msg;
                }
            }
        }


    }
}
