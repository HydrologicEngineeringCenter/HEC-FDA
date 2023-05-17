namespace HEC.FDA.Model.paireddata
{
    public interface IPairedDataProducer
    {
        PairedData SamplePairedData(double probability, bool computeIsDeterministic);
    }
}