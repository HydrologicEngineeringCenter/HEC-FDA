using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace HEC.FDA.View.Utilities
{
    /// <summary>
    /// Interaction logic for UncaughtExceptionWindow.xaml
    /// </summary>
    public partial class UncaughtExceptionWindow : Window
    {

        public string ErrorMessage { get; }
        public UncaughtExceptionWindow(object sender, Exception e)
        {
            InitializeComponent();
            Title = ViewModel.Utilities.StringConstants.FDA_VERSION + " - Uncaught Exception";
            ErrorMsg.Text = e.Message;
            CallStack.Text = e.StackTrace;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // for .NET Core you need to add UseShellExecute = true
            // see https://docs.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
