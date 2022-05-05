using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.ViewModel.Implementations;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel
{
    public delegate void RequestNavigationHandler( IDynamicTab tab, bool newWindow, bool asDialog);

    /// <summary>
    /// The base class for all view model classes. Contains methods that are common among all view model classes
    /// such as validation, navigation, adding rules.
    /// </summary>
    public abstract class BaseViewModel : ValidatingBaseViewModel
    {
        #region Notes
        #endregion
        #region Events
        //public event PropertyChangedEventHandler PropertyChanged;
        public event RequestNavigationHandler RequestNavigation;

        #endregion
        #region Fields
        private bool _HasChanges;
        #endregion
        #region Properties

        public List<string> Errors
        {
            get
            {
                return (List<string>)GetErrors();
            }
        }

        /// <summary>
        /// The StudyCache holds all the elements used in FDA. You can use this to get any of them 
        /// as well as listen for events where elements are added, removed, or updated
        /// </summary>
        public static IStudyCache StudyCache { get; set; }
        public static Saving.PersistenceFactory PersistenceFactory { get; set; }

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


        /// <summary>
        /// Loops over and evaluates the property rules.
        /// </summary>
        //public void Validate()
        //{
        //    HasError = false;
        //    HasFatalError = false;
        //    NotifyPropertyChanged("HasError");
        //    NotifyPropertyChanged("HasFatalError");
        //    StringBuilder errors = new StringBuilder();
        //    Error = "";
        //    foreach (PropertyRule pr in ruleMap.Values)
        //    {
                
        //        pr.Update();
        //        if (pr.HasError)
        //        {
        //            if (pr.HasFatalError == true)
        //            {
        //                HasFatalError = true;
        //                NotifyPropertyChanged("HasFatalError");
        //            }
        //            errors.AppendLine(pr.Error);
        //            HasError = true;
        //            NotifyPropertyChanged("HasError");
        //        }
        //    }
        //    if (HasError)
        //    {
        //        //this is used to display the tooltip on the OK and SAVE buttons
        //        Error = errors.ToString().Remove(errors.ToString().Length - 2);
        //    }

        //    NotifyPropertyChanged(nameof(Error));
        //}

 



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

    }
}
