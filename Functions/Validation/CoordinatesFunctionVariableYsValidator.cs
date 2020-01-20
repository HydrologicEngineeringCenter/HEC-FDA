using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Functions.Validation
{
    internal class CoordinatesFunctionVariableYsValidator : IValidator<ICoordinatesFunction>
    {
        public bool IsValid(ICoordinatesFunction entity, out IEnumerable<IMessage> errors)
        {
            errors = ReportErrors(entity);
            return !errors.Any();
        }

        public IEnumerable<IMessage> ReportErrors(ICoordinatesFunction entity)
        {
            List<IMessage> errors = new List<IMessage>();
            if (entity.Coordinates == null) throw new InvalidConstructorArgumentsException("The list of coordinates were null.");

            if (!IsFunction(entity.Coordinates))
            {
                errors.Add(IMessageFactory.Factory(IMessageLevels.FatalError, "The specified set of coordinate is invalid. At least one x value maps to more than one y value " +
                    "(e.g. the set does not meet the definition of a function)."));
            }


            //if (AreAnyXsTheSame(entity))
            //{
            //    errors.Add(IMessageFactory.Factory(IMessageLevels.FatalError, "The coordinates function has multiple coordinates with the same X value. This is not allowed."));
            //    return errors;
            //}
            if (!AreXsIncreasing(entity))
            {
                errors.Add(IMessageFactory.Factory(IMessageLevels.Message, "The coordinates function has X values that are out of order. " +
                    "The coordinates have been sorted and stored in the database so that all X values are increasing."));

            }
            if (entity.Coordinates.Count == 1)
            {
                errors.Add(IMessageFactory.Factory(IMessageLevels.Error, "The coordinates function has only one coordinate. This will not produce good results"));
            }

            return errors;
        }
        private bool AreAnyXsTheSame(ICoordinatesFunction func)
        {
            List<ICoordinate> coords = func.Coordinates;
            double prevValue = coords[0].X.Value();
            for (int i = 1; i < coords.Count; i++)
            {
                double nextValue = coords[i].X.Value();
                if (nextValue != prevValue)
                {
                    prevValue = nextValue;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private bool AreXsIncreasing(ICoordinatesFunction func)
        {
            List<ICoordinate> coords = func.Coordinates;
            double prevValue = coords[0].X.Value();
            for (int i = 1; i < coords.Count; i++)
            {
                double nextValue = coords[i].X.Value();
                if (nextValue > prevValue)
                {
                    prevValue = nextValue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsValid(List<ICoordinate> coordinates)
        {
            if (Utilities.Validate.IsNullOrEmptyCollection(coordinates as ICollection<ICoordinate>)) return false;
            if (!IsFunction(coordinates)) throw new ArgumentException("The specified set of coordinate is invalid. At least one x value maps to more than one y value (e.g. the set does not meet the definition of a function).");
            return true;
        }
        private bool IsFunction(List<ICoordinate> xys)
        {
            for (int i = 0; i < xys.Count; i++)
            {
                int j = i + 1;
                while (j < xys.Count)
                {
                    if (xys[i].X.Equals(xys[j].X)) return false;
                    else j++;
                }
            }
            return true;
        }
    }
}
