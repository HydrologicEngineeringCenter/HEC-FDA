using Functions;
using Model.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public static class Composer
    {
        public static IFrequencyFunction Compose(this IFrequencyFunction frequencyFx, ITransformFunction transformFx, double pForFreqFx = 0.50, double pForTransformFx = 0.50)
        {
            //todo: Delete me. This is just for testing on 6/16/21
            Utilities.WriteToConsole.WriteCoordinatesToConsole(frequencyFx, "freq Function from Composer.cs ln 14");
            Utilities.WriteToConsole.WriteCoordinatesToConsole(transformFx, "transform Function from Composer.cs ln 15");
            //////////////////////

            //acceptable transform could be an inflow outflow or a rating curve
            if (frequencyFx.ComposeableTypes.Contains(transformFx.ParameterType))
            {
                //IFunction sampledFreqFx = Sampler.Sample(((FdaFunctionBase)frequencyFx)._Function, pForFreqFx);
                //IFunction sampledTranFx = Sampler.Sample(((FdaFunctionBase)transformFx)._Function, pForTransformFx);
                //IFunction composedFreqFx = sampledFreqFx.Compose(sampledTranFx);
                //todo: get rid of this?
                IFunction freqConstant = IFunctionFactory.Factory(frequencyFx.Function.Coordinates, InterpolationEnum.Linear);
                IFunction composedFreqFx = (freqConstant).Compose((IFunction)transformFx.Function);

                return IFrequencyFunctionFactory.Factory(composedFreqFx, transformFx.ParameterType + 1,
                    (transformFx.ParameterType + 1).Print(), frequencyFx.XSeries.Label, transformFx.YSeries.Label, transformFx.YSeries.Units);
            }
            else 
                throw new ArgumentException($"{frequencyFx.ParameterType.ToString()} functions cannot be composed with {transformFx.ParameterType} functions." +
                $"The {frequencyFx.ParameterType.ToString()} function Y parameters represent {frequencyFx.YSeries.ParameterType.ToString()}s while " +
                $"the {transformFx.ParameterType.ToString()} function X parameters represent {transformFx.XSeries.ParameterType.ToString()}s, functional composition would produce an illogical Y axis.");
        }
    }
}
