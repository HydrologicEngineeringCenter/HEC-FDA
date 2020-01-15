using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace Functions.Validation
{
    internal class LinkedCoordinatesFunctionValidator : IValidator<CoordinatesFunctionLinked>
    {
        public bool IsValid(CoordinatesFunctionLinked entity, out IEnumerable<IMessage> errors)
        {
            errors = ReportErrors(entity);
            return !errors.Any();
        }

        public IEnumerable<IMessage> ReportErrors(CoordinatesFunctionLinked entity)
        {
            List<IMessage> errors = new List<IMessage>();
            if (entity.IsNull()) throw new ArgumentNullException(nameof(entity), $"The Linked Coordinates Function could not be validated because it is null.");
            if (entity.Functions == null) throw new InvalidConstructorArgumentsException("The list of functions was null");
            if (entity.Functions.Count == 0) throw new InvalidConstructorArgumentsException("There were no functions to link together");

            if (entity.Interpolators.IsNull() || entity.Interpolators.Count != entity.Functions.Count - 1)
            {
                errors.Add(IMessageFactory.Factory(IMessageLevels.FatalError, "Not enough interpolators. There should be an interpolator between every function."));
            }

            //check that the domains don't overlap
            for (int i = 0; i < entity.Functions.Count - 2; i++)
            {
                //is the previous function's max xValue less than the next function's min xValue
                if (entity.Functions[i].Domain.Max >= entity.Functions[i + 1].Domain.Min)
                {
                    errors.Add(IMessageFactory.Factory(IMessageLevels.FatalError, "The list of functions have overlapping domains. This is not allowed."));
                    break;
                }
            }
            //todo: john, check that ranges don't overlap?
            //todo: implement this correctly
            return errors;
        }
    }
}
