using Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Model.Inputs.Functions.ImpactAreaFunctions
{
    internal sealed class ExteriorStageFrequency : ImpactAreaFunctionBase, IFrequencyFunction
    {
        #region Properties
        public override string XLabel => "Frequency";

        public override string YLabel => "Exterior Stage";
        #endregion

        #region Constructor
        internal ExteriorStageFrequency(ICoordinatesFunction function) : base(function, ImpactAreaFunctionEnum.ExteriorStageFrequency)
        {
            
        }
        #endregion

        #region IFunctionCompose Methods
        public override XElement WriteToXML()
        {
            throw new NotImplementedException();
        }
        public IFrequencyFunction Compose(ITransformFunction transformFunction, double frequencyFunctionProbability, double transformFunctionProbability)
        {
            //if (IsValidComposition(transform) == true)
            //    return ImpactAreaFunctionFactory.CreateNew(Function.Sample(frequencyFunctionProbability).Compose(transform.Sample(transformFunctionProbability).Ordinates), transform.Type + 1);
            //else ReportCompositionError(); return null;
            if (transformFunction.Type - 1 == Type)
            {
                IFunction extStageFreq = Sampler.Sample(Function, frequencyFunctionProbability);
                IFunction transformFunc = Sampler.Sample(transformFunction.Function, transformFunctionProbability);
                IFunction composedFunc = extStageFreq.Compose(transformFunc);
                return new DamageFrequency(composedFunc);
            }
            else
            {
                throw new ArgumentException("Unable to compose the transform function to this outflow frequency function. The transform function " +
                    "must be a rating curve.");
            }
        }
        //private bool IsValidComposition(ITransformFunction transform)
        //{
        //    if (transform.Type == ImpactAreaFunctionEnum.InteriorStageDamage)
        //    {
        //        UseType = ImpactAreaFunctionEnum.InteriorStageFrequency;
        //    }
        //    if (transform.Type - 1 == UseType) return true;
        //    else return false;
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
