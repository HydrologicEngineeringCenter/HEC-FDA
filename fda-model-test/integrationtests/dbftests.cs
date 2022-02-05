using Xunit;
using utilities;

namespace fda_model_test.integrationtests
{
    [Trait("Category", "Integration")]
    public class dbftests
    {
        [Fact]
        public void ReadFile()
        {
            string filepath = @"C:\Users\Q0HECWPL\Downloads\ProbData.dbf";
            dbfreader dbr = new dbfreader(filepath);
            double mean = (double)dbr.GetCell("LOG_MEAN",0);
            Assert.Equal(2.944761, mean);
            double stdev = (double)dbr.GetCell("STD_DEV",0);
            Assert.Equal(0.256108, stdev);
            double skew = (double)dbr.GetCell("SKEW",0);
            Assert.Equal(0.2409, skew);
        }
    }
}
