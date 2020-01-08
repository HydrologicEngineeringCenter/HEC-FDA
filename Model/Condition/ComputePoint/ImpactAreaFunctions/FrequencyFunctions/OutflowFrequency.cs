using System;
using System.Linq;
using System.Collections.Generic;
using Functions;
using Functions.Ordinates;
using System.Xml.Linq;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    internal sealed class OutflowFrequency : ImpactAreaFunctionBase, IFrequencyFunction
    {
        #region Properties
        public override string XLabel => "Stage";

        public override string YLabel => "Flow";
        #endregion

        #region Constructor
        internal OutflowFrequency(ICoordinatesFunction function) : base(function, ImpactAreaFunctionEnum.OutflowFrequency)
        {
           
        }

        public IFrequencyFunction Compose(ITransformFunction transformFunction, double p1, double p2)
        {
            if (transformFunction.Type - 1 == Type)
            {
                IFunction outflowFreq = Sampler.Sample(Function, p1);
                IFunction transformFunc = Sampler.Sample(transformFunction.Function, p2);
                IFunction composedFunc = outflowFreq.Compose(transformFunc);
                return new InteriorStageFrequency(composedFunc);
            }
            else
            {
                throw new ArgumentException("Unable to compose the transform function to this outflow frequency function. The transform function " +
                    "must be a rating curve.");
            }
        }

       

        public override XElement WriteToXML()
        {
            throw new NotImplementedException();
        }




        //public IComputableFrequencyFunction Sample(double p)
        //{
        //    IFunction coordFunc = Function.Sample(p);
        //    return new OutflowFrequencyComputable(coordFunc);
        //}
        #endregion

        //#region IFunctionCompose Methods
        ////NEED TO IMPLEMENT THIS...
        //public IFunctionCompose Sample(double probability)
        //{
        //    throw new NotImplementedException();
        //    //return Function.Sample(probability);
        //}

        //public IFunctionCompose Compose(ITransformFunction transform)
        //{
        //    if (transform.Type - 1 == Type)
        //        return ImpactAreaFunctionFactory.CreateNew(Function.Compose(transform.Ordinates), transform.Type + 1);
        //    else ReportCompositionError(); throw new NotImplementedException();
        //}

        //public IFunctionCompose Compose(ITransformFunction transform, double frequencyFunctionProbability, double transformFunctionProbability)
        //{
        //    if (transform.Type - 1 == Type)
        //        return ImpactAreaFunctionFactory.CreateNew(Function.Sample(frequencyFunctionProbability).Compose(transform.Sample(transformFunctionProbability).Ordinates), transform.Type + 1);
        //    else ReportCompositionError(); throw new NotImplementedException();
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
        //    if (Function.ValidateFrequencyValues() == false) { IsValid = false; messages.Add("The frequency function is invalid because it contains ordinates outside of the valid domain of [0, 1]."); }
        //    return messages;
        //}
        //#endregion
    }
}
