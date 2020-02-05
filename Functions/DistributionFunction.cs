using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Functions.CoordinatesFunctions;
using Utilities;

namespace Functions
{
    internal class DistributionFunction : IFunction, IValidate<ICoordinatesFunction>
    {
        private readonly IDistributedOrdinate _Distribution;
        private readonly CoordinatesFunctionConstants _CoordinatesFunction;
        
        public bool IsInvertible => true;
        public IRange<double> Domain { get; }
        public IRange<double> Range => _Distribution.Range;
        public IOrdinateEnum DistributionType => _Distribution.Type;
        public OrderedSetEnum Order => OrderedSetEnum.StrictlyIncreasing;
        public InterpolationEnum Interpolator => InterpolationEnum.Statistical;
        public List<ICoordinate> Coordinates { get; }
        public bool IsLinkedFunction => false;

        public bool IsValid { get; }
        public IEnumerable<IMessage> Messages { get; }

        internal DistributionFunction(IDistributedOrdinate distribution)
        {
            _Distribution = distribution;
            Domain = IRangeFactory.Factory(0, 1);
            Coordinates = GetCoordinates();
            _CoordinatesFunction = new CoordinatesFunctionConstants(Coordinates, InterpolationEnum.Linear);
            IsValid = Validate(new Validation.DistributionFunctionValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }
        private List<ICoordinate> GetCoordinates()
        {
            double x = 0d, y = Range.Min;
            //Initialize coordinates list with x = 0, P(X <=0) = Range.Min coordinate.
            List<ICoordinate> coordinates = new List<ICoordinate>() { ICoordinateFactory.Factory(x, y) };
            //Interpolation is over a minimum of 1% or the x values or 1% of the y values.
            double xEpsilon = 0.01, nextX = x + xEpsilon, yEpsilon = (Range.Max - Range.Min) / 100, nextY = y + yEpsilon;
            while (!IsLastOrdinate(nextX, nextY))
            {
                if (UseNextX(nextX, nextY, out y))
                {
                    x = nextX;
                    nextX += xEpsilon;
                    coordinates.Add(ICoordinateFactory.Factory(x, y));
                }
                else
                {
                    y = nextY;
                    nextY += yEpsilon;
                    coordinates.Add(ICoordinateFactory.Factory(_Distribution.CDF(y), y));
                }
            }
            return coordinates;            
        }
        private bool IsLastOrdinate(double nextX, double nextY) => nextX >= 1 && nextY >= Range.Max;
        private bool UseNextX(double candidateX, double candidateY, out double nextY)
        {
            nextY = _Distribution.InverseCDF(candidateX);
            if (nextY < candidateY)
            {
                return true;
            }
            else
            {
                nextY = candidateY;
                return false;
            }
        }

        public bool Validate(IValidator<ICoordinatesFunction> validator, out IEnumerable<IMessage> errors)
        {
            return validator.IsValid(this, out errors);
        }

        public IFunction Compose(IFunction g)
        {
            return _CoordinatesFunction.Compose(g);
        }
        public bool Equals(ICoordinatesFunction function)
        {
            if (!(function.IsLinkedFunction == IsLinkedFunction && function.Interpolator == Interpolator && function.Order == Order && function.Coordinates.Count == Coordinates.Count)) return false;
            for (int i = 0; i < Coordinates.Count; i++) if (!function.Coordinates[i].Equals(Coordinates[i])) return false;
            return true;
        }
        public IOrdinate F(IOrdinate x)
        {
            return new Ordinates.Constant(_Distribution.CDF(x.Value()));
        }
        public IOrdinate InverseF(IOrdinate y)
        {
            return new Ordinates.Constant(_Distribution.InverseCDF(y.Value()));
        }
        public double TrapizoidalRiemannSum()
        {
            return _CoordinatesFunction.TrapizoidalRiemannSum();
        }
        public XElement WriteToXML()
        {
            throw new NotImplementedException();
        }  
    }
}
