using System;
using System.Collections.Generic;
namespace paireddata
{
    public interface IPairedData : ISample, IComposable, IIntegrate, IMultiply
    {
        IList<double> Xvals { get; set; }
        IList<double> Yvals { get; set; }
        void add_pair(double x, double y);
    }
}