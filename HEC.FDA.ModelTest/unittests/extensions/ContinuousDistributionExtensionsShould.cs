using HEC.FDA.Model.compute;
using HEC.FDA.Model.extensions;
using HEC.FDA.Model.paireddata;
using Statistics.Distributions;
using System;
using Xunit;

namespace HEC.FDA.ModelTest.unittests.extensions
{
    [Trait("RunsOn","Remote")]
    public class ContinuousDistributionExtensionsShould
    {
        private static LogPearson3 lp3 = new LogPearson3(3.5, 0.22, 0.1, 60);

        [Fact]
        //https://www.hec.usace.army.mil/confluence/sspdocs/ssptutorialsguides/expected-moments-algorithm-study-guide/user-defined-functions-for-lpiii-and-gev-frequency-curves#UserDefinedFunctionsforLPIIIandGEVFrequencyCurves-Exercise
        public void BootstrapToPairedDataInCorrectFormat()
        {
            //Arrange
            RandomProvider rp = new();

            //Act
            PairedData pd = lp3.BootstrapToPairedData(rp, LogPearson3._RequiredExceedanceProbabilitiesForBootstrapping);

            //Assert
            Assert.True(pd.Xvals[0] > pd.Xvals[1]); //probs decrease
            Assert.True(pd.Yvals[0] < pd.Yvals[1]); //vals increase
        }

        [Fact]
        public void BootstrapToUPDInCorrectFormat()
        {
            //Arrange
            RandomProvider rp = new();

            //Act
            UncertainPairedData upd = lp3.BootstrapToUncertainPairedData(rp, LogPearson3._RequiredExceedanceProbabilitiesForBootstrapping);

            //Assert
            Assert.NotEqual(upd.Yvals[0].InverseCDF(.5), upd.Yvals[50].InverseCDF(.5));
        }
    }
}
