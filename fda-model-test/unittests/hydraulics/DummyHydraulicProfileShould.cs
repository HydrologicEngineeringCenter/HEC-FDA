using HEC.FDA.Model.hydraulics.Mock;
using Xunit;

namespace HEC.FDA.ModelTest.unittests.hydraulics
{
    [Trait("Category", "Remote")]
    public class DummyHydraulicProfileShould
    {
        [Fact]
        void ReturnTheFlowsWeSet()
        {
            DummyHydraulicProfile prof = new DummyHydraulicProfile();
            prof.Probability = .5f;
            prof.DummyWSEs = new float[] {0,1,2};
            Assert.True(prof.DummyWSEs == prof.GetWSE(null,Model.hydraulics.enums.HydraulicDataSource.UnsteadyHDF,null));
        }
    }
}
