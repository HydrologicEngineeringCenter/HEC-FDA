using System;
using System.Linq;
using System.Collections.Generic;
using Functions;
using Utilities;
using Model.Validation;
using System.Xml.Linq;

namespace Model.Inputs.Functions.ImpactAreaFunctions
{
    internal sealed class DamageFrequency : ImpactAreaFunctionBase, IFrequencyFunction, IValidate<DamageFrequency>
    {
        #region Properties
        public override string XLabel => "Frequency";

        public override string YLabel => "Damage";
        public bool IsValid { get; }
        /// <summary>
        /// The damage frequency doesn't get composed with anything. It is the last step.
        /// </summary>
        public List<ImpactAreaFunctionEnum> ComposeableTypes => new List<ImpactAreaFunctionEnum>();

        public IEnumerable<IMessage> Errors { get; }

        public IEnumerable<IMessage> Messages => throw new NotImplementedException();
        #endregion

        #region Constructor
        internal DamageFrequency(ICoordinatesFunction function) : base(function, ImpactAreaFunctionEnum.DamageFrequency)
        {
            //IsValid = Validate();
            IsValid = Validate(new DamageFrequencyValidator(), out IEnumerable<IMessage> errors);
            Errors = errors;
        }
        #endregion

        #region IFunctionCompose Methods
        //public IFrequencyFunction Compose(ITransformFunction transform, double frequencyFunctionProbability, double transformFunctionProbability)
        //{
        //    //if (transform.Type - 1 == Type)
        //    //    return ImpactAreaFunctionFactory.CreateNew(Function.Sample(frequencyFunctionProbability).Compose(transform.Sample(transformFunctionProbability).Ordinates), transform.Type + 1);
        //    //else ReportCompositionError(); return null;

        //    //nothing should try to compose with this.
        //    throw new NotImplementedException();

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
        //NEED TO IMPLEMENT THIS...
        //public IFunctionCompose Sample(double randomNumber)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion

        #region IValidateData Methods
        public bool Validate(IValidator<DamageFrequency> validator, out IEnumerable<IMessage> errors)
        {
            //if (AreValidOrdinates() && 
            //    Function.IsValid) return true;
            //else { ReportValidationErrors(); return false; }

            return validator.IsValid(this, out errors);

        }
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
        #endregion

        public override XElement WriteToXML()
        {
            throw new NotImplementedException();
        }
    }
}
