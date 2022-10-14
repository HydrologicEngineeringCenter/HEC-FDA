using System.Collections.Generic;
using Xunit;
using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.Model.structures;

namespace HEC.FDA.ModelTest.unittests.hydraulics
{
    [Trait("Category", "Unit")]
    public class HydraulicDatasetShould
    {
        [Fact]
        void SortHydrulicProfilesbyDescendingProbability()
        {
            List<HydraulicProfile> profiles = new List<HydraulicProfile>();
            double[] ExceedenceProbs = new double[] { 0.5, 0.99, 0.01 };
            foreach (double prob in ExceedenceProbs)
            {
                profiles.Add(new HydraulicProfile(prob,"",""));
            }
            HydraulicDataset dataset = new HydraulicDataset(profiles, HydraulicDataSource.UnsteadyHDF);

            double[] expected = new double[] { 0.99, 0.5, 0.01 };
            double[] actual = new double[3];
            int count = 0;
            foreach (HydraulicProfile profile in dataset.HydraulicProfiles)
            {
                actual[count] = profile.Probability;
                count++;
            }
            Assert.Equal(expected, actual);

        }
        [Fact]
        void CorrectDryStructureDepthsAppropriately()
        {

            float[] firstFloorElevs = new float[] { 100, 19, 5 };
            float[] workingProfile = new float[] { -9999, -9999, 10 };
            float[] nextLargerProfile = new float[] { -9999, 20, 50 };

            HydraulicDataset.CorrectDryStructureDepths(ref workingProfile, firstFloorElevs, nextLargerProfile);

            float[] expected = new float[] { 91, 17, 10 };

            Assert.Equal(workingProfile[0], expected[0]); //structure is very dry
            Assert.Equal(workingProfile[1], expected[1]); //structure is just barely dry
            Assert.Equal(workingProfile[2], expected[2]); //structure is wet
        }
    }
}
