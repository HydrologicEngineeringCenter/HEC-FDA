﻿using HEC.FDA.View.Utilities;
using HEC.FDA.ViewModel.Tabs;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Windows;

namespace HEC.FDA.View
{
    /// <summary>
    /// Interaction logic for ViewWindow.xaml
    /// </summary>
    public partial class ViewWindow : Window
    {
        public ViewWindow()
        {
            InitializeComponent();
            //remove the row that has the pop-in button.
            MainGrid.RowDefinitions[0].Height = new GridLength(0);
            WindowVM vm = DataContext as WindowVM;
            vm.LaunchNewWindow += WindowSpawner;
        }

        public ViewWindow(WindowVM newvm)
        {
            InitializeComponent();
            DataContext = newvm;
            newvm.LaunchNewWindow += WindowSpawner;
        }

        private void btn_PopWindowInToTabs_Click(object sender, RoutedEventArgs e)
        {
            WindowVM vm = DataContext as WindowVM;
            IDynamicTab tab = vm.Tab;
            tab.PopWindowIntoTab();
            Close();
        }

        private void WindowSpawner(WindowVM newvm, bool asDialogue)
        {
            //If we are popping an existing editor out of the tab control,
            //we don't want to clear the HasChanges bool.
            if(newvm.Tab.BaseVM is IDetectChanges && !newvm.Tab.IsPoppingOut)
            {
                newvm.Tab.BaseVM.HasChanges = false;
            }

            newvm.Tab.IsPoppingOut = false;
            newvm.WasCanceled = true;
            ViewWindow newwindow = new ViewWindow(newvm);
            newwindow.Owner = this;

            //hide the top row with the pop in button if this vm doesn't support that
            if (newvm.Tab.CanPopOut == false)
            {
                newwindow.MainGrid.RowDefinitions[0].Height = new GridLength(0);
            }

            if (asDialogue)
            {             
                newwindow.ShowDialog();
            }
            else
            {
                newwindow.Show();
            }
        }
       
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            WindowVM vm = DataContext as WindowVM;
            if (vm.Tab != null)
            {
                IDynamicTab tab = vm.Tab;
                if (tab.IsPoppingIn)
                {
                    tab.RemoveWindow();
                    tab.IsPoppingIn = false;
                }
                else
                {
                    bool userWantsToClose = TabController.UserWantsToClose(tab.BaseVM);
                    if (userWantsToClose)
                    {
                        tab.RemoveWindow();
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
            else
            {
                TextFileMessageSubscriber.Instance.Dispose();
                if (!ViewModel.Storage.Connection.Instance.IsConnectionNull)
                {
                    ViewModel.Storage.Connection.Instance.Close();
                }
                Environment.Exit(0);
            }
        }

        private void btn_PopOut_Click(object sender, RoutedEventArgs e)
        {
            WindowVM winVM = DataContext as WindowVM;
            if(winVM.Tab != null)
            {
                winVM.Tab.PopWindowIntoTab();
                Close();
            }

        }

        /// <summary>
        /// Reads the dimensions defined in the app.xaml (or the default) and sets the window dimensions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowVM winVM = DataContext as WindowVM;          
            Type editorType = winVM.CurrentView.GetType();            
            Dimension dimensions = WindowDimensions.GetWindowDimensions(editorType);
            if(dimensions != null)
            {
                if(dimensions.MinWidth != 0)
                {
                    MinWidth = dimensions.MinWidth;
                }
                if(dimensions.MaxWidth != 0)
                {
                    MaxWidth = dimensions.MaxWidth;
                }

                if(dimensions.MinHeight != 0)
                {
                    MinHeight = dimensions.MinHeight;
                }

                if(dimensions.Width != 0)
                {
                    Width = dimensions.Width;
                }

                if(dimensions.Height != 0)
                {
                    Height = dimensions.Height;
                }

                if(dimensions.MaxHeight != 0)
                {
                    MaxHeight = dimensions.MaxHeight;
                }
            }
            if (winVM.Tab == null)
            {
                //then we are the main window. Launch the splash screen
                winVM.StudyVM.LaunchSplashScreen();
            }
        }
        
    }
}
