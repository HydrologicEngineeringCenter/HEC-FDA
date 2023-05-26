using HEC.FDA.Model.compute;
using HEC.FDA.Model.extensions;
using HEC.FDA.Model.paireddata;
using Statistics.Distributions;
using Xunit;

namespace HEC.FDA.ModelTest.unittests.extensions
{
    [Trait("RunsOn","Remote")]
    public class ContinuousDistributionExtensionsShould
    {
        private static LogPearson3 lp3 = new LogPearson3(3.5, 0.22, 0.1, 60);
        private static double[] _RequiredExceedanceProbabilities = { 0.99900, 0.99000, 0.95000, 0.90000, 0.85000, 0.80000, 0.75000, 0.70000, 0.65000, 0.60000, 0.55000, 0.50000, 0.47500, 0.45000, 0.42500, 0.40000, 0.37500, 0.35000, 0.32500, 0.30000, 0.29000, 0.28000, 0.27000, 0.26000, 0.25000, 0.24000, 0.23000, 0.22000, 0.21000, 0.20000, 0.19500, 0.19000, 0.18500, 0.18000, 0.17500, 0.17000, 0.16500, 0.16000, 0.15500, 0.15000, 0.14500, 0.14000, 0.13500, 0.13000, 0.12500, 0.12000, 0.11500, 0.11000, 0.10500, 0.10000, 0.09500, 0.09000, 0.08500, 0.08000, 0.07500, 0.07000, 0.06500, 0.06000, 0.05900, 0.05800, 0.05700, 0.05600, 0.05500, 0.05400, 0.05300, 0.05200, 0.05100, 0.05000, 0.04900, 0.04800, 0.04700, 0.04600, 0.04500, 0.04400, 0.04300, 0.04200, 0.04100, 0.04000, 0.03900, 0.03800, 0.03700, 0.03600, 0.03500, 0.03400, 0.03300, 0.03200, 0.03100, 0.03000, 0.02900, 0.02800, 0.02700, 0.02600, 0.02500, 0.02400, 0.02300, 0.02200, 0.02100, 0.02000, 0.01950, 0.01900, 0.01850, 0.01800, 0.01750, 0.01700, 0.01650, 0.01600, 0.01550, 0.01500, 0.01450, 0.01400, 0.01350, 0.01300, 0.01250, 0.01200, 0.01150, 0.01100, 0.01050, 0.01000, 0.00950, 0.00900, 0.00850, 0.00800, 0.00750, 0.00700, 0.00650, 0.00600, 0.00550, 0.00500, 0.00490, 0.00450, 0.00400, 0.00350, 0.00300, 0.00250, 0.00200, 0.00195, 0.00190, 0.00185, 0.00180, 0.00175, 0.00170, 0.00165, 0.00160, 0.00155, 0.00150, 0.00145, 0.00140, 0.00135, 0.00130, 0.00125, 0.00120, 0.00115, 0.00110, 0.00105, 0.00100, 0.00095, 0.00090, 0.00085, 0.00080, 0.00075, 0.00070, 0.00065, 0.00060, 0.00055, 0.00050, 0.00045, 0.00040, 0.00035, 0.00030, 0.00025, 0.00020, 0.00015, 0.00010 };

        public ContinuousDistributionExtensionsShould()
        {
            
        }

        [Fact]
        //https://www.hec.usace.army.mil/confluence/sspdocs/ssptutorialsguides/expected-moments-algorithm-study-guide/user-defined-functions-for-lpiii-and-gev-frequency-curves#UserDefinedFunctionsforLPIIIandGEVFrequencyCurves-Exercise
        public void BootstrapToPairedDataInCorrectFormat()
        {
            //Arrange
            RandomProvider rp = new();

            //Act
            PairedData pd = lp3.BootstrapToPairedData(rp.NextRandomSequence(lp3.SampleSize), _RequiredExceedanceProbabilities);

            //Assert
            Assert.True(pd.Xvals[0] < pd.Xvals[1]); //probs increase
            Assert.True(pd.Yvals[0] < pd.Yvals[1]); //vals increase
        }

        [Fact]
        public void BootstrapToUPDInCorrectFormat()
        {
            //Arrange
            RandomProvider rp = new();

            //Act
            UncertainPairedData upd = lp3.BootstrapToUncertainPairedData(rp.NextRandomSequenceSet(10,lp3.SampleSize), _RequiredExceedanceProbabilities);

            //Assert
            Assert.NotEqual(upd.Yvals[0].InverseCDF(.5), upd.Yvals[50].InverseCDF(.5));
        }
    }
}
