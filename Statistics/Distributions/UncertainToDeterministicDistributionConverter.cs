using System;

namespace HEC.FDA.Statistics.Distributions
{
    public static class UncertainToDeterministicDistributionConverter
    {
        public static Deterministic ConvertDistributionToDeterministic(IDistribution iDistribution)
        {
            Deterministic returnedDistribution = new Deterministic();
            switch (iDistribution.Type)
            {
                //TODO: Not sure the best way to handle the not supported case. we could return a dummy deterministic dist. 
                //we should not hit this anyway - there are no other distribution types
                case IDistributionEnum.NotSupported:
                    returnedDistribution = (Deterministic)iDistribution;
                    break;
                case IDistributionEnum.Deterministic:
                    returnedDistribution = (Deterministic)iDistribution;
                    break;
                case IDistributionEnum.Normal:
                    double normalMean = ((Normal)iDistribution).Mean;
                    returnedDistribution = new Deterministic(normalMean);
                    break;
                case IDistributionEnum.Uniform:
                    double median = iDistribution.InverseCDF(0.5);
                    returnedDistribution = new Deterministic(median);
                    break;
                case IDistributionEnum.Triangular:
                    double mostLikely = ((Triangular)iDistribution).MostLikely;
                    returnedDistribution = new Deterministic(mostLikely);
                    break;
                case IDistributionEnum.LogPearsonIII:
                    double logLP3Mean = ((LogPearson3)iDistribution).Mean;
                    double unloggedLP3Mean = Math.Pow(logLP3Mean, 10);
                    returnedDistribution = new Deterministic(unloggedLP3Mean);
                    break;
                case IDistributionEnum.LogNormal:
                    double logNormalMean = ((LogNormal)iDistribution).Mean;
                    double unloggedNormalMean = Math.Exp(logNormalMean);
                    returnedDistribution = new Deterministic(unloggedNormalMean);
                    break;
            }
            return returnedDistribution;
        }
    }
}
