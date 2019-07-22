using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Tabs
{
    public class TabAddedEventArgs
    {
        public IDynamicTab Tab { get; set; }
        public TabAddedEventArgs(IDynamicTab tab)
        {
            Tab = tab;
        }

    }
}
