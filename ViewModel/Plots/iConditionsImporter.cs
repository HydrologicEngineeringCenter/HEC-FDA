using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Plots
{
    public interface iConditionsImporter
    {
        //events
        event EventHandler OKClickedEvent;
        event EventHandler CancelClickedEvent;
        event EventHandler PopImporterOut;

        //properties
        IFdaFunction SelectedCurve { get;  }
        //IFdaFunction BaseFunction { get; }
        string SelectedElementName { get; }
        Utilities.ChildElement SelectedElement { get; }
        bool IsPoppedOut { get; set; }
        

        //voids
        void OKClicked();
        void CancelClicked();
        void PopTheImporterOut();
    }
}
