using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Inputs.Functions;
using Model.Inputs.Functions.ImpactAreaFunctions;
using Model.Inputs.Functions.PercentDamageFunctions;
using Model.Utilities.DataGenerators;

namespace ModelTests.Utilities.DataGenerators
{
    [TestClass()]
    public sealed class FunctionGeneratorsTests
    {
        #region LPIIIGenerator() Tests
        [TestMethod()]
        public void LogPearsonIIIGenerator_100FunctionsAllValid()
        {
            bool HasFailed = false;
            Random numberGenerator = new Random(1);
            for (int i = 0; i < 100; i++)
            {
                IFunctionCompose testFunction = FunctionGenerators.LogPersonIIIGenerator(numberGenerator.Next());
                if (testFunction.IsValid == false) { HasFailed = true; break; }
            
            }
            Assert.IsFalse(HasFailed);
        }
        #endregion

        #region TransformDomainGenerator() Tests
        [TestMethod()]
        public void TransformDomainGenerator_ApproximateDomainWithNoSpanReturnsFalse()
        {
            double[] approximateDomain = new double[2] { 1, 1 };
            try
            {
                FunctionGenerators.TransformFunctionDomainGenerator(approximateDomain);
                Assert.Fail(); // No exception was thrown if it gets here.
            }
            catch (ArgumentException)
            {
                //Will pass if it makes it to this line.
            }
        }
        [TestMethod()]
        public void TransformDomainGenerator_SingleDomainValueReturnsFalse()
        {
            double[] approximateDomain = new double[1] { 1 };
            try
            {
                FunctionGenerators.TransformFunctionDomainGenerator(approximateDomain);
                Assert.Fail(); // No exception was thrown if it gets here.
            }
            catch (ArgumentException)
            {
                //Will pass if it makes it to this line.
            }
        }
        [TestMethod()]
        public void TransformDomainGenerator_100DomainMinsAreLessThanMaxes()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] testDomain = new double[2] { 1, 2 }, domain;
            for (int i = 0; i < 100; i++)
            {
                domain = FunctionGenerators.TransformFunctionDomainGenerator(testDomain, numberGenerator.Next());
                if (domain[0] > testDomain[1]) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformDomainGenerator_100DomainMaxesAreGreaterThanDomainMins()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] testDomain = new double[2] { 1, 2 }, domain;
            for (int i = 0; i < 100; i++)
            {
                domain = FunctionGenerators.TransformFunctionDomainGenerator(testDomain, numberGenerator.Next());
                if (domain[1] < testDomain[0]) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformDomainGenerator_100DomainsAlwaysIncreasing()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] testDomain = new double[2] { 1, 2 }, domain;
            for (int i = 0; i < 100; i++)
            {
                domain = FunctionGenerators.TransformFunctionDomainGenerator(testDomain, numberGenerator.Next());
                for (int j = 1; j < domain.Length; j++) if (domain[j - 1] > domain[j]) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        #endregion

        #region TransformRangeBoundsGenerator() Tests
        [TestMethod()]
        public void TransformRangeBoundsGenerator_WrongEnumThrowsArgumentException()
        {
            double[] domain = new double[2] { 1, 2 };
            for (int i = 0; i < 11; i++)
            {
                if (i == 0 ||
                    i == 10 ||
                    i % 2 == 0) continue;
                else
                {
                    try
                    {
                        FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, (ImpactAreaFunctionEnum)i);
                        Assert.Fail();
                    }
                    catch (ArgumentException)
                    {
                        //If it hits this line its OK.
                    }
                }
            }
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100InflowOutflowRangeMinsAreLessThanDomainMins()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.InflowOutflow, numberGenerator.Next());
                if (range[0] > domain[0]) hasFailed = true; 
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100InflowOutflowRangeMaxesAreGreaterThanDomainMidpoints()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.InflowOutflow, numberGenerator.Next());
                if (range[1] < domain[1] * 0.5d) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_AtLeast1of1000InflowOutflowRangeMaxesAreGreaterThanDoubleDomainMaxes()
        {
            bool hasFailed = true;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 1000; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.InflowOutflow, numberGenerator.Next());
                if (range[1] > domain[1]*2) hasFailed = false;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100RatingRangeMinsAreGreaterThanNegative1371DeadSeaLandElevation()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.Rating, numberGenerator.Next());
                if (range[0] < -1371) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100RatingRangeMinsAreLessThan28971EverestMinus100Elevation()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.Rating, numberGenerator.Next());
                if (range[0] > 28971) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100RatingRangeMaxesAreGreaterThanRangeMins()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.Rating, numberGenerator.Next());
                if (!(range[1] > range[0])) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100RatingRangeMaxesAreLessThanRangeMinsPlus100()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.Rating, numberGenerator.Next());
                if (range[1] > range[0] + 100) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100ExteriorInteriorRangeMinsAreLessThanDomainMins()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.ExteriorInteriorStage, numberGenerator.Next());
                if (range[0] > domain[0]) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100ExteriorInteriorRangeMinsAreGreaterThanDomainMinsPlus100()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.ExteriorInteriorStage, numberGenerator.Next());
                if (range[0] < domain[0] - 100) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100ExteriorInteriorRangeMaxesAreGreaterThanRangeMin()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.ExteriorInteriorStage, numberGenerator.Next());
                if (range[1] < range[0] ) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100ExteriorInteriorRangeMaxesAreLessThanOrEqualToDomainMaxes()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.ExteriorInteriorStage, numberGenerator.Next());
                if (range[1] > domain[1]) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_ApproximatlyHalfOf100ExteriorInteriorRangeMaxesAreEqualToDomainMaxes()
        {
            int maxCount = 0, n = 100;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < n; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.ExteriorInteriorStage, numberGenerator.Next());
                if (range[1] == domain[1]) maxCount++;
            }
            Assert.AreEqual(0.50d, Convert.ToDouble(maxCount) / Convert.ToDouble(n), 0.2d);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100InteriorStageDamageRangeMinsAreEqualTo0()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.InteriorStageDamage, numberGenerator.Next());
                if (range[0] != 0) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100InteriorStageDamageRangeMaxesAreGreaterThan100()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.InteriorStageDamage, numberGenerator.Next());
                if (range[1] < 100) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100InteriorStageDamageRangeMaxesAreLessThan100000()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.InteriorStageDamage, numberGenerator.Next());
                if (range[1] > 100000) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100LeveeFailureRangeMinsAreEqualTo0()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.LeveeFailure, numberGenerator.Next());
                if (range[0] != 0) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100LeveeFailureRangeMaxesAreGreaterThan0()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.LeveeFailure, numberGenerator.Next());
                if (!(range[1] > 0)) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_100LeveeFailureRangeMaxesAreLessThan1()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.LeveeFailure, numberGenerator.Next());
                if (!(range[1] > 0)) hasFailed = true;
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_Approximately25of100LeveeFailureRangeMaxesAreLessThan50Percent()
        {
            int maxCounter = 0;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.LeveeFailure, numberGenerator.Next());
                if (range[1] < 0.50d) maxCounter++;
            }
            Assert.AreEqual(25, maxCounter, 10);
        }
        [TestMethod()]
        public void TransformRangeBoundsGenerator_Approximately20of100LeveeFailureRangeMaxesAre100Percent()
        {
            int maxCounter = 0;
            Random numberGenerator = new Random(1);
            double[] domain = new double[2] { 1, 2 }, range;
            for (int i = 0; i < 100; i++)
            {
                range = FunctionGenerators.TransformFunctionRangeBoundsGenerator(domain, ImpactAreaFunctionEnum.LeveeFailure, numberGenerator.Next());
                if (range[1] == 1) maxCounter++;
            }
            Assert.AreEqual(20, maxCounter, 10);
        }
        #endregion

        #region TransformFunctionRangeGenerator() Tests
        [TestMethod()]
        public void StandardNormalRandomNumberGenerator_10000RandomsApproximately68PercentWithin1StandardDeviation()
        {
            double x;
            int count = 0;
            Random numberGenerator = new Random(1);
            for (int i = 0; i < 10000; i++)
            {
                x = FunctionGenerators.StandardNormalRandomNumberGenerator(numberGenerator);
                if (Math.Abs(x) < 1) count++;
            }
            Assert.AreEqual(6800, count, 1000);
        }
        [TestMethod()]
        public void StandardNormalRandomNumberGenerator_10000RandomsApproximately95PercentWithin2StandardDeviation()
        {
            double x;
            int count = 0;
            Random numberGenerator = new Random(1);
            for (int i = 0; i < 10000; i++)
            {
                x = FunctionGenerators.StandardNormalRandomNumberGenerator(numberGenerator);
                if (Math.Abs(x) < 2) count++;
            }
            Assert.AreEqual(9500, count, 500);
        }
        [TestMethod()]
        public void StandardNormalRandomNumberGenerator_10000RandomsApproximately99PlusPercentWithin3StandardDeviation()
        {
            double x;
            int count = 0;
            Random numberGenerator = new Random(1);
            for (int i = 0; i < 10000; i++)
            {
                x = FunctionGenerators.StandardNormalRandomNumberGenerator(numberGenerator);
                if (Math.Abs(x) < 3) count++;
            }
            Assert.AreEqual(9970, count, 50);
        }
        [TestMethod()]
        public void TransformFunctionRangeGenerator_100RangesForEachTransformAllIncreasing()
        {
            bool hasFailed = false;
            Random r = new Random();
            ImpactAreaFunctionEnum type;
 
            for (int i = 0; i < 11; i++)
            {
                double[] domain = { 37, 0, 2, 100 }, ys;
                if (i != 0 && i % 2 == 0)
                {
                    type = (ImpactAreaFunctionEnum)i;
                    ys = FunctionGenerators.TransformFunctionRangeGenerator(domain, type, r.Next());
                    for (int j = 1; j < ys.Length; j++)
                    {
                        if (ys[j] < ys[j - 1]) hasFailed = true;
                    } 
                }
            }
            Assert.IsFalse(hasFailed);
        }
        #endregion

        #region TransformUncertainRangeGenerator() Tests
        [TestMethod()]
        public void TransformUncertainRangeGenerator_NormalInputEnumReturnsNormalDistribution()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { 1, 2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (ys[i].GetType() != typeof(Statistics.Normal)) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformUncertainRangeGenerator_TraingularInputEnumReturnsTriangularDistribution()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { 1, 2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (ys[i].GetType() != typeof(Statistics.Triangular)) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformUncertainRangeGenerator_UniformInputEnumReturnsUniformDistribution()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { 1, 2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (ys[i].GetType() != typeof(Statistics.Uniform)) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformUncertainRangeGenerator_NoneInputEnumReturnsNormalTriangularUniformDistribution()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { 1, 2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.None;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (ys[i].GetType().BaseType != typeof(Statistics.ContinuousDistribution)) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformUncertainRangeGenerator_NormalMeansAreEqualToCentralTendencyInputs()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { 1, 2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (((Statistics.Normal)ys[i]).GetMean != rangeCentralTendencies[i]) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformUncertainRangeGenerator_NormalStandardDeviationsAreLessThan25PercentMeanValue()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { 1, 2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (((Statistics.Normal)ys[i]).GetStDev > ((Statistics.Normal)ys[i]).GetMean * 0.25) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformUncertainRangeGenerator_TriangularMostLikelyAreEqualToCentralTendencyInputs()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { 1, 2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (((Statistics.Triangular)ys[i]).getMostlikely != ((Statistics.Triangular)ys[i]).getMostlikely) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformUncertainRangeGenerator_PositiveTriangularMinsGreaterThanMostLikely()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { 1, 2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (((Statistics.Triangular)ys[i]).getMin > ((Statistics.Triangular)ys[i]).getMostlikely) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformUncertainRangeGenerator_NegativeTriangularMinsGreaterThanMostLikely()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { -1, -2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (((Statistics.Triangular)ys[i]).getMin > ((Statistics.Triangular)ys[i]).getMostlikely) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformUncertainRangeGenerator_PositiveTriangularMaxesGreaterThanMostLikely()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { 1, 2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (((Statistics.Triangular)ys[i]).getMax < ((Statistics.Triangular)ys[i]).getMostlikely) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformUncertainRangeGenerator_NegativeTriangularMaxesGreaterThanMostLikely()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { -1, -2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (((Statistics.Triangular)ys[i]).getMax < ((Statistics.Triangular)ys[i]).getMostlikely) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        public void TransformUncertainRangeGenerator_UniformCentralTendenciesAreEqualToCentralTendencyInputs()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { 1, 2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (((Statistics.Uniform)ys[i]).GetCentralTendency != rangeCentralTendencies[i]) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformUncertainRangeGenerator_PositiveUniformMinsGreaterThanMostLikely()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { 1, 2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (((Statistics.Uniform)ys[i]).GetMin > ((Statistics.Uniform)ys[i]).GetCentralTendency) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformUncertainRangeGenerator_NegativeUniformMinsGreaterThanMostLikely()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { -1, -2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (((Statistics.Uniform)ys[i]).GetMin > ((Statistics.Uniform)ys[i]).GetCentralTendency) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformUncertainRangeGenerator_PositiveUniformMaxesGreaterThanMostLikely()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { 1, 2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (((Statistics.Uniform)ys[i]).GetMax < ((Statistics.Uniform)ys[i]).GetCentralTendency) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void TransformUncertainRangeGenerator_NegativeUniformMaxesGreaterThanMostLikely()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            double[] rangeCentralTendencies = new double[2] { -1, -2 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum distributionType = Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform;
            Statistics.ContinuousDistribution[] ys = FunctionGenerators.TransformUncertainRangeBoundGenerator(rangeCentralTendencies, ref distributionType, numberGenerator.Next());
            for (int i = 0; i < ys.Length; i++) if (((Statistics.Uniform)ys[i]).GetMax < ((Statistics.Uniform)ys[i]).GetCentralTendency) hasFailed = true;
            Assert.IsFalse(hasFailed);
        }
        #endregion

        #region TranformFunctionGenerator() Tests
        [TestMethod()]
        public void TranformFunctionGenerator_NotSetAndFrequencyFunctionInputEnumsThrowArgumentExceptions()
        {
            double[] approximateDomain = new double[] {  1, 2 };
            for (int i = 0; i < 11; i++)
            {
                ImpactAreaFunctionEnum type = (ImpactAreaFunctionEnum)i;
                if (type == ImpactAreaFunctionEnum.NotSet||
                    (int)type % 2 == 1)
                    try
                    {
                        FunctionGenerators.TransformFunctionGenerator(ref approximateDomain, type);
                        Assert.Fail();
                    }
                    catch (ArgumentException)
                    {
                        //it passed
                    }
            }
        }
        [TestMethod()]
        public void TransformFunctionGenerator_NotSetInputEnumThrowsArgumentException()
        {
            double[] approximateDomain = new double[] { 1, 2 };
            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.NotSet;
            try
            {
                FunctionGenerators.TransformFunctionGenerator(ref approximateDomain, type);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                //it passed
            }
        }
        [TestMethod()]
        public void TransformFunctionGenerator_FrequencyFunctionInputEnumsThrowArgumentExceptions()
        {
            double[] approximateDomain = new double[] { 1, 2 };
            for (int i = 0; i < 11; i++)
            {
                ImpactAreaFunctionEnum type = (ImpactAreaFunctionEnum)i;
                if ((int)type % 2 == 1)
                    try
                    {
                        FunctionGenerators.TransformFunctionGenerator(ref approximateDomain, type);
                        Assert.Fail();
                    }
                    catch (ArgumentException)
                    {
                        //it passed
                    }
            }
        }

        #endregion

        #region DepthPercentDamageGenerator() Tests
        [TestMethod()]
        public void DepthPercentDamageFunctionGenerator_NoUncertaintyReturnsValidResult()
        {
            IPercentDamageFunction testObject = FunctionGenerators.DepthPercentDamageFunctionGenerator(false, 1);
            Assert.IsTrue(testObject.IsValid);
        }
        [TestMethod()]
        public void DepthPercentDamageFunctionGenerator_UncertainRangeReturnsValidResult()
        {
            IPercentDamageFunction testObject = FunctionGenerators.DepthPercentDamageFunctionGenerator(true, 1);
            Assert.IsTrue(testObject.IsValid);
        }
        [TestMethod()]
        public void DepthPercentDamageFunctionGenerator_100RandomsNoUncertaintyAllRetureValidResults()
        {
            bool hasFailed = false;
            Random numberGenerator = new Random(1);
            for (int i = 0; i < 100; i++)
            {
                IPercentDamageFunction testObject = FunctionGenerators.DepthPercentDamageFunctionGenerator(false, numberGenerator.Next());
                if (testObject.IsValid == false)
                {
                    hasFailed = true;
                }
            } 
            Assert.IsFalse(hasFailed);
        }
        #endregion

        #region UncertainPercentDamageRangeGenerator() Tests
        [TestMethod()]
        public void UncertainPercentDamageRangeGenerator_NormalRangeAllPercentDamageValuesLessThan1atPlus3SD()
        {

            bool hasFailed = false;
            double[] rangeMean = new double[5] { 0, 0.25, 0.50, 0.75, 1 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum normalDistribution = Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal;
            Statistics.ContinuousDistribution[] testDistributions = FunctionGenerators.UncertainPercentDamageRangeGenerator(rangeMean, ref normalDistribution, 1);
            Statistics.Normal[] testObject = new Statistics.Normal[testDistributions.Length];
            for (int i = 0; i < testObject.Length; i++) testObject[i] = testDistributions[i] as Statistics.Normal;
            foreach(Statistics.Normal y in testObject)
            {
                if (y.GetMean + y.GetStDev * 3 > 1)
                {
                    hasFailed = true;
                } 
            }
            Assert.IsFalse(hasFailed);            
        }
        [TestMethod()]
        public void UncertainPercentDamageRangeGenerator_NormalRangeAllPercentDamageValuesGreaterThan0atMinus3SD()
        {

            bool hasFailed = false;
            double[] rangeMean = new double[5] { 0, 0.25, 0.50, 0.75, 1 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum normalDistribution = Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal;
            Statistics.ContinuousDistribution[] testDistributions = FunctionGenerators.UncertainPercentDamageRangeGenerator(rangeMean, ref normalDistribution, 1);
            Statistics.Normal[] testObject = new Statistics.Normal[testDistributions.Length];
            for (int i = 0; i < testObject.Length; i++) testObject[i] = testDistributions[i] as Statistics.Normal;
            foreach (Statistics.Normal y in testObject)
            {
                if (y.GetMean - y.GetStDev * 3 < 0)
                {
                    hasFailed = true;
                }
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void UncertainPercentDamageRangeGenerator_TriangularRangeAllPercentDamageValuesLessThan1()
        {

            bool hasFailed = false;
            double[] rangeMean = new double[5] { 0, 0.25, 0.50, 0.75, 1 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum normalDistribution = Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular;
            Statistics.ContinuousDistribution[] testDistributions = FunctionGenerators.UncertainPercentDamageRangeGenerator(rangeMean, ref normalDistribution, 1);
            Statistics.Triangular[] testObject = new Statistics.Triangular[testDistributions.Length];
            for (int i = 0; i < testObject.Length; i++) testObject[i] = testDistributions[i] as Statistics.Triangular;
            foreach (Statistics.Triangular y in testObject)
            {
                if (y.getMax > 1)
                {
                    hasFailed = true;
                }
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void UncertainPercentDamageRangeGenerator_TriangularRangeAllPercentDamageValuesGreaterThan0()
        {

            bool hasFailed = false;
            double[] rangeMean = new double[5] { 0, 0.25, 0.50, 0.75, 1 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum normalDistribution = Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular;
            Statistics.ContinuousDistribution[] testDistributions = FunctionGenerators.UncertainPercentDamageRangeGenerator(rangeMean, ref normalDistribution, 1);
            Statistics.Triangular[] testObject = new Statistics.Triangular[testDistributions.Length];
            for (int i = 0; i < testObject.Length; i++) testObject[i] = testDistributions[i] as Statistics.Triangular;
            foreach (Statistics.Triangular y in testObject)
            {
                if (y.getMin < 0)
                {
                    hasFailed = true;
                }
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void UncertainPercentDamageRangeGenerator_UniformRangeAllPercentDamageValuesLessThan1()
        {

            bool hasFailed = false;
            double[] rangeMean = new double[5] { 0, 0.25, 0.50, 0.75, 1 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum normalDistribution = Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform;
            Statistics.ContinuousDistribution[] testDistributions = FunctionGenerators.UncertainPercentDamageRangeGenerator(rangeMean, ref normalDistribution, 1);
            Statistics.Uniform[] testObject = new Statistics.Uniform[testDistributions.Length];
            for (int i = 0; i < testObject.Length; i++) testObject[i] = testDistributions[i] as Statistics.Uniform;
            foreach (Statistics.Uniform y in testObject)
            {
                if (y.GetMax > 1)
                {
                    hasFailed = true;
                }
            }
            Assert.IsFalse(hasFailed);
        }
        [TestMethod()]
        public void UncertainPercentDamageRangeGenerator_UniformRangeAllPercentDamageValuesGreaterThan0()
        {

            bool hasFailed = false;
            double[] rangeMean = new double[5] { 0, 0.25, 0.50, 0.75, 1 };
            Statistics.UncertainCurveDataCollection.DistributionsEnum normalDistribution = Statistics.UncertainCurveDataCollection.DistributionsEnum.Uniform;
            Statistics.ContinuousDistribution[] testDistributions = FunctionGenerators.UncertainPercentDamageRangeGenerator(rangeMean, ref normalDistribution, 1);
            Statistics.Uniform[] testObject = new Statistics.Uniform[testDistributions.Length];
            for (int i = 0; i < testObject.Length; i++) testObject[i] = testDistributions[i] as Statistics.Uniform;
            foreach (Statistics.Uniform y in testObject)
            {
                if (y.GetMin < 0)
                {
                    hasFailed = true;
                }
            }
            Assert.IsFalse(hasFailed);
        }
        #endregion

    }
}
