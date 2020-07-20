using Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Model.Functions
{
    internal sealed class ExteriorStageFrequency : FdaFunctionBase, IFrequencyFunction
    {
        #region Properties
        public override string Label { get; }
        public override IParameter XSeries { get; }
        public override IParameter YSeries { get; }
        public override UnitsEnum Units { get; }
        public override IParameterEnum ParameterType => IParameterEnum.DamageFrequency;
        public List<IParameterEnum> ComposeableTypes => new List<IParameterEnum>() { IParameterEnum.InteriorStageDamage, IParameterEnum.ExteriorInteriorStage };       
        #endregion

        #region Constructor
        internal ExteriorStageFrequency(IFunction fx, string label = "", string xLabel = "", string yLabel = "", UnitsEnum yUnits = UnitsEnum.Foot) : base(fx)
        {
            Label = label == "" ? ParameterType.Print() : label;
            XSeries = IParameterFactory.Factory(this, true, UnitsEnum.Probability, xLabel);
            YSeries = IParameterFactory.Factory(this, false, yUnits, yLabel);
            Units = YSeries.Units;
        }
        #endregion

        #region Function
        public double Integrate() => _Function.TrapizoidalRiemannSum();
        #endregion

        #region IFunctionCompose Methods
        //public override XElement WriteToXML()
        //{
        //    throw new NotImplementedException();
        //}
        //public IFrequencyFunction Compose(ITransformFunction transformFunction, double frequencyFunctionProbability, double transformFunctionProbability)
        //{
        //    Composer.compose(this,transformFunction)
        //    //acceptable transform could be an inflow outflow or a rating curve
        //    if (IsValidComposition(transform) == true)
        //    {
        //        IFunction inflowFreq = Sampler.Sample(Function, frequencyFunctionProbability);
        //        IFunction transformFunc = Sampler.Sample(transform.Function, transformFunctionProbability);
        //        IFunction newFrequencyFunction = inflowFreq.Compose(transformFunc);
        //        return (IFrequencyFunction)ImpactAreaFunctionFactory.Factory(newFrequencyFunction, transform.Type + 1);
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Composition could not be initialized because the two functions do not share a common set of ordinates.");
        //    }


        //    //if (IsValidComposition(transform) == true)
        //    //    return ImpactAreaFunctionFactory.CreateNew(Function.Sample(frequencyFunctionProbability).Compose(transform.Sample(transformFunctionProbability).Ordinates), transform.Type + 1);
        //    //else ReportCompositionError(); return null;
        //    if (transformFunction.Type - 1 == Type)
        //    {
        //        IFunction extStageFreq = Sampler.Sample(Function, frequencyFunctionProbability);
        //        IFunction transformFunc = Sampler.Sample(transformFunction.Function, transformFunctionProbability);
        //        IFunction composedFunc = extStageFreq.Compose(transformFunc);
        //        return new DamageFrequency(composedFunc);
        //    }
        //    else if(transformFunction.Type == ImpactAreaFunctionEnum.InteriorStageDamage)
        //    {
        //        //then we assume that this exterior stage frequency is the same as interior stage frequency
        //        IFunction extStageFreq = Sampler.Sample(Function, frequencyFunctionProbability);
        //        IFunction transformFunc = Sampler.Sample(transformFunction.Function, transformFunctionProbability);
        //        IFunction composedFunc = extStageFreq.Compose(transformFunc);
        //        return new DamageFrequency(composedFunc);
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Unable to compose the transform function to this outflow frequency function. The transform function " +
        //            "must be a rating curve.");
        //    }
        //}
        //private bool IsValidComposition(ITransformFunction transform)
        //{
        //    if (transform.Type == ImpactAreaFunctionEnum.InteriorStageDamage || transform.Type == ImpactAreaFunctionEnum.ExteriorInteriorStage)
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
        //    return "Composition could not be initialized because no transform function was provided or the two functions do not share a common set of ordinates.";
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
        //public IFunctionCompose Sample(double randomNumber)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
        #region IValidateData Methods
        //public override bool Validate()
        //{
        //    if (Function.ValidateFrequencyValues() == false) return false;
        //    else return Function.IsValid;
        //}
        //public override IEnumerable<string> ReportValidationErrors()
        //{
        //    List<string> messages = Function.ReportValidationErrors().ToList();
        //    if (Function.ValidateFrequencyValues() == false) { IsValid = false; messages.Add("The frequency function is invalid because it contain ordinates outside of the valid domain of [0, 1]."); }
        //    return messages;
        //}
        #endregion
    }
}
