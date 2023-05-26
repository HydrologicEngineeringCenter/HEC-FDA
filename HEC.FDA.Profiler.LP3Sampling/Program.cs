using HEC.FDA.Model.compute;
using HEC.FDA.Model.extensions;
using Statistics.Distributions;

LogPearson3 lp3 = new LogPearson3(3.5, 0.22, 0.1, 60);
lp3.BootstrapToUncertainPairedData(new RandomProvider(1234).NextRandomSequenceSet(10000, LogPearson3._RequiredExceedanceProbabilitiesForBootstrapping.Length), LogPearson3._RequiredExceedanceProbabilitiesForBootstrapping, 0.5);