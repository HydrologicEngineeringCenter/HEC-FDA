using System.Collections.Generic;
using Statistics;
namespace paireddata
{
    public class UncertainPairedData: IPairedDataProducer
    {
        private IList<double> _xvals;
        private IList<IDistribution> _yvals;
        public IList<double> xs(){
            return _xvals;
        }
        public IList<IDistribution> ys(){
            return _yvals;
        }
        public UncertainPairedData(IList<double> xs, IList<IDistribution> ys){
            _xvals = xs;
            _yvals = ys;
        }
        public void add_pair(double x, IDistribution y){
            _xvals.Add(x);
            _yvals.Add(y);
        }
        public IPairedData SamplePairedData(double probability){
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            for(int i=0;i<this._xvals.Count; i++){
                x.Add(xs()[i]);
                y.Add(ys()[i].InverseCDF(probability));
            }
            return new PairedData(x.ToArray(), y.ToArray());
        }
    }
}