namespace HEC.FDA.ImporterTest;

using Importer;
using Xunit;

public class ObjectCopierShould
{
    private static OccupancyType InitOccupancyType()
    {
        OccupancyType aOccType = new OccupancyType()
        {
            _ErrorDistribution = new ErrorDistribution[]
            {
                new ErrorDistribution()
                {
                    ErrorType = ErrorType.NONE,
                    CentralValue = Study.badNumber,
                    StandardDeviationOrMin = Study.badNumber,
                    Maximum = Study.badNumber,
                }
            },
            _SingleDamageFunction = new SingleDamageFunction[]
            {
                new SingleDamageFunction()
                {
                    NumOrdinatesAlloc = 10,
                    NumOrdinates = 10,
                    ErrorType = ErrorType.NONE,
                    DirectDollar = false,
                    Depth = new double[10],
                    Damage = new double[10],
                    StdDev = new double[10],
                    ErrHi = new double[10]
                }
            },
            CategoryId = 4,
            CategoryName = null,
            UsesDollar = false,
            RatioContent = 2.0,
            RatioOther = 4.0,
            RatioCar = 4.0,
            Id = 4,
            Name = "OccupancyType",
            NameShort = "OT",
            Description = "Occupancy Type",
            CalculationDate = DateTime.Now.ToString(),
            New = true,
            IsValid = true,
            IsOutOfDate = false,
            SortClass = 4,
            SortOrder = 1,
            MetaData = "Occupancy Type Metadata"
        };

        // Populate Depth array with random doubles
        Random random = new Random();
        for (int i = 0; i < aOccType._SingleDamageFunction[0].Depth.Length; i++)
        {
            aOccType._SingleDamageFunction[0].Depth[i] = random.NextDouble();
        }

        // Populate Damage array with random doubles
        for (int i = 0; i < aOccType._SingleDamageFunction[0].Damage.Length; i++)
        {
            aOccType._SingleDamageFunction[0].Damage[i] = random.NextDouble();
        }

        // Populate StdDev array with random doubles
        for (int i = 0; i < aOccType._SingleDamageFunction[0].StdDev.Length; i++)
        {
            aOccType._SingleDamageFunction[0].StdDev[i] = random.NextDouble();
        }

        // Populate ErrHi array with random doubles
        for (int i = 0; i < aOccType._SingleDamageFunction[0].ErrHi.Length; i++)
        {
            aOccType._SingleDamageFunction[0].ErrHi[i] = random.NextDouble();
        }
        return aOccType;
    }
    [Fact]
    public void DeepCopyAnOccupancyType()
    {
        //arrange
        OccupancyType aOccType = InitOccupancyType();

        //act
        OccupancyType clone = ObjectCopier.Clone(aOccType);

        //assert
        Assert.NotSame(aOccType, clone);
        Assert.True(aOccType.IsEqualTo(clone));
    }

}
