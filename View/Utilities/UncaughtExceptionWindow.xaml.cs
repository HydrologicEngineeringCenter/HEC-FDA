using HEC.FDA.ViewModel.Utilities;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
            Title = StringConstants.FDA_VERSION + " - Uncaught Exception";
            ErrorMsg.Text = e.Message;
            CallStack.Text = e.StackTrace;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // for .NET Core you need to add UseShellExecute = true
            // see https://docs.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            //Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            OpenUrl(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
