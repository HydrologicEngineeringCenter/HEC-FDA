using Functions.Coordinates;
using Functions.Ordinates;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Utilities;

namespace Functions.CoordinatesFunctions
{
    internal sealed class CoordinatesFunctionVariableYs: ICoordinatesFunction
    {
        #region Properties
        public bool IsInvertible { get; }
        //todo: John i think we can get rid of the IsDistributedXs property since nothing has distributed xs.
        public bool IsDistributedXs => false;
        public bool IsDistributedYs => true;
        public OrderedSetEnum Order { get; }

        public List<ICoordinate> Coordinates { get; }

        public bool IsDistributed => true;

        public Tuple<double, double> Domain { get; }

        public InterpolationEnum Interpolator { get; }
        #endregion

        #region Constructor
        //todo: are we using this interpolator?
        internal CoordinatesFunctionVariableYs(List<ICoordinate> coordinates, InterpolationEnum interpolation = InterpolationEnum.None)
        {
            Interpolator = interpolation;
            if (IsValid(coordinates))
            {
                Coordinates = SortByXs(coordinates);
                IsInvertible = IsInvertibleFunction();
            }
            Domain = new Tuple<double, double>(Coordinates[0].X.Value(), Coordinates[Coordinates.Count - 1].X.Value());

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
        private bool IsValid(List<ICoordinate> coordinates)
        {
            if (Utilities.Validation.IsNullOrEmptyCollection(coordinates as ICollection<ICoordinate>)) return false;
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
        public List<ICoordinate> SortByXs(List<ICoordinate> coordinates) 
            => coordinates.OrderBy(xy => xy.X.Value()).ToList();

        #region F()
        public IOrdinate F(IOrdinate x)
        {
            //todo: John, i tried to do F(1) and it came through as F(1.00000000001) which threw the exception. We might want to try to fix that?
            if (Utilities.Validation.IsNull(x)) throw new ArgumentNullException("The specified x value is invalid because it is null");
            for (int i = 0; i < Coordinates.Count; i++)
            {
                if (Coordinates[i].X.Equals(x))
                {
                    return Coordinates[i].Y;
                }
            }
            throw new ArgumentOutOfRangeException("The specified x value was not found in any of the coordinates. Interpolation is not supported for coordinates with distributed x or y values");      
        }
        #endregion

        #region InverseF()
        public IOrdinate InverseF(IOrdinate y)
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


        #endregion
        #endregion
    }
}
