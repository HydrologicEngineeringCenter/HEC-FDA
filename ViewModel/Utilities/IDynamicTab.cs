using HEC.FDA.ViewModel.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Utilities
{
    public interface IDynamicTab
    {

        bool CanDelete { get; set; }

        bool CanPopOut { get; set; }

        /// <summary>
        /// The BaseViewModel for the tab or window
        /// </summary>
        BaseViewModel BaseVM { get; set; }

        /// <summary>
        /// This is the title of the tab or window
        /// </summary>
        string Header { get; set; }

        /// <summary>
        /// The UniqueName is used to uniquely identify a tab. 
        /// It is used to ensure that the same tab is not opened twice.
        /// </summary>
        string UniqueName { get; set; }
        bool IsDragging { get; set; }
        //bool CanEdit { get; set; }
        //void DisableEditContextMenuItem();
        //void EnableEditContextMenuItem();

        event EventHandler RemoveTabEvent;
        event EventHandler PopTabOutEvent;
        event EventHandler PopWindowIntoTabEvent;
        event EventHandler RemoveWindowEvent;
        event EventHandler PopTabIntoWindowDraggingEvent;


        void PopTabIntoWindow();
        void PopTabIntoWindowDragging();

        void PopWindowIntoTab();
        void RemoveTab();
        bool RemoveWindow();

    }
}
