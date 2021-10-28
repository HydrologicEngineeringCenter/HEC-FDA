using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics;
using Statistics.Histograms;

namespace metrics
{
    public class Performance
{
    private const double AEP_HISTOGRAM_BINWIDTH = .0001;
    private const double CNEP_HISTOGRAM_BINWIDTH = .01;
    private string _thresholdType;
    private double _thresholdValue;
    private Histogram _aep = null;
    private Dictionary<string, Histogram> _ead;
    private Dictionary<double, Histogram> _cnep;



    public Performance(string thresholdType, double thresholdValue)
    {

    }

}
}
