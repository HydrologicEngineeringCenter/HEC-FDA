using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Utilities;

namespace Functions.CoordinatesFunctions
{
    internal sealed class CoordinatesFunctionConstants : IFunction, IValidate<ICoordinatesFunction>
    {
        private readonly List<ICoordinate> _ProvidedCoordinates;
        private readonly MathNet.Numerics.Interpolation.CubicSpline _NaturalCubicSpline;
        private readonly MathNet.Numerics.Interpolation.CubicSpline _InverseNaturalCublicSpline;
        #region Properties
        public List<ICoordinate> Coordinates { get; }
        public Utilities.IRange<double> Range { get; }
        public Utilities.IRange<double> Domain { get; }
        public InterpolationEnum Interpolator { get; }
        public OrderedSetEnum Order { get; }
        private Func<int, double, double> InterpolationFunction { get; }
        private Func<int, double, double> InverseInterpolationFunction { get; }
        public bool IsInvertible { get; }

        public bool IsDistributed => false;
        public IOrdinateEnum DistributionType 
        { 
            get
            {
                if(Coordinates.Count>0)
                {
                    return Coordinates[0].Y.Type;
                }
                else
                {
                    return IOrdinateEnum.NotSupported;
                }

            }
        }
        public bool IsLinkedFunction => false;

        public IMessageLevels State { get; internal set; }
        public IEnumerable<IMessage> Messages { get; set; }
        #endregion

        #region Constructor
        internal CoordinatesFunctionConstants(List<ICoordinate> coordinates, InterpolationEnum interpolation = InterpolationEnum.None)
        {
            if (!Validation.CoordinatesFunctionConstantsValidator.IsConstructable(coordinates, interpolation, out string msg)) throw new InvalidConstructorArgumentsException(msg);
            else
            {
                Interpolator = interpolation;
                _ProvidedCoordinates = SortByXs(coordinates);
                var xys = GetOrdinateArrays(_ProvidedCoordinates);
                if (Interpolator == InterpolationEnum.NaturalCubicSpline) _NaturalCubicSpline = SetCubicSplineFunction(xys);
                InterpolationFunction = SetInterpolator(Interpolator);
                Order = SetTheOrder();
                Domain = IRangeFactory.Factory(_ProvidedCoordinates.First().X.Value(), _ProvidedCoordinates.Last().X.Value());
                Range = SetRange();
                IsInvertible = IsInvertibleFunction();
                if (IsInvertible)
                {
                    if (Interpolator == InterpolationEnum.NaturalCubicSpline) _InverseNaturalCublicSpline = SetInverseCubicSplineFunction(xys);
                    InverseInterpolationFunction = SetInverseInterpolator(Interpolator);
                }            
                Coordinates = SetSortedCoordinates(_ProvidedCoordinates);
            }
            //Coordinates = coordinates;
            //IsValid = Validate(new Validation.CoordinatesFunctionConstantsValidator(), out IEnumerable<IMessage> errors);
            //Messages = errors;
            ////if (IsValid(coordinates))
            //{
            //    Coordinates = SortByXs(coordinates);
            //    IsInvertible = IsInvertibleFunction();
            //}
            //Order = SetTheOrder(); //ComputeSetOrder();
            //Range = Utilities.IRangeFactory.Factory(Coordinates.First().Y.Value(), Coordinates.Last().Y.Value());
            ////Range = new Tuple<double, double>(Coordinates[0].Y.Value(), Coordinates[Coordinates.Count - 1].Y.Value());
            //Domain = Utilities.IRangeFactory.Factory(Coordinates.First().X.Value(), Coordinates.Last().X.Value());
            ////Domain = new Tuple<double, double>(Coordinates[0].X.Value(), Coordinates[Coordinates.Count - 1].X.Value());
            //Interpolator = interpolation;
            //InterpolationFunction = SetInterpolator(interpolation);
            //InverseInterpolationFunction = IsInvertible ? SetInverseInterpolator(interpolation) : null;
        }
        #endregion

        #region Functions
        #region Initialization
        /* To initialize:
         *      (1) Sort coordinates by x values
         *      (2) Set the Interpolation scheme
         *      (3) Determine the order of the function
         *      (4) Set the Range of the function
         *      (5) Based on the order determines if the function is invertible
         *      (6) Checks for a valid object and reports messages (including if it was a function to begin with).
         */
        private List<ICoordinate> SortByXs(List<ICoordinate> coordinates)
        {
            return coordinates.OrderBy(xy => xy.X.Value()).ToList();
        }        
        #region Interpolators
        /* This section:
         *      (1) Sets the Interpolator and InverseInterpolator Properties
         *      (2) Defines those interpolation functions.
         */
        private Func<int, double, double> SetInterpolator(InterpolationEnum methodOfInterpolation = InterpolationEnum.None)
        {
            switch (methodOfInterpolation)
            {
                case InterpolationEnum.Linear: return LinearInterpolator;
                case InterpolationEnum.Piecewise: return PiecewiseInterpolator;
                case InterpolationEnum.NaturalCubicSpline: return NaturalCubicSplineInterpolator;
                default: return NoInterpolator;
            }
        }
        private double LinearInterpolator(int i, double x) => Coordinates[i].Y.Value() + (x - Coordinates[i].X.Value()) / (Coordinates[i + 1].X.Value() - Coordinates[i].X.Value()) * (Coordinates[i + 1].Y.Value() - Coordinates[i].Y.Value());
        private double PiecewiseInterpolator(int i, double x) => ((x - Coordinates[i].X.Value()) < (Coordinates[i + 1].X.Value() - Coordinates[i].X.Value()) / 2) ? Coordinates[i].Y.Value() : Coordinates[i + 1].Y.Value();
        private MathNet.Numerics.Interpolation.CubicSpline SetCubicSplineFunction(Tuple<double[], double[]> xys)
        {
            return MathNet.Numerics.Interpolation.CubicSpline.InterpolateNaturalSorted(xys.Item1, xys.Item2);
        }
        private Tuple<double[], double[]> GetOrdinateArrays(List<ICoordinate> coordinates)
        {
            double[] xs = new double[coordinates.Count], ys = new double[coordinates.Count];
            for (int i = 0; i < coordinates.Count; i++)
            {
                xs[i] = coordinates[i].X.Value();
                ys[i] = coordinates[i].Y.Value();
            }
            return new Tuple<double[], double[]>(xs, ys);
        }
        private double NaturalCubicSplineInterpolator(int i, double x)
        {
            //The index value is irrelevant for this interpolator.
            return _NaturalCubicSpline.Interpolate(x);
        }      
        private double NoInterpolator(int i, double x) => x == Coordinates[i].X.Value() ? Coordinates[i].Y.Value() : throw new InvalidOperationException(String.Format("The F(x) operation cannot produce a result because no interpolation method has been set and the specified x value: {0} was not explicitly provided as part of the function domain.", x));

        private Func<int, double, double> SetInverseInterpolator(InterpolationEnum methodOfInterpolation = InterpolationEnum.None)
        {
            switch (methodOfInterpolation)
            {
                case InterpolationEnum.Linear: return InverseLinearInterpolator;
                case InterpolationEnum.Piecewise: return InversePiecewiseInterpolator;
                case InterpolationEnum.NaturalCubicSpline: return NaturalCubicSplineInterpolator;
                default: return InverseNoInterpolator;
            }
        }
        private double InverseLinearInterpolator(int i, double y) => Coordinates[i].X.Value() + (y - Coordinates[i].Y.Value()) / (Coordinates[i + 1].Y.Value() - Coordinates[i].Y.Value()) * (Coordinates[i + 1].X.Value() - Coordinates[i].X.Value());
        private double InversePiecewiseInterpolator(int i, double y) => ((y - Coordinates[i].Y.Value()) < (Coordinates[i + 1].Y.Value() - Coordinates[i].Y.Value()) / 2) ? Coordinates[i].X.Value() : Coordinates[i + 1].X.Value();
        private MathNet.Numerics.Interpolation.CubicSpline SetInverseCubicSplineFunction(Tuple<double[], double[]> xys)
        {
            return MathNet.Numerics.Interpolation.CubicSpline.InterpolateNaturalSorted(xys.Item2, xys.Item1);
        }
        private double InverseNaturalCubicSpline(int i, double y) => _InverseNaturalCublicSpline.Interpolate(y);
        private double InverseNoInterpolator(int i, double y) => y == Coordinates[i].Y.Value() ? Coordinates[i].X.Value() : throw new InvalidOperationException(string.Format("The InverseF(y) operation cannot produce a result because no interpolation method has been set and the specified y value: {0} was not explicityly provided as part of the function domain.", y));
        #endregion
        private List<ICoordinate> SetSortedCoordinates(List<ICoordinate> coordinates)
        {
            if (Interpolator == InterpolationEnum.NaturalCubicSpline)
            {
                int i = 0;
                List <ICoordinate> displayCoordinates = new List<ICoordinate>();
                double x = coordinates[0].X.Value(),  maxX = coordinates[coordinates.Count - 1].X.Value(), xEpsilon = maxX - x / 100;
                while(x < maxX)
                {
                    if (coordinates[i].X.Value() < x)
                    {
                        displayCoordinates.Add(coordinates[i]);
                        i++;
                    }
                    else
                    {
                        displayCoordinates.Add(ICoordinateFactory.Factory(x, F(x)));
                        x += xEpsilon;
                    }
                }
                return displayCoordinates;
            }
            else return coordinates;
        }
        #region Order
        /* This seems to contain 2 rival methods:
         *      (1) Cody's SetTheOrder() Function
         *      (2) My (John's) ComputeSetOerder() Function
         * SetTheOrder() e.g. #1 is currently being called by the constructor
         */
        private OrderedSetEnum SetTheOrder()
        {
            if (IsStraightLine())
            {
                return OrderedSetEnum.NonMonotonic;
            }
            else if (IsStrictlyIncreasing())
            {
                return OrderedSetEnum.StrictlyIncreasing;
            }
            else if (IsStrictlyDecreasing())
            {
                return OrderedSetEnum.StrictlyDecreasing;
            }
            else if (IsAtLeastWeaklyIncreasing())
            {
                return OrderedSetEnum.WeaklyIncreasing;
            }
            else if (IsAtLeastWeaklyDecreasing())
            {
                return OrderedSetEnum.WeaklyDecreasing;
            }
            else
            {
                //if it is none of the others then it would have to be non monotonic
                return OrderedSetEnum.NonMonotonic;
            }
        }
        private bool IsStraightLine()
        {
            if (_ProvidedCoordinates.Count < 2)
            {
                return false;
            }
            for (int i = 0; i < _ProvidedCoordinates.Count - 1; i++)
            {
                ICoordinate coord1 = _ProvidedCoordinates[i];
                ICoordinate coord2 = _ProvidedCoordinates[i + 1];
                if (!coord1.Y.Equals(coord2.Y))
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsAtLeastWeaklyDecreasing()
        {
            for (int i = 0; i < _ProvidedCoordinates.Count - 1; i++)
            {
                ICoordinate coord1 = _ProvidedCoordinates[i];
                ICoordinate coord2 = _ProvidedCoordinates[i + 1];
                if (!IsAtLeastWeaklyDecreasing(coord1, coord2))
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsAtLeastWeaklyIncreasing()
        {
            for (int i = 0; i < _ProvidedCoordinates.Count - 1; i++)
            {
                ICoordinate coord1 = _ProvidedCoordinates[i];
                ICoordinate coord2 = _ProvidedCoordinates[i + 1];
                if (!IsAtLeastWeaklyIncreasing(coord1, coord2))
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsStrictlyDecreasing()
        {
            for (int i = 0; i < _ProvidedCoordinates.Count - 1; i++)
            {
                ICoordinate coord1 = _ProvidedCoordinates[i];
                ICoordinate coord2 = _ProvidedCoordinates[i + 1];
                if (!IsCoordinatesStrictlyDecreasing(coord1, coord2))
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsStrictlyIncreasing()
        {
            for (int i = 0; i < _ProvidedCoordinates.Count - 1; i++)
            {
                ICoordinate coord1 = _ProvidedCoordinates[i];
                ICoordinate coord2 = _ProvidedCoordinates[i + 1];
                if (!IsCoordinatesStrictlyIncreasing(coord1, coord2))
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsAtLeastWeaklyDecreasing(ICoordinate coord1, ICoordinate coord2)
        {
            bool areXsIncreasing = coord1.X.Value() < coord2.X.Value();
            bool areYsDecreasing = coord1.Y.Value() >= coord2.Y.Value();
            return areXsIncreasing && areYsDecreasing;
        }
        private bool IsAtLeastWeaklyIncreasing(ICoordinate coord1, ICoordinate coord2)
        {
            bool areXsIncreasing = coord1.X.Value() < coord2.X.Value();
            bool areYsIncreasing = coord1.Y.Value() <= coord2.Y.Value();
            return areXsIncreasing && areYsIncreasing;
        }
        private bool IsCoordinatesStrictlyIncreasing(ICoordinate coord1, ICoordinate coord2)
        {
            bool areXsIncreasing = coord1.X.Value() < coord2.X.Value();
            bool areYsIncreasing = coord1.Y.Value() < coord2.Y.Value();
            return areXsIncreasing && areYsIncreasing;
        }
        private bool IsCoordinatesStrictlyDecreasing(ICoordinate coord1, ICoordinate coord2)
        {
            bool areXsIncreasing = coord1.X.Value() < coord2.X.Value();
            bool areYsDecreasing = coord1.Y.Value() > coord2.Y.Value();
            return areXsIncreasing && areYsDecreasing;
        }

        // ComputeSetOrder e.g. Method #2
        private OrderedSetEnum ComputeSetOrder()
        {
            OrderedSetEnum order = InitialOrder(Coordinates);
            for (int i = 0; i < Coordinates.Count - 1; i++)
            {
                order = UpdateOrderOfSet(order, OrderOfPair(Coordinates, i), out bool hasChangedOrder);
                if (hasChangedOrder == true) break;
            }
            return order;
        }
        private OrderedSetEnum InitialOrder(List<ICoordinate> ordinates) => ordinates.Count == 1 ? OrderedSetEnum.NonMonotonic : OrderOfPair(ordinates, 0);
        private OrderedSetEnum OrderOfPair(List<ICoordinate> ordinates, int index)
        {
            if (ordinates[index].Y.Value() == ordinates[index + 1].Y.Value())
            {
                return OrderedSetEnum.NonMonotonic;
            }
            else if (ordinates[index].Y.Value() < ordinates[index + 1].Y.Value())
            {
                return OrderedSetEnum.StrictlyIncreasing;
            }
            else
            {
                return OrderedSetEnum.StrictlyDecreasing;
            }
        }
        private OrderedSetEnum UpdateOrderOfSet(OrderedSetEnum orderOfSet, OrderedSetEnum orderOfPair, out bool hasChangedOrder)
        {
            hasChangedOrder = false;
            if (orderOfSet == orderOfPair)
            {
                return orderOfSet;
            }
            else if (orderOfPair == OrderedSetEnum.NonMonotonic)
            {
                return UpdateSetOrderWithNonMonotonicPair(orderOfSet);
            }
            else
            {
                //TODO: there is an infinite loop here sometimes.
                return UpdateOrderOfSet(orderOfSet, orderOfPair, out hasChangedOrder);
            }
        }
        private OrderedSetEnum UpdateSetOrderWithNonMonotonicPair(OrderedSetEnum orderOfSet) => IsStrictEnum(orderOfSet) ? orderOfSet + 1 : orderOfSet;
        private OrderedSetEnum UpdateSetOrderWithMonotonicPair(OrderedSetEnum orderOfSet, OrderedSetEnum orderOfPair, out bool hasChangedOrder)
        {
            hasChangedOrder = false;
            if (orderOfSet == OrderedSetEnum.NonMonotonic) return orderOfPair + 1;
            else if (IsIncreasingEnum(orderOfSet) == IsIncreasingEnum(orderOfPair)) return (OrderedSetEnum)Math.Min((int)orderOfSet, (int)orderOfPair);
            else
            {
                hasChangedOrder = true;
                return OrderedSetEnum.NonMonotonic;
            }
        }
        private bool IsStrictEnum(OrderedSetEnum orderEnum) => (int)orderEnum % 2 == 1 ? true : false;
        private bool IsIncreasingEnum(OrderedSetEnum orderEnum) => orderEnum == OrderedSetEnum.StrictlyIncreasing || orderEnum == OrderedSetEnum.WeaklyIncreasing ? true : false;
        #endregion
        private IRange<double> SetRange()
        {
            switch (Order) 
            {
                case OrderedSetEnum.NonMonotonic:
                    double minY = double.PositiveInfinity, maxY = double.NegativeInfinity;
                    foreach (var coordinate in _ProvidedCoordinates)
                    {
                        if (coordinate.Y.Value() < minY) minY = coordinate.Y.Value();
                        if (coordinate.Y.Value() > maxY) maxY = coordinate.Y.Value(); 
                    }
                    return IRangeFactory.Factory(minY, maxY);
                case OrderedSetEnum.StrictlyIncreasing:
                case OrderedSetEnum.WeaklyIncreasing:
                    return IRangeFactory.Factory(_ProvidedCoordinates.First().Y.Value(), _ProvidedCoordinates.Last().Y.Value());
                case OrderedSetEnum.StrictlyDecreasing:
                case OrderedSetEnum.WeaklyDecreasing:
                    return IRangeFactory.Factory(_ProvidedCoordinates.Last().Y.Value(), _ProvidedCoordinates.First().Y.Value());
                default:
                    throw new InvalidOperationException($"The {nameof(Order)} property value: {Order} was not properly captured by the SetRange() function.");
            }
        }
        public bool IsInvertibleFunction()
        {
            for (int i = 0; i < _ProvidedCoordinates.Count; i++)
            {
                int j = i + 1;
                while (j < _ProvidedCoordinates.Count)
                {
                    if (_ProvidedCoordinates[i].Y.Value().Equals(_ProvidedCoordinates[j].Y.Value()) && !_ProvidedCoordinates[i].X.Value().Equals(_ProvidedCoordinates[j].Y.Value())) return false;
                    else j++;
                }
            }
            return true;
        }
        public IMessageLevels Validate(IValidator<ICoordinatesFunction> validator, out IEnumerable<IMessage> errors)
        {
            return validator.IsValid(this, out errors);
        }
        
        #region Unused Code
        //private bool IsValid(List<ICoordinate> coordinates)
        //{
        //    if (Utilities.Validation.IsNullOrEmptyCollection(coordinates as ICollection<ICoordinate>))
        //    {
        //        return false;
        //    }
        //    foreach(ICoordinate coord in coordinates)
        //    {
        //        if(!IsCoordinateValid(coord))
        //        {
        //            throw new ArgumentException("One or more coordinates have an invalid value.");
        //        }
        //    }
        //    if (!IsFunction(coordinates))
        //    {
        //        throw new ArgumentException("The specified set of coordinate is invalid. At least one x value maps to more than one y value (e.g. the set does not meet the definition of a function).");
        //    }
        //    return true;
        //}

        /// <summary>
        /// Checks that the ordinates that make up the coordinate have valid values:
        /// no double.nan, no infinity
        /// </summary>
        /// <returns></returns>
        private bool IsCoordinateValid(ICoordinate coord)
        {
            if(double.IsNaN( coord.X.Value()) || double.IsNaN(coord.Y.Value()) ||
                double.IsInfinity(coord.X.Value()) || double.IsInfinity(coord.Y.Value()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
        #endregion

        #region F(x)
        public Constant F(Constant x)
        {
            if (Utilities.ValidationExtensions.IsNull(x)) throw new ArgumentNullException("The specified x value is invalid because it is null");
            else return F(x);
        }
        public IOrdinate F(IOrdinate x)
        {
            if (!IsOnDomain(x.Value()))
            {
                throw new ArgumentOutOfRangeException(string.Format("The specified x value: {0} is invalid because it is not on the domain of the coordinates function [{1}, {2}].",
                x, Coordinates[0].X.Value(), Coordinates[Coordinates.Count - 1].X.Value()));
            }
            else return new Constant(F(x.Value()));
        }
        private bool IsOnDomain(double x) => x < Domain.Min || x > Domain.Max ? false : true;
        private double F(double x)
        {
            /* 3 cases to deal with:
             *      (1) Interpolation is cubic spline -> just use that interpolator
             *      (2) x is an exact match for coordinate X value -> return coordinate Y value.
             *      (3) the value must be interpolated (by something other than the cubic spline interpolator).
             */
            // Case 1: Cubic Spline
            if (Interpolator == InterpolationEnum.NaturalCubicSpline) return InterpolationFunction(0, x);
            // Case 2: Look for match
            for (int i = 0; i < Coordinates.Count; i++)
            {
                if (Coordinates[i].X.Value() == x)
                {
                    return Coordinates[i].Y.Value();
                }
            }
            // Case 3: Need (non cubic spline) Interpolator
            int j = 0;
            if (!(j == Coordinates.Count - 1))
            {
                while (Coordinates[j + 1].X.Value() < x) j++;
            }
            return InterpolationFunction(j, x);
        }
        #endregion
        #region InverseF(y)    
        public IOrdinate InverseF(IOrdinate y)
        {
            if (Utilities.ValidationExtensions.IsNull(y)) throw new ArgumentNullException("The specified y value is invalid because it is null");
            if (!Utilities.ValidationExtensions.IsFinite(y.Value())) throw new ArgumentOutOfRangeException(string.Format("The specified y value: {0} is not finite.", y));
            if (!IsInvertible) throw new InvalidOperationException("The function InverseF(y) is invalid for this set of coordinates. The inverse of F(x) is not a function, because one or more y values maps to multiple x values");
            if (!IsOnRange(y.Value())) throw new ArgumentOutOfRangeException(string.Format("The specified y values: {0} is invalid because it is not on the domain of the inverse coordinates function [{1}, {2}] (e.g. range of coordinates function).",
                y, Coordinates[0].Y.Value(), Coordinates[Coordinates.Count - 1].Y.Value()));
            int i = 0;
            if (!(i == Coordinates.Count - 1))
            {
                while (Coordinates[i + 1].Y.Value() < y.Value()) i++;
                if (Coordinates[i + 1].Y.Value() == y.Value()) return Coordinates[i + 1].X;
            }
            return new Constant( SetInverseInterpolator(Interpolator)(i, y.Value()));
        }
        private double InverseF(double y, int i)
        {
            // TODO: IsFinite() IsNaN() check
            if ( !Utilities.ValidationExtensions.IsFinite(y)) throw new ArgumentOutOfRangeException(string.Format("The specified y value: {0} is not finite.", y));
            // TODO: OnRange()  check  - so this works with decreasing functions.
            if (!IsOnRange(y)) throw new ArgumentOutOfRangeException(string.Format("The specified y values: {0} is invalid because it is not on the domain of the inverse coordinates function [{1}, {2}] (e.g. range of coordinates function).",
                y, Coordinates[0].Y.Value(), Coordinates[Coordinates.Count - 1].Y.Value()));
            if (y < Coordinates[i].Y.Value() || y > Coordinates[i + 1].Y.Value()) throw new ArgumentException(
                string.Format("The InverseF operation could not be completed because the specified y: {0} is not on the implicitly defined range: [{1}, {2}].",
                y, Coordinates[i].Y.Value(), Coordinates[i + 1].Y.Value()));
            if (Coordinates[i + 1].Y.Value() == y) return Coordinates[i + 1].Y.Value();
            else return InverseInterpolationFunction(i, y);
        }
        private bool IsOnRange(double y) => y < Range.Min || y > Range.Max ? false : true;
        #endregion
        #region Compose()     
        public IFunction Compose(IFunction g)
        {
            // Advance F Ordinate index until F[i].y >= G[0].x 
            int i = FirstX(g), I = Coordinates.Count; // - 1;
            if (i == I) throw new InvalidOperationException(NoOverlapMessage(g));
            // Advance G Ordinate index until G[j].x >= F[0].y - then move back to j - 1.
            int j = FirstZ(g), J = g.Coordinates.Count; // - 1;
            if (j == J) throw new InvalidOperationException(NoOverlapMessage(g));

            List<ICoordinate> fog = new List<ICoordinate>();
            while (!IsComplete(i, I, j, J, g)) // InOverlapping Portion
            {
                if (Coordinates[i].Y.Value() == g.Coordinates[j].X.Value()) //Matching ordinate
                {
                    fog.Add(ICoordinateFactory.Factory(Coordinates[i].X.Value(), new Constant(g.Coordinates[j].Y.Value()).Value()));
                    i++;
                    j++;
                }
                else // Mismatching ordinate
                {
                    if (Coordinates[i].Y.Value() < g.Coordinates[j].X.Value()) // An X should be added and Z interpolated
                    {
                        // Add new ordinate to FoG if G allows interpolation between ordinates
                        if (!(g.Interpolator == InterpolationEnum.None))
                            fog.Add(ICoordinateFactory.Factory(Coordinates[i].X.Value(), g.F(Coordinates[i].Y).Value()));
                        i++;
                    }
                    else // A Z should be added and X interpolated
                    {
                        // Add new ordinate to FoG if F allows Interpolation between ordinates
                        if (!(Interpolator == InterpolationEnum.None))
                            fog.Add(ICoordinateFactory.Factory(InverseF(g.Coordinates[j].X.Value(), i - 1), g.Coordinates[j].Y.Value()));
                        j++;
                    }
                }
            }
            if (fog.Count == 0)
            {
                throw new ArgumentException("Error composing two functions. The composition produced zero coordinates.");
            }
            // Past overlapping area or at end of both functions
            IFunction composedFunction = IFunctionFactory.Factory(fog, g.Interpolator);

            return composedFunction;
        }
        private int FirstX(IFunction g)
        {
            int i = 0, I = Coordinates.Count;
            while (Coordinates[i].Y.Value() < g.Coordinates[0].X.Value())
            {
                i++;
                if (i == I) break;
            }
            return i;
        }
        private int FirstZ(IFunction g)
        {
            int j = 0, J = g.Coordinates.Count; //- 1;
            while (g.Coordinates[j].X.Value() < Coordinates[0].Y.Value())
            {
                j++;
                if (j == J) break;
            }
            return j;
        }
        private bool IsComplete(int i, int I, int j, int J, IFunction g)
        {
            return (IsFinalIndex(i, I, j, J) || (IsXOffOverlap(i, J, g) && IsZOffOverlap(I, j, g)));
        }
        private bool IsXOffOverlap(int i, int J, IFunction g)
        {
            bool retval = Coordinates[i].Y.Value() > g.Coordinates[J - 1].X.Value();
            return retval;
        }
        private bool IsZOffOverlap(int I, int j, IFunction g)
        {
            bool retval = Coordinates[I - 1].Y.Value() < g.Coordinates[j].X.Value();
            return retval;
        }
        private bool IsFinalIndex(int i, int I, int j, int J)
        {
            bool retval = (i == I || j == J);
            return retval;
        }
        private string NoOverlapMessage(IFunction g) => string.Format("The functional composition operation could not be performed. The range of F: [{0}, {1}] in the composition equation F(G(x)) does not overlap the domain of G: [{2}, {3}].", Range.Min, Range.Max, g.Domain.Min, g.Domain.Max);
        #endregion
        #region RiemannSum()
        public double TrapizoidalRiemannSum()
        {
            double riemannSum = 0;
            for (int i = 0; i < Coordinates.Count - 1; i++)
            {
                riemannSum += (Coordinates[i + 1].Y.Value() + Coordinates[i].Y.Value()) * (Coordinates[i + 1].X.Value() - Coordinates[i].X.Value()) / 2;
            }
            return riemannSum;
        }
        #endregion
        #region Equals()
        public bool Equals(ICoordinatesFunction function)
        {
            //I don't think i have to check the domain, range, or order because
            //if the coordinates and interpolator are all the same then those values
            //should be the same.
            if (!function.GetType().Equals(typeof(CoordinatesFunctionConstants)))
            {
                return false;
            }
            if (Coordinates.Count != function.Coordinates.Count)
            {
                return false;
            }
            for (int i = 0; i < Coordinates.Count; i++)
            {
                if (!Coordinates[i].X.Equals(function.Coordinates[i].X))
                {
                    return false;
                }
                if (!Coordinates[i].Y.Equals(function.Coordinates[i].Y))
                {
                    return false;
                }
            }
            if (Interpolator != function.Interpolator)
            {
                return false;
            }
            return true;
        }
        public bool Equals(CoordinatesFunctionConstants n)
        {
            //return n.Range.Equals(Range) && n.Domain.Equals(Domain) && n.Coordinates.SequenceEqual(Coordinates);
            bool rangesEqual = n.Range.Equals(Range);
            bool domainsEqual = n.Domain.Equals(Domain);
            bool coordsEqual = n.Coordinates.SequenceEqual(Coordinates);

            return rangesEqual && domainsEqual && coordsEqual;
        }

        /* This region contains 2 functions that can probably be deleted:
         *      (1) override bool Equals(Object)
         *      (2) override int GetHashCode()
         */
        public override bool Equals(Object obj)
        {
            if (!(obj is CoordinatesFunctionConstants) || obj is null)
            {
                return false;
            }
            else
            {
                return Equals(obj as CoordinatesFunctionConstants);
            }
        }
        /// <summary>
        /// Custom Hash Code implementation based off of this link:
        /// https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        /// </summary>
        /// <returns> The hash code as an integer. </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Range.GetHashCode();
                hash = hash * 23 + Domain.GetHashCode();
                hash = hash * 23 + Interpolator.GetHashCode();
                // NOTE: Delegate functions have shoddy hashcode implementations, as noted here:
                // https://stackoverflow.com/questions/6624151/why-do-2-delegate-instances-return-the-same-hashcode
                // Will need to fix
                hash = hash * 23 + Interpolator.GetHashCode();
                hash = hash * 23 + InverseInterpolationFunction.GetHashCode();
                foreach (ICoordinate i in Coordinates) hash = hash * 23 + i.GetHashCode();
                return hash;
            }
        }
        #endregion

        public XElement WriteToXML()
        {
            XElement functionsElem = new XElement("Functions");
            functionsElem.SetAttributeValue("Type", "NotLinked");

            XElement funcElem = new XElement("Function");
            funcElem.SetAttributeValue("Interpolator", Interpolator);

            foreach (ICoordinate coord in Coordinates)
            {
                funcElem.Add(coord.WriteToXML());
            }

            functionsElem.Add(funcElem);
            return functionsElem;
        }

        //This is a bit of code I'm adding slowly - to allow me to interpolate graphical frequency functions.
        public ICoordinate Spline(double t)
        {
            // I think this requires that t is a double [0, 1]
            t = t - (int)t;
            double t2 = t * t, t3 = t2 * t;
            // These ps are index points.
            // I think the ps below *might need to* be adjusted based on the value of the actual coordinates.
            int p0 = (int)t, p1 = p0 + 1, p2 = p1 + 1, p3 = p2 + 1;
            //these are influential field values (how much the line is being pulled toward each point.
            double q1 = -t3 + 2d * t2 - t;
            double q2 =  3d * t3 - 5d * t2 + 2d;
            double q3 = -3d * 4d * t2 + t;
            double q4 = t3 - t2;
            // tx
            double tx = 0.5 * Coordinates[p0].X.Value() * q1 + Coordinates[p1].X.Value() * q2 + Coordinates[p2].X.Value() * q3 + Coordinates[p3].X.Value() * q4;
            double ty = 0.5 * Coordinates[p0].Y.Value() * q1 + Coordinates[p1].Y.Value() * q2 + Coordinates[p2].Y.Value() * q3 + Coordinates[p3].Y.Value() * q4;
            return ICoordinateFactory.Factory(tx, ty);
        }

      
        #endregion


    }
}
