using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statistics.Graphical
{
    class InterpolateQuantiles
    {

        private double[] _InputXValues;
        private double[] _InputYValues;
        public InterpolateQuantiles(double[] xvalues, double[] yvalues)
        {
            _InputXValues = xvalues;
            _InputYValues = yvalues;
        }
        public double[] ComputeQuantiles(double[] probabilities)
        {
            double[] output = new double[probabilities.Count()];
            Distributions.Normal standardNormalDistribution = new Distributions.Normal();
            double p;
            double xn;
            int inputOrdinate = 0;
            for (int i = 0; i < probabilities.Count(); i++)
            {
                p = probabilities[i];
                xn = standardNormalDistribution.InverseCDF(p);
                //
                for (int j = 0; j < _InputXValues.Count(); j++)
                {
                    if ((p - _InputYValues[j]) > -1.0e-5)
                    {
                        inputOrdinate = j;
                        break;
                    }
                }
                if (inputOrdinate == 0)
                {
                    output[i] = _InputXValues[inputOrdinate];

                }
                else
                {
                    if (_InputYValues[inputOrdinate - 1] < .99999)
                    {
                        double xk = standardNormalDistribution.InverseCDF(probabilities[i]);
                        double xk1 = standardNormalDistribution.InverseCDF(_InputYValues[inputOrdinate - 1]);
                        double xk2 = standardNormalDistribution.InverseCDF(_InputYValues[inputOrdinate]);
                        output[i] = _InputXValues[inputOrdinate - 1] + ((xk - xk1) / (xk2 - xk1)) * (_InputXValues[inputOrdinate] - _InputXValues[inputOrdinate - 1]);
                    }
                    else//out at the tail, use linear interpolation...
                    {
                        output[i] = _InputXValues[inputOrdinate - 1] + ((probabilities[i] - _InputYValues[inputOrdinate - 1]) / (_InputYValues[inputOrdinate] - _InputYValues[inputOrdinate - 1])) * (_InputXValues[inputOrdinate] - _InputXValues[inputOrdinate - 1]);
                    }
                }
            }
            return output;
        }
    }
}
