using System;
using System.Collections.Generic;
namespace paireddata
{
    public interface IPairedData : ISample, IComposable, IIntegrate, IMultiply
    {
        IList<double> xs();
        IList<double> ys();
        void add_pair(double x, double y);
    }
}