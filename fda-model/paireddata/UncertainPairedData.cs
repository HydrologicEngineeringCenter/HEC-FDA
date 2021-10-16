using System.Collections.Generic;
using Statistics;
using interfaces;
namespace paireddata
{
    public class UncertainPairedData: IPairedDataProducer, ICategory, ICanBeNull
    {
        private double[] _xvals;
        private IDistribution[] _yvals;
        public string Category {get;}
        public bool IsNull { get; }
        public double[] xs(){
            return _xvals;
        }
        public IDistribution[] ys(){
            return _yvals;
        }
        public UncertainPairedData()
        {
            IsNull = true;
        }
        public UncertainPairedData(double[] xs, IDistribution[] ys){
            _xvals = xs;
            _yvals = ys;
            Category = "Default";
            IsNull = false;
        }
        public UncertainPairedData(double[] xs, IDistribution[] ys, string category){
            _xvals = xs;
            _yvals = ys;
            Category = category;
            IsNull = false;
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