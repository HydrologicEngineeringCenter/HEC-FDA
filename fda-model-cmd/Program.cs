// See https://aka.ms/new-console-template for more information
using Base.Events;
using compute;
using paireddata;
using Statistics;
using Statistics.Distributions;

Console.WriteLine("Hello, World!");

System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
sw.Start();
int iterations = 1000000;
int seed = 2345;
 IDistribution LP3Distribution = new LogPearson3(3.537, .438, .075, 125);
 double[] RatingCurveFlows = { 0, 1500, 2120, 3140, 4210, 5070, 6240, 7050, 9680 };

string xLabel = "x label";
 string yLabel = "y label";
 string name = "name";
 string description = "description";
int id = 1;


 IDistribution[] StageDistributions =
{
            new Normal(458,0.00001),
            new Normal(468.33,.312),
            new Normal(469.97,.362),
            new Normal(471.95,.422),
            new Normal(473.06,.456),
            new Normal(473.66,.474),
            new Normal(474.53,.5),
            new Normal(475.11,.5),
            new Normal(477.4,.5)
                //note that the rating curve domain lies within the stage-damage domain
        };
 double[] StageDamageStages = { 470, 471, 472, 473, 474, 475, 476, 477, 478, 479 };
 IDistribution[] DamageDistrbutions =
{
            new Normal(0,0.00001),
            new Normal(.04,.16),
            new Normal(.66,1.02),
            new Normal(2.83,2.47),
            new Normal(7.48,3.55),
            new Normal(17.82,7.38),
            new Normal(39.87,12.35),
            new Normal(76.91,13.53),
            new Normal(124.82,13.87),
            new Normal(173.73,13.12),
        };

 double[] FragilityStages = { 470, 471, 472, 473, 474, 475 };
 IDistribution[] FragilityProbabilities =
{
            new Deterministic(0),
            new Deterministic(.1),
            new Deterministic(.2),
            new Deterministic(.5),
            new Deterministic(.75),
            new Deterministic(1)
        };






double topOfLeveeElevation = 475;
Statistics.ContinuousDistribution flowFrequency = new Statistics.Distributions.LogPearson3(3.537, .438, .075, 125);
UncertainPairedData flowStage = new UncertainPairedData(RatingCurveFlows, StageDistributions, xLabel, yLabel, name, description);
UncertainPairedData stageDamage = new UncertainPairedData(StageDamageStages, DamageDistrbutions, xLabel, yLabel, name, description, "residential");
List<UncertainPairedData> stageDamageList = new List<UncertainPairedData>();
stageDamageList.Add(stageDamage);

double epsilon = 0.0001;
double[] leveestages = new double[] { 0.0d, topOfLeveeElevation - epsilon, topOfLeveeElevation };
IDistribution[] leveefailprobs = new IDistribution[3];
for (int i = 0; i < 2; i++)
{
    leveefailprobs[i] = new Statistics.Distributions.Deterministic(0); //probability at the top must be 1
}
leveefailprobs[2] = new Statistics.Distributions.Deterministic(1);
UncertainPairedData leveeFragilityFunction = new UncertainPairedData(leveestages, leveefailprobs, "stages", "failure probabilities", "default function", "internally configured default function");

Simulation simulation = Simulation.builder()
    .withFlowFrequency(flowFrequency)
    .withFlowStage(flowStage)
    .withStageDamages(stageDamageList)
    .withLevee(leveeFragilityFunction, topOfLeveeElevation)
    .build();
compute.RandomProvider randomProvider = new RandomProvider(seed);
ConvergenceCriteria cc = new ConvergenceCriteria(minIterations: 1000, maxIterations: iterations);
simulation.ProgressReport += WriteProgress;

void WriteProgress(object sender, ProgressReportEventArgs progress)
{
    Console.WriteLine("compute progress: " + progress.Progress);
}

metrics.Results results = simulation.Compute(randomProvider, cc);

double EAD = results.ExpectedAnnualDamageResults.MeanEAD("residential");
Console.WriteLine("EAD was " + EAD);
double meanActualAEP = results.PerformanceByThresholds.ThresholdsDictionary[0].ProjectPerformanceResults.MeanAEP();
Console.WriteLine("AEP was " + meanActualAEP);
double cnp90 = results.PerformanceByThresholds.ThresholdsDictionary[0].ProjectPerformanceResults.ConditionalNonExceedanceProbability(.9);
Console.WriteLine("CNEP(.90) was " + cnp90);
double cnp98 = results.PerformanceByThresholds.ThresholdsDictionary[0].ProjectPerformanceResults.ConditionalNonExceedanceProbability(.98);
Console.WriteLine("CNEP(.98) was " + cnp98);
double cnp99 = results.PerformanceByThresholds.ThresholdsDictionary[0].ProjectPerformanceResults.ConditionalNonExceedanceProbability(.99);
Console.WriteLine("CNEP(.99) was " + cnp99);
double cnp996 = results.PerformanceByThresholds.ThresholdsDictionary[0].ProjectPerformanceResults.ConditionalNonExceedanceProbability(.996);
Console.WriteLine("CNEP(.996) was " + cnp996);
double cnp998 = results.PerformanceByThresholds.ThresholdsDictionary[0].ProjectPerformanceResults.ConditionalNonExceedanceProbability(.998);
Console.WriteLine("CNEP(.998) was " + cnp998);

if (results.IsConverged())
{
    Console.WriteLine("Converged");
    Console.WriteLine(results.ExpectedAnnualDamageResults.HistogramsOfEADs["Total"].SampleSize + " iterations completed");
}
else
{
    Console.WriteLine("Not Converged");
    Console.WriteLine(results.ExpectedAnnualDamageResults.HistogramsOfEADs["Total"].SampleSize + " iterations completed");
}
sw.Stop();
TimeSpan t = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                        t.Hours,
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);
Console.WriteLine("Time taken was: " + answer);