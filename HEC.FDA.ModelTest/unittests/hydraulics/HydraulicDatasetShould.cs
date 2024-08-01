using System.Collections.Generic;
using Xunit;
using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.Model.structures;
using System.Linq;
using HEC.FDA.Model.hydraulics.Interfaces;
using HEC.FDA.Model.paireddata;
using NuGet.Frameworks;
using Geospatial.GDALAssist;

namespace HEC.FDA.ModelTest.unittests.hydraulics
{
    [Trait("RunsOn", "Remote")]
    [Collection("Serial")]
    public class HydraulicDatasetShould
    {
        private const string ParentDirectoryToSteadyResult = @"..\..\..\..\HEC.FDA.ModelTest\Resources\MuncieSteadyResult";
        private const string SteadyHDFFileName = @"Muncie.p10.hdf";
        private const string PathToIndexPointShapefile = @"..\..\..\..\HEC.FDA.ModelTest\Resources\MuncieIndexPoints\MuncieIndexPts.shp";
        private const string PathToProjection = @"..\..\..\..\HEC.FDA.ModelTest\Resources\MuncieIndexPoints\MuncieIndexPts.prj";

        public HydraulicDatasetShould()
        {
            Geospatial.GDALAssist.GDALSetup.InitializeMultiplatform();
        }

        [Fact]
        void retreiveGraphicalFrequencyFunctionsAsPairedData()
        {
            //Arrange
            List<IHydraulicProfile> profiles = new List<IHydraulicProfile>();
            profiles.Add( new HydraulicProfile(0.50, SteadyHDFFileName, "2"));
            profiles.Add(new HydraulicProfile(0.002, SteadyHDFFileName, "500"));
            HydraulicDataset dataset = new HydraulicDataset(profiles, HydraulicDataSource.SteadyHDF);
            Projection projection = Projection.FromFile(PathToProjection);

            //Act
            List<UncertainPairedData> graphicalFreqCurves = dataset.GetGraphicalStageFrequency(PathToIndexPointShapefile, ParentDirectoryToSteadyResult, projection);

            //Assert
            Assert.Equal(graphicalFreqCurves.Count, 3);
            Assert.Equal(graphicalFreqCurves[0].Yvals.Length, 2);
        }
        [Fact]
        void SortHydrulicProfilesbyDescendingProbability()
        {
            List<HydraulicProfile> profiles = new List<HydraulicProfile>();
            double[] ExceedenceProbs = new double[] { 0.5, 0.99, 0.01 };
            foreach (double prob in ExceedenceProbs)
            {
                profiles.Add(new HydraulicProfile(prob,"",""));
            }
            HydraulicDataset dataset = new HydraulicDataset(new (profiles), HydraulicDataSource.UnsteadyHDF);
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

            workingProfile = HydraulicDataset.CorrectDryStructureWSEs(workingProfile, firstFloorElevs, nextLargerProfile);

            float[] expected = new float[] { 91, 17, 10 };

            Assert.Equal(workingProfile[0], expected[0]); //structure is very dry
            Assert.Equal(workingProfile[1], expected[1]); //structure is just barely dry
            Assert.Equal(workingProfile[2], expected[2]); //structure is wet
        }
    }
}
