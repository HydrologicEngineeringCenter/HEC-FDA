using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Plots
{
    public interface ILinkedPlot
    {

        double MaxX { get; set; }
        double MaxY { get; set; }
        double MinX { get; set; }
        double MinY { get; set; }

        Model.IFdaFunction Function{get;set;}

        //FdaModel.Functions.BaseFunction BaseFunction { get; set; }
        SharedAxisEnum NextPlotSharedAxisEnum { get; set; }
        bool IsStartNode { get; set; }
        bool IsEndNode { get; set; }
        bool FreezeNextTracker { get; set; }
        bool FreezePreviousTracker { get; set; }
        bool TrackerIsOutsideTheCurveRange { get; set; }
        bool FlipFrequencyAxis { get; set; }
        //todo: Refactor: CO
        //Statistics.CurveIncreasing Curve { get; set; }
        void DisplayNextTracker(double x, double y);

        void DisplayPreviousTracker(double x, double y);

        //void SetAsStartNode();
        //void SetAsEndNode();
        string SelectedElementName { get; set; }

        void SetNextPlotLinkage(ILinkedPlot plot);
        void SetPreviousPlotLinkage(ILinkedPlot plot);

        void TurnOutsideOfRangeOn();
        void TurnOutsideOfRangeOff();

    }
}
