using HEC.FDA.Model.paireddata;
using Statistics;
using Statistics.Distributions;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Utilities
{
    public static class DefaultCurveData
    {
        #region Convergence Criteria
        public static double CONFIDENCE = 95;
        public static double TOLERANCE = .01;
        public static int MIN = 1000;
        public static int MAX = 500000;
        #endregion

        private static List<double> _ExteriorInteriorXValues = new List<double>() {474, 474.1, 474.3, 474.5, 478};
        private static List<IDistribution> _ExteriorInteriorYValues = new List<IDistribution>() 
        { 
            new Deterministic(472),
            new Deterministic(473),
            new Deterministic(474),
            new Deterministic(474.1),
            new Deterministic(478)
        };

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

        private static List<double> _GraphicalStageFreqXValues = new List<double>() { .999, .5, .2, .1, .04, .02, .01, .004, .002};
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

        public static double LP3Mean = 3.3;
        public static double LP3StDev = .254;
        public static double LP3Skew = -.1021;
        public static int LP3POR = 48;

        private static List<double> _RegulatedUnregulatedXValues = new List<double>() { 900, 1500, 2120,3140,4210,5070,6240,7050,9680};
        private static List<IDistribution> _RegulatedUnregulatedYValues = new List<IDistribution>()
        {
            new Deterministic(900),
            new Deterministic(1500),
            new Deterministic(2000),
            new Deterministic(2010),
            new Deterministic(2020),
            new Deterministic(2050),
            new Deterministic(5500),
            new Deterministic(7050),
            new Deterministic(9680)
        };

        private static List<double> _StageDamageXValues = new List<double>() { 463,464,465,466,467,468,469,470,471,472,473,474,475,476,477,478,479,480,481,482};
        private static List<IDistribution> _StageDamageYValues = new List<IDistribution>()
        {
            new Normal(0,0),
            new Normal(0,0),
            new Normal(0,0),
            new Normal(.04, .16),
            new Normal(.66,1.02),
            new Normal(2.83,2.47),
            new Normal(7.48,3.55),
            new Normal(17.82,7.38),
            new Normal(39.87, 12.35),
            new Normal(76.91, 13.53),
            new Normal(124.82, 13.87),
            new Normal(173.73, 13.12),
            new Normal(218.32, 12.03),
            new Normal(257.83, 11.1),
            new Normal(292.52, 10.31),
            new Normal(370.12,12.3),
            new Normal(480.94,20.45),
            new Normal(890.76,45.67),
            new Normal(1287.45,62.34),
            new Normal(2376.23,134.896),
        };

        private static List<double> _StageDischargeXValues = new List<double>() { 0,1500,2120,3140,4210,5070,6240,7050,9680 };
        private static List<IDistribution> _StageDischargeYValues = new List<IDistribution>()
        {
            new Normal(458,0),
            new Normal(468.33,.312),
            new Normal(469.97,.362),
            new Normal(471.95,.422),
            new Normal(473.06,.456),
            new Normal(475.66,.474),
            new Normal(477.53,0.5),
            new Normal(479.11,0.5),
            new Normal(481.44, 0.5),
        };

        private static List<double> _FailureXValues = new List<double>() { 458,468,470,471,472,472,473,474 };
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

        public static double DefaultLeveeElevation = 476;

        private static double[] _DepthPercentDamageXValues = new double[] {-2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private static IDistribution[] _DepthPercentDamageYValues = new IDistribution[]
        {
            new Normal(0,0),
            new Normal(2.5,2.7),
            new Normal(13.4, 2.0),
            new Normal(23.3, 1.6),
            new Normal(32.1, 1.6),
            new Normal(40.1, 1.8),
            new Normal(47.1, 1.9),
            new Normal(53.2, 2.0),
            new Normal(58.6, 2.1),
            new Normal(63.2, 2.2),
            new Normal(67.2, 2.3),
            new Normal(70.5, 2.4),
            new Normal(73.2, 2.7),
            new Normal(75.4,3),
            new Normal(77.2, 3.3),
            new Normal(78.5, 3.7),
            new Normal(79.5, 4.1),
            new Normal(80.2, 4.5),
            new Normal(80.7,4.9)

        };


        public static UncertainPairedData DepthPercentDamageDefaultCurve()
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.OCCTYPE_DEPTH, StringConstants.OCCTYPE_PERCENT_DAMAGE, StringConstants.OCCUPANCY_TYPES);
            return new UncertainPairedData(_DepthPercentDamageXValues, _DepthPercentDamageYValues, curveMetaData);
        }
        public static UncertainPairedData ExteriorInteriorDefaultCurve()
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.EXT_STAGE, StringConstants.INT_STAGE, StringConstants.EXT_INT);
            return new UncertainPairedData(_ExteriorInteriorXValues.ToArray(), _ExteriorInteriorYValues.ToArray(), curveMetaData);
        }

        public static UncertainPairedData GraphicalDefaultCurve()
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.FREQUENCY, StringConstants.DISCHARGE, StringConstants.GRAPHICAL_FREQUENCY);
            return new UncertainPairedData(_GraphicalXValues.ToArray(), _GraphicalYValues.ToArray(), curveMetaData);
        }

        public static UncertainPairedData GraphicalStageFreqDefaultCurve()
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.FREQUENCY, StringConstants.DISCHARGE, StringConstants.GRAPHICAL_STAGE_FREQUENCY);
            return new UncertainPairedData(_GraphicalStageFreqXValues.ToArray(), _GraphicalStageFreqYValues.ToArray(), curveMetaData);
        }

        public static UncertainPairedData RegulatedUnregulatedDefaultCurve()
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.UNREGULATED, StringConstants.REGULATED, StringConstants.REGULATED_UNREGULATED);
            return new UncertainPairedData(_RegulatedUnregulatedXValues.ToArray(), _RegulatedUnregulatedYValues.ToArray(), curveMetaData);
        }

        public static UncertainPairedData StageDamageDefaultCurve()
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.STAGE, StringConstants.DAMAGE, StringConstants.STAGE_DAMAGE);
            return new UncertainPairedData(_StageDamageXValues.ToArray(), _StageDamageYValues.ToArray(), curveMetaData);
        }

        public static UncertainPairedData StageDischargeDefaultCurve()
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.STAGE, StringConstants.DISCHARGE, StringConstants.STAGE_DISCHARGE);
            return new UncertainPairedData(_StageDischargeXValues.ToArray(), _StageDischargeYValues.ToArray(), curveMetaData);
        }

        public static UncertainPairedData FailureDefaultCurve()
        {
            CurveMetaData curveMetaData = new CurveMetaData(StringConstants.STAGE, StringConstants.FREQUENCY, StringConstants.FAILURE_FREQUENCY);
            return new UncertainPairedData(_FailureXValues.ToArray(), _FailureYValues.ToArray(), curveMetaData);
        }

    }
}
