using System;
using System.Windows;
using System.Windows.Controls;

namespace View.UserControls
{
    public partial class TextBoxFileBrowserControl : UserControl
    {
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(TextBoxFileBrowserControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(PathChangedCallBack)));

        public TextBoxFileBrowserControl()
        {
            InitializeComponent();
            Filter = "XML files (*.xml) |*.xml";
            FileExists = true;
        }
        public string Path
        {
            get { return Convert.ToString(GetValue(PathProperty)); }
            set { SetValue(PathProperty, value);}
        }
        public bool FileExists { get; set; }
        public bool IsOpenDialog { get; set; }
        public string Filter { get; set; }
        private static void PathChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TextBoxFileBrowserControl owner = sender as TextBoxFileBrowserControl;
            string p = e.NewValue.ToString();
            owner.Path = p;
            owner.TextBox.Text = p;
        }

        private void OpenBrowser(object sender, RoutedEventArgs e)
        {
            if (IsOpenDialog)
            {
                Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
                fileDialog.Filter = Filter;
                fileDialog.Multiselect = false;
                fileDialog.CheckFileExists = FileExists;
                fileDialog.Title = "Open";
                fileDialog.ShowDialog();
                Path = fileDialog.FileName;

            }
            else
            {
                Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
                fileDialog.Title = "Save As" ;
                fileDialog.Filter = Filter;
                fileDialog.ShowDialog();
                Path = fileDialog.FileName;
            }
        }
    }
}
