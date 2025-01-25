namespace HEC.FDA.Model.paireddata.Interfaces
{
    //TODO: This class does not have utility in the current design 
    //REMOVE
    public interface IPairedData : ISample, IComposable, IIntegrate, IMultiply
    {
        double[] Xvals { get; }
        double[] Yvals { get; }
        void ForceMonotonicity(double max = double.MaxValue, double min = double.MinValue);
    }
}