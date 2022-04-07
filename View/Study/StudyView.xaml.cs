using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HEC.FDA.ViewModel.Study;

namespace HEC.FDA.View.Study
{
    /// <summary>
    /// Interaction logic for StudyView.xaml
    /// </summary>
    public partial class StudyView : UserControl
    {
        public bool WasXClicked { get; set; } = false;

        public StudyView()
        { 
            InitializeComponent();
        }

        private void lbl_Study_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            lbl_Study.ContextMenu.IsOpen = true;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WasXClicked = true;
        }

    }
}
