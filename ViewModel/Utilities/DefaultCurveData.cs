using paireddata;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Utilities
{
    public static class DefaultCurveData
    {

        private static List<double> _ExteriorInteriorXValues = new List<double>() {478, 479, 480, 481};
        private static List<IDistribution> _ExteriorInteriorYValues = new List<IDistribution>() 
        { 
            new Deterministic(460),
            new Deterministic(478),
            new Deterministic(479.5),
            new Deterministic(481)
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

        public static double LP3Mean = 3.1718;
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
            new Normal(0,0),
            new Normal(0,0),
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
            new Normal(292.52, 10.31)
        };

        public static UncertainPairedData ExteriorInteriorDefaultCurve()
        {
            return new UncertainPairedData(_ExteriorInteriorXValues.ToArray(), _ExteriorInteriorYValues.ToArray(), "Exterior Stage", "Interior Stage", "Exterior - Interior");
        }

        public static UncertainPairedData GraphicalDefaultCurve()
        {
            return new UncertainPairedData(_GraphicalXValues.ToArray(), _GraphicalYValues.ToArray(), "Frequency", "Flow", "Graphical Frequency");
        }

        public static UncertainPairedData GraphicalStageFreqDefaultCurve()
        {
            return new UncertainPairedData(_GraphicalStageFreqXValues.ToArray(), _GraphicalStageFreqYValues.ToArray(), "Frequency", "Flow", "Graphical Stage Frequency");
        }

        public static UncertainPairedData RegulatedUnregulatedDefaultCurve()
        {
            return new UncertainPairedData(_RegulatedUnregulatedXValues.ToArray(), _RegulatedUnregulatedYValues.ToArray(), "Regulated Flow", "Unregulated Flow", "Regulated Unregulated");
        }

        public static UncertainPairedData StageDamageDefaultCurve()
        {
            return new UncertainPairedData(_StageDamageXValues.ToArray(), _StageDamageYValues.ToArray(), "Stage", "Damage", "Stage Damage");
        }

    }
}
