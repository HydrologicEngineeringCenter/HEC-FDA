﻿using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.TableWithPlot;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Utilities
{
    public static class DefaultData
    {
        #region Private Default Curve Data
        private static readonly double minFlow = 900;
        private static readonly double maxFlow = 10000;
        private static readonly double  minStage = 463;
        private static readonly double maxStage = 482;
        private static readonly double minDepth = -2;
        private static readonly double maxDepth = 8;
        private static readonly double minDamage = 0;
        private static readonly double maxDamage = 2500;
        private static readonly int coordinateQuantity = 10;

        private static readonly List<double> _GraphicalXValues = new() {.99, .5, .2, .1, .04, .02, .01, .004, .002 };
        private static readonly List<IDistribution> _GraphicalYValues = new()
        {
            new Deterministic(1200),
            new Deterministic(1500),
            new Deterministic(2120),
            new Deterministic(3140),
            new Deterministic(4210),
            new Deterministic(5070),
            new Deterministic(6240),
            new Deterministic(7050),
            new Deterministic(9680)
        };

        private static readonly List<double> _GraphicalStageFreqXValues = new() { .999, .5, .2, .1, .04, .02, .01, .004, .002 };
        private static readonly List<IDistribution> _GraphicalStageFreqYValues = new()
        {
            new Deterministic(458),
            new Deterministic(468.33),
            new Deterministic(469.97),
            new Deterministic(471.95),
            new Deterministic(473.06),
            new Deterministic(473.66),
            new Deterministic(474.53),
            new Deterministic(475.11),
            new Deterministic(477.4)
        };
        private static readonly List<double> _FailureXValues = new() { 458, 468, 470, 471, 472, 473, 474, 475 };
        private static readonly List<IDistribution> _FailureYValues = new()
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
        public static int ConvergenceMinIterations = 10000;
        public static int ConvergenceMaxIterations = 500000;
        #endregion

        #region Default LP3 Parameter Values
        public static double LP3Mean = 3.3;
        public static double LP3StDev = .254;
        public static double LP3Skew = -.1021;
        public static int PeriodOfRecord = 48;
        #endregion

        #region Occupancy Type Default Parameter Values
        public static double ValueUncertaintyNormalStandardDeviation = 10;
        public static double ValueUncertaintyMin = 10;
        public static double ValueUncertaintyMax = 20;
        public static Deterministic ValueRatioUncertaintyDeterministic = new(50);
        public static Normal ValueRatioUncertaintyNormal = new(50, 5);
        public static Triangular ValueRatioUncertaintyTriangular = new(45, 50, 60);
        public static Uniform ValueRatioUncertaintyUniform = new(45, 60);
        public static double FirstFloorUncertaintyNormalStandardDeviation = .5;
        public static double FirstFloorUncertaintyMin = .5;
        //TODO where should this be used 
        public static double FirstFloorUncertaintyMax = 1;
        #endregion

        #region Other Default Parameter Values
        public static double DefaultLeveeElevation = 474;
        #endregion

        #region Default Curve Component VMs
        public static CurveComponentVM RatingComputeComponent()
        {
            CurveComponentVM curveComponentVM = new(StringConstants.STAGE_DISCHARGE, StringConstants.DISCHARGE, StringConstants.STAGE);
            curveComponentVM.SetPairedData(StageDischargeDefaultCurve(IDistributionEnum.Normal));
            curveComponentVM.SetPairedData(StageDischargeDefaultCurve(IDistributionEnum.Triangular));
            curveComponentVM.SetPairedData(StageDischargeDefaultCurve(IDistributionEnum.Uniform));
            curveComponentVM.SetPairedData(StageDischargeDefaultCurve(IDistributionEnum.LogNormal));
            curveComponentVM.SetPairedData(StageDischargeDefaultCurve(IDistributionEnum.Deterministic));
            return curveComponentVM;
        }
        public static CurveComponentVM UnregulatedRegulatedComputeComponent()
        {
            CurveComponentVM curveComponentVM = new(StringConstants.REGULATED_UNREGULATED, StringConstants.UNREGULATED, StringConstants.REGULATED);
            curveComponentVM.SetPairedData(RegulatedUnregulatedDefaultCurve(IDistributionEnum.Normal));
            curveComponentVM.SetPairedData(RegulatedUnregulatedDefaultCurve(IDistributionEnum.Uniform));
            curveComponentVM.SetPairedData(RegulatedUnregulatedDefaultCurve(IDistributionEnum.LogNormal));
            curveComponentVM.SetPairedData(RegulatedUnregulatedDefaultCurve(IDistributionEnum.Deterministic));
            curveComponentVM.SetPairedData(RegulatedUnregulatedDefaultCurve(IDistributionEnum.Triangular));
            return curveComponentVM;
        }
        public static CurveComponentVM ExteriorInteriorComputeComponent()
        {
            CurveComponentVM curveComponentVM = new(StringConstants.EXT_INT, StringConstants.EXT_STAGE, StringConstants.INT_STAGE);
            curveComponentVM.SetPairedData(ExteriorInteriorDefaultCurve(IDistributionEnum.Normal));
            curveComponentVM.SetPairedData(ExteriorInteriorDefaultCurve(IDistributionEnum.Uniform));
            curveComponentVM.SetPairedData(ExteriorInteriorDefaultCurve(IDistributionEnum.LogNormal));
            curveComponentVM.SetPairedData(ExteriorInteriorDefaultCurve(IDistributionEnum.Triangular));
            curveComponentVM.SetPairedData(ExteriorInteriorDefaultCurve(IDistributionEnum.Deterministic));
            return curveComponentVM;
        }
        public static CurveComponentVM StageDamageCurveComputeComponent()
        {
            CurveComponentVM curveComponentVM = new(StringConstants.STAGE_DAMAGE, StringConstants.STAGE, StringConstants.DAMAGE);
            curveComponentVM.SetPairedData(StageDamageDefaultCurve(IDistributionEnum.Normal));
            curveComponentVM.SetPairedData(StageDamageDefaultCurve(IDistributionEnum.Uniform));
            curveComponentVM.SetPairedData(StageDamageDefaultCurve(IDistributionEnum.LogNormal));
            curveComponentVM.SetPairedData(StageDamageDefaultCurve(IDistributionEnum.Triangular));
            curveComponentVM.SetPairedData(StageDamageDefaultCurve(IDistributionEnum.Deterministic));
            return curveComponentVM;
        }
        public static CurveComponentVM DefaultLeveeComputeComponent()
        {
            CurveComponentVM defaultCurve = new(StringConstants.SYSTEM_RESPONSE_CURVE, StringConstants.STAGE, 
                StringConstants.FAILURE_FREQUENCY, distOptions:DistributionOptions.DETERMINISTIC_ONLY);
            defaultCurve.SetPairedData(DefaultData.FailureDefaultCurve());
            defaultCurve.SetMinMaxValues(0, 1);
            return defaultCurve;
        }
        #endregion

        #region Default Curves 
        public static UncertainPairedData GeneralUseDefaultCurve(IDistributionEnum distributionEnum = IDistributionEnum.Deterministic)
        {
            return StageDischargeDefaultCurve(distributionEnum);
        }
        public static UncertainPairedData GraphicalFlowFreqDefaultCurve()
        {
            CurveMetaData curveMetaData = new(StringConstants.FREQUENCY, StringConstants.DISCHARGE, StringConstants.GRAPHICAL_FREQUENCY);
            return new UncertainPairedData(_GraphicalXValues.ToArray(), _GraphicalYValues.ToArray(), curveMetaData);
        }
        public static UncertainPairedData GraphicalStageFreqDefaultCurve()
        {
            CurveMetaData curveMetaData = new(StringConstants.FREQUENCY, StringConstants.DISCHARGE, StringConstants.GRAPHICAL_STAGE_FREQUENCY);
            return new UncertainPairedData(_GraphicalStageFreqXValues.ToArray(), _GraphicalStageFreqYValues.ToArray(), curveMetaData);
        }
        //TODO how to use this with the new argument 
        private static UncertainPairedData RegulatedUnregulatedDefaultCurve(IDistributionEnum distributionEnum = IDistributionEnum.Deterministic)
        {
            CurveMetaData curveMetaData = new(StringConstants.UNREGULATED, StringConstants.REGULATED, StringConstants.REGULATED_UNREGULATED);
            List<double> xValues = new();
            List<IDistribution> yValues = new();
            double greatestDiffIndex = 4.5;
            for (int i = 0; i < coordinateQuantity; i++)
            {
                IDistribution regulatedFlowDistribution = new Normal();
                double unregulatedFlow = minFlow + (maxFlow - minFlow) * ((double)i / coordinateQuantity);
                double regulatedFlow = unregulatedFlow - 100*(1 / Math.Pow((double)i - greatestDiffIndex, 2));
                double min = regulatedFlow - regulatedFlow * 0.01;
                double max = regulatedFlow + regulatedFlow * 0.07;
                switch (distributionEnum)
                {
                    case IDistributionEnum.Deterministic:
                        regulatedFlowDistribution = new Deterministic(regulatedFlow);
                        break;
                    case IDistributionEnum.Normal:
                        double standardDeviation = regulatedFlow / 115;
                        regulatedFlowDistribution = new Normal(regulatedFlow, standardDeviation);
                        break;
                    case IDistributionEnum.Triangular:

                        regulatedFlowDistribution = new Triangular(min, regulatedFlow, max);
                        break;
                    case IDistributionEnum.LogNormal:
                        double logRegulatedFlow = Math.Log(regulatedFlow);
                        double logStandardDeviation = logRegulatedFlow / 115 + (double)i/coordinateQuantity;
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
        private static UncertainPairedData StageDischargeDefaultCurve(IDistributionEnum distributionEnum = IDistributionEnum.Deterministic)
        {
            CurveMetaData curveMetaData = new(StringConstants.STAGE, StringConstants.DISCHARGE, StringConstants.STAGE_DISCHARGE);
            List<double> xValues = new();
            List<IDistribution> yValues = new();
            for (int i = 0; i < coordinateQuantity; i++)
            {
                IDistribution stageDistribution = new Normal();
                double stepShare = (double)i / coordinateQuantity;
                double discharge = minFlow + (maxFlow - minFlow) * stepShare;
                double stage = minStage + (maxStage - minStage)* stepShare;
                double min = stage - stage * 0.01;
                double max = stage + stage * 0.07;
                switch (distributionEnum)
                {
                    case IDistributionEnum.Deterministic:
                        stageDistribution = new Deterministic(stage);
                        break;
                    case IDistributionEnum.Normal:
                        double standardDeviation = stage / 115;
                        stageDistribution = new Normal(stage, standardDeviation);
                        break;
                    case IDistributionEnum.Triangular:

                        stageDistribution = new Triangular(min, stage, max);
                        break;
                    case IDistributionEnum.LogNormal:
                        double logStage = Math.Log(stage);
                        double logStandardDeviation = logStage / 115;
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
        private static UncertainPairedData ExteriorInteriorDefaultCurve(IDistributionEnum distributionEnum = IDistributionEnum.Deterministic)
        {
            CurveMetaData curveMetaData = new(StringConstants.EXT_STAGE, StringConstants.INT_STAGE, StringConstants.EXT_INT);
            List<double> xValues = new();
            List<IDistribution> yValues = new();
            for (int i = 0; i < coordinateQuantity; i++)
            {
                IDistribution interiorStageDistribution = new Normal();
                double exteriorStage = minStage + (maxStage - minStage) * ((double)i / coordinateQuantity);
                double interiorStage;
                if(i > 0 && i < 7)
                {
                    interiorStage = 464 - 1 + (double)i/coordinateQuantity;
                } else
                {
                    interiorStage = exteriorStage;
                }
                double min = interiorStage - interiorStage * 0.03;
                double max = interiorStage + interiorStage * 0.07;
                switch (distributionEnum)
                {
                    case IDistributionEnum.Deterministic:
                        interiorStageDistribution = new Deterministic(interiorStage);
                        break;
                    case IDistributionEnum.Normal:
                        double standardDeviation = interiorStage / 115;
                        interiorStageDistribution = new Normal(interiorStage, standardDeviation);
                        break;
                    case IDistributionEnum.Triangular:
                        interiorStageDistribution = new Triangular(min, interiorStage, max);
                        break;
                    case IDistributionEnum.LogNormal:
                        double logInteriorStage = Math.Log(interiorStage);
                        double logStandardDeviation = logInteriorStage / 115 + (double)i / coordinateQuantity;
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
        private static UncertainPairedData FailureDefaultCurve()
        {
            CurveMetaData curveMetaData = new(StringConstants.STAGE, StringConstants.FREQUENCY, StringConstants.FAILURE_FREQUENCY);
            return new UncertainPairedData(_FailureXValues.ToArray(), _FailureYValues.ToArray(), curveMetaData);
        }
        public static UncertainPairedData DepthPercentDamageDefaultCurve(IDistributionEnum distributionEnum = IDistributionEnum.Deterministic)
        {
            CurveMetaData curveMetaData = new(StringConstants.OCCTYPE_DEPTH, StringConstants.OCCTYPE_PERCENT_DAMAGE, StringConstants.OCCUPANCY_TYPES);
            List<double> xValues = new();
            List<IDistribution> yValues = new();
            for (int i = 0; i < coordinateQuantity; i++)
            {
                IDistribution percentDamageDistribution = new Normal();
                double depth = minDepth + (maxDepth - minDepth) * ((double)i / coordinateQuantity);
                double percentDamage = 0 + (100) * ((double)i / coordinateQuantity);
                double min = percentDamage - percentDamage * 0.03;
                double max = Math.Min(percentDamage + percentDamage * .07,100);
                switch (distributionEnum)
                {
                    case IDistributionEnum.Deterministic:
                        percentDamageDistribution = new Deterministic(percentDamage);
                        break;
                    case IDistributionEnum.Normal:
                        double standardDeviation = percentDamage / 115;
                        percentDamageDistribution = new Normal(percentDamage, standardDeviation);
                        break;
                    case IDistributionEnum.Triangular:

                        percentDamageDistribution = new Triangular(min, percentDamage, max);
                        break;
                    case IDistributionEnum.LogNormal:
                        double logStage = Math.Log(percentDamage + .5);
                        double logStandardDeviation = logStage / 115 + (double)i / coordinateQuantity; 
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
        private static UncertainPairedData StageDamageDefaultCurve(IDistributionEnum distributionEnum = IDistributionEnum.Deterministic)
        {
            CurveMetaData curveMetaData = new(StringConstants.STAGE, StringConstants.DAMAGE, StringConstants.STAGE_DAMAGE);
            List<double> xValues = new();
            List<IDistribution> yValues = new();
            for (int i = 0; i < coordinateQuantity; i++)
            {
                IDistribution damageDistribution = new Normal();
                double stage = minStage + (maxStage - minStage) * ((double)i / coordinateQuantity);
                double damage = minDamage + (maxDamage - minDamage) * ((double)i / coordinateQuantity);
                double min = damage - damage * 0.03;
                double max = damage + damage * 0.07;
                switch (distributionEnum)
                {
                    case IDistributionEnum.Deterministic:
                        damageDistribution = new Deterministic(damage);
                        break;
                    case IDistributionEnum.Normal:
                        double standardDeviation = damage / 105 + i*2;
                        damageDistribution = new Normal(damage, standardDeviation);
                        break;
                    case IDistributionEnum.Triangular:

                        damageDistribution = new Triangular(min, damage, max);
                        break;
                    case IDistributionEnum.LogNormal:
                        double tmpDamage = 5;
                        double logStage = Math.Log(tmpDamage+damage);
                        double logStandardDeviation = logStage / 115 + (double)i / coordinateQuantity;
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
