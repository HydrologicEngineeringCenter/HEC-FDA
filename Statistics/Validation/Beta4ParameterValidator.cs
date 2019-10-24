using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Statistics.Distributions;

using Utilities;

namespace Statistics.Validation
{
    internal class Beta4ParameterValidator: IValidator<Beta4Parameters>
    {
        public bool IsValid(Beta4Parameters entity, out IEnumerable<IMessage> errors)
        {
            errors = ReportErrors(entity);
            return !errors.Any();
        } 

        public IEnumerable<IMessage> ReportErrors(Beta4Parameters entity)
        {
            if (entity.IsNull()) throw new ArgumentNullException(nameof(entity), $"The Beta Distribution could not be validated because it is null.");
            if (!(entity.A.IsFinite() && entity.B.IsFinite() && entity.Location.IsFinite() && entity.Scale.IsFinite())) yield return IMessageFactory.Factory(IMessageLevels.Error, $"The Scaled 4 Parameter Beta Distribution is invalid because one or more of its parameters {entity.Print()} are not finite.");
            if (entity.SampleSize < 2) yield return IMessageFactory.Factory(IMessageLevels.Error, $"The Beta Distribution Sample Size {entity.SampleSize} is invalid because it is less than 2.");
        }
    }
}
