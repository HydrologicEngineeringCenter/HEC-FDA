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
        public paired_data Sample(double probability){
            //ToDo implement me.
            ArrayList<double> x = new System.Collections.ArrayList<double>();
            ArrayList<double> y = new System.Collections.ArrayList<double>();
            return new paired_data(x,y);
        }
    }
}