using System;
using System.Collections;
namespace paireddata
{
    public class paired_data: IPairedDataProducer
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
        public add_pair(double x, statistics.IDistributedVariable y){
            _xvals.Add(x);
            _yvals.Add(y);
        }
        public PairedData Sample(double probability){
            ArrayList<double> x = new System.Collections.ArrayList<double>();
            ArrayList<double> y = new System.Collections.ArrayList<double>();
            for(i=0;i<this._xvals; i++){
                x.Add(this._xvals[i]);
                y.Add(this._yvals[i].inv_cdf(probability));

            }
            return new PairedData(x,y);
        }
    }
}