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
        #region Fields
        private readonly IDistributedOrdinate _Distribution;
        private readonly CoordinatesFunctionConstants _CoordinatesFunction;
        #endregion
        #region Properties
        public bool IsInvertible => true;
        public IRange<double> Domain { get; }
        public IRange<double> Range => _Distribution.Range;
        public IOrdinateEnum DistributionType => _Distribution.Type;
        public OrderedSetEnum Order => OrderedSetEnum.StrictlyIncreasing;
        public InterpolationEnum Interpolator => InterpolationEnum.Statistical;
        public List<ICoordinate> Coordinates { get; }
        public bool IsLinkedFunction => false;

        public IMessageLevels State { get; }
        public IEnumerable<IMessage> Messages { get; }
        #endregion
        #region Constructor
        internal DistributionFunction(IDistributedOrdinate distribution)
        {
            _Distribution = distribution;
            Domain = IRangeFactory.Factory(0d, 1d);
            Coordinates = FillInCoordinates();
            _CoordinatesFunction = new CoordinatesFunctionConstants(Coordinates, InterpolationEnum.Linear);
            State = Validate(new Validation.DistributionFunctionValidator(), out IEnumerable<IMessage> msgs);
            Messages = msgs;
        }
        #endregion
        #region Functions
        private List<ICoordinate> FillInCoordinates()
        {
            /* 
             * This is produces an approximation of the statistical distribution with a set of coordinates. It limits:
             *  (1) probability steps to 1% of 'y' range (i.e. 0.01)
             *  (2) 10 probability steps between p = 0.99 and .999 (big values)
             *  (3) x steps to 1% of the x range between p = 0.001 and 0.999 (up to 1000 year for flood analysis)
             */
            double pEpsilon = 0.01, pMax = 0.99, p = 0.001;
            double y = _Distribution.InverseCDF(0.001), yMax = _Distribution.InverseCDF(0.999), yEpsilon = (yMax - y) / 100;
            List<ICoordinate> expandedCoordinates = new List<ICoordinate>();
            while (p < pMax)
            {
                if (p < pEpsilon)
                {
                    expandedCoordinates.Add(ICoordinateFactory.Factory(p, y));
                    p = 0;
                }
                p = UpdateP(p + pEpsilon, y + yEpsilon, pMax);
                expandedCoordinates.Add(ICoordinateFactory.Factory(p, F(p)));
            }
            pMax = 0.999;
            pEpsilon = 0.001;
            while (p < pMax)
            {
                p = UpdateP(p + pEpsilon, y + yEpsilon, pMax);
                expandedCoordinates.Add(ICoordinateFactory.Factory(p, F(p)));
            }    
            return expandedCoordinates;
        }
        private double UpdateP(double nextP, double nextY, double pMax)
        {
            /*
             * Finds minimum of:
             *  (1) p + pEpsilon
             *  (2) p associated with y + yEpsilon
             *  (3) pMax
             */
            double p = nextP < InverseF(nextY) ? nextP : InverseF(nextY);
            return p < pMax ? p : pMax;
        }
        public List<ICoordinate> GetExpandedCoordinates() => Coordinates;

        public IMessageLevels Validate(IValidator<ICoordinatesFunction> validator, out IEnumerable<IMessage> errors)
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
        /// <summary>
        /// Computes <see cref="IDistributedOrdinate.InverseCDF(double)"/>, returning the value from the <see cref="IDistributedOrdinate"/> distribution associated with the specified non-exceedance probability, <paramref name="p"/>.
        /// <b> Note</b>: this inverts the statistical convention of F(x) representing the non-exceedance probabilities, and x values representing the distributed random number values.
        /// </summary>
        /// <param name="p"> The value from the <see cref="IDistributedOrdinate"/>. </param>
        /// <returns> A <see cref="double"/> precision non-exceedance value provided as an <see cref="IOrdinate"/>. </returns>
        public IOrdinate F(IOrdinate p)
        {
            return new Ordinates.Constant(F(p.Value()));
        }
        public double F(double p)
        {
            // Here we flip x and y from statistics.
            return _Distribution.InverseCDF(p);
        } 
        /// <summary>
        /// Computes <see cref="IDistributedOrdinate.CDF(double)"/>, returning the non-exceedance probability from the <see cref="IDistributedOrdinate"/> distribution associated with the specified random value of <paramref name="y"/>.
        /// <b> Note</b>: this inverts the statistical convention of F(x) representing the non-exceedance probabilities, and x values representing the distributed random number values. 
        /// </summary>
        /// <param name="y"> A non exceedance value. </param>
        /// <returns> A distributed <see cref="double"/> precision value from the underlying <see cref="IDistributedOrdinate"/>. </returns>
        public IOrdinate InverseF(IOrdinate y)
        {
            return new Ordinates.Constant(InverseF(y.Value()));
        }
        public double InverseF(double y)
        {
            // Here we flip x and y from statistics.
            return _Distribution.CDF(y);
        }
        public double TrapizoidalRiemannSum()
        {
            return _CoordinatesFunction.TrapizoidalRiemannSum();
        }
        public XElement WriteToXML()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
