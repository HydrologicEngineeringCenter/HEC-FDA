using System;
using System.Collections.Generic;
using paireddata;
namespace flowfrequency
{
    public class FlowFrequency
    {
        private statistics.IBootstrap _dist;
        private Int64 _eyor;
        public FlowFrequency(statistics.IBootstrap dist, Int64 eyor){
            _dist = dist;
            _eyor = eyor;
        }
        public IPairedData SamplePairedData(double probability){
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            for(int i=0;i<_eyor; i++){
                double prob = ((double)i)/((double)_eyor);//this is wrong it should be ordinates not eyor... need to fix
                x.Add(prob);
                y.Add(this._dist.inv_cdf(probability));

            }
            return new PairedData(x.ToArray(), y.ToArray());
        }
    }
}