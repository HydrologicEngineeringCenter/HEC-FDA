using System;
using System.Collections.Generic;

namespace Model.Inputs.Functions
{
    public interface IFunctionBase: IValidateData
    {
        bool ValidateFrequencyValues();
        /// <summary>
        /// Samples a new function by getting Y values from the Y values cummulative density function, based on the input probability.
        /// </summary>
        /// <param name="probability"> The probability of the values to be sampled from the Y values cummulative density functions. </param>
        /// <returns> A new, shifted function with a definate set of Y values. </returns>
        IFunctionBase Sample(double probability);
        IList<Tuple<double, double>> Compose(IList<Tuple<double, double>> transformOrdinates);
        IList<Tuple<double, double>> GetOrdinates();
        double GetXfromY(double y);
        double GetYfromX(double x);
        double TrapezoidalRiemannSum();
        
    }
}
