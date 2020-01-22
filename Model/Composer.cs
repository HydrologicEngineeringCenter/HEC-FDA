using Functions;
using Model.Inputs.Functions.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public static class Composer
    {
        public static IFrequencyFunction Compose(this IFrequencyFunction frequencyFunction, ITransformFunction transformFunction, double frequencyFunctionProbability, double transformFunctionProbability)
        {
            //acceptable transform could be an inflow outflow or a rating curve
            if(frequencyFunction.ComposeableTypes.Contains(transformFunction.Type))
            { 
                IFunction sampledFreq = Sampler.Sample(frequencyFunction.Function, frequencyFunctionProbability);
                IFunction sampledTrans = Sampler.Sample(transformFunction.Function, transformFunctionProbability);
                IFunction newFrequencyFunction = sampledFreq.Compose(sampledTrans);
                return ImpactAreaFunctionFactory.FactoryFrequency(newFrequencyFunction, transformFunction.Type + 1);
            }
            else
            {
                throw new ArgumentException($"The {frequencyFunction.Type} function can not be composed with the {transformFunction.Type} function because the frequency function yvalues ({frequencyFunction.YLabel})" +
                    $" do not match the tranform function xvalues ({transformFunction.XLabel}).");
            }
        }

       
    }
}
