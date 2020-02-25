using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Functions.Validation
{
    internal class CoordinatesFunctionConstantsValidator : IValidator<ICoordinatesFunction>
    {
        public IMessageLevels IsValid(ICoordinatesFunction entity, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(entity);
            return msgs.Max();
        }

        public IEnumerable<IMessage> ReportErrors(ICoordinatesFunction obj)
        {
            List<IMessage> errors = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException($"The {nameof(CoordinatesFunctionConstants)} cannot be validated because it is null.");
            if (!IsFunction(obj.Coordinates, out IEnumerable<IMessage> msgs)) errors.Add(IMessageFactory.Factory(msgs.Max(), $"The {nameof(ICoordinatesFunction)} is invalid because it is not a true function: \r\n {msgs.PrintTabbedListOfMessages()}"));
            if (IsCoordinateMessages(obj.Coordinates, out IEnumerable<IMessage> coordinatemsgs)) errors.Add(IMessageFactory.Factory(coordinatemsgs.Max(), $"The function coordinates contain the following messages: \r\n {coordinatemsgs.PrintTabbedListOfMessages()}"));
            if (obj.Interpolator != InterpolationEnum.None && obj.Coordinates.Count == 0) errors.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The coordinates function specified an interpolation scheme but interpolation cannot occur because only one coordinate was provided."));
            return errors;
        }
        public static bool IsConstructable(List<ICoordinate> coordinates, InterpolationEnum interpolator, out string msg)
        {
            msg = "";
            if (coordinates.IsNullOrEmpty())
            {
                msg += $"The {nameof(ICoordinatesFunction)} cannot be constructed because the specified set of coordinates is null or empty.";
                return false;
            }
            else
            {
                for (int i = 0; i < coordinates.Count; i++)
                {
                    if (coordinates[i].IsNull())
                    {
                        msg += $"The {i}th {nameof(ICoordinate)} is null and preventing construction of the {nameof(ICoordinatesFunction)}. ";
                    }
                    else
                    {
                        if (coordinates[i].X.IsNull() || coordinates[i].Y.IsNull()) msg += $"The {i}th {nameof(ICoordinate)} X or Y {nameof(IOrdinate)} is null and preventing construction of the {nameof(ICoordinatesFunction)}. ";
                    }
                }              
            }
            if (interpolator == InterpolationEnum.NaturalCubicSpline) msg += $"A natural cubic spline interpolation scheme was specified which requires at least 3 coordinates, but only {coordinates.Count} were provided.";
            return msg.Length == 0;
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
            for(int i = 1;i<coords.Count;i++)
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
        private bool IsFunction(List<ICoordinate> xys, out IEnumerable<IMessage> msgs)
        {
            // Ensures each X value maps to exactly 1 Y value.
            bool passed = true;
            List<IMessage> errors = new List<IMessage>();
            for (int i = 0; i < xys.Count; i++)
            {
                int j = i + 1;
                while (j < xys.Count)
                {
                    if (xys[i].X.Value() == xys[j].X.Value())
                    {
                        if (xys[i].Y.Value() != xys[j].Y.Value())
                        {
                            passed = false;
                            errors.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The X {nameof(IOrdinate)}: {xys[i].X.Print(true)} maps to the following 2 Y {nameof(IOrdinate)}s: {{{xys[i].Y.Print(true)}, {xys[j].Y.Print(true)}}}."));
                        }
                        else errors.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The {nameof(ICoordinate)}: {xys[i].Print(true)} is duplicated in the {nameof(ICoordinatesFunction)}."));
                    }
                    else j++;
                }
            }
            msgs = errors;
            return passed;
        }
        private bool IsCoordinateMessages(List<ICoordinate> coordinates, out IEnumerable<IMessage> msgs)
        {
            List<IMessage> errors = new List<IMessage>();
            foreach (var coordinate in coordinates)
            {
                if (coordinate.Messages.Any())
                {
                    errors.Add(IMessageFactory.Factory(coordinate.Messages.Max(), $"The coordinate: {coordinate.Print(true)} contains the following messages: \r\n {coordinate.Messages.PrintTabbedListOfMessages()}"));
                }
            }
            msgs = errors;
            return msgs.Any();
        }
    }
}
