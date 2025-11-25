using HEC.FDA.ViewModel.Study;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            if (DataContext is FdaStudyVM study)
            {
                if (study.CurrentStudyElement != null)
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
            if (DataContext is FdaStudyVM study && study.CurrentStudyElement != null)
            {
                int i = 1;
                foreach (string path in study.CurrentStudyElement.RegistryStudyPaths)
                {
                    MenuItem menuItem = new();
                    menuItem.Header = i + ": " + System.IO.Path.GetFileNameWithoutExtension(path);
                    menuItem.Click += study.CurrentStudyElement.OpenStudyFromRecent;
                    menuItem.Tag = path;
                    FileMenu.Items.Add(menuItem);
                    i++;
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AddRecentStudiesMenuItems();
        }

        private void QuickStartGuide_Click(object sender, RoutedEventArgs e)
        {

            string quickStartLink = "https://www.hec.usace.army.mil/fwlink/?linkid=fda-qsg";
            ProcessStartInfo startInfo = new ProcessStartInfo(quickStartLink);
            startInfo.UseShellExecute = true;
            Process.Start(startInfo);
            e.Handled = true;
        }

        private void Discourse_Click(object sender, RoutedEventArgs e)
        {
            string discourseLink = "https://www.hec.usace.army.mil/fwlink/?linkid=fda-discourse";
            ProcessStartInfo startInfo = new ProcessStartInfo(discourseLink);
            startInfo.UseShellExecute = true;
            Process.Start(startInfo);
            e.Handled = true;
        }

        private void StudyTreeView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Ignore double-clicks
            if (e.ClickCount == 2)
            {
                return;
            }

            var treeView = sender as TreeView;
            var item = GetTreeViewItemFromPoint(treeView, e.GetPosition(treeView));

            if (item != null && item.IsSelected)
            {
                item.IsSelected = false;
                e.Handled = true;
            }
        }

        private TreeViewItem GetTreeViewItemFromPoint(TreeView treeView, Point point)
        {
            var element = treeView.InputHitTest(point) as DependencyObject;

            while (element != null && !(element is TreeViewItem))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            return element as TreeViewItem;
        }

    }
}
