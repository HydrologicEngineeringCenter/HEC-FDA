using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;
using HEC.FDA.ViewTest.FrequencyEditor;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.View.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HEC.FDA.ViewTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            FrequencyEditorVM vm = new FrequencyEditorVM();
            BasicWindow window = new BasicWindow(vm);
            window.Show();
        }
    }
}
