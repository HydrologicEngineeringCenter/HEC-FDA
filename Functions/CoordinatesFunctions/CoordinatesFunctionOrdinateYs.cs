using Functions.Coordinates;
using Functions.Ordinates;
using Statistics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Functions.CoordinatesFunctions
{
    /// <summary>
    /// This class exists as a way to more generically hold a list of functions. 
    /// This is necessary for the LinkedCoordinatesFunction.
    /// </summary>
    internal class CoordinatesFunctionOrdinateYs : ICoordinatesFunction<Constant, IOrdinate>
    {
        private CoordinatesFunctionConstants _constFunction;
        private CoordinatesFunctionVariableYs _distributedFunction;
        public bool IsDistributed { get; }
        public Tuple<double, double> Range
        {
            get
            {
                if (IsDistributed)
                {
                    return null;
                }
                else
                {
                    return _constFunction.Range;
                }
            }
        }

        public OrderedSetEnum Order
        {
            get
            {
                if (IsDistributed)
                {
                    return OrderedSetEnum.NonMonotonic;
                }
                else
                {
                    return _constFunction.Order;
                }
            }
        }
        public List<ICoordinate<Constant, IOrdinate>> Coordinates
        {
            get
            {
                if (IsDistributed)
                {
                    return ConvertDistributedYsToOrdinates(_distributedFunction.Coordinates);
                }
                else
                {
                    return ConvertConstantYsToOrdinates(_constFunction.Coordinates);
                }
            }
        }
        public InterpolationEnum Interpolator { get; }


        internal CoordinatesFunctionOrdinateYs(CoordinatesFunctionConstants constFunction)
        {
            _constFunction = constFunction;
            IsDistributed = false;
            Interpolator = constFunction.Interpolator;
        }

        internal CoordinatesFunctionOrdinateYs(CoordinatesFunctionVariableYs distributedFunction)
        {
            _distributedFunction = distributedFunction;
            IsDistributed = true;
            Interpolator = distributedFunction.Interpolator;
        }



        private List<ICoordinate<Constant, IOrdinate>> ConvertDistributedYsToOrdinates(List<ICoordinate<Constant, Distribution>> coords)
        {
            List<ICoordinate<Constant, IOrdinate>> retval = new List<ICoordinate<Constant, IOrdinate>>();
            foreach (ICoordinate<Constant, IDistribution> coord in coords)
            {
                retval.Add(new CoordinateOrdinateY(coord.X, new Distribution(coord.Y)));
            }
            return retval;
        }

        private List<ICoordinate<Constant, IOrdinate>> ConvertConstantYsToOrdinates(List<ICoordinate<Constant, Constant>> coords)
        {
            List<ICoordinate<Constant, IOrdinate>> retval = new List<ICoordinate<Constant, IOrdinate>>();
            foreach (ICoordinate<Constant, Constant> coord in coords)
            {
                retval.Add(new CoordinateOrdinateY(coord.X, coord.Y));
            }
            return retval;
        }


        public Tuple<double, double> Domain
        {
            get
            {
                if (IsDistributed)
                {
                    return _distributedFunction.Domain;
                }
                else
                {
                    return _constFunction.Domain;
                }
            }
        }


        public IOrdinate F(Constant x)
        {

            if (IsDistributed)
            {
                return _distributedFunction.F(x);
            }
            else
            {
                return _constFunction.F(x);
            }

        }

        public Constant InverseF(IOrdinate y)
        {

            if (IsDistributed)
            {
                IDistribution dist = ((Distribution)y).GetDistribution;
                return _distributedFunction.InverseF(new Distribution(dist));
            }
            else
            {
                return _constFunction.InverseF(new Constant( y.Value()));
            }

        }

        //public IFunction Sample(double p)
        //{
        //    if (IsDistributed)
        //    {
        //        return new CoordinatesFunctionConstants(_distributedFunction.Sample(p).Coordinates, Interpolator); ;
        //    }
        //    else
        //    {
        //        return _constFunction.Sample(p);
        //    }
        //}

        //public IFunction Sample(double p, InterpolationEnum interpolator)
        //{
        //    if (IsDistributed)
        //    {
        //        return _distributedFunction.Sample(p, interpolator);
        //    }
        //    else
        //    {
        //        return _constFunction.Sample(p, interpolator);
        //    }
        //}
    }
}
