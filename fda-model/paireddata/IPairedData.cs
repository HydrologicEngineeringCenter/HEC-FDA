using System;
using System.Collections.Generic;
namespace paireddata
{
    public interface IPairedData : ISample, IComposable, IIntegrate, IMultiply, IMetaData
    {
        double[] Xvals { get; }
        double[] Yvals { get; }
        void ForceMonotonic(double max = double.MaxValue, double min = double.MinValue);
    }
}