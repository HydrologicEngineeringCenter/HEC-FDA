using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Functions.CoordinatesFunctions
{
    internal abstract class CoordinatesFunction: ICoordinatesFunction<IOrdinate, IOrdinate>
    {
        #region Properties
        public bool IsInvertible { get; }
        public Tuple<double, double> Domain { get; }

        public IImmutableList<ICoordinate<IOrdinate, IOrdinate>> Coordinates { get; }

        //todo: do this
        public bool IsDistributed => throw new NotImplementedException();
        #endregion

        #region Constructor
        internal CoordinatesFunction(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> coordinates)
        {
            if (IsValid(coordinates))
            {
                Coordinates = SortByXs(coordinates);
                IsInvertible = IsInvertibleFunction();
            }
        }
        #endregion

        #region Functions
        #region Initialization Functions
        private bool IsValid(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> coordinates)
        {
            if (Utilities.Validation.IsNullOrEmptyCollection(coordinates as ICollection<ICoordinate<IOrdinate, IOrdinate>>)) return false;
            if (!IsFunction(coordinates)) throw new ArgumentException("The specified set of coordinate is invalid. At least one x value maps to more than one y value (e.g. the set does not meet the definition of a function).");
            return true;
        }
        private bool IsFunction(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> xys)
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
        public static IImmutableList<ICoordinate<IOrdinate, IOrdinate>> SortByXs(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> coordinates) => coordinates.OrderBy(xy => xy.X.Value()).ToImmutableList();
        public bool IsInvertibleFunction()
        {
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
        #endregion

        #region F(x) InverseF(y)
        public abstract IOrdinate F(IOrdinate x);
        public abstract IOrdinate InverseF(IOrdinate y);
        #endregion
        #endregion
    }
}
