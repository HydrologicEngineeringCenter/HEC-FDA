using FdaViewModel.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    /// <summary>
    /// Contains all the data needed to display a tab or window in FDA. When you pass this into the Navigate() method
    /// the UI will bind to the BaseVM which is a base view model object. There are several events that are used to 
    /// remove and pop in and out of tabs and windows.
    /// </summary>
    public class DynamicTabVM:BaseViewModel, IDynamicTab
    {

        public event EventHandler RemoveTabEvent;
        public event EventHandler PopTabOutEvent;
        public event EventHandler PopWindowIntoTabEvent;
        public event EventHandler RemoveWindowEvent;
        public event EventHandler PopTabIntoWindowDraggingEvent;

        private BaseViewModel _BaseVM;
        private string _Header;
        private string _UniqueName;
        private bool _IsDragging;

        /// <summary>
        /// Used to know if the tab is being dragged out into a window.
        /// </summary>
        public bool IsDragging
        {
            get { return _IsDragging; }
            set { _IsDragging = value; }
        }
        /// <summary>
        /// The header or title of the tab or window
        /// </summary>
        public string Header
        {
            get { return _Header; }
            set { _Header = value; NotifyPropertyChanged(); }
        }
        /// <summary>
        /// Used to disable and hide the red x button on the tab. This won't work well if not combined with 
        /// CanPopOut = false; because if you can pop it out then the user will have access to the red x on the 
        /// window and will be able to delete it.
        /// </summary>
        public bool CanDelete { get; set; }

        /// <summary>
        /// The base view model for the tab or window
        /// </summary>
        public BaseViewModel BaseVM
        {
            get { return _BaseVM;  }
            set { _BaseVM = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// If false then the user will not be able to pop the tab into a window
        /// </summary>
        public bool CanPopOut { get; set; }

        /// <summary>
        /// Used to uniquely identify each tab so that we do not open multiples of the same tab.
        /// </summary>
        public string UniqueName
        {
            get { return _UniqueName; }
            set { _UniqueName = value; }
        }

        /// <summary>
        /// Creates the tab object that can be displayed by passing it into Navigate().
        /// </summary>
        /// <param name="header">Title of the tab or window</param>
        /// <param name="baseVM">The base view model</param>
        /// <param name="uniqueName">Used to uniquely identify each tab so that we do not open multiples of the same tab.</param>
        /// <param name="canDelete">Used to disable and hide the red x button on the tab</param>
        /// <param name="canPopOut">If false then the user will not be able to pop the tab into a window</param>
        public DynamicTabVM(string header, BaseViewModel baseVM, string uniqueName, bool canDelete = true, bool canPopOut = true)
        {
            _UniqueName = uniqueName;
            CanDelete = canDelete;
            Header = header;
            BaseVM = baseVM;
            CanPopOut = canPopOut;
        }
        
        /// <summary>
        /// Event that gets attached by the TabController when adding a tab. This event will remove the tab
        /// and add it as a window.
        /// </summary>
        public void PopTabIntoWindow()
        {
            PopTabOutEvent?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// This event is for when the user clicks on the tab header and draggs the mouse. The tab gets removed and
        /// a window is added. Event gets attached by the TabController.
        /// </summary>
        public void PopTabIntoWindowDragging()
        {
            PopTabIntoWindowDraggingEvent?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Removes this tab. Event gets attached by the TabController.
        /// </summary>
        public void RemoveTab()
        {
            RemoveTabEvent?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Removes the window and adds the tab as a tab in the main UI. Event
        /// gets attached by the TabController.
        /// </summary>
        public void PopWindowIntoTab()
        {
            PopWindowIntoTabEvent?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Event used to close the window that this tab is in.
        /// Event that gets attached by the TabController.
        /// </summary>
        public void RemoveWindow()
        {
            RemoveWindowEvent?.Invoke(this, new EventArgs());
        }
    }
}
