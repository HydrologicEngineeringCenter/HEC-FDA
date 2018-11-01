using FdaViewModel.Study;
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
    public delegate void RequestNavigationHandler( BaseViewModel vm, bool newWindow, bool asDialog, string title);
    public delegate void RequestShapefilePathsHandler(ref List<string> files);
    public delegate void RequestShapefilePathsOfTypeHandler(ref List<string> files, Utilities.VectorFeatureType type);
    public delegate void RequestAddToMapWindowHandler(object sender, Utilities.AddMapFeatureEventArgs args);//needs to be capable of passing a geopackage connection??
    public delegate void RequestRemoveFromMapWindowHandler(object sender, Utilities.RemoveMapFeatureEventArgs args);//needs to be capable of passing a geopackage connection??
    public abstract class BaseViewModel : System.ComponentModel.INotifyPropertyChanged, System.ComponentModel.IDataErrorInfo, IDisposable
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
        public BaseViewModel()
        {
            //_MessagesAction = new Utilities.NamedAction();
            //_ErrorsAction = new Utilities.NamedAction();
            //_HelpAction = new Utilities.NamedAction();

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

        public Action<DynamicTabVM, bool> AddThisToTabs { get; set; }
        public Action<Guid> RemoveFromTabsDictionary { get; set; }
        public Guid TabUniqueID { get; set; }
        /// <summary>
        /// This is used to tell the ui if there should be a top row with the pop in button
        /// </summary>
        public bool CanPopIn { get; set; } = false;
        public bool CanOpenMultipleTimes { get; set; } = false;
        public void AddPopThisIntoATabAction( Action<DynamicTabVM,bool> addTab)
        {
            AddThisToTabs = addTab;
            CanPopIn = true;
        }

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
        virtual public void AddValidationRules() { }
        public void Validate()
        {
            _HasError = false;
            _HasFatalError = false;
            NotifyPropertyChanged("HasError");
            NotifyPropertyChanged("HasFatalError");
            StringBuilder errors = new StringBuilder();
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
                NotifyPropertyChanged(nameof(Error));
            }
            else
            {
                Error = null;
            }
        }
        //virtual public void Save() { }
        
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
        public void AddRule(string PropertyName, Func<bool> ruleDelegate, string errorMessage, bool hasFatalError = true)
        {
            if (ruleMap.ContainsKey(PropertyName))
            {
                ruleMap[PropertyName].AddRule(ruleDelegate, errorMessage, hasFatalError);
            }
            else
            {
                ruleMap.Add(PropertyName, new PropertyRule(ruleDelegate, errorMessage, hasFatalError));
            }
        }
     
        public void Navigate( BaseViewModel vm, bool newWindow = true, bool asDialog = true, string title = "FDA 2.0")
        {
            if (RequestNavigation != null)
            {
                vm.WasCanceled = true;
                RequestNavigation(vm, newWindow, asDialog, title);
            }
            else
            {
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("Navigation requested from " + this.GetType().Name + " to " + vm.GetType().Name + " and no handler had been assigned.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel & FdaModel.Utilities.Messager.ErrorMessageEnum.Major));
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
                Navigate(new Utilities.MessageVM(error.Message), true, true, "Error Report");
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

        

        #endregion
        #region Functions

        #endregion
        #region InternalClasses
        protected class PropertyRule
        {
            private List<Rule> _rules = new List<Rule>();

            internal bool HasError { get; set; }
            internal bool HasFatalError { get; set; }
            //internal bool IsDirty { get; set; }
            internal string Error { get; set; }
            internal PropertyRule(Func<bool> rule, string errormessage, bool hasFatalError = true)
            {
                _rules.Add(new Rule(rule, errormessage, hasFatalError));
            }
            internal void AddRule(Func<bool> rule, string errormessage, bool hasFatalError = true)
            {
                _rules.Add(new Rule(rule, errormessage, hasFatalError));
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
