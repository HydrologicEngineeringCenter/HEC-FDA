namespace HEC.FDA.Model.interfaces
{
    public interface IProvideRandomNumbers
    {
        double NextRandom();
        double[] NextRandomSequence(long size);
        int Seed { get; }
    }
}
