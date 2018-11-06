using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    public interface IDynamicTab
    {

        bool CanDelete { get; set; }
        BaseViewModel BaseVM { get; set; }
        string Header { get; set; }

        //bool CanEdit { get; set; }
        //void DisableEditContextMenuItem();
        //void EnableEditContextMenuItem();
         event EventHandler RemoveEvent;
         event EventHandler PopTabOutEvent;

        void PopTabOut();
        void RemoveTab();
    }
}
