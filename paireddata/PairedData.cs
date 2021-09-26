using System;
using System.Collections.Generic;
using System.Linq;
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
        public PairedData(IList<double> xs, IList<double> ys){
            _xvals = xs;
            _yvals = ys;
        }
        public void add_pair(double x, double y){
            _xvals.Add(x);
            _yvals.Add(y);
        }
        ///f implements ISample on PairedData, for a given input double x f produces an output double that represents the linearly interoplated value for y given x.
        public double f(double x){
            //binary search.
            double[] xarr = _xvals.ToArray();
            Int32 idx = System.Array.BinarySearch(xarr, x);
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
        ///compose implements the IComposable interface on PairedData, which allows a PairedData object to take the input y values as the x value (to determine the commensurate y value) from the subject function. Ultimately it creates a composed function with the Y from the subject, and the commensurate x from the input.
        public IPairedData compose(IPairedData input){
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            for (int i = 0; i < input.xs().Count; i++){
                y.Add(this.f(input.ys()[i]));
                x.Add(input.xs()[i]);
            }
            return new PairedData(x, y);
        }
        ///integrate implements IIntegrate on PairedData, it calcualtes the area under the paired data curve across the range of x values using trapizoidal integration.
        public double integrate(){
            double triangle;
            double square;
            //assume x vals are increasing from zero.
            double x1=0.0;
            double y1=0.0;
            double ead=0.0;
            for(int i=0; i<this._xvals.Count; i ++){
                double xdelta = this._xvals[i]-x1;
                square = xdelta * y1;
                triangle = ((xdelta)*(this._yvals[i] - y1))/2.0;
                ead += square + triangle;
                x1 = this._xvals[i];
                y1 = this._yvals[i];
            }
            if (x1 != 0.0){
                double xdelta = 1.0-x1;
                ead += xdelta*y1;
            }
            return ead;
        }
        public StepwisePairedData ToStepwisePairedData(){
            return new StepwisePairedData(_xvals, _yvals);
        }
    }
}