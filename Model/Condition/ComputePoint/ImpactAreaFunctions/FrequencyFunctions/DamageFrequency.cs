using System;
using System.Linq;
using System.Collections.Generic;
using Functions;
using Utilities;
using Model.Validation;

namespace Model.Condition.ComputePoint.ImpactAreaFunctions
{
    internal sealed class DamageFrequency<YType> : ImpactAreaFunctionBase<YType>, IFrequencyFunction<YType>, IValidate<DamageFrequency<YType>>
    {
        #region Properties
        public override string XLabel => "Frequency";

        public override string YLabel => "Damage";

        public bool IsValid { get; }

        public IEnumerable<IMessage> Errors { get; }
        #endregion


        #region Constructor
        internal DamageFrequency(ICoordinatesFunction<double, YType> function) : base(function, ImpactAreaFunctionEnum.DamageFrequency)
        {
            IsValid = Validate(new DamageFrequencyValidator<YType>(), out IEnumerable<IMessage> errors);
            Errors = errors;
        }

        #endregion

        #region IFunctionCompose Methods

        //IComputableFrequencyFunction IFrequencyFunction<YType>.Sample(double p)
        //{
        //    IFunction coordFunc = Function.Sample(p);
        //    return new DamageFrequencyComputable(coordFunc);
        //}

        public bool Validate(IValidator<DamageFrequency<YType>> validator, out IEnumerable<IMessage> errors)
        {
            return validator.IsValid(this, out errors);
        }
        #endregion
    }
}
