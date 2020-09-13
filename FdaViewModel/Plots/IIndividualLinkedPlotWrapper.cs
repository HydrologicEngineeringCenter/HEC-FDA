using FdaViewModel.Conditions;
using Model;
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

        Model.IMetric Metric { get; set; }
        int SelectedElementID { get; set; }
        void AddCurveToPlot(IFdaFunction function, string elementName,int elementID, FdaCrosshairChartModifier chartModifier);
        //bool DisplayImportButton { get; set; }

        //string Title { get; }
        //string XAxisLabel { get; }
        //string YAxisLabel { get; }
    }
}
