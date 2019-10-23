using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Functions.CoordinatesFunctions
{
    internal sealed class CoordinatesFunctionVariableYs: CoordinatesFunction
    {
        #region Properties
        public bool IsDistributedXs { get; }
        public bool IsDistributedYs { get; }
        public OrderedSetEnum Order { get; }
        #endregion

        #region Constructor
        internal CoordinatesFunctionVariableYs(IImmutableList<ICoordinate<IOrdinate, IOrdinate>> coordinates): base(coordinates)
        {
            IsDistributedXs = FindDistributedXs();
            IsDistributedYs = FindDistributedYs();
        }
        #endregion

        #region Functions
        #region Initialization Functions
        private bool FindDistributedXs()
        {
            foreach (var xy in Coordinates) if (xy.X.IsDistributed) return true;
            return false;
        }
        private bool FindDistributedYs()
        {
            foreach (var xy in Coordinates) if (xy.Y.IsDistributed) return true;
            return false;
        }
        #endregion

        #region F()
        public override IOrdinate F(IOrdinate x)
        {
            if (Utilities.Validation.IsNull(x)) throw new ArgumentNullException("The specified x value is invalid because it is null");
            for (int i = 0; i < Coordinates.Count; i++) if (Coordinates[i].X.Equals(x)) return Coordinates[i].Y;
            throw new ArgumentOutOfRangeException("The specified x value was not found in any of the coordinates. Interpolation is not supported for coordinates with distributed x or y values");      
        }
        #endregion

        #region InverseF()
        public override IOrdinate InverseF(IOrdinate y)
        {
            if (Utilities.Validation.IsNull(y)) throw new ArgumentNullException("The specified y value is invalid because it is null");
            if (IsInvertible == false) throw new InvalidOperationException("The function InverseF(y) is invalid for this set of coordinates. The inverse of F(x) is not a function, because one or more y values maps to multiple x values");
            for (int i = 0; i < Coordinates.Count; i++) if (Coordinates[i].Y.Equals(y)) return Coordinates[i].Y;
            throw new ArgumentOutOfRangeException("The specified y value was not found in any of the coordinates. Interpolation is not supported for coorindates with distributed x or y values.");
        }
        #endregion
        #endregion
    }
}
