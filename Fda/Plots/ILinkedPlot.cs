using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fda.Plots
{
    public interface ILinkedPlot
    {
        
        FdaModel.Functions.BaseFunction BaseFunction { get; set; }
        int NextPlotSharedAxisEnum { get; set; }
        bool ThisIsStartNode { get; set; }
        bool ThisIsEndNode { get; set; }
        bool FreezeNextTracker { get; set; }
        bool FreezePreviousTracker { get; set; }

        bool FlipFrequencyAxis { get; set; }
        Statistics.CurveIncreasing Curve { get; set; }
        void DisplayNextTracker(double x, double y);

        void DisplayPreviousTracker(double x , double y);
        //void SetAsStartNode();
        //void SetAsEndNode();

        void SetNextPlotLinkage(ILinkedPlot plot, string thisAxis, string theirAxis);
        void SetPreviousPlotLinkage(ILinkedPlot plot);

    }
}
