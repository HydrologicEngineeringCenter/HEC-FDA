using System;
namespace paireddata
{
    public interface IPairedDataProducer
    {
        IPairedData SamplePairedData(double probability);
    }
}