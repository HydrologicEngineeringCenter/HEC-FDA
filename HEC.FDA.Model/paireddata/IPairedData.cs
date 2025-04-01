namespace HEC.FDA.Model.paireddata
{
    //TODO: This class does not have utility in the current design 
    //REMOVE
    public interface IPairedData : ISample, IComposable, IIntegrate, IMultiply
    {
        double[] Xvals { get; }
        double[] Yvals { get; }
    }
}