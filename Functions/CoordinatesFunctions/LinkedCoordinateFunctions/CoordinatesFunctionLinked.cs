using Functions.Ordinates;
using Functions.Validation;
using MathNet.Numerics.Interpolation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Utilities;
//using Utilities.Validation;

namespace Functions.CoordinatesFunctions
{
    public class CoordinatesFunctionLinked : CoordinatesFunctionLinkedBase, ICoordinatesFunction, IValidate<ICoordinatesFunction>
    {
        #region Properties
        public override IRange<double> Range { get; }
        public bool IsDistributed
        {
            //If any function is distributed then return true;
            get
            {
                bool retval = false;
                foreach (ICoordinatesFunction func in Functions)
                {
                    if (typeof(CoordinatesFunctionVariableYs).IsAssignableFrom(func.GetType()))
                    {
                        retval = true;
                        break;
                    }
                }
                return retval;
            }
        }
        public bool IsLinkedFunction => true;
        public IEnumerable<IMessage> Messages => null;
        public IOrdinateEnum DistributionType
        {
            // todo: Cody this doesn't seem right.
            get
            {
                if (Coordinates.Count > 0)
                {
                    return Coordinates[0].Y.Type;
                }
                else
                {
                    return IOrdinateEnum.NotSupported;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// The list of functions must be in the correct order.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="interpolators"></param>
        internal CoordinatesFunctionLinked(List<ICoordinatesFunction> functions)
        {
            //the list of functions are not guaranteed to be in the correct order.
            //sort them on the min x value of each function
            //todo: if i sort, i will lose track of the list of interpolators.
            //List<ICoordinatesFunction<IOrdinate, IOrdinate>> sortedFunctions = Functions.OrderBy(func => func.Domain.Item1).ToList();
            Functions = functions;
            Range = SetRange();
            CombineCoordinates();
            SetOrder();
            State = Validate(new LinkedCoordinatesFunctionValidator(), out IEnumerable<IMessage> errors);
            Errors = errors;
            
        }
        #endregion

        #region Functions

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
        private double LinearInterpolator(ICoordinate coordinateLeft, ICoordinate coordinateRight, double x)
        {
            return coordinateLeft.Y.Value() + (x - coordinateLeft.X.Value()) / (coordinateRight.X.Value() - coordinateLeft.X.Value()) * (coordinateRight.Y.Value() - coordinateLeft.Y.Value());
        }
        private double PiecewiseInterpolator(ICoordinate coordinateLeft, ICoordinate coordinateRight, double x)
        {

            if (x < coordinateRight.X.Value())
            {
                return coordinateLeft.Y.Value();
            }
            else
            {
                return coordinateRight.Y.Value();
            }
        }
        private double NoInterpolator(ICoordinate coordinateLeft, double x)
        {
            if (x == coordinateLeft.X.Value())
            {
                return coordinateLeft.Y.Value();
            }
            else
            {
                throw new InvalidOperationException(String.Format("The F(x) operation cannot produce a result because no interpolation method has been set and the specified x value: {0} was not explicitly provided as part of the function domain.", x));
            }
        }

        private double CubicSplineInterpolator(ICoordinatesFunction leftFunction, ICoordinatesFunction rightFunction, double x)
        {
            //the only way we get here is if the x value is between two functions with the function on the left having a cubic spline interpolation type
            //get xs and ys array
            double[] xs = GetXValuesForCubicSplineInterpolation(leftFunction, rightFunction);
            double[] ys = GetYValuesForCubicSplineInterpolation(leftFunction, rightFunction);

            CubicSpline cs = CubicSpline.InterpolateNaturalSorted(xs, ys);
            double retval = cs.Interpolate(x);
            return retval;
        }

        private double InverseCubicSplineInterpolator(ICoordinatesFunction leftFunction, ICoordinatesFunction rightFunction, double x)
        {
            //the only way we get here is if the x value is between two functions with the function on the left having a cubic spline interpolation type
            //get xs and ys array
            double[] xs = GetXValuesForCubicSplineInterpolation(leftFunction, rightFunction);
            double[] ys = GetYValuesForCubicSplineInterpolation(leftFunction, rightFunction);

            CubicSpline cs = CubicSpline.InterpolateNaturalSorted(xs, ys);
            double retval = cs.Interpolate(x);
            return retval;
        }

        /// <summary>
        /// Gets all the x values from the left function and the first x of the right function. 
        /// </summary>
        /// <param name="leftFunction"></param>
        /// <param name="rightFunction"></param>
        /// <returns></returns>
        private double[] GetXValuesForCubicSplineInterpolation(ICoordinatesFunction leftFunction, ICoordinatesFunction rightFunction)
        {
            List<double> xs = new List<double>();
            //foreach (ICoordinate coord in leftFunction.Coordinates)
            //{
            //    xs.Add(coord.X.Value());
            //}

            //skip the first one because it is a duplicate.
            for(int i = 0;i<leftFunction.Coordinates.Count;i++)
            {
                xs.Add(leftFunction.Coordinates[i].X.Value());
            }

            //for(int i = leftFunction.Coordinates.Count-3;i<leftFunction.Coordinates.Count;i++)
            //{
            //    xs.Add(leftFunction.Coordinates[i].X.Value());
            //}
            xs.Add(rightFunction.Coordinates.First().X.Value());
            return xs.ToArray();
        }

        /// <summary>
        /// Gets all the y values from the left function and the first y of the right function. 
        /// </summary>
        /// <param name="leftFunction"></param>
        /// <param name="rightFunction"></param>
        /// <returns></returns>
        private double[] GetYValuesForCubicSplineInterpolation(ICoordinatesFunction leftFunction, ICoordinatesFunction rightFunction)
        {
            List<double> ys = new List<double>();
            //foreach (ICoordinate coord in leftFunction.Coordinates)
            //{
            //    ys.Add(coord.Y.Value());
            //}

            //skip the first one because it is a duplicate.
            for (int i = 0; i < leftFunction.Coordinates.Count; i++)
            {
                ys.Add(leftFunction.Coordinates[i].Y.Value());
            }

            //for (int i = leftFunction.Coordinates.Count - 3; i < leftFunction.Coordinates.Count; i++)
            //{
            //    ys.Add(leftFunction.Coordinates[i].Y.Value());
            //}
            ys.Add(rightFunction.Coordinates.First().Y.Value());
            return ys.ToArray();
        }

        private double InverseLinearInterpolator(ICoordinate coordinateLeft, ICoordinate coordinateRight, double y)
        {
            return coordinateLeft.X.Value() + (y - coordinateLeft.Y.Value()) /
                 (coordinateRight.Y.Value() - coordinateLeft.Y.Value()) *
                 (coordinateRight.X.Value() - coordinateLeft.X.Value());
        }
        private double InversePiecewiseInterpolator(ICoordinate coordinateLeft, ICoordinate coordinateRight, double y)
        {
            if (y == coordinateLeft.Y.Value())
            {
                return coordinateLeft.X.Value();
            }
            else
            {
                return coordinateRight.X.Value();
            }
        }
        private double InverseNoInterpolator(ICoordinate coordinateLeft, double y)
        {
            return y == coordinateLeft.Y.Value() ? coordinateLeft.X.Value()
                : throw new InvalidOperationException(string.Format("The InverseF(y) operation cannot produce a " +
                "result because no interpolation method has been set and the specified y value: {0} was not " +
                "explicityly provided as part of the function domain.", y));
        }
        #endregion

        public bool Equals(ICoordinatesFunction function)
        {
            //I don't think i have to check the domain, range, or order because
            //if the coordinates and interpolator are all the same then those values
            //should be the same.
            if (!function.GetType().Equals(typeof(CoordinatesFunctionLinked)))
            {
                return false;
            }
            CoordinatesFunctionLinked functionToCompare = (CoordinatesFunctionLinked)function;
            if(Functions.Count != functionToCompare.Functions.Count)
            {
                return false;
            }
            for(int i = 0;i<Functions.Count;i++)
            {
                if(!Functions[i].Equals(functionToCompare.Functions[i]))
                {
                    return false;
                }
            }

            return true;
        }
        public IOrdinate F(IOrdinate x)
        {
            //there should be no overlapping domains because we check that in the ctor during validation
            //try to find the function that has this point within its domain.
            foreach (ICoordinatesFunction function in Functions)
            {
                Utilities.IRange<double> funcDomain = function.Domain;
                if (x.Value() >= funcDomain.Min && x.Value() <= funcDomain.Max)
                {
                    return function.F(x);
                }
            }

            //if we get here then the x value is outside the function domains. 
            //check to see if it is between functions
            for (int i = 0; i < Functions.Count - 1; i++)
            {

                ICoordinatesFunction currentFunction = Functions[i];
                ICoordinatesFunction nextFunction = Functions[i+1];

                if (x.Value() > currentFunction.Coordinates.Last().X.Value() && x.Value() < nextFunction.Coordinates.First().X.Value())
                {
                    //then this x is between func1 and func2
                    InterpolationEnum interpolator = Functions[i].Interpolator;
                    double yValue = Interpolate(interpolator, currentFunction, nextFunction, x.Value());
                    return new Constant(yValue);
                }
            }
            //if we get here then the x value is outside the range of all functions
            throw new ArgumentOutOfRangeException("Could not calculate f(x) because the x-value: " + x.Value() + " was outside the domain of all functions.");
        }

        private double Interpolate(InterpolationEnum interpolator, ICoordinatesFunction leftFunction, ICoordinatesFunction rightFunction, double x)
        {
            double retval = Double.NaN;

            ICoordinate coordinateLeft = leftFunction.Coordinates.Last();
            ICoordinate coordinateRight = rightFunction.Coordinates.First();
            if (interpolator == InterpolationEnum.Linear)
            {
                retval = LinearInterpolator(coordinateLeft, coordinateRight, x);
            }
            else if (interpolator == InterpolationEnum.Piecewise)
            {
                retval = PiecewiseInterpolator(coordinateLeft, coordinateRight, x);
            }
            else if (interpolator == InterpolationEnum.None)
            {
                retval = NoInterpolator(coordinateLeft, x);
            }
            else if(interpolator == InterpolationEnum.NaturalCubicSpline)
            {
                retval = CubicSplineInterpolator(leftFunction, rightFunction, x);
            }
            return retval;

        }
        private double InverseInterpolate(InterpolationEnum interpolator, ICoordinate coordinateLeft, ICoordinate coordinateRight, double y)
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
            else if (interpolator == InterpolationEnum.None)
            {
                retval = InverseNoInterpolator(coordinateLeft, y);
            }
            return retval;

        }

        public IOrdinate InverseF(IOrdinate y)
        {
            //i only do this if this function is strict monotonic
            //find the function that the y cooresponds to and call its inverseF
            //if inbetween then use the interpolation method that was passed in.(using the min and max points around it)
            if (Order == OrderedSetEnum.StrictlyIncreasing || Order == OrderedSetEnum.StrictlyDecreasing)
            {
                //then we can do the inverse
                //try to find the function that has this point within its range.
                foreach (ICoordinatesFunction function in Functions)
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
                    //todo: I don't like having to check the type and then casting but when this
                    //gets called we know that the order is strictlyIncreasing which must mean that this
                    //is a constant function.
                    if (Functions[i].GetType() == typeof(CoordinatesFunctionConstants) &&
                        Functions[i + 1].GetType() == typeof(CoordinatesFunctionConstants))
                    {

                        IRange<double> func1Range = ((IFunction)Functions[i]).Range;
                        IRange<double> func2Range = ((IFunction)Functions[i + 1]).Range;

                        if (Order == OrderedSetEnum.StrictlyIncreasing)
                        {
                            if (y.Value() > func1Range.Max && y.Value() < func2Range.Min)
                            {
                                //then this y is between func1 and func2
                                InterpolationEnum interpolator = Functions[i].Interpolator;
                                return new Constant(InverseInterpolate(interpolator, Functions[i].Coordinates.Last(), Functions[i + 1].Coordinates.First(), y.Value()));

                            }
                        }
                        else if(Order == OrderedSetEnum.StrictlyDecreasing)
                        {
                            if (y.Value() < func1Range.Min && y.Value() > func2Range.Max)
                            {
                                //then this y is between func1 and func2
                                InterpolationEnum interpolator = Functions[i].Interpolator;
                                return new Constant(InverseInterpolate(interpolator, Functions[i].Coordinates.Last(), Functions[i + 1].Coordinates.First(), y.Value()));

                            }
                        }
                    }
                }
            }
            throw new InvalidOperationException("The function InverseF(y) is invalid for this set of coordinates. The inverse of F(x) is not a function, because one or more y values maps to multiple x values");
        }
        private bool IsYValueInFunctionRange(ICoordinatesFunction function, double yValue)
        {
            bool retval = false;
            //todo: I don't like having to check the type and then casting but when this
            //gets called we know that the order is strictlyIncreasing which must mean that this
            //is a constant function.
            if (function.GetType() == typeof(CoordinatesFunctionConstants))
            {
                IRange<double> range = ((IFunction)function).Range;
                if (yValue >= range.Min && yValue <= range.Max)
                {
                    retval = true;
                }
            }
            return retval;
        }

        public IMessageLevels Validate(IValidator<ICoordinatesFunction> validator, out IEnumerable<IMessage> errors)
        {
            return validator.IsValid(this, out errors);
        }

        public List<ICoordinate> GetExpandedCoordinates()
        {
            List<ICoordinate> expandedCoordinates = new List<ICoordinate>();
            foreach (ICoordinatesFunction fx in Functions)
            {
                expandedCoordinates.AddRange(fx.GetExpandedCoordinates());
            }
            return expandedCoordinates;
        }
        private IRange<double> SetRange()
        {
            double minRange = double.NaN, maxRange = double.NaN;
            foreach (ICoordinatesFunction fx in Functions)
            {
                if (minRange == double.NaN || fx.Range.Min < minRange)
                {
                    minRange = fx.Range.Min;
                }
                if (maxRange == double.NaN || fx.Range.Max > maxRange)
                {
                    maxRange = fx.Range.Max;
                }
            }
            if (!minRange.IsFinite() || !maxRange.IsFinite()) throw new InvalidOperationException("The range was never set because no component function with a non finite range was found.");
            else return IRangeFactory.Factory(minRange, maxRange);
        }
        //public IFunction Sample(double p)
        //{
        //    //todo check that 0<=p<=1 argument out of range

        //    List<ICoordinatesFunction<double, double>> constantFunctions = new List<ICoordinatesFunction<double, double>>();
        //    foreach (ICoordinatesFunction<double, IOrdinate> func in Functions)
        //    {
        //        ICoordinatesFunction<double, double> constFunc = func.Sample(p);
        //        constantFunctions.Add(constFunc);
        //    }

        //    CoordinatesFunctionLinkedConstants linkedFunc = new CoordinatesFunctionLinkedConstants(constantFunctions, Interpolators);
        //    return linkedFunc;
        //}

        ///// <summary>
        ///// This will call the sample on each function and will override all interpolators with the one passed in.
        ///// </summary>
        ///// <param name="p"></param>
        ///// <param name="interpolator">This interpolator will be used as the interpolation method for all functions and inbetween functions.</param>
        ///// <returns></returns>
        //public IFunction Sample(double p, InterpolationEnum interpolator)
        //{
        //    //todo check that 0<=p<=1 argument out of range

        //    List<ICoordinatesFunction<double, double>> constantFunctions = new List<ICoordinatesFunction<double, double>>();
        //    foreach (ICoordinatesFunction<double, IOrdinate> func in Functions)
        //    {
        //        ICoordinatesFunction<double, double> constFunc = func.Sample(p, interpolator);
        //        constantFunctions.Add(constFunc);
        //    }

        //    //create a list of interpolators that will be the interpolators between the functions
        //    int numFunctions = constantFunctions.Count;
        //    List<InterpolationEnum> betweenInterpolators = new List<InterpolationEnum>();
        //    for (int i = 0; i < numFunctions - 1; i++)
        //    {
        //        betweenInterpolators.Add(interpolator);
        //    }

        //    CoordinatesFunctionLinkedConstants linkedFunc = new CoordinatesFunctionLinkedConstants(constantFunctions, betweenInterpolators);
        //    return linkedFunc;

        //}
        internal override bool AreRangesStrictlyIncreasing()
        {
            bool retval = true;
            for (int i = 0; i < Functions.Count - 2; i++)
            {
                IFunction func1 = (IFunction)Functions[i];
                IFunction func2 = (IFunction)Functions[i + 1];

                if (func1.Range.Max >= func2.Range.Min)
                {
                    retval = false;
                    break;
                }
            }
            return retval;
        }

        public XElement WriteToXML()
        {
            XElement functionsElem = new XElement("Functions");
            functionsElem.SetAttributeValue("Type", "Linked");

            foreach (ICoordinatesFunction func in Functions)
            {

                XElement funcElem = new XElement("Function");
                funcElem.SetAttributeValue("Interpolator", func.Interpolator);

                foreach (ICoordinate coord in func.Coordinates)
                {
                    funcElem.Add(coord.WriteToXML());
                }

                functionsElem.Add(funcElem);
            }

            //foreach(InterpolationEnum interpolator in Interpolators)
            //{
            //    XElement interpElem = new XElement("Interpolator");
            //    interpElem.SetAttributeValue("Type", interpolator);
            //    functionsElem.Add(interpElem);
            //}

            return functionsElem;
        }
        #endregion
    }
}
