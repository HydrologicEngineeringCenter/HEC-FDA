using Functions;
using System.Collections.Generic;
using Utilities;

namespace Model.Functions
{
    internal sealed class InteriorStageFrequency : FdaFunctionBase, IFrequencyFunction
    {
        #region Properties
        public override string Label { get; }
        public override IParameterRange XSeries { get; }
        public override IParameterRange YSeries { get; }
        public override IParameterEnum ParameterType => IParameterEnum.InteriorStageFrequency;
        public List<IParameterEnum> ComposeableTypes => new List<IParameterEnum>() { IParameterEnum.InteriorStageDamage};

        public override IMessageLevels State { get; }
        public override IEnumerable<IMessage> Messages { get; }
        #endregion

        #region Constructor
        internal InteriorStageFrequency(IFunction fx, string label = "", string xLabel = "", string yLabel = "", UnitsEnum yUnits = UnitsEnum.Foot) : base(fx)
        {
            Label = label == "" ? ParameterType.Print() : label;
            XSeries = IParameterFactory.Factory(fx, IParameterEnum.NonExceedanceProbability, true, true, UnitsEnum.Probability, xLabel);
            YSeries = IParameterFactory.Factory(fx, IParameterEnum.InteriorElevation, IsConstant, false, yUnits, yLabel);
            State = Validate(new Validation.Functions.FdaFunctionBaseValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }
        #endregion

        #region Functions
        public double Integrate() => _Function.TrapizoidalRiemannSum();
        #endregion

        #region IFunctionCompose Methods
        //public IFrequencyFunction Compose(ITransformFunction transform, double frequencyFunctionProbability, double transformFunctionProbability)
        //{
        //    //if (transform.Type - 1 == Type)
        //    //    return ImpactAreaFunctionFactory.CreateNew(Function.Sample(frequencyFunctionProbability).Compose(transform.Sample(transformFunctionProbability).Ordinates), transform.Type + 1);
        //    //else ReportCompositionError(); return null;

        //    if (transform.Type - 1 == Type)
        //    {
        //        IFunction intStageFreq = Sampler.Sample(Function, frequencyFunctionProbability);
        //        IFunction transformFunc = Sampler.Sample(transform.Function, transformFunctionProbability);
        //        IFunction composedFunc = intStageFreq.Compose(transformFunc);
        //        return new DamageFrequency(composedFunc);
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
        //    if (Function.ValidateFrequencyValues() == false) { IsValid = false; messages.Add("The frequency function is invalid because it contain ordinates outside of the valid domain of [0, 1]."); }
        //    return messages;
        //}

        #endregion
        //public override XElement WriteToXML()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
