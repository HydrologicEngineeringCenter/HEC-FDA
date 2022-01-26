using System;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ViewModel.Study;

namespace View.Study
{
    /// <summary>
    /// Interaction logic for StudyView.xaml
    /// </summary>
    public partial class StudyView : UserControl
    {
        public bool WasXClicked { get; set; } = false;
        public bool WasPopOutClicked { get; set; } = false;

        public StudyView()
        { 
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FdaStudyVM vm = DataContext as FdaStudyVM;
            vm?.AddMapsTab(MapTreeView);
        }

        private void lbl_Study_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            lbl_Study.ContextMenu.IsOpen = true;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WasXClicked = true;
        }

        private void txt_PopOut_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WasPopOutClicked = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                FdaStudyVM vm = DataContext as FdaStudyVM;
                //todo: testing only
                vm?.UpdateMapTabTest();
            }));
        }
    }
}
