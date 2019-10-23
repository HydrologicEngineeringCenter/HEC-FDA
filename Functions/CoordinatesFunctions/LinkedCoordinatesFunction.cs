using Functions.Ordinates;
using Functions.Validation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Utilities.Validation;

namespace Functions.CoordinatesFunctions
{
    internal class LinkedCoordinatesFunction : ICoordinatesFunction<IOrdinate, IOrdinate>, IValidate<LinkedCoordinatesFunction>
    {
        public List<InterpolationEnum> Interpolators { get; }

        public List<ICoordinatesFunction<IOrdinate, IOrdinate>> Functions { get; }

        public OrderedSetEnum Order { get; private set; }

        public IImmutableList<ICoordinate<IOrdinate, IOrdinate>> Coordinates { get; private set; }

        public bool IsDistributed
        {
            //If any function is distributed then return true;
            get
            {
                bool retval = false;
                foreach(ICoordinatesFunction<IOrdinate, IOrdinate> func in Functions)
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

        public Tuple<double, double> Domain
        {
            get
            {
                if (Functions != null)
                {
                    double min = Double.MaxValue;
                    double max = Double.MinValue;
                    foreach(ICoordinatesFunction<IOrdinate, IOrdinate> func in Functions)
                    {
                        double funcMin = func.Domain.Item1;
                        double funcMax = func.Domain.Item2;
                        if(funcMin < min) { min = funcMin; }
                        if(funcMax > max) { max = funcMax; }
                    }
                    return new Tuple<double, double>(min, max);
                }
                else
                {
                    //todo: john should i throw an exception?
                    return new Tuple<double, double>(0, 0);
                }
            }
        }

        public bool IsValid { get; }

        public IEnumerable<string> Errors { get; }

        /// <summary>
        /// The list of functions must be in the correct order.
        /// </summary>
        /// <param name="functions"></param>
        /// <param name="interpolators"></param>
        internal LinkedCoordinatesFunction(List<ICoordinatesFunction<IOrdinate, IOrdinate>> functions, List<InterpolationEnum> interpolators)
        {
            ValidateInputs();
            //the list of functions are not gauranteed to be in the correct order.
            //sort them on the min x value of each function
            //todo: if i sort, i will lose track of the list of interpolators.
            //List<ICoordinatesFunction<IOrdinate, IOrdinate>> sortedFunctions = Functions.OrderBy(func => func.Domain.Item1).ToList();
            Functions = functions;
            Interpolators = interpolators;

            GatherCoordinates();
            SetOrder();
            IsValid = Validate(new LinkedCoordinatesFunctionValidator(), out IEnumerable<string> errors);
            Errors = errors;
        }

        #region setting the function order

        private void SetOrder()
        {
            //default to not set
            Order = OrderedSetEnum.NotSet;

            if (IsDistributed)
            {
                Order = OrderedSetEnum.NonMonotonic;
                return;
            }

            //now we can assume that all functions are Not Distributed
            //add the orders to a list
            List<OrderedSetEnum> functionOrders = new List<OrderedSetEnum>();
            foreach (CoordinatesFunction func in Functions)
            {
                functionOrders.Add(((CoordinatesFunctionConstants)func).Order);
            }

            if (IsAnyFunctionNonMonotonicOrNotSet(functionOrders))
            {
                Order = OrderedSetEnum.NonMonotonic;
                return;
            }

            else if (IsFunctionsIncreasingAndDecreasing(functionOrders))
            {
                Order = OrderedSetEnum.NonMonotonic;
                return;
            }

            else if (IsFunctionsStrictlyDecreasing(functionOrders))
            {
                //need to check that the domains and ranges don't overlap
                if (AreDomainsIncreasing() && AreRangesDecreasing())
                {
                    Order = OrderedSetEnum.StrictlyDecreasing;
                }
                else
                { 
                    //todo: john is this an error state?
                    Order = OrderedSetEnum.NonMonotonic;
                }
            }

            else if (IsFunctionsStrictlyIncreasing(functionOrders))
            {
                //need to check that the domains and ranges don't overlap
                if (AreDomainsIncreasing() && AreRangesIncreasing())
                {
                    Order = OrderedSetEnum.StrictlyIncreasing;
                }
                else
                {
                    //todo: john is this an error state?
                    Order = OrderedSetEnum.NonMonotonic;
                }
            }

            else if (IsFunctionsWeaklyIncreasing(functionOrders))
            {
                //need to check that the domains and ranges don't overlap
                if (AreDomainsIncreasing() && AreRangesIncreasing())
                {
                    Order = OrderedSetEnum.WeaklyIncreasing;
                }
                else
                {
                    //this is an error state?
                    Order = OrderedSetEnum.NonMonotonic;
                }

            }

            else if (IsFunctionsWeaklyDecreasing(functionOrders))
            {
                //need to check that the domains and ranges don't overlap
                if (AreDomainsIncreasing() && AreRangesDecreasing())
                {
                    Order = OrderedSetEnum.WeaklyDecreasing;
                }
                else
                {
                    //this is an error state?
                    Order = OrderedSetEnum.NonMonotonic;
                }

            }

        }

        private bool AreDomainsIncreasing()
        {
            bool retval = true;
            for (int i = 0; i < Functions.Count - 2; i++)
            {
                //is the previous function's max xValue less than the next function's min xValue
                if (Functions[i].Domain.Item2 >= Functions[i + 1].Domain.Item1)
                {
                    retval = false;
                    break;
                }
            }
            return retval;
        }

        private bool AreRangesIncreasing()
        {
            bool retval = true;
            for (int i = 0; i < Functions.Count - 2; i++)
            {
                CoordinatesFunctionConstants func1 = (CoordinatesFunctionConstants)Functions[i];
                CoordinatesFunctionConstants func2 = (CoordinatesFunctionConstants)Functions[i+1];

                if (func1.Range.Item2 >= func2.Range.Item1)
                {
                    retval = false;
                    break;
                }
            }
            return retval;
        }

        private bool AreRangesDecreasing()
        {
            bool retval = true;
            for (int i = 0; i < Functions.Count - 2; i++)
            {
                CoordinatesFunctionConstants func1 = (CoordinatesFunctionConstants)Functions[i];
                CoordinatesFunctionConstants func2 = (CoordinatesFunctionConstants)Functions[i + 1];

                if (func1.Range.Item2 <= func2.Range.Item1)
                {
                    retval = false;
                    break;
                }
            }
            return retval;
        }

        private bool IsFunctionsWeaklyIncreasing(List<OrderedSetEnum> functionOrders)
        {
            if (functionOrders.Contains(OrderedSetEnum.NonMonotonic) ||
                functionOrders.Contains(OrderedSetEnum.NotSet) ||
                functionOrders.Contains(OrderedSetEnum.StrictlyDecreasing) ||
                functionOrders.Contains(OrderedSetEnum.WeaklyDecreasing))
            {
                return false;
            }
            //there is now only strictly increasing or weakly increasing orders
            bool areThereWeakOrders = false;
            foreach (OrderedSetEnum order in functionOrders)
            {
                if (order == OrderedSetEnum.WeaklyIncreasing)
                {
                    areThereWeakOrders = true;
                    break;
                }
            }
            return areThereWeakOrders;
        }

        private bool IsFunctionsWeaklyDecreasing(List<OrderedSetEnum> functionOrders)
        {
            if (functionOrders.Contains(OrderedSetEnum.NonMonotonic) ||
                functionOrders.Contains(OrderedSetEnum.NotSet) ||
                functionOrders.Contains(OrderedSetEnum.StrictlyIncreasing) ||
                functionOrders.Contains(OrderedSetEnum.WeaklyIncreasing))
            {
                return false;
            }
            //there is now only strictly increasing or weakly increasing orders
            bool areThereWeakOrders = false;
            foreach (OrderedSetEnum order in functionOrders)
            {
                if (order == OrderedSetEnum.WeaklyDecreasing)
                {
                    areThereWeakOrders = true;
                    break;
                }
            }
            return areThereWeakOrders;
        }

        private bool IsFunctionsStrictlyIncreasing(List<OrderedSetEnum> functionOrders)
        {
            bool retval = true;
            foreach (OrderedSetEnum order in functionOrders)
            {
                if (order != OrderedSetEnum.StrictlyIncreasing)
                {
                    retval = false;
                    break;
                }
            }
            return retval;
        }

        private bool IsFunctionsStrictlyDecreasing(List<OrderedSetEnum> functionOrders)
        {
            bool retval = true;
            foreach (OrderedSetEnum order in functionOrders)
            {
                if (order != OrderedSetEnum.StrictlyDecreasing)
                {
                    retval = false;
                    break;
                }
            }
            return retval;
        }

        private bool IsAnyFunctionNonMonotonicOrNotSet(List<OrderedSetEnum> functionOrders)
        {
            bool retval = false;
            foreach (OrderedSetEnum order in functionOrders)
            {
                if (order == OrderedSetEnum.NonMonotonic || order == OrderedSetEnum.NotSet)
                {
                    retval = true;
                    break;
                }
            }
            return retval;
        }
        private bool IsFunctionsIncreasingAndDecreasing(List<OrderedSetEnum> functionOrders)
        {
            bool retval = false;
            if (functionOrders.Contains(OrderedSetEnum.StrictlyIncreasing) ||
                functionOrders.Contains(OrderedSetEnum.WeaklyIncreasing))
            {
                //if there is a strict increasing and any decreasing then it has to be non monotonic
                if (functionOrders.Contains(OrderedSetEnum.StrictlyDecreasing) ||
                    functionOrders.Contains(OrderedSetEnum.WeaklyDecreasing) ||
                    functionOrders.Contains(OrderedSetEnum.NotSet) ||
                    functionOrders.Contains(OrderedSetEnum.NonMonotonic))
                {
                    retval = true;
                }
            }
            return retval;
        }

        #endregion

        private double LinearInterpolator(ICoordinate<IOrdinate, IOrdinate> coordinateLeft, ICoordinate<IOrdinate, IOrdinate> coordinateRight, double x)
        {
            return coordinateLeft.Y.Value() + (x - coordinateLeft.X.Value()) / (coordinateRight.X.Value() - coordinateLeft.X.Value()) * (coordinateRight.Y.Value() - coordinateLeft.Y.Value());
        }
        private double PiecewiseInterpolator(ICoordinate<IOrdinate, IOrdinate> coordinateLeft, ICoordinate<IOrdinate, IOrdinate> coordinateRight, double x)
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
        private double NoInterpolator(ICoordinate<IOrdinate, IOrdinate> coordinateLeft, double x)
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

        private double InverseLinearInterpolator(ICoordinate<IOrdinate, IOrdinate> coordinateLeft, ICoordinate<IOrdinate, IOrdinate> coordinateRight, double y)
        {
           return coordinateLeft.X.Value() + (y - coordinateLeft.Y.Value()) / 
                (coordinateRight.Y.Value() - coordinateLeft.Y.Value()) * 
                (coordinateRight.X.Value() - coordinateLeft.X.Value());
        }
        private double InversePiecewiseInterpolator(ICoordinate<IOrdinate, IOrdinate> coordinateLeft, ICoordinate<IOrdinate, IOrdinate> coordinateRight, double y)
        {
            return ((y - coordinateLeft.Y.Value()) < (coordinateRight.Y.Value() - coordinateLeft.Y.Value()) / 2) 
                ? coordinateLeft.X.Value() : coordinateRight.X.Value();
        }
        private double InverseNoInterpolator(ICoordinate<IOrdinate, IOrdinate> coordinateLeft, double y)
        {
            return y == coordinateLeft.Y.Value() ? coordinateLeft.X.Value() 
                : throw new InvalidOperationException(string.Format("The InverseF(y) operation cannot produce a " +
                "result because no interpolation method has been set and the specified y value: {0} was not " +
                "explicityly provided as part of the function domain.", y));
        }


        private void ValidateInputs()
        {
            if (Functions == null || Interpolators == null)
            {
                throw new ArgumentException();
            }
            if (Interpolators.Count != Functions.Count - 1)
            {
                throw new ArgumentException();
            }
            //todo check that the funcs don't overlap
        }

        private void GatherCoordinates()
        {
            List<ICoordinate<IOrdinate, IOrdinate>> allCoords = new List<ICoordinate<IOrdinate, IOrdinate>>();
            foreach (ICoordinatesFunction<IOrdinate, IOrdinate> function in Functions)
            {
               // coordinates.AddRange(function.Coordinates);
               allCoords.AddRange(function.Coordinates);
            }
            //now all the coordinates are in the list, but not necessarily in the right order
            //todo i need to sort by the x value?
            //List<ICoordinate<IOrdinate, IOrdinate>> sortedCoords =
            //    allCoords.OrderBy(coord => coord.X.Value);

            //List<ICoordinate<IOrdinate, IOrdinate>> sortedCoords =
            //    allCoords.Sort(coord);
            List<ICoordinate<IOrdinate, IOrdinate>> sortedList = allCoords.OrderBy(coord => coord.X).ToList();

            ImmutableList<ICoordinate<IOrdinate, IOrdinate>> coordinates = ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>();
            foreach (ICoordinate<IOrdinate, IOrdinate> coord in allCoords)
            {
                coordinates.Add(coord);
            }

            Coordinates = coordinates;
        }

       

        public IOrdinate F(IOrdinate x)
        {
            //there should be no overlapping domains because we check that in the ctor during validation
            //try to find the function that has this point within its domain.
            foreach (CoordinatesFunction function in Functions)
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
                    double yValue = Interpolate(interpolator, Functions[i].Coordinates.Last(), Functions[i+1].Coordinates.Last(), x.Value());
                    return new Constant(yValue);
                }
            }
            //if we get here then the x value is outside the range of all functions
            return null;
        }

        private double Interpolate(InterpolationEnum interpolator, ICoordinate<IOrdinate, IOrdinate> coordinateLeft, ICoordinate<IOrdinate, IOrdinate> coordinateRight, double x)
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

        private double InverseInterpolate(InterpolationEnum interpolator, ICoordinate<IOrdinate, IOrdinate> coordinateLeft, ICoordinate<IOrdinate, IOrdinate> coordinateRight, double y)
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

        public IOrdinate InverseF(IOrdinate y)
        {
            //i only do this if this function is strict monotonic
            //find the function that the y cooresponds to and call its inverseF
            //if inbetween then use the interpolation method that was passed in.(using the min and max points around it)
            if(Order == OrderedSetEnum.StrictlyIncreasing || Order == OrderedSetEnum.StrictlyDecreasing )
            {
                //then we can do the inverse
                //try to find the function that has this point within its range.
                foreach (CoordinatesFunction function in Functions)
                {
                    if(IsYValueInFunctionRange(function, y.Value()))
                    {
                        return function.InverseF(y);
                    }
                }
                //if we get here then the y value is outside the function ranges. 
                //check to see if it is between functions
                for (int i = 0; i < Functions.Count - 1; i++)
                {
                    if (Functions[i].GetType() == typeof(CoordinatesFunctionConstants) &&
                        Functions[i + 1].GetType() == typeof(CoordinatesFunctionConstants))
                    {

                        Tuple<double, double> func1Range = ((CoordinatesFunctionConstants)Functions[i]).Range;
                        Tuple<double, double> func2Range = ((CoordinatesFunctionConstants)Functions[i+1]).Range;

                        if (y.Value() > func1Range.Item2 && y.Value() < func2Range.Item1)
                        {
                            //then this y is between func1 and func2
                            InterpolationEnum interpolator = Interpolators[i];
                            double yValue = InverseInterpolate(interpolator, Functions[i].Coordinates.Last(), Functions[i + 1].Coordinates.Last(), y.Value());
                            return new Constant(yValue);
                        }
                    }
                }
            }
            else
            {
                //inverse is not possible.
            }
            return null;
        }

        private bool IsYValueInFunctionRange(ICoordinatesFunction<IOrdinate, IOrdinate> function, double yValue)
        {
            bool retval = false;
            if(function.GetType() == typeof(CoordinatesFunctionConstants))
            {
                Tuple<double, double> range = ((CoordinatesFunctionConstants)function).Range;
                if(yValue >= range.Item1 && yValue <= range.Item2)
                {
                    retval = true;
                }
            }
            return retval;
        }

        public bool Validate(IValidator<LinkedCoordinatesFunction> validator, out IEnumerable<string> errors)
        {
            return validator.IsValid(this, out errors);
        }
    }
}
