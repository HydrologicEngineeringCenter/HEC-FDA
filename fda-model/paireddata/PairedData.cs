using System;
using System.Collections.Generic;
using System.Linq;
namespace paireddata
{
    public class PairedData: IPairedData
    {
        public double[] Xvals { get; }
        public double[] Yvals { get; }

        public PairedData(double[] xs, double[] ys){
            Xvals = xs;
            Yvals = ys;
        }

        /// <summary>
        /// f implements ISample on PairedData, for a given input double x f produces an output double that represents the linearly interoplated value for y given x.
        /// </summary>
        public double f(double x){
            //binary search.
            double[] xarr = Xvals.ToArray();
            Int32 idx = Array.BinarySearch(xarr, x);
            if(idx >=0)
            {
                //Matches a value exactly
                return Yvals[idx];
            }
            else
            {
                //This is the next LARGER value.
                idx = ~idx;

                if(idx == Xvals.Count()) {return Yvals[Xvals.Length-1];}

                if(idx == 0) {return Yvals[0];}

                //Ok. Interpolate Y=mx+b
                double m = (Yvals[idx] - Yvals[idx - 1]) / (Xvals[idx] - Xvals[idx - 1]);
                double b = Yvals[idx - 1];
                double dx = x - Xvals[idx - 1];
                return m * dx + b;
            }
        }
        /// <summary>
        /// compose implements the IComposable interface on PairedData, which allows a PairedData object to take the input y values as the x value (to determine the commensurate y value) from the subject function.
        /// Ultimately it creates a composed function with the Y from the subject, and the commensurate x from the input.
        /// </summary>
        public IPairedData compose(IPairedData input){
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            for (int i = 0; i < input.Xvals.Length; i++){
                y.Add(f(input.Yvals[i]));
                x.Add(input.Xvals[i]);
            }
            return new PairedData(x.ToArray(), y.ToArray());
        }

        /// <summary>
        /// integrate implements IIntegrate on PairedData, it calcualtes the area under the paired data curve across the range of x values using trapizoidal integration. Assumes X vals are probabilities decreasing from 1
        /// </summary>
        public double integrate(){
            double triangle;
            double square;
            double x1=1.0;
            double y1=0.0;
            double ead=0.0;
            for(int i=0; i<Xvals.Length; i ++){
                double xdelta = x1-Xvals[i];
                square = xdelta * y1;
                triangle = ((xdelta)*(Yvals[i] - y1))/2.0;
                ead += square + triangle;
                x1 = Xvals[i];
                y1 = Yvals[i];
            }
            if (x1 != 0.0){
                double xdelta = x1-0;
                ead += xdelta*y1;
            }
            return ead;
        }
        public StepwisePairedData ToStepwisePairedData(){
            return new StepwisePairedData(Xvals, Yvals);
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
	        for(int idx = 0; idx<g.Xvals.Count(); idx++){
		        //modify
                double lcx = g.Xvals[idx];
		        double damage = this.f(lcx) * g.Yvals[idx];
		        newXvals.Add(lcx);
		        newYvals.Add(damage);
	        }
            if (g.Xvals[g.Xvals.Count()-1] < Xvals[(Xvals.Count()-1)] ){
                //add in the damage curve ordinates without modification.
                double top = g.Xvals.Last();
                newXvals.Add(top);
                double damage = f(top) * g.Yvals.Last();
                newYvals.Add(damage);
                //create a point at the bottom of the fragility curve
                newXvals.Add(top+.00000001);
                double damageabove = f(top+.00000001) * aboveFragilityCurveValue;
                newYvals.Add(damageabove);
                for (int idx = 0; idx<Xvals.Count();idx++){
                    double dcx = Xvals[idx];
                    if (dcx > top) {
                        //set to max val
                        newXvals.Add(dcx);
                        double d = Yvals[idx] * aboveFragilityCurveValue;
                        newYvals.Add(d);
                    }
                }
            }
            return new PairedData(newXvals.ToArray(),newYvals.ToArray());
        }
    }
}