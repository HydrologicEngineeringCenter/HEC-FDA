using System.Collections.Generic;
using Statistics;
namespace paireddata
{
    public class UncertainPairedData: IPairedDataProducer, ICategory
    {
        private double[] _xvals;
        private IDistribution[] _yvals;
        public string Category {get;}
        public double[] xs(){
            return _xvals;
        }
        public IDistribution[] ys(){
            return _yvals;
        }
        public UncertainPairedData(double[] xs, IDistribution[] ys){
            _xvals = xs;
            _yvals = ys;
            Category = "Default";
        }
        public UncertainPairedData(double[] xs, IDistribution[] ys, string category){
            _xvals = xs;
            _yvals = ys;
            Category = category;
        }
        public IPairedData SamplePairedData(double probability){
            double[] y = new double[_yvals.Length];
            for (int i=0;i<_xvals.Length; i++){
                y[i] = ys()[i].InverseCDF(probability);
            }
            return new PairedData(xs(), y, Category);
        }
    }
}