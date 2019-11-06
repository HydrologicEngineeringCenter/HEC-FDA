//using Functions.Validation;
//using System;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Linq;
//using System.Text;
//using Utilities;
//using Utilities.Validation;

//namespace Functions.CoordinatesFunctions
//{
//    /// <summary>
//    /// This class exists as a way to return a function composed of constant functions to the LinkedCoordinatesFunction Sample() method.
//    /// </summary>
//    internal class CoordinatesFunctionLinkedConstants : CoordinatesFunctionLinkedBase<double, double>, IFunction, IValidate<CoordinatesFunctionLinkedConstants>
//    {
       

//        public bool IsDistributed => false;

//        public bool IsInvertible => throw new NotImplementedException();

//        public Tuple<double, double> Range => throw new NotImplementedException();

//        internal CoordinatesFunctionLinkedConstants(List<ICoordinatesFunction<double, double>> functions, List<InterpolationEnum> interpolators)
//        {
//            Functions = functions;
//            Interpolators = interpolators;
//            IsValid = Validate(new CoordinatesFunctionLinkedConstantValidator(), out IEnumerable<IMessage> errors);
//            CombineCoordinates();
//            SetOrderEnum();
//            Errors = errors;

//        }

//        #region Interpolators
//        private double LinearInterpolator(ICoordinate<double, double> coordinateLeft, ICoordinate<double, double> coordinateRight, double x)
//        {
//            return coordinateLeft.Y + (x - coordinateLeft.X) / (coordinateRight.X - coordinateLeft.X) * (coordinateRight.Y - coordinateLeft.Y);
//        }
//        private double PiecewiseInterpolator(ICoordinate<double, double> coordinateLeft, ICoordinate<double, double> coordinateRight, double x)
//        {
//            if ((x - coordinateLeft.X) < (coordinateRight.X - coordinateLeft.X) / 2)
//            {
//                return coordinateLeft.Y;
//            }
//            else
//            {
//                return coordinateRight.Y;
//            }
//        }
//        private double NoInterpolator(ICoordinate<double, double> coordinateLeft, double x)
//        {
//            if (x == coordinateLeft.X)
//            {
//                return coordinateLeft.Y;
//            }
//            else
//            {
//                throw new InvalidOperationException(String.Format("The F(x) operation cannot produce a result because no interpolation method has been set and the specified x value: {0} was not explicitly provided as part of the function domain.", x));
//            }
//        }

//        private double InverseLinearInterpolator(ICoordinate<double, double> coordinateLeft, ICoordinate<double, double> coordinateRight, double y)
//        {
//            return coordinateLeft.X + (y - coordinateLeft.Y) /
//                 (coordinateRight.Y - coordinateLeft.Y) *
//                 (coordinateRight.X - coordinateLeft.X);
//        }
//        private double InversePiecewiseInterpolator(ICoordinate<double, double> coordinateLeft, ICoordinate<double, double> coordinateRight, double y)
//        {
//            return ((y - coordinateLeft.Y) < (coordinateRight.Y - coordinateLeft.Y) / 2)
//                ? coordinateLeft.X : coordinateRight.X;
//        }
//        private double InverseNoInterpolator(ICoordinate<double, double> coordinateLeft, double y)
//        {
//            return y == coordinateLeft.Y ? coordinateLeft.X
//                : throw new InvalidOperationException(string.Format("The InverseF(y) operation cannot produce a " +
//                "result because no interpolation method has been set and the specified y value: {0} was not " +
//                "explicityly provided as part of the function domain.", y));
//        }

//        #endregion

//        public double F(double x)
//        {
//            //there should be no overlapping domains because we check that in the ctor during validation
//            //try to find the function that has this point within its domain.
//            foreach (ICoordinatesFunction<double, double> function in Functions)
//            {
//                Tuple<double, double> funcDomain = function.Domain;
//                if (x >= funcDomain.Item1 && x <= funcDomain.Item2)
//                {
//                    return function.F(x);
//                }
//            }

//            //if we get here then the x value is outside the function domains. 
//            //check to see if it is between functions
//            for (int i = 0; i < Functions.Count - 1; i++)
//            {

//                Tuple<double, double> func1Domain = Functions[i].Domain;
//                Tuple<double, double> func2Domain = Functions[i + 1].Domain;

//                if (x > func1Domain.Item2 && x < func2Domain.Item1)
//                {
//                    //then this x is between func1 and func2
//                    InterpolationEnum interpolator = Interpolators[i];
//                    double yValue = Interpolate(interpolator, Functions[i].Coordinates.Last(), Functions[i + 1].Coordinates.First(), x);
//                    return yValue;
//                }
//            }
//            //if we get here then the x value is outside the range of all functions
//            throw new ArgumentOutOfRangeException($"The value {x} was outside the domain of all functions.");
//        }

//        private double Interpolate(InterpolationEnum interpolator, ICoordinate<double, double> coordinateLeft, ICoordinate<double, double> coordinateRight, double x)
//        {
//            double retval = Double.NaN;
//            if (interpolator == InterpolationEnum.Linear)
//            {
//                retval = LinearInterpolator(coordinateLeft, coordinateRight, x);
//            }
//            else if (interpolator == InterpolationEnum.Piecewise)
//            {
//                retval = PiecewiseInterpolator(coordinateLeft, coordinateRight, x);
//            }
//            else if (interpolator == InterpolationEnum.NoInterpolation)
//            {
//                retval = NoInterpolator(coordinateLeft, x);
//            }
//            return retval;

//        }

//        public double InverseF(double y)
//        {
//            //i only do this if this function is strict monotonic
//            //find the function that the y cooresponds to and call its inverseF
//            //if inbetween then use the interpolation method that was passed in.(using the min and max points around it)
//            if (Order == OrderedSetEnum.StrictlyIncreasing || Order == OrderedSetEnum.StrictlyDecreasing)
//            {
//                //then we can do the inverse
//                //try to find the function that has this point within its range.
//                foreach (ICoordinatesFunction<double, double> function in Functions)
//                {
//                    if (IsYValueInFunctionRange(function, y))
//                    {
//                        return function.InverseF(y);
//                    }
//                }
//                //if we get here then the y value is outside the function ranges. 
//                //check to see if it is between functions
//                for (int i = 0; i < Functions.Count - 1; i++)
//                {
//                    if (Functions[i].GetType() == typeof(CoordinatesFunctionConstants) &&
//                        Functions[i + 1].GetType() == typeof(CoordinatesFunctionConstants))
//                    {

//                        Tuple<double, double> func1Range = ((CoordinatesFunctionConstants)Functions[i]).Range;
//                        Tuple<double, double> func2Range = ((CoordinatesFunctionConstants)Functions[i + 1]).Range;

//                        if (y > func1Range.Item2 && y < func2Range.Item1)
//                        {
//                            //then this y is between func1 and func2
//                            InterpolationEnum interpolator = Interpolators[i];
//                            return InverseInterpolate(interpolator, Functions[i].Coordinates.Last(), Functions[i + 1].Coordinates.Last(), y);

//                        }
//                    }
//                }
//            }
//            throw new InvalidOperationException("The function InverseF(y) is invalid for this set of coordinates. The inverse of F(x) is not a function, because one or more y values maps to multiple x values");
//        }

//        private double InverseInterpolate(InterpolationEnum interpolator, ICoordinate<double, double> coordinateLeft, ICoordinate<double, double> coordinateRight, double y)
//        {
//            double retval = Double.NaN;
//            if (interpolator == InterpolationEnum.Linear)
//            {
//                retval = InverseLinearInterpolator(coordinateLeft, coordinateRight, y);
//            }
//            else if (interpolator == InterpolationEnum.Piecewise)
//            {
//                retval = InversePiecewiseInterpolator(coordinateLeft, coordinateRight, y);
//            }
//            else if (interpolator == InterpolationEnum.NoInterpolation)
//            {
//                retval = InverseNoInterpolator(coordinateLeft, y);
//            }
//            return retval;

//        }


//        private bool IsYValueInFunctionRange(ICoordinatesFunction<double, double> function, double yValue)
//        {
//            bool retval = false;
//            if (function.GetType() == typeof(CoordinatesFunctionOrdinateYs))
//            {
//                Tuple<double, double> range = ((CoordinatesFunctionOrdinateYs)function).Range;
//                if (yValue >= range.Item1 && yValue <= range.Item2)
//                {
//                    retval = true;
//                }
//            }
//            return retval;
//        }

//        public IFunction Sample(double p)
//        {
//            return this;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="p"></param>
//        /// <param name="interpolator">This interpolator will be set the interpolation type for all functions and the space between functions</param>
//        /// <returns></returns>
//        public IFunction Sample(double p, InterpolationEnum interpolator)
//        {
//            List<ICoordinatesFunction<double, double>> constFunctions = new List<ICoordinatesFunction<double, double>>();
//            foreach(ICoordinatesFunction<double, double> func in Functions)
//            {
//                constFunctions.Add( new CoordinatesFunctionConstants(func.Coordinates, interpolator));
//            }
//            //create a list of interpolators that will be the interpolators between the functions
//            int numFunctions = constFunctions.Count;
//            List<InterpolationEnum> betweenInterpolators = new List<InterpolationEnum>();
//            for (int i = 0; i < numFunctions - 1; i++)
//            {
//                betweenInterpolators.Add(interpolator);
//            }
//            return new CoordinatesFunctionLinkedConstants(constFunctions, betweenInterpolators);
//        }

//        public bool Validate(IValidator<CoordinatesFunctionLinkedConstants> validator, out IEnumerable<IMessage> errors)
//        {
//            return validator.IsValid(this, out errors);
//        }

//        internal override bool AreRangesStrictlyIncreasing()
//        {
//            bool retval = true;
//            for (int i = 0; i < Functions.Count - 2; i++)
//            {
//                CoordinatesFunctionConstants func1 = (CoordinatesFunctionConstants)Functions[i];
//                CoordinatesFunctionConstants func2 = (CoordinatesFunctionConstants)Functions[i + 1];

//                if (func1.Range.Item2 >= func2.Range.Item1)
//                {
//                    retval = false;
//                    break;
//                }
//            }
//            return retval;
//        }

//        public double RiemannSum()
//        {
//            throw new NotImplementedException();
//        }

//        public IFunction Compose(IFunction g)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
