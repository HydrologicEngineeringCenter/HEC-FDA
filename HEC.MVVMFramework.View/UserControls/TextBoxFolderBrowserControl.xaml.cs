using System;
using System.Windows;
using System.Windows.Forms;

namespace HEC.MVVMFramework.View.UserControls
{
    public partial class TextBoxFolderBrowserControl : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(nameof(Path), typeof(string), typeof(TextBoxFolderBrowserControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(PathChangedCallBack)));
        public static readonly DependencyProperty InitialDirectoryProperty = DependencyProperty.Register(nameof(InitialDirectory), typeof(string), typeof(TextBoxFolderBrowserControl), new PropertyMetadata(""));

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
        private static void PathChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TextBoxFolderBrowserControl owner = sender as TextBoxFolderBrowserControl;
            string p = e.NewValue.ToString();
            owner.TextBox.Text = p;
        }
        private void OpenBrowser(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Path = dialog.SelectedPath;
            }
        }
    }
}
