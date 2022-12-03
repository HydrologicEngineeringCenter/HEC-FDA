namespace HEC.FDA.Model.paireddata
{
    public interface IPairedDataProducer
    {
        IPairedData SamplePairedData(double probability, bool computeIsDeterministic);
    }
}