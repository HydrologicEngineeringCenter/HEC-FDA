using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Plots
{
    interface iIASControl
    {
        //events
        event EventHandler SelectedCurveUpdated;
        event EventHandler PlotIsShowing;
        event EventHandler PlotIsNotShowing;

        //properties
        BaseViewModel CurrentVM { get; set; }
        BaseViewModel PreviousVM { get; set; }

        IndividualLinkedPlotVM PreviousPlot { get; set; }
        IIndividualLinkedPlotWrapper IndividualPlotWrapperVM { get; set; }
        ICoverButton ImportButtonVM { get; set; }
        iIASImporter CurveImporterVM { get; set; }


        //voids
        void ShowPreviousVM(object sender, EventArgs e);
        
        void ShowTheImportButton(object sender, EventArgs e);

        void ImportButtonClicked(object sender, EventArgs e);

        void AddCurveToPlot(object sender, EventArgs e);

        void PlotIsNotShowingAnymore(object sender, EventArgs e);
    }
}
