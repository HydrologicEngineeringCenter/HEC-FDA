﻿using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace HEC.FDA.ViewModel.Tabs
{
    /// <summary>
    /// The main class for controlling the tabs/windows. This class holds two lists of IDynamicTab. One is for
    /// tabs and the other is for tabs that have been popped out into windows. Contains methods to add and remove tabs
    /// and windows. This is a singleton class, call "Instance" to get the one instance of it.
    /// </summary>
    public sealed class TabController : BaseViewModel
    {
        private static readonly TabController _Instance = new TabController();
        
        private int _SelectedTabIndex;
        private ObservableCollection<IDynamicTab> _Tabs = new ObservableCollection<IDynamicTab>();
        private List<IDynamicTab> _Windows = new List<IDynamicTab>();

        #region Properties
        /// <summary>
        /// The list of tabs in the main UI
        /// </summary>
        public ObservableCollection<IDynamicTab> Tabs
        {
            get { return _Tabs; }
            set { _Tabs = value; NotifyPropertyChanged(); }
        }
        /// <summary>
        /// The selected tab. Changing this value will tell the UI to select that tab.
        /// </summary>
        public int SelectedDynamicTabIndex
        {
            get { return _SelectedTabIndex; }
            set { _SelectedTabIndex = value; NotifyPropertyChanged();}
        }
        #endregion


        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static TabController()
        {
        }

        private TabController()
        {
        }

        /// <summary>
        /// Gets the instance of this singleton class
        /// </summary>
        public static TabController Instance
        {
            get
            {
                return _Instance;
            }
        }

        #region public methods

        /// <summary>
        /// Finds the enclosing window of the class passed in. If the window is the main window then we know we are a tab
        /// and so we remove the selected tab. If the window is not the main window then we know we are in a popped out window
        /// and so we remove that window. Use this when not using the OkCloseControl.
        /// </summary>
        /// <param name="callingClass">The UI class that will either be the view inside of a tab or a window.</param>
        public void CloseTabOrWindow(DependencyObject callingClass)
        {
            var window = Window.GetWindow(callingClass);
            if (window.DataContext is WindowVM)
            {
                WindowVM winVM = (WindowVM)window.DataContext;
                if (winVM.Tab == null)
                {
                    //remove the selected tab
                    if (SelectedDynamicTabIndex != -1)
                    {
                        IDynamicTab selectedTab = Tabs[SelectedDynamicTabIndex];
                        if (selectedTab.BaseVM.IsOkToClose())
                        {
                            bool userWantsToClose = UserWantsToClose(selectedTab.BaseVM);
                            if (userWantsToClose)
                            {
                                Tabs.Remove(Tabs[SelectedDynamicTabIndex]);
                            }
                        }
                    }
                }
                else
                {
                    if (winVM.Tab.BaseVM.IsOkToClose())
                    {
                        //I do not need to remove the window from the _Windows list because when the window closes
                        //the ViewWindow.Window_Closing() will remove the window from the list.
                        window.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new tab to the list of tabs. This will automatically add the tab to the UI
        /// </summary>
        /// <param name="tab">The tab you want to add</param>
        public void AddTab(IDynamicTab tab)
        {
            if(tab.BaseVM is IDetectChanges)
            {
                tab.BaseVM.HasChanges = false;
            }

            int indexOfTab = IsAlreadyOpenInTabs(tab.UniqueName);
            if (indexOfTab != -1)
            {
                //then select the tab
                SelectedDynamicTabIndex = indexOfTab;
            }
            else if(IsAlreadyOpenInWindows(tab.UniqueName))
            {
                //then select that window
                PutFocusOnWindow(tab);
            }
            else
            {
                //this is a new tab
                tab.PopTabOutEvent += PopTabIntoWindow;
                tab.RemoveTabEvent += RemoveTab;
                tab.PopWindowIntoTabEvent += PopWindowIntoTab;
                tab.RemoveWindowEvent += RemoveWindow;

                _Tabs.Add(tab);
                SelectedDynamicTabIndex = Tabs.Count - 1;
            }
        }

        #endregion

        #region private methods
        private void PutFocusOnWindow(IDynamicTab tab)
        {
            foreach(Window window in Application.Current.Windows)
            {
                WindowVM vm = (WindowVM)window.DataContext;
                if(vm != null && vm.Tab != null && vm.Tab.UniqueName.Equals(tab.UniqueName))
                {
                    if (window.WindowState == WindowState.Minimized)
                    {
                        window.WindowState = WindowState.Normal;
                    }
                    window.Activate();
                    return;
                }

            }
        }

        private bool IsAlreadyOpenInWindows(string tabUniqueName)
        {
            for (int i = 0; i < _Windows.Count; i++)
            {
                if (_Windows[i].UniqueName.Equals(tabUniqueName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// -1 if the tab doesn't exist in the tabs list. Integer index in the list if it does.
        /// </summary>
        /// <param name="tabUniqueName">The unique string that identifies this tab</param>
        /// <returns></returns>
        private int IsAlreadyOpenInTabs(string tabUniqueName)
        {
            int retval = -1;

            for (int i = 0; i < Tabs.Count; i++)
            {
                if (Tabs[i].UniqueName.Equals(tabUniqueName))
                {
                    retval = i;
                    break;
                }
            }

            return retval;
        }
        
        private void PopWindowIntoTab(object sender, EventArgs e)
        {
            DynamicTabVM tabToPopIn = (DynamicTabVM)sender;
            tabToPopIn.IsPoppingIn = true;
            //you don't have to remove the tab from the _Windows here because 
            //when the window closes it will call RemoveWindow()
            _Tabs.Add(tabToPopIn);
            SelectedDynamicTabIndex = Tabs.Count - 1;
        }

        private void RemoveWindow(object sender, EventArgs e)
        {
            DynamicTabVM tab = (DynamicTabVM)sender;
            _Windows.Remove(tab);
        }

        private void RemoveTab(object sender, EventArgs e)
        {
            IDynamicTab tab = (IDynamicTab)sender;
            if(tab.BaseVM is IDetectChanges)
            {
                bool userWantsToClose = UserWantsToClose(tab.BaseVM);
                if (userWantsToClose)
                {
                    _Tabs.Remove(tab);
                }
            }
            else
            {
                _Tabs.Remove(tab);
            }
        }

        public static bool UserWantsToClose(BaseViewModel BaseVM)
        {
            bool userWantsToClose = true;
            //BaseVM.WasCanceled is true when the user clicked the close button or the red x
            //This method gets called when the user clicks the OK button. We do not want to prompt
            //the user when the OK button is clicked.
            if(BaseVM is IDetectChanges && BaseVM.HasChanges && BaseVM.WasCanceled)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("There are unsaved changes. Are you sure you want to close?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if(messageBoxResult == MessageBoxResult.No)
                {
                    userWantsToClose = false;
                }
            }
            return userWantsToClose;
        }

        private void PopTabIntoWindow(object sender, EventArgs e)
        {
            DynamicTabVM tabToPopOut = (DynamicTabVM)sender;
            tabToPopOut.IsPoppingOut = true;
            _Tabs.Remove(tabToPopOut);
            _Windows.Add(tabToPopOut);           
            Navigate(tabToPopOut, true, false);
        }

        public void CloseTabsAndWindowsOpeningNewStudy()
        {
            _Windows.Clear();

            //remove all the tabs except for the importing tab
            List<IDynamicTab> dynamicTabsToRemove = _Tabs.Where(tab => !tab.UniqueName.Equals(StudyElement.IMPORT_STUDY_UNIQUE_STRING)).ToList();

            foreach (IDynamicTab itemToRemove in dynamicTabsToRemove)
            {
                _Tabs.Remove(itemToRemove);
            }

            //remove all the windows except for the importing tab window and the main window.
            foreach (Window win in Application.Current.Windows)
            {
                //the main window doesn't have a tab. Everything else does.
                if (win.DataContext is WindowVM windowVM && windowVM.Tab != null && !windowVM.Tab.UniqueName.Equals(StudyElement.IMPORT_STUDY_UNIQUE_STRING))
                {
                    win.Close();
                }
            }
        }

        #endregion
    }
}
