using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Interfaces
{
    public interface INavigate
    {
        event Events.NavigationEventHandler NavigationEvent;
        void Navigate(object sender, Events.NavigationEventArgs e);
    }
}
