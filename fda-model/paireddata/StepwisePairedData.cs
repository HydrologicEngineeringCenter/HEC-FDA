using System;
using System.Collections.Generic;
using System.Linq;
namespace paireddata
{
    public class StepwisePairedData: IPairedData
    {

        public IList<double> Xvals { get; set; }
        public IList<double> Yvals { get; set; }

        public StepwisePairedData(IList<double> xs, IList<double> ys){
            Xvals = xs;
            Yvals = ys;
        }
        public void add_pair(double x, double y){
            Xvals.Add(x);
            Yvals.Add(y);
        }
        public double f(double x){
            //binary search.
            double[] xarr = Xvals.ToArray();
            Int32 idx = Array.BinarySearch(xarr, x);
            if (idx < -1){
                idx = -1*idx-1;
                if (idx == Yvals.Count){
                    return Yvals[Yvals.Count-1];
                }else{
                    return Yvals[idx];//stepwise interpolation (i think).
                }
            }else if (idx == -1){
                return Yvals[0];
            }else{
                return Yvals[idx];
            }
        }
        public IPairedData compose(IPairedData input){
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            for (int i = 0; i < input.Xvals.Count; i++){
                y.Add(this.f(input.Yvals[i]));
                x.Add(input.Xvals[i]);
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
            for(int i=0; i<this.Xvals.Count; i ++){
                double xdelta = this.Xvals[i]-x1;
                square = xdelta * y1;
                triangle = ((xdelta)*(this.Yvals[i] - y1))/2.0;
                ead += square + triangle;
                x1 = this.Xvals[i];
                y1 = this.Yvals[i];
            }
            if (x1 != 0.0){
                double xdelta = 1.0-x1;
                ead += xdelta*y1;
            }
            return ead;
        }

        public IPairedData multiply(IPairedData g)
        {
            double belowFragilityCurveValue = 0.0;
	        double aboveFragilityCurveValue = 1.0;
	        List<double> newXvals = new List<double>();
	        List<double> newYvals = new List<double>();
	        if (Xvals[0] < g.Xvals[0]) {
		        //cacluate no damage until the bottom of the fragility curve
		        double bottom = g.Xvals[0];
		        foreach( double dcx in Xvals) {
			        if (dcx < bottom) {
				        //set to zero
				        newXvals.Add(dcx);
				        newYvals.Add(belowFragilityCurveValue);
			        } else {
				        //create a point on the curve just below the bottom of the levee at damage zero.
				        newXvals.Add(bottom-.000000000001);
				        newYvals.Add(belowFragilityCurveValue);
				        //create a point at the bottom of the fragility curve
				        newXvals.Add(bottom);
				        double damage = this.f(bottom) * g.Yvals[0];
				        newYvals.Add(damage);
				        break;
			        }
		        }
	        }
	        for(int idx = 0; idx<g.Xvals.Count; idx++){
		        //modify
                double lcx = g.Xvals[idx];
		        double damage = this.f(lcx) * g.Yvals[idx];
		        newXvals.Add(lcx);
		        newYvals.Add(damage);
	        }
            if (g.Xvals[g.Xvals.Count-1] < Xvals[(Xvals.Count-1)] ){
                //add in the damage curve ordinates without modification.
                double top = g.Xvals.Last();
                newXvals.Add(top);
                double damage = f(top) * g.Yvals.Last();
                newYvals.Add(damage);
                //create a point at the bottom of the fragility curve
                newXvals.Add(top+.00000001);
                double damageabove = f(top+.00000001) * aboveFragilityCurveValue;
                newYvals.Add(damageabove);
                for (int idx = 0; idx<Xvals.Count;idx++){
                    double dcx = Xvals[idx];
                    if (dcx > top) {
                        //set to max val
                        newXvals.Add(dcx);
                        double d = Yvals[idx] * aboveFragilityCurveValue;
                        newYvals.Add(d);
                    }
                }
            }
            return new PairedData(newXvals,newYvals);
        }
    }
}