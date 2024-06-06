namespace HEC.FDA.ImporterTest;

using Importer;
using Xunit;

public class ObjectCopierShould
{
    private static OccupancyType InitOccupancyType()
    {
        OccupancyType aOccType = new()
        {
            _ErrorDistribution = new ErrorDistribution[]
            {
                new()
                {
                    ErrorType = ErrorType.NONE,
                    CentralValue = Study.badNumber,
                    StandardDeviationOrMin = Study.badNumber,
                    Maximum = Study.badNumber,
                },
               new()
                {
                    ErrorType = ErrorType.NONE,
                    CentralValue = Study.badNumber,
                    StandardDeviationOrMin = Study.badNumber,
                    Maximum = Study.badNumber,
                },
               new()
                {
                    ErrorType = ErrorType.UNIFORM,
                    CentralValue = Study.badNumber,
                    StandardDeviationOrMin = Study.badNumber,
                    Maximum = Study.badNumber,
                },
                 new()
                {
                    ErrorType = ErrorType.NONE,
                    CentralValue = Study.badNumber,
                    StandardDeviationOrMin = Study.badNumber,
                    Maximum = Study.badNumber,
                },
                 new()
                {
                    ErrorType = ErrorType.TRIANGULAR,
                    CentralValue = Study.badNumber,
                    StandardDeviationOrMin = Study.badNumber,
                    Maximum = Study.badNumber,
                }
            },
            _SingleDamageFunction = new SingleDamageFunction[]
            {
                new()
                {
                    NumOrdinatesAlloc = 10,
                    NumOrdinates = 10,
                    ErrorType = ErrorType.NONE,
                    DirectDollar = false,
                    Depth = new double[10],
                    Damage = new double[10],
                    StdDev = new double[10],
                    ErrHi = new double[10]
                },
                new()
                {
                    NumOrdinatesAlloc = 10,
                    NumOrdinates = 10,
                    ErrorType = ErrorType.NONE,
                    DirectDollar = false,
                    Depth = new double[10],
                    Damage = new double[10],
                    StdDev = new double[10],
                    ErrHi = new double[10]
                },
                 new()
                {
                    NumOrdinatesAlloc = 10,
                    NumOrdinates = 10,
                    ErrorType = ErrorType.NONE,
                    DirectDollar = false,
                    Depth = new double[10],
                    Damage = new double[10],
                    StdDev = new double[10],
                    ErrHi = new double[10]
                },
                 new()
                {
                    NumOrdinatesAlloc = 10,
                    NumOrdinates = 10,
                    ErrorType = ErrorType.NONE,
                    DirectDollar = false,
                    Depth = new double[10],
                    Damage = new double[10],
                    StdDev = new double[10],
                    ErrHi = new double[10]
                },
                new()
                {
                    NumOrdinatesAlloc = 10,
                    NumOrdinates = 10,
                    ErrorType = ErrorType.NONE,
                    DirectDollar = false,
                    Depth = new double[10],
                    Damage = new double[10],
                    StdDev = new double[10],
                    ErrHi = new double[10]
                },
            },
            CategoryId = 4,
            CategoryName = "Whatever",
            UsesDollar = true,
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

        foreach(SingleDamageFunction sdf in aOccType._SingleDamageFunction)
        {
            // Populate Depth array with random doubles
            Random random = new Random();
            for (int i = 0; i < sdf.Depth.Length; i++)
            {
                sdf.Depth[i] = random.NextDouble();
            }

            // Populate Damage array with random doubles
            for (int i = 0; i < sdf.Damage.Length; i++)
            {
                sdf.Damage[i] = random.NextDouble();
            }

            // Populate StdDev array with random doubles
            for (int i = 0; i < sdf.StdDev.Length; i++)
            {
                sdf.StdDev[i] = random.NextDouble();
            }

            // Populate ErrHi array with random doubles
            for (int i = 0; i < sdf.ErrHi.Length; i++)
            {
                sdf.ErrHi[i] = random.NextDouble();
            }
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

        //OccupancyType Props
        Assert.Equal(aOccType.CategoryId, clone.CategoryId);
        Assert.Equal(aOccType.CategoryName, clone.CategoryName);
        Assert.Equal(aOccType.UsesDollar, clone.UsesDollar);
        Assert.Equal(aOccType.RatioContent, clone.RatioContent);
        Assert.Equal(aOccType.RatioOther, clone.RatioOther);
        Assert.Equal(aOccType.RatioCar, clone.RatioCar);

        //FdDataObject Props (OccupancyTypeParent)
        Assert.Equal(aOccType.Id, clone.Id);
        Assert.Equal(aOccType.Name, clone.Name);
        Assert.Equal(aOccType.NameShort, clone.NameShort);
        Assert.Equal(aOccType.Description, clone.Description);
        Assert.Equal(aOccType.CalculationDate, clone.CalculationDate);
        Assert.Equal(aOccType.New, clone.New);
        Assert.Equal(aOccType.IsValid, clone.IsValid);
        Assert.Equal(aOccType.IsOutOfDate, clone.IsOutOfDate);
        Assert.Equal(aOccType.SortClass, clone.SortClass);
        Assert.Equal(aOccType.SortOrder, clone.SortOrder);
        Assert.Equal(aOccType.MetaData, clone.MetaData);

        //Damage Function Props
        for (int i = 0; i < aOccType._SingleDamageFunction.Length; i++)
        {
            SingleDamageFunction damfunc = aOccType._SingleDamageFunction[i];
            SingleDamageFunction cloneDamfunc = clone._SingleDamageFunction[i];
            Assert.Equal(damfunc.NumOrdinatesAlloc, cloneDamfunc.NumOrdinatesAlloc);
            Assert.Equal(damfunc.NumOrdinates, cloneDamfunc.NumOrdinates);
            Assert.Equal(damfunc.ErrorType, cloneDamfunc.ErrorType);
            Assert.Equal(damfunc.DirectDollar, cloneDamfunc.DirectDollar);
            for (int j = 0; j < aOccType._SingleDamageFunction[i].Depth.Length; j++)
            {
                Assert.Equal(damfunc.Depth[j], cloneDamfunc.Depth[j]);
                Assert.Equal(damfunc.Damage[j], cloneDamfunc.Damage[j]);
                Assert.Equal(damfunc.StdDev[j], cloneDamfunc.StdDev[j]);
                Assert.Equal(damfunc.ErrHi[j], cloneDamfunc.ErrHi[j]);
            }
        }

        //Error Distribution Props
        for (int i = 0; i < aOccType._ErrorDistribution.Length; i++)
        {
            ErrorDistribution errDist = aOccType._ErrorDistribution[i];
            ErrorDistribution cloneErrDist = clone._ErrorDistribution[i];
            Assert.Equal(errDist.ErrorType, cloneErrDist.ErrorType);
            Assert.Equal(errDist.CentralValue, cloneErrDist.CentralValue);
            Assert.Equal(errDist.StandardDeviationOrMin, cloneErrDist.StandardDeviationOrMin);
            Assert.Equal(errDist.Maximum, cloneErrDist.Maximum);
            Assert.Equal(errDist.ErrorTypeCode, cloneErrDist.ErrorTypeCode);
        }   



    }
}
