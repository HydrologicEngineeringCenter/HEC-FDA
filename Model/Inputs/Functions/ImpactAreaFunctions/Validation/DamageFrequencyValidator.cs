using Model.Condition.ComputePoint.ImpactAreaFunctions;
using Model.Inputs.Functions.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Model.Validation
{
    class DamageFrequencyValidator : IValidator<DamageFrequency>
    {
        public bool IsValid(DamageFrequency entity, out IEnumerable<IMessage> errors)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMessage> ReportErrors(DamageFrequency entity)
        {
            throw new NotImplementedException();
        }
    }
}
