using System;
using System.Collections.Generic;
namespace paireddata
{
    public class UncertainPairedData: IPairedDataProducer
    {
        private IList<double> _xvals;
        private IList<statistics.IDistributedVariable> _yvals;
        public IList<double> xs(){
            return _xvals;
        }
        public IList<statistics.IDistributedVariable> ys(){
            return _yvals;
        }
        public UncertainPairedData(IList<double> xs, IList<statistics.IDistributedVariable> ys){
            _xvals = xs;
            _yvals = ys;
        }
        public void add_pair(double x, statistics.IDistributedVariable y){
            _xvals.Add(x);
            _yvals.Add(y);
        }
        public IPairedData SamplePairedData(double probability){
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            for(int i=0;i<this._xvals.Count; i++){
                x.Add(this.xs()[i]);
                y.Add(this.ys()[i].inv_cdf(probability));

            }
            return new PairedData(x.ToArray(), y.ToArray());
        }
    }
}