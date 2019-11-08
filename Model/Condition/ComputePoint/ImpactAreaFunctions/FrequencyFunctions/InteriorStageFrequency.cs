using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Functions;
using Functions.Ordinates;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    internal sealed class InteriorStageFrequency<YType> : ImpactAreaFunctionBase
    {
        #region Properties
        public override string XLabel => "Stage";

        public override string YLabel => "Flow";
        #endregion

        #region Constructor
        internal InteriorStageFrequency(ICoordinatesFunction function) : base(function, ImpactAreaFunctionEnum.InteriorStageFrequency)
        {
            
        }

        //public IComputableFrequencyFunction Sample(double p)
        //{
        //    IFunction coordFunc = Function.Sample(p);
        //    return new InteriorStageFrequencyComputable(coordFunc);
        //}
        #endregion

        //#region IFunctionCompose Methods
        //public IFunctionCompose Compose(ITransformFunction transform, double frequencyFunctionProbability, double transformFunctionProbability)
        //{
        //    if (transform.Type - 1 == Type)
        //        return ImpactAreaFunctionFactory.CreateNew(Function.Sample(frequencyFunctionProbability).Compose(transform.Sample(transformFunctionProbability).Ordinates), transform.Type + 1);
        //    else ReportCompositionError(); return null;
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
        //#endregion

        //#region IValidateData Methods
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
        ////NEED TO IMPLEMENT THIS>>>
        //public IFunctionCompose Sample(double randomNumber)
        //{
        //    throw new NotImplementedException();
        //}
        //#endregion
    }
}
