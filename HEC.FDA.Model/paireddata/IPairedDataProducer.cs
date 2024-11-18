namespace HEC.FDA.Model.paireddata
{
    public interface IPairedDataProducer
    {
        PairedData SamplePairedData(long iteration, bool computeIsDeterministic);
        PairedData SamplePairedData(double probability);
    }
}