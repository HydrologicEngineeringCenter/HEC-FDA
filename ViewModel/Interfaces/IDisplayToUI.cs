using HEC.MVVMFramework.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Interfaces
{
    public interface IDisplayToUI: INamed
    {
        bool IsEnabled { get; set; }
        bool IsVisible { get; set; }
    }
}
