namespace HEC.FDA.ImporterTest;

using Importer;
using Xunit;

[Trait("RunsOn", "Remote")]
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
        OccupancyType original = InitOccupancyType();

        //act
        OccupancyType clone = ObjectCopier.Clone(original);

        //assert
        Assert.NotSame(original, clone);

        //OccupancyType Props
        Assert.Equal(original.CategoryId, clone.CategoryId);
        Assert.Equal(original.CategoryName, clone.CategoryName);
        Assert.Equal(original.UsesDollar, clone.UsesDollar);
        Assert.Equal(original.RatioContent, clone.RatioContent);
        Assert.Equal(original.RatioOther, clone.RatioOther);
        Assert.Equal(original.RatioCar, clone.RatioCar);

        //FdDataObject Props (OccupancyTypeParent)
        Assert.Equal(original.Id, clone.Id);
        Assert.Equal(original.Name, clone.Name);
        Assert.Equal(original.NameShort, clone.NameShort);
        Assert.Equal(original.Description, clone.Description);
        Assert.Equal(original.CalculationDate, clone.CalculationDate);
        Assert.Equal(original.New, clone.New);
        Assert.Equal(original.IsValid, clone.IsValid);
        Assert.Equal(original.IsOutOfDate, clone.IsOutOfDate);
        Assert.Equal(original.SortClass, clone.SortClass);
        Assert.Equal(original.SortOrder, clone.SortOrder);
        Assert.Equal(original.MetaData, clone.MetaData);

        //Damage Function Props
        for (int i = 0; i < original._SingleDamageFunction.Length; i++)
        {
            SingleDamageFunction damfunc = original._SingleDamageFunction[i];
            SingleDamageFunction cloneDamfunc = clone._SingleDamageFunction[i];
            Assert.Equal(damfunc.NumOrdinatesAlloc, cloneDamfunc.NumOrdinatesAlloc);
            Assert.Equal(damfunc.NumOrdinates, cloneDamfunc.NumOrdinates);
            Assert.Equal(damfunc.ErrorType, cloneDamfunc.ErrorType);
            Assert.Equal(damfunc.DirectDollar, cloneDamfunc.DirectDollar);
            for (int j = 0; j < original._SingleDamageFunction[i].Depth.Length; j++)
            {
                Assert.Equal(damfunc.Depth[j], cloneDamfunc.Depth[j]);
                Assert.Equal(damfunc.Damage[j], cloneDamfunc.Damage[j]);
                Assert.Equal(damfunc.StdDev[j], cloneDamfunc.StdDev[j]);
                Assert.Equal(damfunc.ErrHi[j], cloneDamfunc.ErrHi[j]);
            }
        }

        //Error Distribution Props
        for (int i = 0; i < original._ErrorDistribution.Length; i++)
        {
            ErrorDistribution errDist = original._ErrorDistribution[i];
            ErrorDistribution cloneErrDist = clone._ErrorDistribution[i];
            Assert.Equal(errDist.ErrorType, cloneErrDist.ErrorType);
            Assert.Equal(errDist.CentralValue, cloneErrDist.CentralValue);
            Assert.Equal(errDist.StandardDeviationOrMin, cloneErrDist.StandardDeviationOrMin);
            Assert.Equal(errDist.Maximum, cloneErrDist.Maximum);
            Assert.Equal(errDist.ErrorTypeCode, cloneErrDist.ErrorTypeCode);
        }   



    }
}
