using System;
using System.Linq;
using System.Collections.Generic;
using Functions;
using System.Xml.Linq;

namespace Model.Functions
{
    internal sealed class InflowFrequency : FdaFunctionBase, IFrequencyFunction
    {
        #region Properties
        public override IParameterSeries XSeries { get; }
        public override IParameterSeries YSeries { get; }
        public override IParameterEnum ParameterType => IParameterEnum.InflowFrequency;
        public List<IParameterEnum> ComposeableTypes => new List<IParameterEnum>() { IParameterEnum.InflowOutflow, IParameterEnum.Rating };       
        #endregion

        #region Constructors
        internal InflowFrequency(IFunction fx, string xLabel = "", UnitsEnum yUnits = UnitsEnum.CubicMeterPerSecond, string yLabel = "") : base(fx)
        {
            XSeries = IParameterFactory.Factory(this, true, UnitsEnum.Probability, xLabel);
            YSeries = IParameterFactory.Factory(this, false, yUnits, yLabel);
        }
        #endregion

        #region Functions
        public double Integrate() => _Function.TrapizoidalRiemannSum();
        #endregion

        #region IFunctionCompose Methods
        ///// <summary> Generates either (a) an outflow frequency function by composing the inflow frequency function with a inflow outflow transform function, or (b) an exterior stage frequency function by composing the inflow frequency function (being used as an outflow frequency function) with a rating function. If transform function that is passed in is not an inflow outflow or rating function an exception is thrown, because the functions do not share a common set of ordinates. </summary>
        ///// <param name="transform"> The transform function to be used in the composition equation, often written as f(g(x)). An exception will be thrown if the computation function type does not share a common set of ordinates with the frequency function used in the composition. </param>
        ///// <param name="frequencyFunctionProbability"> An optional sampling parameter between 0 and 1 that is set to 0.50 as a default. The parameter value represents the chance that the frequency function values are less than or equal to sampled frequency function values. The central tendency of the statistically distrubuted frequency function is computed as a default. If the input frequency function is not statistically distributed the input frequency function values are computed. </param>
        ///// <param name="transformFunctionProbability"> An optional sampling parameter between 0 and 1 that is set to 0.50 as a default. The parameter value represents the chance that the transform function values are less than or equal to sampled transform function values. The central tendency of the statistically distrubuted transform function is computed as a default. If the input transform function is not statistically distributed the input transform function values are computed. </param>
        ///// <returns> A new frequency function if the transform functions range can be mapped to the frequency function domain. </returns>
        //public IFrequencyFunction Compose(ITransformFunction transform, double frequencyFunctionProbability = 0.5, double transformFunctionProbability = 0.5)
        //{
        //    //acceptable transform could be an inflow outflow or a rating curve
        //    if (IsValidComposition(transform) == true)
        //    {
        //        IFunction inflowFreq = Sampler.Sample(Function, frequencyFunctionProbability);
        //        IFunction transformFunc = Sampler.Sample(transform.Function, transformFunctionProbability);
        //        IFunction newFrequencyFunction = inflowFreq.Compose(transformFunc);
        //        return (IFrequencyFunction) ImpactAreaFunctionFactory.Factory(newFrequencyFunction, transform.Type + 1);
        //    }
        //    else
        //    {
        //        throw new ArgumentException( "Composition could not be initialized because the two functions do not share a common set of ordinates.");
        //    }

        //    //if (transform.Type - 1 == Type)
        //    //{
        //    //    IFunction inflowFreq = Sampler.Sample(Function, frequencyFunctionProbability);
        //    //    IFunction transformFunc = Sampler.Sample(transform.Function, transformFunctionProbability);
        //    //    IFunction composedFunc = inflowFreq.Compose(transformFunc);
        //    //    return new OutflowFrequency(composedFunc);
        //    //}
        //    //else if(transform.Type == ImpactAreaFunctionEnum.Rating)
        //    //{
        //    //    //then we assume that the inflow frequency is actually outflow frequency
        //    //    // create an interior stage frequency
        //    //    IFunction inflowFreq = Sampler.Sample(Function, frequencyFunctionProbability);
        //    //    IFunction transformFunc = Sampler.Sample(transform.Function, transformFunctionProbability);
        //    //    IFunction composedFunc = inflowFreq.Compose(transformFunc);
        //    //    return new InteriorStageFrequency(composedFunc);
        //    //}
        //    //else
        //    //{
        //    //    throw new ArgumentException(ReportCompositionError());
        //    //}

        //}
        ////internal IFunctionCompose Compose(ITransformFunction transformFunction)
        ////{
        ////    if (IsValidComposition(transformFunction)) return ImpactAreaFunctionFactory.CreateNew(Function.Compose(transformFunction.Ordinates), transformFunction.Type + 1);
        ////    else { ReportCompositionError(); throw new ArgumentException("Composition could not be initialized because the two functions do not share a set of common ordinates."); }
        ////}
        //private bool IsValidComposition(ITransformFunction transform)
        //{
        //    if (transform.Type == ImpactAreaFunctionEnum.Rating || transform.Type == ImpactAreaFunctionEnum.InflowOutflow)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //private string ReportCompositionError()
        //{
        //    return "Composition could not be initialized because the two functions do not share a common set of ordinates.";
        //}
        //public double GetXFromY(double y)
        //{
        //    return Function.GetXfromY(y);
        //}
        //public double GetYFromX(double x)
        //{
        //    return Function.GetYfromX(x);
        //}
        //public double Integrate()
        //{
        //    return Function.TrapezoidalRiemannSum();
        //}
        //public IFunctionCompose Sample(double seed)
        //{
        //    return new InflowFrequency(Function.Sample(seed));
        //}
        #endregion
        #region IValidateData Methods
        //public override bool Validate()
        //{
        //    if (!(Function.GetType() == typeof(FrequencyFunction))) return false;
        //    else return Function.IsValid;
        //}
        //public override IEnumerable<string> ReportValidationErrors()
        //{
        //    List<string> errors = Function.ReportValidationErrors().ToList();
        //    if (Function.GetType() != typeof(FrequencyFunction)) errors.Add("A valid statistical relationship must define inflow frequency functions. The input inflow frequency function is not defined by a valid statistical relationship, it will be held in an uncomputable state until this error has been corrected.");
        //    return errors;
        //}
        #endregion
    }
}
