using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

namespace HEC.MVVMFramework.View.UserControls
{
    public partial class TextBoxFolderBrowserControl : UserControl
    {
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(nameof(Path), typeof(string), typeof(TextBoxFolderBrowserControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(PathChangedCallBack)));
        public static readonly DependencyProperty InitialDirectoryProperty = DependencyProperty.Register(nameof(InitialDirectory), typeof(string), typeof(TextBoxFolderBrowserControl), new PropertyMetadata(""));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(TextBoxFolderBrowserControl), new PropertyMetadata("Forgot to set this title didn't you?"));

        public TextBoxFolderBrowserControl()
        {
            InitializeComponent();
        }
        public string Path
        {
            get { return Convert.ToString(GetValue(PathProperty)); }
            set { SetValue(PathProperty, value); }
        }
        public string InitialDirectory
        {
            get { return Convert.ToString(GetValue(InitialDirectoryProperty)); }
            set { SetValue(InitialDirectoryProperty, value); }
        }
        public string Title
        {
            get { return Convert.ToString(GetValue(TitleProperty)); }
            set { SetValue(TitleProperty, value); }
        }
        private static void PathChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TextBoxFolderBrowserControl owner = sender as TextBoxFolderBrowserControl;
            string p = e.NewValue.ToString();
            owner.TextBox.Text = p;
        }
        private void OpenBrowser(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = InitialDirectory;
            dialog.IsFolderPicker = true;
            dialog.Title = Title;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Path = dialog.FileName;
            }
        }
    }
}
