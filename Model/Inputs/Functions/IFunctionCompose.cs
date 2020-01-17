using System;
using Model.Inputs.Functions.ImpactAreaFunctions;

namespace Model.Inputs.Functions
{
    public interface IFunctionCompose: IValidateData
    {
        ImpactAreaFunctionEnum Type { get; }
        /// <summary> Generates a new frequency function by composing a freqency with transform function containing a matching set of ordinates. An exception is thrown a frequency function type and transform function type do not share a common set of ordinates. </summary>
        /// <param name="transformFunction"> The transform function to be used in the composition equation, often written as f(g(x)). An exception will be thrown if the computation function type does not share a common set of ordinates with the frequency function used in the composition. </param>
        /// <param name="frequencyFunctionProbability"> An optional sampling parameter between 0 and 1 that is set to 0.50 as a default. The parameter value represents the chance that the frequency function values are less than or equal to sampled frequency function values. The central tendency of the statistically distrubuted frequency function is computed as a default. If the input frequency function is not statistically distributed the input frequency function values are computed. </param>
        /// <param name="transformFunctionProbability"> An optional sampling parameter between 0 and 1 that is set to 0.50 as a default. The parameter value represents the chance that the transform function values are less than or equal to sampled transform function values. The central tendency of the statistically distrubuted transform function is computed as a default. If the input transform function is not statistically distributed the input transform function values are computed. </param>
        /// <returns> A new frequency function if the transform functions range can be mapped to the frequency function domain. </returns>
        IFunctionCompose Compose(ITransformFunction transformFunction, double frequencyFunctionProbability = 0.50, double transformFunctionProbability = 0.50);
        //IFunctionCompose Compose(IFunctionTransform transformFunction); --> Need to Implement this...
        double GetXFromY(double y);
        double GetYFromX(double x);
        double Integrate();
        IFunctionCompose Sample(double randomNumber); 
        //T SampleNew<T>(double probability) where T : IFunctionCompose, IFunctionTransform;
    }
}
