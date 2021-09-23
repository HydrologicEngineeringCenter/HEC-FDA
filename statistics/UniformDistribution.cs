using System;
namespace statistics
{
    //we should absolutely use the c# statistics library we already have. this is just to mock in something.
    public class UniformDistribution: IDistributedVariable
    {
        private double _min;
        private double _max;
        public UniformDistribution(double min, double max){
            _min = min;
            _max = max;
        }
        public double inv_cdf(double probability){
            return _min + (probability*(_max-_min));
        }
    }
}