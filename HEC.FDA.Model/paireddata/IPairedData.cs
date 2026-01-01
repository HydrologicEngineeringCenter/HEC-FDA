using System.Collections.Generic;

namespace HEC.FDA.Model.paireddata
{
    //TODO: This class does not have utility in the current design 
    //REMOVE
    public interface IPairedData : ISample, IComposable, IIntegrate, IMultiply
    {
        IReadOnlyList<double> Xvals { get; }
        IReadOnlyList<double> Yvals { get; }
    }
}