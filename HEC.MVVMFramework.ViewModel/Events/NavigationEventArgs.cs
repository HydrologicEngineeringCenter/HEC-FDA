using HEC.MVVMFramework.ViewModel.Enumerations;
using System;

namespace HEC.MVVMFramework.ViewModel.Events
{
    public delegate void NavigationEventHandler(object sender, NavigationEventArgs e);
    public class NavigationEventArgs : EventArgs
    {
        public Interfaces.IBaseViewModel ViewModel { get; }
        public NavigationOptionsEnum NavigationProperties { get; }
        public string WindowTitle { get; }
        public NavigationEventArgs(Interfaces.IBaseViewModel vm, NavigationOptionsEnum navigationProperties, string newWindowName)
        {
            ViewModel = vm;
            NavigationProperties = navigationProperties;
            WindowTitle = newWindowName;
        }
    }
}
