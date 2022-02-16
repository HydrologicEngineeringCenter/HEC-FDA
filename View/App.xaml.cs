using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace HEC.FDA.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
           
            MessageBox.Show(e.ExceptionObject.ToString(), "Exception",MessageBoxButton.OK, MessageBoxImage.Error);

        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            DisplayErrorToUser(e.Exception);
            //you can use this to open note pad and have it write the stack trace out.
            //process.start(filePath)
            e.Handled = true;
        }

        private void DisplayErrorToUser(Exception e)
        {
            //todo: add email address to this error message
            StringBuilder sb = new StringBuilder().AppendLine("It is advised that you close the " +
                "program and then reopen.").AppendLine();
            sb.AppendLine(e.Message).AppendLine(e.StackTrace);
            MessageBox.Show(sb.ToString(), "Please Email Screenshot of this Unhandled Exception to the FDA Developers", MessageBoxButton.OK, MessageBoxImage.Error);

        }
    }
}
