using HEC.FDA.Model.paireddata;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Utilities
{
    public static class DefaultCurveData
    {
        #region Private Default Curve Data
        private static double minFlow = 900;
        private static double maxFlow = 10000;
        private static double minStage = 463;
        private static double maxStage = 482;
        private static double minDepth = -2;
        private static double maxDepth = 7;
        private static double minDamage = 0;
        private static double maxDamage = 2500;
        private static int coordinateQuantity = 10;

        private static List<double> _GraphicalXValues = new List<double>() { .5, .2, .1, .04, .02, .01, .004, .002 };
        private static List<IDistribution> _GraphicalYValues = new List<IDistribution>()
        {
            new Deterministic(1500),
            new Deterministic(2120),
            new Deterministic(3140),
            new Deterministic(4210),
            new Deterministic(5070),
            new Deterministic(6240),
            new Deterministic(7050),
            new Deterministic(9680)
        };

        private static List<double> _GraphicalStageFreqXValues = new List<double>() { .999, .5, .2, .1, .04, .02, .01, .004, .002 };
        private static List<IDistribution> _GraphicalStageFreqYValues = new List<IDistribution>()
        {
            new Deterministic(458),
            new Deterministic(468.33),
            new Deterministic(469.97),
            new Deterministic(471.95),
            new Deterministic(473.06),
            new Deterministic(437.66),
            new Deterministic(474.53),
            new Deterministic(475.11),
            new Deterministic(477.4)
        };
        private static List<double> _FailureXValues = new List<double>() { 458, 468, 470, 471, 472, 472, 473, 474 };
        private static List<IDistribution> _FailureYValues = new List<IDistribution>()
        {
            new Deterministic(0),
            new Deterministic(.01),
            new Deterministic(.05),
            new Deterministic(.07),
            new Deterministic(.1),
            new Deterministic(.8),
            new Deterministic(.9),
            new Deterministic(1),
        };
        #endregion

        #region Default Convergence Criteria
        public static double ConvergenceConfidence = 95;
        public static double ConvergenceTolerance = .01;
        public static int ConvergenceMinIterations = 1000;
        public static int ConvergenceMaxIterations = 500000;
        #endregion

        #region Default LP3 Parameter Values
        public static double LP3Mean = 3.3;
        public static double LP3StDev = .254;
        public static double LP3Skew = -.1021;
        public static int LP3POR = 48;
        #endregion

        #region Occupancy Type Default Parameter Values
        public static double ValueUncertaintyNormalStandardDeviation = 10;
        public static double ValueUncertaintyMin = 10;
        public static double ValueUncertaintyMax = 20;
        public static Deterministic ValueRatioUncertaintyDeterministic = new Deterministic(50);
        public static Normal ValueRatioUncertaintyNormal = new Normal(50, 5);
        public static Triangular ValueRatioUncertaintyTriangular = new Triangular(45, 50, 60);
        public static Uniform ValueRatioUncertaintyUniform = new Uniform(45, 60);
        public static double FirstFloorUncertaintyNormalStandardDeviation = .5;
        public static double FirstFloorUncertaintyMin = .5;
        public static double FirstFloorUncertaintyMax = 1;
        #endregion

        #region Other Default Parameter Values
        public static double DefaultLeveeElevation = 476;
        #endregion

        #region Default Curves 
        public static UncertainPairedData GraphicalFlowFreqDefaultCurve()
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.FREQUENCY, StringConstants.DISCHARGE, StringConstants.GRAPHICAL_FREQUENCY);
            return new UncertainPairedData(_GraphicalXValues.ToArray(), _GraphicalYValues.ToArray(), curveMetaData);
        }
        public static UncertainPairedData GraphicalStageFreqDefaultCurve()
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.FREQUENCY, StringConstants.DISCHARGE, StringConstants.GRAPHICAL_STAGE_FREQUENCY);
            return new UncertainPairedData(_GraphicalStageFreqXValues.ToArray(), _GraphicalStageFreqYValues.ToArray(), curveMetaData);
        }
        public static UncertainPairedData RegulatedUnregulatedDefaultCurve(IDistributionEnum distributionEnum = IDistributionEnum.Deterministic)
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.UNREGULATED, StringConstants.REGULATED, StringConstants.REGULATED_UNREGULATED);
            List<double> xValues = new List<double>();
            List<IDistribution> yValues = new List<IDistribution>();
            double greatestDiffIndex = 4.5;
            for (int i = 0; i < coordinateQuantity; i++)
            {
                IDistribution regulatedFlowDistribution = new Normal();
                double unregulatedFlow = minFlow + (maxFlow - minFlow) * (i / coordinateQuantity);
                double regulatedFlow = unregulatedFlow + 1 / Math.Pow(i - greatestDiffIndex, 2);
                double min = regulatedFlow - regulatedFlow * 0.1;
                double max = regulatedFlow + regulatedFlow * 1.3;
                switch (distributionEnum)
                {
                    case IDistributionEnum.Deterministic:
                        regulatedFlowDistribution = new Deterministic(regulatedFlow);
                        break;
                    case IDistributionEnum.Normal:
                        double standardDeviation = regulatedFlow / 10;
                        regulatedFlowDistribution = new Normal(regulatedFlow, standardDeviation);
                        break;
                    case IDistributionEnum.Triangular:

                        regulatedFlowDistribution = new Triangular(min, regulatedFlow, max);
                        break;
                    case IDistributionEnum.LogNormal:
                        double logRegulatedFlow = Math.Log(regulatedFlow);
                        double logStandardDeviation = logRegulatedFlow / 10;
                        regulatedFlowDistribution = new LogNormal(logRegulatedFlow, logStandardDeviation);
                        break ;
                    case IDistributionEnum.Uniform:
                        regulatedFlowDistribution = new Uniform(min, max);
                        break;
                }
                xValues.Add(unregulatedFlow);
                yValues.Add(regulatedFlowDistribution);
            }
            return new UncertainPairedData(xValues.ToArray(), yValues.ToArray(), curveMetaData);
        }
        public static UncertainPairedData StageDischargeDefaultCurve(IDistributionEnum distributionEnum = IDistributionEnum.Deterministic)
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.STAGE, StringConstants.DISCHARGE, StringConstants.STAGE_DISCHARGE);
            List<double> xValues = new List<double>();
            List<IDistribution> yValues = new List<IDistribution>();
            for (int i = 0; i < coordinateQuantity; i++)
            {
                IDistribution stageDistribution = new Normal();
                double discharge = minFlow + (maxFlow - minFlow) * (i / coordinateQuantity);
                double stage = minStage + (maxStage - minStage)*(i / coordinateQuantity);
                double min = stage - stage * 0.1;
                double max = stage + stage * 1.3;
                switch (distributionEnum)
                {
                    case IDistributionEnum.Deterministic:
                        stageDistribution = new Deterministic(stage);
                        break;
                    case IDistributionEnum.Normal:
                        double standardDeviation = stage / 10;
                        stageDistribution = new Normal(stage, standardDeviation);
                        break;
                    case IDistributionEnum.Triangular:

                        stageDistribution = new Triangular(min, stage, max);
                        break;
                    case IDistributionEnum.LogNormal:
                        double logStage = Math.Log(stage);
                        double logStandardDeviation = logStage / 10;
                        stageDistribution = new LogNormal(logStage, logStandardDeviation);
                        break;
                    case IDistributionEnum.Uniform:
                        stageDistribution = new Uniform(min, max);
                        break;
                }
                xValues.Add(discharge);
                yValues.Add(stageDistribution);
            }
            return new UncertainPairedData(xValues.ToArray(), yValues.ToArray(), curveMetaData);
        }
        public static UncertainPairedData ExteriorInteriorDefaultCurve(IDistributionEnum distributionEnum = IDistributionEnum.Deterministic)
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.EXT_STAGE, StringConstants.INT_STAGE, StringConstants.EXT_INT);
            List<double> xValues = new List<double>();
            List<IDistribution> yValues = new List<IDistribution>();
            double greatestDiffIndex = 4.5;
            for (int i = 0; i < coordinateQuantity; i++)
            {
                IDistribution interiorStageDistribution = new Normal();
                double exteriorStage = minStage + (maxStage - minStage) * (i / coordinateQuantity);
                double interiorStage = exteriorStage + 1 / Math.Pow(i - greatestDiffIndex, 2);
                double min = interiorStage - interiorStage * 0.1;
                double max = interiorStage + interiorStage * 1.3;
                switch (distributionEnum)
                {
                    case IDistributionEnum.Deterministic:
                        interiorStageDistribution = new Deterministic(interiorStage);
                        break;
                    case IDistributionEnum.Normal:
                        double standardDeviation = interiorStage / 10;
                        interiorStageDistribution = new Normal(interiorStage, standardDeviation);
                        break;
                    case IDistributionEnum.Triangular:
                        interiorStageDistribution = new Triangular(min, interiorStage, max);
                        break;
                    case IDistributionEnum.LogNormal:
                        double logInteriorStage = Math.Log(interiorStage);
                        double logStandardDeviation = logInteriorStage / 10;
                        interiorStageDistribution = new LogNormal(logInteriorStage, logStandardDeviation);
                        break;
                    case IDistributionEnum.Uniform:
                        interiorStageDistribution = new Uniform(min, max);
                        break;
                }
                xValues.Add(exteriorStage);
                yValues.Add(interiorStageDistribution);
            }
            return new UncertainPairedData(xValues.ToArray(), yValues.ToArray(), curveMetaData);
        }
        public static UncertainPairedData FailureDefaultCurve()
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.STAGE, StringConstants.FREQUENCY, StringConstants.FAILURE_FREQUENCY);
            return new UncertainPairedData(_FailureXValues.ToArray(), _FailureYValues.ToArray(), curveMetaData);
        }
        public static UncertainPairedData DepthPercentDamageDefaultCurve(IDistributionEnum distributionEnum = IDistributionEnum.Deterministic)
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.OCCTYPE_DEPTH, StringConstants.OCCTYPE_PERCENT_DAMAGE, StringConstants.OCCUPANCY_TYPES);
            List<double> xValues = new List<double>();
            List<IDistribution> yValues = new List<IDistribution>();
            for (int i = 0; i < coordinateQuantity; i++)
            {
                IDistribution percentDamageDistribution = new Normal();
                double depth = minDepth + (maxDepth - minDepth) * (i / coordinateQuantity);
                double percentDamage = 0 + (100) * (i / coordinateQuantity);
                double min = percentDamage - percentDamage * 0.1;
                double max = Math.Min(percentDamage + percentDamage * 1.3,100);
                switch (distributionEnum)
                {
                    case IDistributionEnum.Deterministic:
                        percentDamageDistribution = new Deterministic(percentDamage);
                        break;
                    case IDistributionEnum.Normal:
                        double standardDeviation = percentDamage / 10;
                        percentDamageDistribution = new Normal(percentDamage, standardDeviation);
                        break;
                    case IDistributionEnum.Triangular:

                        percentDamageDistribution = new Triangular(min, percentDamage, max);
                        break;
                    case IDistributionEnum.LogNormal:
                        double logStage = Math.Log(percentDamage);
                        double logStandardDeviation = logStage / 10;
                        percentDamageDistribution = new LogNormal(logStage, logStandardDeviation);
                        break;
                    case IDistributionEnum.Uniform:
                        percentDamageDistribution = new Uniform(min, max);
                        break;
                }
                xValues.Add(depth);
                yValues.Add(percentDamageDistribution);
            }
            return new UncertainPairedData(xValues.ToArray(), yValues.ToArray(), curveMetaData);
        }
        public static UncertainPairedData StageDamageDefaultCurve(IDistributionEnum distributionEnum = IDistributionEnum.Deterministic)
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.STAGE, StringConstants.DAMAGE, StringConstants.STAGE_DAMAGE);
            List<double> xValues = new List<double>();
            List<IDistribution> yValues = new List<IDistribution>();
            for (int i = 0; i < coordinateQuantity; i++)
            {
                IDistribution damageDistribution = new Normal();
                double stage = minStage + (maxStage - minStage) * (i / coordinateQuantity);
                double damage = minDamage + (maxDamage - minDamage) * (i / coordinateQuantity);
                double min = damage - damage * 0.1;
                double max = damage + damage * 1.3;
                switch (distributionEnum)
                {
                    case IDistributionEnum.Deterministic:
                        damageDistribution = new Deterministic(damage);
                        break;
                    case IDistributionEnum.Normal:
                        double standardDeviation = damage / 10;
                        damageDistribution = new Normal(damage, standardDeviation);
                        break;
                    case IDistributionEnum.Triangular:

                        damageDistribution = new Triangular(min, damage, max);
                        break;
                    case IDistributionEnum.LogNormal:
                        double logStage = Math.Log(damage);
                        double logStandardDeviation = logStage / 10;
                        damageDistribution = new LogNormal(logStage, logStandardDeviation);
                        break;
                    case IDistributionEnum.Uniform:
                        damageDistribution = new Uniform(min, max);
                        break;
                }
                xValues.Add(stage);
                yValues.Add(damageDistribution);
            }
            return new UncertainPairedData(xValues.ToArray(), yValues.ToArray(), curveMetaData);
        }
        #endregion

    }
}
