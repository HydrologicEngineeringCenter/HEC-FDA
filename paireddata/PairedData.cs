using System;
using System.Collections;
namespace paireddata
{
    public class PairedData: IPairedData
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
            Int32 idx = Array.BinarySerch(_xvals, x);
            if (idx < -1){
                idx = -1*idx-1;
                if (idx == _yvals.Count){
                    return _yvals[_yvals.Count-1];
                }else{
                    return _yvals[idx- 1] + (x - _xvals[idx - 1]) / (_xvals[idx] - _xvals[idx - 1]) * (_yvals[idx] - _yvals[idx - 1]);
                }
            }else if (idx == -1){
                return _yvals[0];
            }else{
                return _yvals[idx];
            }
        }
        public PairedData compose(PairedData input){
            ArrayList<double> x = new System.Collections.ArrayList<double>();
            ArrayList<double> y = new System.Collections.ArrayList<double>();
            for (int i = 0; i < input._xvals.Count; i++){
                y.Add(this.f(input._yvals[i]));
                x.Add(input._xvals[i]);
            }
            return new PairedData(x, y);
        }
    }
}