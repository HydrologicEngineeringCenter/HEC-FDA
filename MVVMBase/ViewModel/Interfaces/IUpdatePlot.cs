using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Interfaces
{
    public interface IUpdatePlot
    {
        event Events.UpdatePlotEventHandler UpdatePlotEvent;
        void InvalidatePlotModel(bool updateData);
        void UpdatePlot(object sender, Events.UpdatePlotEventArgs e);
    }
}
