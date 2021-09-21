using System;
using System.Collections;
namespace paireddata
{
    public class paired_data: IPairedData
    {
        private IList<double> _xvals;
        private IList<double> _yvals;
        public IList<double> xs(){
            return _xvals;
        }
        public IList<double> ys(){
            return _yvals;
        }
        public paired_data(IList<double> xs, IList<double> ys){
            _xvals = xs;
            _yvals = ys;
        }
        public add_pair(double x, double y){
            _xvals.Add(x);
            _yvals.Add(y);
        }
        public double f(double x){
            //binary search.
            return x;
        }
        public paired_data compose(paired_data input){
            //ToDo implement me.
            return this;
        }
    }
}