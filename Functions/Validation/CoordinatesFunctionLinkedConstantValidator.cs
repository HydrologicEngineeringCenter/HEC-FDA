using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.Validation;

namespace Functions.Validation
{
    class CoordinatesFunctionLinkedConstantValidator : IValidator<CoordinatesFunctionLinkedConstants>
    {
        public bool IsValid(CoordinatesFunctionLinkedConstants entity, out IEnumerable<string> errors)
        {
            errors = ReportErrors(entity);
            return !errors.Any();
        }

        public IEnumerable<string> ReportErrors(CoordinatesFunctionLinkedConstants entity)
        {
            List<string> errors = new List<string>();
            if (entity.IsNull()) throw new ArgumentNullException(nameof(entity), $"The Linked Coordinates Function could not be validated because it is null.");
            if (entity.Functions == null) throw new InvalidConstructorArgumentsException("The list of functions was null");
            if (entity.Functions.Count == 0) throw new InvalidConstructorArgumentsException("There were no functions to link together");

            if (entity.Interpolators.IsNull() || entity.Interpolators.Count != entity.Functions.Count - 1)
            {
                errors.Add("Not enough interpolators. There should be an interpolator between every function.");
            }

            //check that the domains don't overlap
            for (int i = 0; i < entity.Functions.Count - 2; i++)
            {
                //is the previous function's max xValue less than the next function's min xValue
                if (entity.Functions[i].Domain.Item2 >= entity.Functions[i + 1].Domain.Item1)
                {
                    errors.Add("The list of functions have overlapping domains. This is not allowed.");
                    break;
                }
            }
            //todo: john, check that ranges don't overlap?
            return errors;
        }
    }
}
