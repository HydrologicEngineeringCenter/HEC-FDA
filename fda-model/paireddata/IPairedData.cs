namespace HEC.FDA.Model.paireddata
{
    public interface IPairedData : ISample, IComposable, IIntegrate, IMultiply, IMetaData
    {
        double[] Xvals { get; }
        double[] Yvals { get; }
        void ForceMonotonic(double max = double.MaxValue, double min = double.MinValue);
    }
}