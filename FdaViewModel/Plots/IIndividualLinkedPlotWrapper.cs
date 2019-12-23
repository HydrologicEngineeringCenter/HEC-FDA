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
        event EventHandler CurveUpdated;
        //event EventHandler TrackerIsOutsideRange;
        string SubTitle { get; set; }
        IndividualLinkedPlotVM PlotVM { get; set; }

        bool TrackerVisible { get; set; }
        bool AreaPlotVisible { get; set; }

        Model.ComputationPoint.PerformanceThreshold Threshold { get; set; }

        bool DisplayImportButton { get; set; }
    }
}
