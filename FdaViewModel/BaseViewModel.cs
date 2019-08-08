using FdaModel.Messaging;
using FdaViewModel.Study;
using FdaViewModel.Tabs;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FdaViewModel
{
    public delegate void RequestNavigationHandler( IDynamicTab tab, bool newWindow, bool asDialog);
    public delegate void RequestShapefilePathsHandler(ref List<string> files);
    public delegate void RequestShapefilePathsOfTypeHandler(ref List<string> files, Utilities.VectorFeatureType type);
    public delegate void RequestAddToMapWindowHandler(object sender, Utilities.AddMapFeatureEventArgs args);//needs to be capable of passing a geopackage connection??
    public delegate void RequestRemoveFromMapWindowHandler(object sender, Utilities.RemoveMapFeatureEventArgs args);//needs to be capable of passing a geopackage connection??

    /// <summary>
    /// The base class for all view model classes. Contains methods that are common among all view model classes
    /// such as validation, navigation, adding rules.
    /// </summary>
    public abstract class BaseViewModel : System.ComponentModel.INotifyPropertyChanged, System.ComponentModel.IDataErrorInfo, IDisposable, IReportMessage
    {
        #region Notes
        #endregion
        #region Events
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public event RequestNavigationHandler RequestNavigation;
        public event RequestShapefilePathsHandler RequestShapefilePaths;
        public event RequestShapefilePathsOfTypeHandler RequestShapefilePathsOfType;
        public event RequestAddToMapWindowHandler RequestAddToMapWindow;
        public event RequestRemoveFromMapWindowHandler RequestRemoveFromMapWindow;
        public event MessageReportedEventHandler MessageReport;

        public TransactionEventHandler TransactionEvent;


        #endregion
        #region Fields
        /// <summary>
        /// This is a dictionary of property names, and any rules that go with that property.
        /// </summary>
        private Dictionary<string, PropertyRule> ruleMap = new Dictionary<string, PropertyRule>();
        private bool _HasError;
        private bool _HasFatalError;
        private bool _WasCanceled;
        private bool _HasChanges;
        private List<FdaModel.Utilities.Messager.ErrorMessage> _messages;
        //private Utilities.NamedAction _MessagesAction;
        //private Utilities.NamedAction _ErrorsAction;
        //private Utilities.NamedAction _HelpAction;




        private string _Name;

        #endregion
        #region Properties
        public static IStudyCache StudyCache { get; set; }
        public static Saving.PersistenceFactory PersistenceFactory{get;set;}

        public virtual string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }

    


        public string LastEditDate { get; set; }

        
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
        /// </summary>
        public string Error
        {
            get;
            set;
        }

        public string ValidationErrorMessage { get; set; }
        //public Utilities.NamedAction MessagesAction
        //{
        //    get { return _MessagesAction; }
        //    set { _MessagesAction = value; NotifyPropertyChanged(); }
        //}
        //public Utilities.NamedAction ErrorsAction
        //{
        //    get { return _ErrorsAction; }
        //    set { _ErrorsAction = value; NotifyPropertyChanged(); }
        //}
        //public Utilities.NamedAction HelpAction
        //{
        //    get { return _HelpAction; }
        //    set { _HelpAction = value; NotifyPropertyChanged(); }
        //}
        public List<FdaModel.Utilities.Messager.ErrorMessage> Messages
        {
            get { return _messages; }
            
        }
        public bool HasError { get { return _HasError; } }
        public bool HasFatalError { get { return _HasFatalError; } }
        public bool HasChanges { get { return _HasChanges; } }
        public bool WasCanceled
        {
            get { return _WasCanceled; }
            set { _WasCanceled = value; } //NotifyPropertyChanged(nameof(WasCancled)); }    
        }

        #endregion
        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public BaseViewModel()
        {
            //_MessagesAction = new Utilities.NamedAction();
            //_ErrorsAction = new Utilities.NamedAction();
            //_HelpAction = new Utilities.NamedAction();
            MessageHub.Register(this);
            AddValidationRules();
            //Utilities.NamedAction messageAction = new Utilities.NamedAction();
            ////_MessagesAction = new Utilities.NamedAction();
            //messageAction.Header = "";
            //messageAction.IsEnabled = false;
            //messageAction.IsVisible = false;
            //messageAction.Action = DisplayMessages;
            //MessagesAction = messageAction;

            //Utilities.NamedAction errorAction = new Utilities.NamedAction();
            //errorAction.Header = "";
            //errorAction.IsEnabled = false;
            //errorAction.IsVisible = false;
            //errorAction.Action = DisplayErrors;
            //ErrorsAction = errorAction;

            //Utilities.NamedAction helpAction = new Utilities.NamedAction();
            //helpAction.Header = "";
            //helpAction.IsEnabled = true;
            //helpAction.IsVisible = true;
            //helpAction.Action = DisplayHelp;
            //HelpAction = helpAction;
        }
        #endregion
        #region Voids     

        public void AddTransaction(object sender, Utilities.Transactions.TransactionEventArgs transaction)
        {
            TransactionEvent?.Invoke(this, transaction);
        }

        //private void DisplayErrors(object arg1, EventArgs arg2)
        //{
        //    Utilities.MessageVM mvm = new Utilities.MessageVM(Error);
        //    Navigate(mvm, true, false, "Errors");

        //}
        //private void DisplayHelp(object arg1, EventArgs arg2)
        //{
        //    Utilities.MessageVM mvm = new Utilities.MessageVM("Help");
        //    Navigate(mvm, true, false, "Help for " + GetType().Name);
        //}
        //private void DisplayMessages(object arg1, EventArgs arg2)
        //{
        //    //Utilities.MessagesVM mvm = new Utilities.MessagesVM(_messages);
        //    Utilities.MessagesVM mvm = new Utilities.MessagesVM();

        //    Navigate(mvm, true, false, "Messages");
        //}


        /// <summary>
        /// Virtual method that can be overriden in any view model class to add its own validation rules. 
        /// This gets called in the constructor of BaseViewModel.
        /// </summary>
        virtual public void AddValidationRules() { }

        /// <summary>
        /// 
        /// </summary>
        public void Validate()
        {
            _HasError = false;
            _HasFatalError = false;
            NotifyPropertyChanged("HasError");
            NotifyPropertyChanged("HasFatalError");
            StringBuilder errors = new StringBuilder();
            Error = "";
            foreach (PropertyRule pr in ruleMap.Values)
            {
                pr.Update();
                if (pr.HasError)
                {
                    if (pr.HasFatalError == true)
                    {
                        _HasFatalError = true;
                        NotifyPropertyChanged("HasFatalError");
                    }
                    errors.AppendLine(pr.Error);
                    _HasError = true;
                    NotifyPropertyChanged("HasError");
                }
            }
            if (HasError)
            {
                Error = errors.ToString().Remove(errors.ToString().Length - 2);
            }
            else
            {
                Error = null;
            }
            NotifyPropertyChanged(nameof(Error));

        }
        //virtual public void Save() { }

            /// <summary>
            /// Gets called anytime a property gets changed. The main purpose of this is to call validate() 
            /// which will go over the rules and see if any are in error.
            /// </summary>
            /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            _HasChanges = true;


            if (propertyName.Equals(nameof(HasError)) || propertyName.Equals(nameof(HasFatalError)) || propertyName.Equals(nameof(Error)) || propertyName.Equals(nameof(Messages)))
            {
                //do nothing
                //if (propertyName.Equals(nameof(HasError)))
                //{
                //    if (HasError)
                //    {
                //       // _ErrorsAction.IsEnabled = true;
                //       // _ErrorsAction.IsVisible = true;

                //        //_MessagesAction.IsEnabled = true;
                //       // _MessagesAction.IsVisible = true;

                //    }else
                //    {
                //        //_ErrorsAction.IsEnabled = false;
                //       // _ErrorsAction.IsVisible = false;
                //    }
                //}
            }
            else
            {
                Validate();
                //ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("The property " + propertyName + " has changed", FdaModel.Utilities.Messager.ErrorMessageEnum.Fatal, GetType().Name));
            }
            this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
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

        /// <summary>
        /// Recursively bubbles up the tree structure to WindowVM.CurrentView_RequestNavigation(). Takes the vm and finds its matching view and displays it.
        /// Launches either a new window or a new tab in the main window.
        /// </summary>
        /// <param name="vm">The view model that you want to display</param>
        /// <param name="newWindow">True: displays the view in a new window. False: displays the view as a new tab in the main window.</param>
        /// <param name="asDialog">If newWindow is true, this determines if it is modal or not</param>
        /// <param name="title">The title for the tab or window.</param>
        public void Navigate( IDynamicTab tab, bool newWindow = true, bool asDialog = true)
        {
            string name = this.Name;
            string type = this.GetType().ToString();
            if (RequestNavigation != null)
            {
                tab.BaseVM.WasCanceled = true;
                RequestNavigation( tab, newWindow, asDialog);
            }
            else
            {
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("Navigation requested from " + this.GetType().Name + " to " + tab.BaseVM.GetType().Name + " and no handler had been assigned.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel & FdaModel.Utilities.Messager.ErrorMessageEnum.Major));
            }
        }
        public void ShapefilePaths(ref List<string> paths)
        {
            if (RequestShapefilePaths != null)
            {
                RequestShapefilePaths(ref paths);
            }
            else
            {
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("Shapefiles requested from " + this.GetType().Name + " and no handler had been assigned.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel & FdaModel.Utilities.Messager.ErrorMessageEnum.Major));
            }
        }
        public void ShapefilePathsOfType(ref List<string> paths, Utilities.VectorFeatureType type)
        {
            if (RequestShapefilePathsOfType != null)
            {
                RequestShapefilePathsOfType(ref paths, type);
            }
            else
            {
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("Shapefiles requested from " + this.GetType().Name + " and no handler had been assigned.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel & FdaModel.Utilities.Messager.ErrorMessageEnum.Major));
            }
        }
        public void AddToMapWindow(object sender, Utilities.AddMapFeatureEventArgs args)
        {
            if (RequestAddToMapWindow != null)
            {
                RequestAddToMapWindow(sender, args);
            }
            else
            {
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage(this.GetType().Name + " attemped to add a shapefile but no handler had been assigned.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel & FdaModel.Utilities.Messager.ErrorMessageEnum.Major));
            }
        }
        public void RemoveFromMapWindow(object sender, Utilities.RemoveMapFeatureEventArgs args)
        {
            if (RequestRemoveFromMapWindow != null)
            {
                RequestRemoveFromMapWindow(sender,args);
            }
            else
            {
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage(this.GetType().Name + " attemped to remove a shapefile but no handler had been assigned.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel & FdaModel.Utilities.Messager.ErrorMessageEnum.Major));
            }
        }
        public void ReportMessage(FdaModel.Utilities.Messager.ErrorMessage error)
        {
            FdaModel.Utilities.Messager.Logger.Instance.ReportMessage(error);
            if (error.ReportedFrom == GetType().Name)
            {
                if (_messages == null)
                {
                    _messages = new List<FdaModel.Utilities.Messager.ErrorMessage>();
                    _messages.Add(error);
                }
                else
                {
                    _messages.Add(error);
                }
                NotifyPropertyChanged(nameof(Messages));
                //_MessagesAction.IsEnabled = true;
                //_MessagesAction.IsVisible = true;
            }
            //if ((error.ErrorLevel & FdaModel.Utilities.Messager.ErrorMessageEnum.Report) > 0)
            {
                //Utilities.WindowVM
                //Navigate( new Utilities.MessageVM(error.Message), true, true, "Error Report");
            }
            //else
            {

            }
        }

        //public void UndoElement(ISaveUndoRedo editorVM)
        //{
        //    OwnedElement elem = editorVM.CurrentElement;
        //    DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(elem.ChangeTableName());
        //    if (elem.ChangeIndex < changeTableView.NumberOfRows - 1)
        //    {
        //        OwnedElement prevElement = (OwnedElement)elem.GetPreviousElementFromChangeTable(elem.ChangeIndex + 1);
        //        if (prevElement != null)// null if out of range index
        //        {
        //            //cast to ISaveelement and tell it to assign its values
        //            editorVM.AssignValuesFromElementToEditor(prevElement);
        //            elem.ChangeIndex += 1;
        //        }
        //       // editorVM.UpdateUndoRedoButtons();
        //       ((BaseViewModel)editorVM).UpdateUndoRedoVisibility(changeTableView, elem.ChangeIndex);
        //        editorVM.UpdateTheUndoRedoRowItems();
        //    }
        //}

        //public void RedoElement(ISaveUndoRedo editorVM)
        //{
        //    OwnedElement elem = editorVM.CurrentElement;
        //    //get the previous state
        //    if (elem.ChangeIndex > 0)
        //    {
        //        OwnedElement nextElement = (OwnedElement)elem.GetNextElementFromChangeTable(elem.ChangeIndex - 1);
        //        if (nextElement != null)// null if out of range index
        //        {
        //            editorVM.AssignValuesFromElementToEditor(nextElement);
        //            elem.ChangeIndex -= 1;
        //        }
        //        //editorVM.UpdateUndoRedoButtons();
        //        DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(elem.ChangeTableName());
        //        ((BaseViewModel)editorVM).UpdateUndoRedoVisibility(changeTableView, elem.ChangeIndex);
        //        editorVM.UpdateTheUndoRedoRowItems();

        //    }
        //}

     
        public virtual void OnClosing(object sender, EventArgs e)
        {
            Dispose();
        }
        public virtual void Dispose()
        {
            FdaModel.Utilities.Messager.Logger.Instance.Flush(Storage.Connection.Instance.Reader);
            
            
        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }



        #endregion
        #region Functions

        #endregion
        #region InternalClasses
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
        #endregion
    }
}
