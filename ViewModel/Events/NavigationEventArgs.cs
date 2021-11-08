using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Events
{
    public delegate void NavigationEventHandler(object sender, NavigationEventArgs e);
    public class NavigationEventArgs: EventArgs
    {
        public Interfaces.IBaseViewModel ViewModel { get; }
        public Enumerations.NavigationOptionsEnum NavigationProperties { get; }
        public string WindowTitle { get; }
        public NavigationEventArgs(Interfaces.IBaseViewModel vm, Enumerations.NavigationOptionsEnum navigationProperties, string newWindowName)
        {
            ViewModel = vm;
            NavigationProperties = navigationProperties;
            WindowTitle = newWindowName;
        }
    }
}
