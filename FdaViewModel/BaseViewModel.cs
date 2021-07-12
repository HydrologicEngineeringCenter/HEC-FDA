using FdaLogging;
using FdaViewModel.Study;
using FdaViewModel.Utilities;
using FdaViewModel.Utilities.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FdaViewModel
{
    public delegate void RequestNavigationHandler( IDynamicTab tab, bool newWindow, bool asDialog);
    public delegate void RequestShapefilePathsHandler(ref List<string> files);
    public delegate void RequestShapefilePathsOfTypeHandler(ref List<string> files, VectorFeatureType type);
    public delegate void RequestAddToMapWindowHandler(object sender, AddMapFeatureEventArgs args);//needs to be capable of passing a geopackage connection??
    public delegate void RequestRemoveFromMapWindowHandler(object sender, RemoveMapFeatureEventArgs args);//needs to be capable of passing a geopackage connection??

    /// <summary>
    /// The base class for all view model classes. Contains methods that are common among all view model classes
    /// such as validation, navigation, adding rules.
    /// </summary>
    public abstract class BaseViewModel : System.ComponentModel.IDataErrorInfo
    {
        private static readonly FdaLogging.FdaLogger LOGGER = new FdaLogging.FdaLogger("BaseViewModel");

        #region Notes
        #endregion
        #region Events
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

        private bool _HasChanges;

        private string _Name;
        private string _Error;

        #endregion
        #region Properties
        //default values
        private double _Width = 300;
        private double _Height = 300;
        private double _MinWidth = 300;
        private double _MinHeight = 300;
        public double Width
        {
            get { return _Width; }
            set { _Width = value; NotifyPropertyChanged(); }
        }
        public double Height
        {
            get { return _Height; }
            set { _Height = value; NotifyPropertyChanged(); }
        }
        public double MinWidth
        {
            get { return _MinWidth; }
            set { _MinWidth = value; NotifyPropertyChanged(); }
        }
        public double MinHeight
        {
            get { return _MinHeight; }
            set { _MinHeight = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// The StudyCache holds all the elements used in FDA. You can use this to get any of them 
        /// as well as listen for events where elements are added, removed, or updated
        /// </summary>
        public static IStudyCache StudyCache { get; set; }
        public static Saving.PersistenceFactory PersistenceFactory { get; set; }
        /// <summary>
        /// The name of the object
        /// </summary>
        public virtual string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }

        public string LastEditDate { get; set; }

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

        public bool HasError { get; private set; }
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
            set { _HasChanges = value; NotifyPropertyChanged(); } 
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

        public void AddTransaction(object sender, TransactionEventArgs transaction)
        {
            TransactionEvent?.Invoke(this, transaction);
        }

        /// <summary>
        /// Virtual method that can be overriden in any view model class to add its own validation rules. 
        /// This gets called in the constructor of BaseViewModel.
        /// </summary>
        virtual public void AddValidationRules() { }

        /// <summary>
        /// Loops over and evaluates the property rules.
        /// </summary>
        public void Validate()
        {
            HasError = false;
            HasFatalError = false;
            NotifyPropertyChanged("HasError");
            NotifyPropertyChanged("HasFatalError");
            StringBuilder errors = new StringBuilder();
            List<LogItem> errorMessages = new List<LogItem>();
            Error = "";
            foreach (PropertyRule pr in ruleMap.Values)
            {
                
                pr.Update();
                if (pr.HasError)
                {
                    if (pr.HasFatalError == true)
                    {
                        HasFatalError = true;
                        NotifyPropertyChanged("HasFatalError");
                    }
                    errors.AppendLine(pr.Error);
                    errorMessages.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, pr.Error));
                    HasError = true;
                    NotifyPropertyChanged("HasError");
                }
            }
            if (HasError)
            {
                //this is used to display the tooltip on the OK and SAVE buttons
                Error = errors.ToString().Remove(errors.ToString().Length - 2);
            }

            if (this is IDisplayLogMessages)
            {
                ((IDisplayLogMessages)this).TempErrors = errorMessages;
                ((IDisplayLogMessages)this).UpdateMessages(false);
            }
            NotifyPropertyChanged(nameof(Error));
        }

        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {

            //todo: I don't like excluding properties like this, but if the validate is going to update
            //properties, then you will get an infinite loop if you don't exclude them.
            if (propertyName.Equals(nameof(HasError))
                || propertyName.Equals(nameof(HasFatalError))
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
        /// Recursively goes up the tree structure to WindowVM.CurrentView_RequestNavigation(). Takes the vm and finds its matching view and displays it.
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
        }

        /// <summary>
        /// Recursively goes up to the ViewWindow.xaml.cs and gets any shapefiles in the map window
        /// </summary>
        /// <param name="paths"></param>
        public void ShapefilePaths(ref List<string> paths)
        {
            RequestShapefilePaths?.Invoke(ref paths);
        }
        /// <summary>
        /// Recursively goes up to the ViewWindow.xaml.cs and gets any files of the desired VectorFeatureType
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="type">Point, Line, Polygon</param>
        public void ShapefilePathsOfType(ref List<string> paths, Utilities.VectorFeatureType type)
        {
            RequestShapefilePathsOfType?.Invoke(ref paths, type);      
        }
        /// <summary>
        /// Recursively goes up to the ViewWindow.xaml.cs and adds "this" to the map window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddToMapWindow(object sender, Utilities.AddMapFeatureEventArgs args)
        {
            RequestAddToMapWindow?.Invoke(sender, args);
        }
        /// <summary>
        /// Recursively goes up to the ViewWindow.xaml.cs and removes "this" from the map window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RemoveFromMapWindow(object sender, Utilities.RemoveMapFeatureEventArgs args)
        {
            RequestRemoveFromMapWindow?.Invoke(sender, args);
        }   

        public virtual void OnClosing(object sender, EventArgs e)
        {

        }
        public virtual void Dispose()
        {

        }

        /// <summary>
        /// Used to set the dimensions of the window when a view model is displayed in a window.
        /// It actually gets set to the VM that the view window is pointing to in WindowVM ctor.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="minWidth"></param>
        /// <param name="minHeight"></param>
        public void SetDimensions(double width, double height, double minWidth, double minHeight)
        {
            Width = width;
            Height = height;
            MinWidth = minWidth;
            MinHeight = minHeight;
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
    }
}
