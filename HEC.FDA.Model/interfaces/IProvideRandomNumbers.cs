namespace HEC.FDA.Model.interfaces
{
    public interface IProvideRandomNumbers
    {
        double NextRandom();
        double[] NextRandomSequence(long size);
        public double[][] NextRandomSequenceSet(long numArrays, long numMembersInArray);
        int Seed { get; }
    }
}
