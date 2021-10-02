
using ead;
using paireddata;

namespace fda_model_test
{
    class simulationShould
    {
        //These were previously used in pairedDataTest but were moved here to be used for ead compute testing. 
        static double[] Probabilities = { .99, .95, .9, .8, .7, .6, .5, .4, .3, .2, .1, .01, .002, .001 };
        static double[] Flows = { 10, 100, 1000, 2000, 2500, 4000, 5000, 10000, 25000, 50000, 100000, 120000, 150000, 175000 };
        static double[] Stages = { 556, 562, 565, 566, 570, 575, 578, 600, 610, 650, 700, 750, 800, 850 };
        static double[] Damages = { 1, 10, 30, 45, 59, 78, 89, 102, 140, 180, 240, 330, 350, 370 };
        static double[] ProbabilitiesOfFailure = { .001, .01, .1, .5, 1 };
        static double[] ElevationsOfFailure = { 600, 610, 650, 700, 750 };

        PairedData myRatingCurve = new PairedData(Flows, Stages);
        PairedData myFlowFrequencyCurve = new PairedData(Probabilities, Flows);
        PairedData myDamageFrequencyCurve = new PairedData(Probabilities, Damages);
        PairedData myFragilityCurve = new PairedData(ElevationsOfFailure, ProbabilitiesOfFailure);
        PairedData myStageDamageCurve = new PairedData(Stages, Damages);
    }
}
