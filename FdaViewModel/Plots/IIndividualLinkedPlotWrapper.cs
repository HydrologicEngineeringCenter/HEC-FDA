using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Plots
{
    public interface IIndividualLinkedPlotWrapper
    {
        event EventHandler ShowImportButton;
        event EventHandler ShowTheImporter;
        string SubTitle { get; set; }
        IndividualLinkedPlotVM PlotVM { get; set; }


    }
}
