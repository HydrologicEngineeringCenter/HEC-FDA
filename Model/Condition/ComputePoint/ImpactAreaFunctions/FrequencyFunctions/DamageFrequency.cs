using System;
using System.Linq;
using System.Collections.Generic;
using Functions;
using Utilities;
using Model.Validation;
using Functions.Ordinates;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    internal sealed class DamageFrequency : ImpactAreaFunctionBase,IFrequencyFunction, IValidate<DamageFrequency>
    {
        #region Properties
        public override string XLabel => "Frequency";

        public override string YLabel => "Damage";

        public bool IsValid { get; }

        public IEnumerable<IMessage> Errors { get; }
        #endregion


        #region Constructor
        internal DamageFrequency(ICoordinatesFunction function) : base(function, ImpactAreaFunctionEnum.DamageFrequency)
        {
            IsValid = Validate(new DamageFrequencyValidator(), out IEnumerable<IMessage> errors);
            Errors = errors;
        }

        #endregion

        #region IFunctionCompose Methods

        public bool Validate(IValidator<DamageFrequency> validator, out IEnumerable<IMessage> errors)
        {
            return validator.IsValid(this, out errors);
        }

        public IFrequencyFunction Compose(ITransformFunction transformFunction, double probability1, double probability2)
        {
            //nothing should try to compose with this.
            throw new NotImplementedException();
        }
        #endregion

        //public IFunctionCompose Compose(ITransformFunction transform, double frequencyFunctionProbability, double transformFunctionProbability)
        //{
        //    if (transform.Type - 1 == Type)
        //        return ImpactAreaFunctionFactory.CreateNew(Function.Sample(frequencyFunctionProbability).Compose(transform.Sample(transformFunctionProbability).Ordinates), transform.Type + 1);
        //    else throw new ArgumentException(ReportCompositionError());
        //}

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
