using System;
using System.Linq;
using System.Collections.Generic;
using Functions;
using System.Xml.Linq;

namespace Model.Functions
{
    internal sealed class OutflowFrequency : FdaFunctionBase, IFrequencyFunction
    {
        #region Properties
        public override IParameterSeries XSeries { get; }
        public override IParameterSeries YSeries { get; }
        public override IParameterEnum ParameterType => IParameterEnum.OutflowFrequency;
        public List<IParameterEnum> ComposeableTypes => new List<IParameterEnum>() { IParameterEnum.Rating, IParameterEnum.ExteriorInteriorStage };
        #endregion

        #region Constructor
        internal OutflowFrequency(IFunction fx, string xLabel = "", UnitsEnum yUnits = UnitsEnum.CubicFootPerSecond, string yLabel = ""): base(fx)
        {
            XSeries = IParameterFactory.Factory(this, true, UnitsEnum.Probability, xLabel);
            YSeries = IParameterFactory.Factory(this, false, yUnits, yLabel);
        }
        #endregion

        #region Functions
        public double Integrate() => _Function.TrapizoidalRiemannSum();
        #endregion

        #region Throw Away
        #region IFunctionCompose Methods
        //public IFunctionCompose Compose(ITransformFunction transform)
        //{
        //    if (transform.Type - 1 == Type)
        //        return ImpactAreaFunctionFactory.CreateNew(Function.Compose(transform.Ordinates), transform.Type + 1);
        //    else ReportCompositionError(); throw new NotImplementedException();
        //}

        //public IFrequencyFunction Compose(ITransformFunction transform, double frequencyFunctionProbability, double transformFunctionProbability)
        //{
        //    //if (transform.Type - 1 == Type)
        //    //    return ImpactAreaFunctionFactory.CreateNew(Function.Sample(frequencyFunctionProbability).Compose(transform.Sample(transformFunctionProbability).Ordinates), transform.Type + 1);
        //    //else ReportCompositionError(); throw new NotImplementedException();


        //    if (transform.Type - 1 == Type)
        //    {
        //        IFunction outflowFreq = Sampler.Sample(Function, frequencyFunctionProbability);
        //        IFunction transformFunc = Sampler.Sample(transform.Function, transformFunctionProbability);
        //        IFunction composedFunc = outflowFreq.Compose(transformFunc);
        //        return new InteriorStageFrequency(composedFunc);
        //    }
        //    else
        //    {
        //        throw new ArgumentException(ReportCompositionError());
        //    }

        //}
        //private string ReportCompositionError()
        //{
        //    return "Composition could not be initialized because no transform function was provided or the two functions do not share a common set of ordinates.";
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
        //    if (Function.ValidateFrequencyValues() == false) { IsValid = false; messages.Add("The frequency function is invalid because it contains ordinates outside of the valid domain of [0, 1]."); }
        //    return messages;
        //}
        #endregion
        #endregion
    }
}
