using Functions.Coordinates;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Functions.CoordinatesFunctions
{
    internal sealed class CoordinatesFunctionVariableYs: ICoordinatesFunction<double, IDistribution>
    {
        #region Properties
        public bool IsInvertible { get; }
        //todo: John i think we can get rid of the IsDistributedXs property since nothing has distributed xs.
        public bool IsDistributedXs => false;
        public bool IsDistributedYs => true;
        public OrderedSetEnum Order { get; }

        public List<ICoordinate<double, IDistribution>> Coordinates { get; }

        public bool IsDistributed => true;

        public Tuple<double, double> Domain { get; }

        public InterpolationEnum Interpolator => throw new NotImplementedException();
        #endregion

        #region Constructor
        //todo: are we using this interpolator?
        internal CoordinatesFunctionVariableYs(List<ICoordinate<double, IDistribution>> coordinates, InterpolationEnum interpolation = InterpolationEnum.NoInterpolation)
        {
            if (IsValid(coordinates))
            {
                Coordinates = SortByXs(coordinates);
                IsInvertible = IsInvertibleFunction();
            }
            Domain = new Tuple<double, double>(Coordinates[0].X, Coordinates[Coordinates.Count - 1].X);

        }
        #endregion

        #region Functions
        public bool IsInvertibleFunction()
        {
            //todo: John, how is this working? How can you compare ys that are distributed?
            for (int i = 0; i < Coordinates.Count; i++)
            {
                int j = i + 1;
                while (j < Coordinates.Count)
                {
                    if (Coordinates[i].Y.Equals(Coordinates[j].Y) && !Coordinates[i].X.Equals(Coordinates[j].Y)) return false;
                    else j++;
                }
            }
            return true;
        }
        private bool IsValid(List<ICoordinate<double, IDistribution>> coordinates)
        {
            if (Utilities.Validation.IsNullOrEmptyCollection(coordinates as ICollection<ICoordinate<double, IDistribution>>)) return false;
            if (!IsFunction(coordinates)) throw new ArgumentException("The specified set of coordinate is invalid. At least one x value maps to more than one y value (e.g. the set does not meet the definition of a function).");
            return true;
        }
        private bool IsFunction(List<ICoordinate<double, IDistribution>> xys)
        {
            for (int i = 0; i < xys.Count; i++)
            {
                int j = i + 1;
                while (j < xys.Count)
                {
                    if (xys[i].X.Equals(xys[j].X) && !xys[i].Y.Equals(xys[j].Y)) return false;
                    else j++;
                }
            }
            return true;
        }
        public List<ICoordinate<double, IDistribution>> SortByXs(List<ICoordinate<double, IDistribution>> coordinates) 
            => coordinates.OrderBy(xy => xy.X).ToList();

        #region F()
        public IDistribution F(double x)
        {
            //todo: John, i tried to do F(1) and it came through as F(1.00000000001) which threw the exception. We might want to try to fix that?
            if (Utilities.Validation.IsNull(x)) throw new ArgumentNullException("The specified x value is invalid because it is null");
            for (int i = 0; i < Coordinates.Count; i++) if (Coordinates[i].X.Equals(x)) return Coordinates[i].Y;
            throw new ArgumentOutOfRangeException("The specified x value was not found in any of the coordinates. Interpolation is not supported for coordinates with distributed x or y values");      
        }
        #endregion

        #region InverseF()
        public double InverseF(IDistribution y)
        {
            if (Utilities.Validation.IsNull(y)) throw new ArgumentNullException("The specified y value is invalid because it is null");
            if (IsInvertible == false) throw new InvalidOperationException("The function InverseF(y) is invalid for this set of coordinates. The inverse of F(x) is not a function, because one or more y values maps to multiple x values");
            for (int i = 0; i < Coordinates.Count; i++)
            {
                if (Coordinates[i].Y.Equals(y))
                {
                    return Coordinates[i].X;
                }
            }
            throw new ArgumentOutOfRangeException("The specified y value was not found in any of the coordinates. Interpolation is not supported for coorindates with distributed x or y values.");
        }

        public IFunction Sample(double p)
        {
            return new CoordinatesFunctionConstants(ConvertCoordinatesToConstants(p));
        }

        public IFunction Sample(double p, InterpolationEnum interpolator)
        {
            return new CoordinatesFunctionConstants(ConvertCoordinatesToConstants(p), interpolator);
        }

        private List<ICoordinate<double, double>> ConvertCoordinatesToConstants(double p)
        {
            List<ICoordinate<double, double>> coords = new List<ICoordinate<double, double>>();
            foreach (ICoordinate<double, IDistribution> coord in Coordinates)
            {
                coords.Add(new CoordinateConstants(coord.X, coord.Y.InverseCDF(p)));
            }
            return coords;
        }


        #endregion
        #endregion
    }
}
