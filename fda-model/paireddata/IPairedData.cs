using System;
using System.Collections.Generic;
namespace paireddata
{
    public interface IPairedData : ISample, IComposable, IIntegrate, IMultiply
    {
        double[] Xvals { get; }
        double[] Yvals { get; }
    }
}