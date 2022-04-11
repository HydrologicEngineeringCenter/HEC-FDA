using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statistics.Graphical
{
    public class InterpolateQuantiles
    {

        private double[] _InputXValues;
        private double[] _InputYValues;
        public InterpolateQuantiles(double[] xvalues, double[] yvalues)
        {
            _InputXValues = xvalues;
            _InputYValues = yvalues;
        }
        public double[] ComputeQuantiles(double[] finalExceedanceProbabilities)
        {
            double[] output = new double[finalExceedanceProbabilities.Count()];
            Distributions.Normal standardNormalDistribution = new Distributions.Normal();
            double exceedanceProbability;
            int inputOrdinate = 0;
            for (int i = 0; i < finalExceedanceProbabilities.Count(); i++)
            {
                exceedanceProbability = finalExceedanceProbabilities[i];
                for (int j = 0; j < _InputXValues.Count(); j++) //look over input exceedance probabilities
                {
                    if ((exceedanceProbability - _InputYValues[j]) > -1.0e-5) //if the required exceedance probability matches the input exceedance probability
                    {
                        inputOrdinate = j; //get the index of the input flow or exceedance value 
                        break;
                    }
                }
                if (inputOrdinate == 0) //if the index is for the first input flow or stage value
                {
                    output[i] = _InputXValues[inputOrdinate];  

                }
                else
                {
                    if (_InputYValues[inputOrdinate - 1] < .99999)
                    {
                        double xk = standardNormalDistribution.InverseCDF(finalExceedanceProbabilities[i]);
                        double xk1 = standardNormalDistribution.InverseCDF(_InputYValues[inputOrdinate - 1]);
                        double xk2 = standardNormalDistribution.InverseCDF(_InputYValues[inputOrdinate]);
                        output[i] = _InputXValues[inputOrdinate - 1] + ((xk - xk1) / (xk2 - xk1)) * (_InputXValues[inputOrdinate] - _InputXValues[inputOrdinate - 1]);
                    }
                    else//out at the tail, use linear interpolation...
                    {
                        output[i] = _InputXValues[inputOrdinate - 1] + ((finalExceedanceProbabilities[i] - _InputYValues[inputOrdinate - 1]) / (_InputYValues[inputOrdinate] - _InputYValues[inputOrdinate - 1])) * (_InputXValues[inputOrdinate] - _InputXValues[inputOrdinate - 1]);
                    }
                }
            }
            return output;
        }
    }
}
