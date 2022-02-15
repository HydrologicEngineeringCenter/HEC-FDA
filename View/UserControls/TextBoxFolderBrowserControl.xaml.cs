using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

namespace HEC.MVVMFramework.View.UserControls
{
    public partial class TextBoxFolderBrowserControl : UserControl
    {
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(TextBoxFolderBrowserControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(PathChangedCallBack)));

        public TextBoxFolderBrowserControl()
        {
            InitializeComponent();
        }
        public string Path
        {
            get { return Convert.ToString(GetValue(PathProperty)); }
            set { SetValue(PathProperty, value); }
        }
        public string InitialDirectory { get; set; } = Directory.GetCurrentDirectory();
        private static void PathChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TextBoxFolderBrowserControl owner = sender as TextBoxFolderBrowserControl;
            string p = e.NewValue.ToString();
            owner.Path = p;
            owner.TextBox.Text = p;
        }
        private void OpenBrowser(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = InitialDirectory;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Path = dialog.FileName;
            }
        }
    }
}
