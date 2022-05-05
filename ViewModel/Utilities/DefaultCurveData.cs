using paireddata;
using Statistics;
using Statistics.Distributions;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Utilities
{
    public static class DefaultCurveData
    {

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

        public static UncertainPairedData ExteriorInteriorDefaultCurve()
        {
            return new UncertainPairedData(_ExteriorInteriorXValues.ToArray(), _ExteriorInteriorYValues.ToArray(), StringConstants.EXT_STAGE, StringConstants.INT_STAGE, StringConstants.EXT_INT);
        }

        public static UncertainPairedData GraphicalDefaultCurve()
        {
            return new UncertainPairedData(_GraphicalXValues.ToArray(), _GraphicalYValues.ToArray(), StringConstants.FREQUENCY, StringConstants.DISCHARGE, StringConstants.GRAPHICAL_FREQUENCY);
        }

        public static UncertainPairedData GraphicalStageFreqDefaultCurve()
        {
            return new UncertainPairedData(_GraphicalStageFreqXValues.ToArray(), _GraphicalStageFreqYValues.ToArray(), StringConstants.FREQUENCY, StringConstants.DISCHARGE, StringConstants.GRAPHICAL_STAGE_FREQUENCY);
        }

        public static UncertainPairedData RegulatedUnregulatedDefaultCurve()
        {
            return new UncertainPairedData(_RegulatedUnregulatedXValues.ToArray(), _RegulatedUnregulatedYValues.ToArray(), StringConstants.REGULATED, StringConstants.UNREGULATED, StringConstants.REGULATED_UNREGULATED);
        }

        public static UncertainPairedData StageDamageDefaultCurve()
        {
            return new UncertainPairedData(_StageDamageXValues.ToArray(), _StageDamageYValues.ToArray(), StringConstants.STAGE, StringConstants.DAMAGE, StringConstants.STAGE_DAMAGE);
        }

        public static UncertainPairedData StageDischargeDefaultCurve()
        {
            return new UncertainPairedData(_StageDischargeXValues.ToArray(), _StageDischargeYValues.ToArray(), StringConstants.STAGE, StringConstants.DISCHARGE, StringConstants.STAGE_DISCHARGE);
        }

        public static UncertainPairedData FailureDefaultCurve()
        {
            return new UncertainPairedData(_FailureXValues.ToArray(), _FailureYValues.ToArray(), StringConstants.STAGE, StringConstants.FREQUENCY, StringConstants.FAILURE_FREQUENCY);
        }

    }
}
