using System;
using Model.Inputs;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModelTests.InputsTests
{
    [TestClass()]
    public class WaterSurfaceProfileTests
    {
        [TestMethod()]
        public void Validate_GoodXSDataReturnsTrue()
        {
            List<Statistics.CurveIncreasing> profiles = new List<Statistics.CurveIncreasing>();
            profiles.Add(new Statistics.CurveIncreasing(new double[8] { 0, 1, 2, 3, 4, 5, 6, 7 }, new double[8] { 01, 10, 20, 30, 40, 50, 60, 70 }, true, false));
            profiles.Add(new Statistics.CurveIncreasing(new double[8] { 1, 2, 3, 4, 5, 6, 7, 8 }, new double[8] { 11, 21, 31, 41, 51, 61, 71, 81 }, true, false));
           
            Dictionary<float, Statistics.CurveIncreasing> dictionary = new Dictionary<float, Statistics.CurveIncreasing>();
            dictionary.Add(0, profiles[0]);
            dictionary.Add(1, profiles[1]);

            WaterSurfaceProfiles test = new WaterSurfaceProfiles(dictionary, new double[8] { 0.5, 0.2, 0.1, 0.04, 0.02, 0.01, 0.005, 0.002 });
            Assert.IsTrue(test.IsValid);
        }

        [TestMethod()]
        public void Validate_UnsortedGoodXSDataReturnsTrue()
        {
            List<Statistics.CurveIncreasing> profiles = new List<Statistics.CurveIncreasing>();
            profiles.Add(new Statistics.CurveIncreasing(new double[8] { 1, 2, 3, 4, 5, 6, 7, 8 }, new double[8] { 11, 21, 31, 41, 51, 61, 71, 81 }, true, false));
            profiles.Add(new Statistics.CurveIncreasing(new double[8] { 0, 1, 2, 3, 4, 5, 6, 7 }, new double[8] { 01, 10, 20, 30, 40, 50, 60, 70 }, true, false));

            Dictionary<float, Statistics.CurveIncreasing> dictionary = new Dictionary<float, Statistics.CurveIncreasing>();
            dictionary.Add(1, profiles[0]);
            dictionary.Add(0, profiles[1]);

            WaterSurfaceProfiles test = new WaterSurfaceProfiles(dictionary, new double[8] { 0.5, 0.2, 0.1, 0.04, 0.02, 0.01, 0.005, 0.002 });
            Assert.IsTrue(test.IsValid);
        }

        [TestMethod()]
        public void Validate_NonMonotonicOrdinatesReturnFalse()
        {
            List<Statistics.CurveIncreasing> profiles = new List<Statistics.CurveIncreasing>();
            profiles.Add(new Statistics.CurveIncreasing(new double[8] { 1, 2, 1, 4, 5, 6, 7, 8 }, new double[8] { 11, 21, 11, 41, 51, 61, 71, 81 }, true, false));
            profiles.Add(new Statistics.CurveIncreasing(new double[8] { 0, 1, 2, 3, 4, 5, 6, 7 }, new double[8] { 01, 10, 20, 30, 40, 50, 60, 70 }, true, false));

            Dictionary<float, Statistics.CurveIncreasing> dictionary = new Dictionary<float, Statistics.CurveIncreasing>();
            dictionary.Add(2, profiles[1]);
            dictionary.Add(1, profiles[0]);

            WaterSurfaceProfiles test = new WaterSurfaceProfiles(dictionary, new double[8] { 0.5, 0.2, 0.1, 0.04, 0.02, 0.01, 0.005, 0.002 });
            Assert.IsFalse(test.IsValid);
        }

        [TestMethod()]
        public void Validate_NonMonotonicXSProfilesReturnFalse()
        {
            List<Statistics.CurveIncreasing> profiles = new List<Statistics.CurveIncreasing>();
            profiles.Add(new Statistics.CurveIncreasing(new double[8] { 1, 2, 1, 4, 5, 6, 7, 8 }, new double[8] { 11, 21, 11, 41, 51, 61, 71, 81 }, true, false));
            profiles.Add(new Statistics.CurveIncreasing(new double[8] { 0, 1, 2, 3, 4, 5, 6, 7 }, new double[8] { 01, 10, 20, 30, 40, 50, 60, 70 }, true, false));

            Dictionary<float, Statistics.CurveIncreasing> dictionary = new Dictionary<float, Statistics.CurveIncreasing>();
            dictionary.Add(2, profiles[0]); // 1st one has smaller flows needs upstream crossection number
            dictionary.Add(1, profiles[1]); // 0th one has bigger flows needs downstream crossection number

            WaterSurfaceProfiles test = new WaterSurfaceProfiles(dictionary, new double[8] { 0.5, 0.2, 0.1, 0.04, 0.02, 0.01, 0.005, 0.002 });
            Assert.IsFalse(test.IsValid);
        }
    }
}
