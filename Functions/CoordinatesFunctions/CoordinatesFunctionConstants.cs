using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Functions.CoordinatesFunctions
{
    internal sealed class CoordinatesFunctionConstants : IFunction
    {
        #region Properties
        public List<ICoordinate<Constant, Constant>> Coordinates { get; }

        public Tuple<double, double> Range { get; }
        public Tuple<double, double> Domain { get; }
        public InterpolationEnum Interpolator { get; }
        public OrderedSetEnum Order { get; }
        private Func<int, double, double> InterpolationFunction { get; }
        private Func<int, double, double> InverseInterpolationFunction { get; }
        public bool IsInvertible { get; }

        public bool IsDistributed => false;

        #endregion

        #region Constructor
        internal CoordinatesFunctionConstants(List<ICoordinate<Constant, Constant>> coordinates, InterpolationEnum interpolation = InterpolationEnum.NoInterpolation) 
        {
            if (IsValid(coordinates))
            {
                Coordinates = SortByXs(coordinates);
                IsInvertible = IsInvertibleFunction();
            }
            Order = ComputeSetOrder();
            Range = new Tuple<double, double>(Coordinates[0].Y.Value(), Coordinates[Coordinates.Count - 1].Y.Value());
            Domain = new Tuple<double, double>(Coordinates[0].X.Value(), Coordinates[Coordinates.Count - 1].X.Value());
            Interpolator = interpolation;
            InterpolationFunction = SetInterpolator(interpolation);
            InverseInterpolationFunction = IsInvertible ? SetInverseInterpolator(interpolation) : null;

        }


        #endregion

        #region Functions
        #region Initialization Functions
        public List<ICoordinate<Constant, Constant>> SortByXs(List<ICoordinate<Constant, Constant>> coordinates)
        {
            return coordinates.OrderBy(xy => xy.X.Value()).ToList();
        }

        private bool IsValid(List<ICoordinate<Constant, Constant>> coordinates)
        {
            if (Utilities.Validation.IsNullOrEmptyCollection(coordinates as ICollection<ICoordinate<Constant, Constant>>))
            {
                return false;
            }
            if (!IsFunction(coordinates))
            {
                throw new ArgumentException("The specified set of coordinate is invalid. At least one x value maps to more than one y value (e.g. the set does not meet the definition of a function).");
            }
            return true;
        }
        private bool IsFunction(List<ICoordinate<Constant, Constant>> xys)
        {
            for (int i = 0; i < xys.Count; i++)
            {
                int j = i + 1;
                while (j < xys.Count)
                {
                    if (xys[i].X.Equals(xys[j].X) && !xys[i].Y.Value().Equals(xys[j].Y.Value())) return false;
                    else j++;
                }
            }
            return true;
        }
        private Func<int, double, double> SetInterpolator(InterpolationEnum methodOfInterpolation = InterpolationEnum.NoInterpolation)
        {
            switch (methodOfInterpolation)
            {
                case InterpolationEnum.Linear: return LinearInterpolator;
                case InterpolationEnum.Piecewise: return PiecewiseInterpolator;
                default: return NoInterpolator;
            }
        }
        private double LinearInterpolator(int i, double x) => Coordinates[i].Y.Value() + (x - Coordinates[i].X.Value()) / (Coordinates[i + 1].X.Value() - Coordinates[i].X.Value()) * (Coordinates[i + 1].Y.Value() - Coordinates[i].Y.Value());
        private double PiecewiseInterpolator(int i, double x) => ((x - Coordinates[i].X.Value()) < (Coordinates[i + 1].X.Value() - Coordinates[i].X.Value()) / 2) ? Coordinates[i].Y.Value() : Coordinates[i + 1].Y.Value();
        private double NoInterpolator(int i, double x) => x == Coordinates[i].X.Value() ? Coordinates[i].Y.Value() : throw new InvalidOperationException(String.Format("The F(x) operation cannot produce a result because no interpolation method has been set and the specified x value: {0} was not explicitly provided as part of the function domain.", x));

        private Func<int, double, double> SetInverseInterpolator(InterpolationEnum methodOfInterpolation = InterpolationEnum.NoInterpolation)
        {
            switch (methodOfInterpolation)
            {
                case InterpolationEnum.Linear: return InverseLinearInterpolator;
                case InterpolationEnum.Piecewise: return InversePiecewiseInterpolator;
                default: return InverseNoInterpolator;
            }
        }
        private double InverseLinearInterpolator(int i, double y) => Coordinates[i].X.Value() + (y - Coordinates[i].Y.Value()) / (Coordinates[i + 1].Y.Value() - Coordinates[i].Y.Value()) * (Coordinates[i + 1].X.Value() - Coordinates[i].X.Value());
        private double InversePiecewiseInterpolator(int i, double y) => ((y - Coordinates[i].Y.Value()) < (Coordinates[i + 1].Y.Value() - Coordinates[i].Y.Value()) / 2) ? Coordinates[i].X.Value() : Coordinates[i + 1].X.Value();
        private double InverseNoInterpolator(int i, double y) => y == Coordinates[i].Y.Value() ? Coordinates[i].X.Value() : throw new InvalidOperationException(string.Format("The InverseF(y) operation cannot produce a result because no interpolation method has been set and the specified y value: {0} was not explicityly provided as part of the function domain.", y));

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
        private OrderedSetEnum InitialOrder(List<ICoordinate<Constant, Constant>> ordinates) => ordinates.Count == 1 ? OrderedSetEnum.NonMonotonic : OrderOfPair(ordinates, 0);
        private OrderedSetEnum OrderOfPair(List<ICoordinate<Constant, Constant>> ordinates, int index) => ordinates[index].Y.Value() == ordinates[index + 1].Y.Value() ? OrderedSetEnum.NonMonotonic : ordinates[index].Y.Value() < ordinates[index + 1].Y.Value() ? OrderedSetEnum.StrictlyIncreasing : OrderedSetEnum.StrictlyDecreasing;
        private OrderedSetEnum UpdateOrderOfSet(OrderedSetEnum orderOfSet, OrderedSetEnum orderOfPair, out bool hasChangedOrder)
        {
            hasChangedOrder = false;
            return orderOfSet == orderOfPair ? orderOfSet : orderOfPair == OrderedSetEnum.NonMonotonic ? UpdateSetOrderWithNonMonotonicPair(orderOfSet) : UpdateOrderOfSet(orderOfSet, orderOfPair, out hasChangedOrder);
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
        public bool IsInvertibleFunction()
        {
            for (int i = 0; i < Coordinates.Count; i++)
            {
                int j = i + 1;
                while (j < Coordinates.Count)
                {
                    if (Coordinates[i].Y.Value().Equals(Coordinates[j].Y.Value()) && !Coordinates[i].X.Value().Equals(Coordinates[j].Y.Value())) return false;
                    else j++;
                }
            }
            return true;
        }
        #region F(x)
        public Constant F(Constant x)
        {
            if (Utilities.Validation.IsNull(x)) throw new ArgumentNullException("The specified x value is invalid because it is null");
            else return new Constant(F(x.Value()));
        }
        public double F(double x)
        {
            if (!IsOnDomain(x)) throw new ArgumentOutOfRangeException(
                string.Format("The specified x value: {0} is invalid because it is not on the domain of the coordinates" +
                " function [{1}, {2}].", x, Coordinates[0].X.Value(), Coordinates[Coordinates.Count - 1].X.Value()));
            int i = 0;
            if (!(i == Coordinates.Count - 1))
            {
                while (Coordinates[i + 1].X.Value() < x) i++;
                if (Coordinates[i + 1].X.Value() == x) return Coordinates[i + 1].Y.Value();
            }
            return InterpolationFunction(i, x);
        }
        private bool IsOnDomain(double x) => x < Domain.Item1 || x > Domain.Item2 ? false : true;
        #endregion

        #region InverseF(y)
       
        public Constant InverseF(Constant y)
        {
            if (Utilities.Validation.IsNull(y)) throw new ArgumentNullException("The specified y value is invalid because it is null");
            if (!Utilities.Validation.IsFinite(y.Value())) throw new ArgumentOutOfRangeException(string.Format("The specified y value: {0} is not finite.", y));
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
            if (!Utilities.Validation.IsFinite(y)) throw new ArgumentOutOfRangeException(string.Format("The specified y value: {0} is not finite.", y));
            // TODO: OnRange()  check  - so this works with decreasing functions.
            if (!IsOnRange(y)) throw new ArgumentOutOfRangeException(string.Format("The specified y values: {0} is invalid because it is not on the domain of the inverse coordinates function [{1}, {2}] (e.g. range of coordinates function).",
                y, Coordinates[0].Y.Value(), Coordinates[Coordinates.Count - 1].Y.Value()));
            if (y < Coordinates[i].Y.Value() || y > Coordinates[i + 1].Y.Value()) throw new ArgumentException(
                string.Format("The InverseF operation could not be completed because the specified y: {0} is not on the implicitly defined range: [{1}, {2}].",
                y, Coordinates[i].Y.Value(), Coordinates[i + 1].Y.Value()));
            if (Coordinates[i + 1].Y.Value() == y) return Coordinates[i + 1].Y.Value();
            else return InverseInterpolationFunction(i, y);
        }
        private bool IsOnRange(double y) => y < Range.Item1 || y > Range.Item2 ? false : true;     
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

            List<ICoordinate<Constant, Constant>> fog = new List<ICoordinate<Constant, Constant>>();
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
                        if (!(g.Interpolator == InterpolationEnum.NoInterpolation))
                            fog.Add(ICoordinateFactory.Factory(Coordinates[i].X.Value(),   g.F(Coordinates[i].Y).Value()));
                        i++;
                    }
                    else // A Z should be added and X interpolated
                    {
                        // Add new ordinate to FoG if F allows Interpolation between ordinates
                        if (!(Interpolator == InterpolationEnum.NoInterpolation))
                            fog.Add(ICoordinateFactory.Factory(new Constant(InverseF(g.Coordinates[j].X.Value(), i - 1)).Value(), new Constant(g.Coordinates[j].Y.Value()).Value()));
                        j++;
                    }
                }
            }
            // Past overlapping area or at end of both functions
            return IFunctionFactory.Factory(fog, g.Interpolator);
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
        private bool IsComplete(int i, int I, int j, int J, IFunction g) => (IsFinalIndex(i, I, j, J) || IsXOffOverlap(i, J, g) || IsZOffOverlap(I, j, g)) ? true : false;
        private bool IsXOffOverlap(int i, int J, IFunction g) => Coordinates[i].Y.Value() > g.Coordinates[J - 1].X.Value() ? true : false;
        private bool IsZOffOverlap(int I, int j, IFunction g) => Coordinates[I - 1].Y.Value() < g.Coordinates[j].X.Value() ? true : false;
        private bool IsFinalIndex(int i, int I, int j, int J) => (i == I || j == J) ? true : false;
        private string NoOverlapMessage(IFunction g) => string.Format("The functional composition operation could not be performed. The range of F: [{0}, {1}] in the composition equation F(G(x)) does not overlap the domain of G: [{2}, {3}].", Range.Item1, Range.Item2, g.Domain.Item1, g.Domain.Item2);
        #endregion

        #region RiemannSum()
        public double RiemannSum()
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
        public bool Equals(CoordinatesFunctionConstants n)
        {
            //return n.Range.Equals(Range) && n.Domain.Equals(Domain) && n.Coordinates.SequenceEqual(Coordinates);
            bool rangesEqual = n.Range.Equals(Range);
            bool domainsEqual = n.Domain.Equals(Domain);
            bool coordsEqual = n.Coordinates.SequenceEqual(Coordinates);

            return rangesEqual && domainsEqual && coordsEqual;
        }

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
                foreach (ICoordinate<IOrdinate, IOrdinate> i in Coordinates) hash = hash * 23 + i.GetHashCode();
                return hash;
            }
        }

        //public IFunction Sample(double p)
        //{
        //    return this;
        //}

        //public IFunction Sample(double p, InterpolationEnum interpolator)
        //{
        //    return new CoordinatesFunctionConstants(Coordinates, interpolator);
        //}

        #endregion
        #endregion
    }
}
