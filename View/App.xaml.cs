using HEC.FDA.View.Utilities;
using System;
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
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if(e.IsTerminating)
            {
                //todo: notify user that fda will close?
            }

            if(e.ExceptionObject is Exception exception)
            {
                DisplayErrorToUser(sender, exception);
            }
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            DisplayErrorToUser(sender, e.Exception);
            e.Handled = true;
        }

        private void DisplayErrorToUser(object sender, Exception e)
        {
            UncaughtExceptionWindow window = new UncaughtExceptionWindow(sender, e);
            window.ShowDialog();         
        }
        
    }
}
