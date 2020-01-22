using Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Model.Inputs.Functions.ImpactAreaFunctions
{
    internal sealed class InteriorStageFrequency : ImpactAreaFunctionBase, IFrequencyFunction
    {
        #region Properties
        public override string XLabel => "Stage";

        public override string YLabel => "Flow";
        public List<ImpactAreaFunctionEnum> ComposeableTypes => new List<ImpactAreaFunctionEnum>() { ImpactAreaFunctionEnum.InteriorStageDamage};

        #endregion

        #region Constructor
        internal InteriorStageFrequency(ICoordinatesFunction function) : base(function, ImpactAreaFunctionEnum.InteriorStageFrequency)
        {
         
        }
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
        public override XElement WriteToXML()
        {
            throw new NotImplementedException();
        }
    }
}
