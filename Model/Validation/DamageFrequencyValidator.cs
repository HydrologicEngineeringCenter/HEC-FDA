using Model.Condition.ComputePoint.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Validation
{
    class DamageFrequencyValidator<YType> : IValidator<DamageFrequency<YType>>
    {
        public bool IsValid(DamageFrequency<YType> entity, out IEnumerable<IMessage> errors)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMessage> ReportErrors(DamageFrequency<YType> entity)
        {
            throw new NotImplementedException();
        }
    }
}
