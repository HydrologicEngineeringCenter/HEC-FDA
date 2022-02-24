using HEC.FDA.ViewModel.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Utilities
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
