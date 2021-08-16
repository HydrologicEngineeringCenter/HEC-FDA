using HEC.Plotting.SciChart2D.Charts;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.Plots
{
    public interface ILinkedPlotControl
    {
        IParameterEnum FunctionType { get; }
        Chart2D Chart { get; }
    }
}
