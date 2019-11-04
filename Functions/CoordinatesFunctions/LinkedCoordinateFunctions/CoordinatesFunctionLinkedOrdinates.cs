using Functions.Ordinates;
using Functions.Validation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Validation;

namespace Functions.CoordinatesFunctions
{
    internal class CoordinatesFunctionLinkedOrdinates : CoordinatesFunctionLinkedBase<double, IOrdinate>, ICoordinatesFunctions<double, IOrdinate>, IValidate<CoordinatesFunctionLinkedOrdinates>
    {
        
        public bool IsDistributed
        {
            //If any function is distributed then return true;
            get
            {
                bool retval = false;
                foreach(ICoordinatesFunction<double, IOrdinate> func in Functions)
                {
                    if(func.IsDistributed)
                    {
                        retval = true;
                        break;
                    }
                }
                return retval;
            }
        }

        /// <summary>
        /// The list of functions must be in the correct order.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="interpolators"></param>
        internal CoordinatesFunctionLinkedOrdinates(List<ICoordinatesFunction<double, IOrdinate>> functions, List<InterpolationEnum> interpolators)
        {
            //the list of functions are not gauranteed to be in the correct order.
            //sort them on the min x value of each function
            //todo: if i sort, i will lose track of the list of interpolators.
            //List<ICoordinatesFunction<IOrdinate, IOrdinate>> sortedFunctions = Functions.OrderBy(func => func.Domain.Item1).ToList();
            Functions = functions;
            Interpolators = interpolators;

            IsValid = Validate(new LinkedCoordinatesFunctionValidator(), out IEnumerable<IMessage> errors);
            CombineCoordinates();
            SetOrder();
            Errors = errors;
        }

        #region setting the function order

        private void SetOrder()
        {
            if (IsDistributed)
            {
                Order = OrderedSetEnum.NonMonotonic;
                return;
            }
            //now we can assume that all functions are Not Distributed
            //add the orders to a list
            SetOrderEnum();
        }







        #endregion

        #region Interpolators
        private double LinearInterpolator(ICoordinate<double, IOrdinate> coordinateLeft, ICoordinate<double, IOrdinate> coordinateRight, double x)
        {
            return coordinateLeft.Y.Value() + (x - coordinateLeft.X) / (coordinateRight.X - coordinateLeft.X) * (coordinateRight.Y.Value() - coordinateLeft.Y.Value());
        }
        private double PiecewiseInterpolator(ICoordinate<double, IOrdinate> coordinateLeft, ICoordinate<double, IOrdinate> coordinateRight, double x)
        {
            if ((x - coordinateLeft.X) < (coordinateRight.X - coordinateLeft.X) / 2)
            {
                return coordinateLeft.Y.Value(); 
            }
            else 
            { 
                return coordinateRight.Y.Value(); 
            }
        }
        private double NoInterpolator(ICoordinate<double, IOrdinate> coordinateLeft, double x)
        {
            if (x == coordinateLeft.X)
            {
                return coordinateLeft.Y.Value();
            }
            else
            {
                throw new InvalidOperationException(String.Format("The F(x) operation cannot produce a result because no interpolation method has been set and the specified x value: {0} was not explicitly provided as part of the function domain.", x));
            }
        }

        private double InverseLinearInterpolator(ICoordinate<double, IOrdinate> coordinateLeft, ICoordinate<double, IOrdinate> coordinateRight, double y)
        {
           return coordinateLeft.X + (y - coordinateLeft.Y.Value()) / 
                (coordinateRight.Y.Value() - coordinateLeft.Y.Value()) * 
                (coordinateRight.X - coordinateLeft.X);
        }
        private double InversePiecewiseInterpolator(ICoordinate<double, IOrdinate> coordinateLeft, ICoordinate<double, IOrdinate> coordinateRight, double y)
        {
            return ((y - coordinateLeft.Y.Value()) < (coordinateRight.Y.Value() - coordinateLeft.Y.Value()) / 2) 
                ? coordinateLeft.X : coordinateRight.X;
        }
        private double InverseNoInterpolator(ICoordinate<double, IOrdinate> coordinateLeft, double y)
        {
            return y == coordinateLeft.Y.Value() ? coordinateLeft.X 
                : throw new InvalidOperationException(string.Format("The InverseF(y) operation cannot produce a " +
                "result because no interpolation method has been set and the specified y value: {0} was not " +
                "explicityly provided as part of the function domain.", y));
        }


        #endregion



        public IOrdinate F(double x)
        {
            //there should be no overlapping domains because we check that in the ctor during validation
            //try to find the function that has this point within its domain.
            foreach (ICoordinatesFunction<double, IOrdinate> function in Functions)
            {
                Tuple<double, double> funcDomain = function.Domain;
                if (x >= funcDomain.Item1 && x <= funcDomain.Item2)
                {
                    return function.F(x);
                }
            }

            //if we get here then the x value is outside the function domains. 
            //check to see if it is between functions
            for (int i = 0; i < Functions.Count - 1; i++)
            {

                Tuple<double, double> func1Domain = Functions[i].Domain;
                Tuple<double, double> func2Domain = Functions[i + 1].Domain;

                if (x > func1Domain.Item2 && x < func2Domain.Item1)
                {
                    //then this x is between func1 and func2
                    InterpolationEnum interpolator = Interpolators[i];
                    double yValue = Interpolate(interpolator, Functions[i].Coordinates.Last(), Functions[i+1].Coordinates.First(), x);
                    return new Constant(yValue);
                }
            }
            //if we get here then the x value is outside the range of all functions
            return null;
        }

        private double Interpolate(InterpolationEnum interpolator, ICoordinate<double, IOrdinate> coordinateLeft, ICoordinate<double, IOrdinate> coordinateRight, double x)
        {
            double retval = Double.NaN;
            if(interpolator == InterpolationEnum.Linear)
            {
                retval = LinearInterpolator(coordinateLeft, coordinateRight, x);
            }
            else if(interpolator == InterpolationEnum.Piecewise)
            {
                retval = PiecewiseInterpolator(coordinateLeft, coordinateRight, x);
            }
            else if(interpolator == InterpolationEnum.NoInterpolation)
            {
                retval = NoInterpolator(coordinateLeft, x);
            }
            return retval;

        }

        private double InverseInterpolate(InterpolationEnum interpolator, ICoordinate<double, IOrdinate> coordinateLeft, ICoordinate<double, IOrdinate> coordinateRight, double y)
        {
            double retval = Double.NaN;
            if (interpolator == InterpolationEnum.Linear)
            {
                retval = InverseLinearInterpolator(coordinateLeft, coordinateRight, y);
            }
            else if (interpolator == InterpolationEnum.Piecewise)
            {
                retval = InversePiecewiseInterpolator(coordinateLeft, coordinateRight, y);
            }
            else if (interpolator == InterpolationEnum.NoInterpolation)
            {
                retval = InverseNoInterpolator(coordinateLeft, y);
            }
            return retval;

        }

        public double InverseF(IOrdinate y)
        {
            //i only do this if this function is strict monotonic
            //find the function that the y cooresponds to and call its inverseF
            //if inbetween then use the interpolation method that was passed in.(using the min and max points around it)
            if (Order == OrderedSetEnum.StrictlyIncreasing || Order == OrderedSetEnum.StrictlyDecreasing)
            {
                //then we can do the inverse
                //try to find the function that has this point within its range.
                foreach (ICoordinatesFunction<double, IOrdinate> function in Functions)
                {
                    if (IsYValueInFunctionRange(function, y.Value()))
                    {
                        return function.InverseF(y);
                    }
                }
                //if we get here then the y value is outside the function ranges. 
                //check to see if it is between functions
                for (int i = 0; i < Functions.Count - 1; i++)
                {
                    if (Functions[i].GetType() == typeof(CoordinatesFunctionOrdinateYs) &&
                        Functions[i + 1].GetType() == typeof(CoordinatesFunctionOrdinateYs))
                    {

                        Tuple<double, double> func1Range = ((CoordinatesFunctionOrdinateYs)Functions[i]).Range;
                        Tuple<double, double> func2Range = ((CoordinatesFunctionOrdinateYs)Functions[i + 1]).Range;

                        if (y.Value() > func1Range.Item2 && y.Value() < func2Range.Item1)
                        {
                            //then this y is between func1 and func2
                            InterpolationEnum interpolator = Interpolators[i];
                            return InverseInterpolate(interpolator, Functions[i].Coordinates.Last(), Functions[i + 1].Coordinates.Last(), y.Value());

                        }
                    }
                }
            }
            throw new InvalidOperationException("The function InverseF(y) is invalid for this set of coordinates. The inverse of F(x) is not a function, because one or more y values maps to multiple x values");
        }

        private bool IsYValueInFunctionRange(ICoordinatesFunction<double, IOrdinate> function, double yValue)
        {
            bool retval = false;
            if(function.GetType() == typeof(CoordinatesFunctionOrdinateYs))
            {
                Tuple<double, double> range = ((CoordinatesFunctionOrdinateYs)function).Range;
                if(yValue >= range.Item1 && yValue <= range.Item2)
                {
                    retval = true;
                }
            }
            return retval;
        }

        public bool Validate(IValidator<CoordinatesFunctionLinkedOrdinates> validator, out IEnumerable<IMessage> errors)
        {
            return validator.IsValid(this, out errors);
        }

        public IFunction Sample(double p)
        {
            //todo check that 0<=p<=1 argument out of range

            List<ICoordinatesFunction<double, double>> constantFunctions = new List<ICoordinatesFunction<double, double>>();
            foreach (ICoordinatesFunction<double, IOrdinate> func in Functions)
            {
                ICoordinatesFunction<double, double> constFunc = func.Sample(p);
                constantFunctions.Add(constFunc);
            }

            CoordinatesFunctionLinkedConstants linkedFunc = new CoordinatesFunctionLinkedConstants(constantFunctions, Interpolators);
            return linkedFunc;
        }

        /// <summary>
        /// This will call the sample on each function and will override all interpolators with the one passed in.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="interpolator">This interpolator will be used as the interpolation method for all functions and inbetween functions.</param>
        /// <returns></returns>
        public IFunction Sample(double p, InterpolationEnum interpolator)
        {
            //todo check that 0<=p<=1 argument out of range

            List<ICoordinatesFunction<double, double>> constantFunctions = new List<ICoordinatesFunction<double, double>>();
            foreach (ICoordinatesFunction<double, IOrdinate> func in Functions)
            {
                ICoordinatesFunction<double, double> constFunc = func.Sample(p, interpolator);
                constantFunctions.Add(constFunc);
            }

            //create a list of interpolators that will be the interpolators between the functions
            int numFunctions = constantFunctions.Count;
            List<InterpolationEnum> betweenInterpolators = new List<InterpolationEnum>();
            for(int i = 0;i<numFunctions-1;i++)
            {
                betweenInterpolators.Add(interpolator);
            }

            CoordinatesFunctionLinkedConstants linkedFunc = new CoordinatesFunctionLinkedConstants(constantFunctions, betweenInterpolators);
            return linkedFunc;

        }
        internal override bool AreRangesStrictlyIncreasing()
        {
            bool retval = true;
            for (int i = 0; i < Functions.Count - 2; i++)
            {
                CoordinatesFunctionOrdinateYs func1 = (CoordinatesFunctionOrdinateYs)Functions[i];
                CoordinatesFunctionOrdinateYs func2 = (CoordinatesFunctionOrdinateYs)Functions[i + 1];

                if (func1.Range.Item2 >= func2.Range.Item1)
                {
                    retval = false;
                    break;
                }
            }
            return retval;
        }

    }
}
