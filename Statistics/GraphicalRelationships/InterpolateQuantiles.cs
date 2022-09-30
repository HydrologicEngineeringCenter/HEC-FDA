﻿using System;
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
            double[] quantiles = new double[finalExceedanceProbabilities.Count()];
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
                    quantiles[i] = _InputXValues[inputOrdinate];  

                }
                else
                {
                    if (_InputYValues[inputOrdinate - 1] < .99999)
                    {
                        double zValueExceedanceProbability = standardNormalDistribution.InverseCDF(exceedanceProbability);
                        double zValueSmallerInputExceedanceProbability = standardNormalDistribution.InverseCDF(_InputYValues[inputOrdinate - 1]);
                        double zValueLargerExceedanceProbability = standardNormalDistribution.InverseCDF(_InputYValues[inputOrdinate]);
                        double fractionOfQuantileDifference = ((zValueExceedanceProbability - zValueSmallerInputExceedanceProbability) / (zValueLargerExceedanceProbability - zValueSmallerInputExceedanceProbability));
                        quantiles[i] = fractionOfQuantileDifference * _InputXValues[inputOrdinate] + (1-fractionOfQuantileDifference)* _InputXValues[inputOrdinate - 1];
                        
                    }
                    else//out at the tail, use linear interpolation...
                    {
                        quantiles[i] = _InputXValues[inputOrdinate - 1] + ((finalExceedanceProbabilities[i] - _InputYValues[inputOrdinate - 1]) / (_InputYValues[inputOrdinate] - _InputYValues[inputOrdinate - 1])) * (_InputXValues[inputOrdinate] - _InputXValues[inputOrdinate - 1]);
                    }
                }
            }
            return quantiles;
        }
    }
}
