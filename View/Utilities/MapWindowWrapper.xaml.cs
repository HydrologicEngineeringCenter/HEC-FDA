using ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace View.Utilities
{
    /// <summary>
    /// Interaction logic for MapWindowWrapper.xaml
    /// </summary>
    public partial class MapWindowWrapper : UserControl
    {
        public MapWindowWrapper()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MapWindowControlVM vm = (MapWindowControlVM)this.DataContext;
            vm.SetFocusToMapWindow += Vm_SetFocusToMapWindow;
        }

        private void Vm_SetFocusToMapWindow(object sender, EventArgs e)
        {
            MapWindowControl.MapWindow.MapWindow.PlotFeatures();
            //MapWindowControl.MapWindow.MapWindow.CaptureScreenToGpu();
            //MapWindowControl.MapWindow_Mouse(sender, e);
        }
    }
}
