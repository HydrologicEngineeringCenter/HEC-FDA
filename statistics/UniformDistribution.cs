using System;
namespace statistics
{
    public class UniformDistribution: IDistributedVariable
    {
        private double _min;
        private double _max;
        public UniformDistribution(double min, double max){
            _min = min;
            _max = max;
        }
        public double inv_cdf(double probability){
            return _min + (probability*(_max-min));
        }
    }
}