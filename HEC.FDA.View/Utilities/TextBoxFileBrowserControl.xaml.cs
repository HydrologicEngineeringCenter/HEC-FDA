using System;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Utilities
{
    public partial class TextBoxFileBrowserControl : UserControl
    {
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(TextBoxFileBrowserControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(PathChangedCallBack)));

        public TextBoxFileBrowserControl()
        {
            InitializeComponent();
            Filter = "All files(*.*) | *.*";
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
            owner.TxtFile.Text = p;
        }

        private void OpenBrowser(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Title = "Select File";
            fileDialog.Filter = Filter;
            fileDialog.ShowDialog();
            Path = fileDialog.FileName;
        }
    }
}
