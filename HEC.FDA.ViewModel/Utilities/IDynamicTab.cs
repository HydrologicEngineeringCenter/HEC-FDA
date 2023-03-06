using System;

namespace HEC.FDA.ViewModel.Utilities
{
    public interface IDynamicTab
    {
        bool CanDelete { get; set; }

        bool CanPopOut { get; set; }

        bool IsPoppingIn { get; set; }
        bool IsPoppingOut { get; set; }

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

        event EventHandler RemoveTabEvent;
        event EventHandler PopTabOutEvent;
        event EventHandler PopWindowIntoTabEvent;
        event EventHandler RemoveWindowEvent;

        void PopTabIntoWindow();
        void PopWindowIntoTab();
        void RemoveTab();
        bool RemoveWindow();
    }
}
