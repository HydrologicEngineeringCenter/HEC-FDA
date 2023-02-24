using System.Windows;
using HEC.MVVMFramework.View.Implementations;
using HEC.MVVMFramework.View.Windows;
using HEC.MVVMFramework.Base.Enumerations;
using ViewTest.ViewModel;

namespace ViewTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            BasicWindow window = new BasicWindow(new CounterVM());
            window.Show();
        }
    }
}
