using HEC.FDA.Model.hydraulics.Mock;
using Xunit;

namespace HEC.FDA.ModelTest.unittests.hydraulics
{
    public class DummyHydraulicProfileShould
    {
        [Fact]
        void ReturnTheFlowsWeSet()
        {
            DummyHydraulicProfile prof = new DummyHydraulicProfile();
            prof.Probability = .5f;
            prof.DummyDepths = new float[] {0,1,2};
            Assert.True(prof.DummyDepths == prof.GetWSE(null,Model.hydraulics.enums.HydraulicDataSource.UnsteadyHDF,null));
        }
    }
}
