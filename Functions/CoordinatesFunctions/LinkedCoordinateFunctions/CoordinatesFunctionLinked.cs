using Functions.Ordinates;
using Functions.Validation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Utilities;
using Utilities.Validation;

namespace Functions.CoordinatesFunctions
{
    public class CoordinatesFunctionLinked : CoordinatesFunctionLinkedBase, ICoordinatesFunction, IValidate<CoordinatesFunctionLinked>
    {

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

        /// <summary>
        /// The list of functions must be in the correct order.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="interpolators"></param>
        internal CoordinatesFunctionLinked(List<ICoordinatesFunction> functions, List<InterpolationEnum> interpolators)
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
        private double LinearInterpolator(ICoordinate coordinateLeft, ICoordinate coordinateRight, double x)
        {
            return coordinateLeft.Y.Value() + (x - coordinateLeft.X.Value()) / (coordinateRight.X.Value() - coordinateLeft.X.Value()) * (coordinateRight.Y.Value() - coordinateLeft.Y.Value());
        }
        private double PiecewiseInterpolator(ICoordinate coordinateLeft, ICoordinate coordinateRight, double x)
        {
            if ((x - coordinateLeft.X.Value()) < (coordinateRight.X.Value() - coordinateLeft.X.Value()) / 2)
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

        private double InverseLinearInterpolator(ICoordinate coordinateLeft, ICoordinate coordinateRight, double y)
        {
            return coordinateLeft.X.Value() + (y - coordinateLeft.Y.Value()) /
                 (coordinateRight.Y.Value() - coordinateLeft.Y.Value()) *
                 (coordinateRight.X.Value() - coordinateLeft.X.Value());
        }
        private double InversePiecewiseInterpolator(ICoordinate coordinateLeft, ICoordinate coordinateRight, double y)
        {
            return ((y - coordinateLeft.Y.Value()) < (coordinateRight.Y.Value() - coordinateLeft.Y.Value()) / 2)
                ? coordinateLeft.X.Value() : coordinateRight.X.Value();
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
                Tuple<double, double> funcDomain = function.Domain;
                if (x.Value() >= funcDomain.Item1 && x.Value() <= funcDomain.Item2)
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

                if (x.Value() > func1Domain.Item2 && x.Value() < func2Domain.Item1)
                {
                    //then this x is between func1 and func2
                    InterpolationEnum interpolator = Interpolators[i];
                    double yValue = Interpolate(interpolator, Functions[i].Coordinates.Last(), Functions[i + 1].Coordinates.First(), x.Value());
                    return new Constant(yValue);
                }
            }
            //if we get here then the x value is outside the range of all functions
            return null;
        }

        private double Interpolate(InterpolationEnum interpolator, ICoordinate coordinateLeft, ICoordinate coordinateRight, double x)
        {
            double retval = Double.NaN;
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

                        Tuple<double, double> func1Range = ((IFunction)Functions[i]).Range;
                        Tuple<double, double> func2Range = ((IFunction)Functions[i + 1]).Range;

                        if (y.Value() > func1Range.Item2 && y.Value() < func2Range.Item1)
                        {
                            //then this y is between func1 and func2
                            InterpolationEnum interpolator = Interpolators[i];
                            return new Constant(InverseInterpolate(interpolator, Functions[i].Coordinates.Last(), Functions[i + 1].Coordinates.Last(), y.Value()));

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
                Tuple<double, double> range = ((IFunction)function).Range;
                if (yValue >= range.Item1 && yValue <= range.Item2)
                {
                    retval = true;
                }
            }
            return retval;
        }

        public bool Validate(IValidator<CoordinatesFunctionLinked> validator, out IEnumerable<IMessage> errors)
        {
            return validator.IsValid(this, out errors);
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

                if (func1.Range.Item2 >= func2.Range.Item1)
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
                funcElem.SetAttributeValue("Interpolator", Interpolator);

                foreach (ICoordinate coord in Coordinates)
                {
                    funcElem.Add(coord.WriteToXML());
                }

                functionsElem.Add(funcElem);
            }

            foreach(InterpolationEnum interpolator in Interpolators)
            {
                XElement interpElem = new XElement("Interpolator");
                interpElem.SetAttributeValue("Type", interpolator);
                functionsElem.Add(interpElem);
            }

            return functionsElem;
        }
    }
}
