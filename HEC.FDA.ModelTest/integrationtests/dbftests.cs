using Xunit;
using HEC.FDA.Model.utilities;

namespace HEC.FDA.ModelTest.integrationtests
{
    [Trait("RunsOn", "Remote")]
    [Collection("Serial")]
    public class Dbftests
    {
        [Fact]
        public void ReadFile()
        {
            string filepath = @"..\..\..\..\HEC.FDA.ModelTest\Resources\ProbData.dbf";
            Dbfreader dbr = new Dbfreader(filepath);
            double mean = (double)dbr.GetCell("LOG_MEAN", 0);
            Assert.Equal(2.944761, mean);
            double stdev = (double)dbr.GetCell("STD_DEV", 0);
            Assert.Equal(0.256108, stdev);
            double skew = (double)dbr.GetCell("SKEW", 0);
            Assert.Equal(0.2409, skew);
        }
    }
}
