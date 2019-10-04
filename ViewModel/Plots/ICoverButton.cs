using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Plots
{
    public interface ICoverButton
    {
        event EventHandler Clicked;
        bool IsEnabled { get; set; }
        void ButtonClicked();

    }
}
