using System.Collections.Generic;
using Xunit;
using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.enums;

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
                profiles.Add(new HydraulicProfile(prob, "", HydraulicDataSource.UnsteadyHDF, ""));
            }
            HydraulicDataset dataset = new HydraulicDataset(profiles);

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
    }
}
