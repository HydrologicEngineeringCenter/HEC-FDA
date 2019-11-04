using System;
using System.Collections.Generic;
using System.Text;
using Functions;
using Functions.CoordinatesFunctions;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    internal class DamageFrequencyComputable :ImpactAreaFunctionBase<double>, IComputableFrequencyFunction
    {
        public override string XLabel => "Frequency";

        public override string YLabel => "Damage";

        private IFunction _Function;

        internal DamageFrequencyComputable(IFunction function) : base(function, ImpactAreaFunctionEnum.DamageFrequency)
        {
            _Function = function;
        }

        public IComputableFrequencyFunction Compose(IComputableTransformFunction transform)
        {
            if(transform.Type - 1 == this.Type)
            {
                IFunction composedFunction = _Function.Compose(transform.TransformFunction);
                
                return ImpactAreaFunctionFactory.CreateNew(composedFunction, transform.Type + 1);
            }
            else
            {
                throw new ArgumentException(ReportCompositionError());
            }
        }      

        public double Integrate()
        {
            return _Function.RiemannSum();

        }


        //public IFunctionCompose Compose(ITransformFunction transform, double frequencyFunctionProbability, double transformFunctionProbability)
        //{
        //    if (transform.Type - 1 == Type)
        //        return ImpactAreaFunctionFactory.CreateNew(Function.Sample(frequencyFunctionProbability).Compose(transform.Sample(transformFunctionProbability).Ordinates), transform.Type + 1);
        //    else throw new ArgumentException(ReportCompositionError());
        //}
        private string ReportCompositionError()
        {
            return "Composition could not be initialized because no transform function was provided or the two functions do not share a common set of ordinates.";
        }
       

        //#region IValidateData Methods
        //public override bool Validate()
        //{
        //    if (AreValidOrdinates() &&
        //        Function.IsValid) return true;
        //    else { ReportValidationErrors(); return false; }
        //}
        //public override IEnumerable<string> ReportValidationErrors()
        //{
        //    IList<string> messages = Function.IsValid ? new List<string>() : Function.ReportValidationErrors().ToList();

        //    foreach (var pair in Function.GetOrdinates())
        //    {
        //        if (IsValidOrdinate(pair) == false)
        //            messages.Add(string.Format("The damage frequency ordinate ({0}, {1}) is invalid. " +
        //            "The frequency values (e.g. xs) must be on the interval [0, 1], the damage values (e.g. ys) must be greater than 0.", pair.Item1, pair.Item2));
        //    }
        //    return messages;
        //}
        //private bool AreValidOrdinates()
        //{
        //    foreach (var pair in Function.GetOrdinates())
        //    {
        //        if (IsValidOrdinate(pair) == false) return false;
        //    }
        //    return true;
        //}
        //private bool IsValidOrdinate(Tuple<double, double> ordinate)
        //{
        //    if (ordinate.Item1 < 0 ||
        //        ordinate.Item1 > 1 ||
        //        ordinate.Item2 < 0) return false;
        //    else return true;
        //}

    }
}
