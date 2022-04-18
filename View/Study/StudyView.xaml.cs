using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;

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

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WasXClicked = true;
        }

        private void New_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(DataContext is FdaStudyVM study)
            {
                if(study.CurrentStudyElement != null)
                {
                    study.CurrentStudyElement.CreateNewStudyMenuItemClicked();
                }
            }
        }

        private void Open_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FdaStudyVM study)
            {
                if (study.CurrentStudyElement != null)
                {
                    study.CurrentStudyElement.OpenStudyMenuItemClicked();
                }
            }
        }

        private void Properties_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FdaStudyVM study)
            {
                if (study.CurrentStudyElement != null)
                {
                    study.CurrentStudyElement.StudyProperties();
                }
            }
        }

        private void ImportStudy_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FdaStudyVM study)
            {
                if (study.CurrentStudyElement != null)
                {
                    study.CurrentStudyElement.ImportStudyFromOldFda();
                }
            }
        }

        public void AddRecentStudiesMenuItems()
        {
            if (DataContext is FdaStudyVM study)
            {
                if (study.CurrentStudyElement != null)
                {
                    int i = 1;
                    foreach (string path in study.CurrentStudyElement.RegistryStudyPaths)
                    {
                        MenuItem menuItem = new MenuItem();
                        menuItem.Header = i + ": " + System.IO.Path.GetFileNameWithoutExtension(path);
                        menuItem.Click += study.CurrentStudyElement.OpenStudyFromRecent;
                        menuItem.Tag = path;
                        FileMenu.Items.Add(menuItem);
                        i++;
                    }
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AddRecentStudiesMenuItems();
        }
    }
}
