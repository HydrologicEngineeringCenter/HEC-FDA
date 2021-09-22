using System;
using System.Collections;
namespace paireddata
{
    public class StepwisePairedData: IPairedData
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
                    return _yvals[idx];//stepwise interpolation (i think).
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
        public double integrate(){
            double triangle;
            double square;
            //assume x vals are increasing from zero.
            double x1=0.0;
            double y1=0.0;
            double ead=0.0;
            for(int i=0; i<this._xvals.length; i ++){
                double xdelta = this.xvals[i]-x1;
                square = xdelta * y1;
                triangle = ((xdelta)*(this.yvals[i] - y1))/2.0;
                ead += square + triangle;
                x1 = this.xvals[i];
                y1 = this.yvals[i];
            }
            if (x1 != 0.0){
                double xdelta = 1.0-x1;
                ead += xdelta*y1;
            }
            return ead;
        }
    }
}