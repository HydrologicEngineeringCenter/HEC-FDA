using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Utilities;

namespace Functions.CoordinatesFunctions
{
    public abstract class CoordinatesFunctionLinkedBase
    {
        public List<ICoordinatesFunction> Functions { get; internal set; }

        public List<ICoordinate> Coordinates { get; internal set; }

        public bool IsValid { get; internal set; }
        public OrderedSetEnum Order { get; internal set; }
        /// <summary>
        /// These interpolators go between the functions. The number of interpolators
        /// should always be the number of functions -1
        /// </summary>
        public List<InterpolationEnum> Interpolators { get; internal set; }

        public IEnumerable<IMessage> Errors { get; internal set; }

        public Tuple<double, double> Domain
        {
            get
            {
                if (Functions != null)
                {
                    double min = Double.MaxValue;
                    double max = Double.MinValue;
                    foreach (ICoordinatesFunction func in Functions)
                    {
                        double funcMin = func.Domain.Item1;
                        double funcMax = func.Domain.Item2;
                        if (funcMin < min) { min = funcMin; }
                        if (funcMax > max) { max = funcMax; }
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

        /// <summary>
        /// This interpolator is only if all the other interpolators are the same
        /// </summary>
        public InterpolationEnum Interpolator { get; internal set; }

        internal CoordinatesFunctionLinkedBase()
        {
        }



        internal void SetOrderEnum()
        {
            //default to not set
            Order = OrderedSetEnum.NotSet;



            List<OrderedSetEnum> functionOrders = new List<OrderedSetEnum>();
            foreach (ICoordinatesFunction func in Functions)
            {
                OrderedSetEnum order = func.Order;
                functionOrders.Add(order);
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
                if (AreDomainsIncreasing())
                {
                    if (AreRangesStrictlyDecreasing())
                    {
                        Order = OrderedSetEnum.StrictlyDecreasing;
                    }
                    else if (AreRangesWeaklyDecreasing())
                    {
                        Order = OrderedSetEnum.WeaklyDecreasing;
                    }
                    else
                    {
                        Order = OrderedSetEnum.NonMonotonic;
                    }
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
                if (AreDomainsIncreasing())
                {
                    if (AreRangesStrictlyIncreasing())
                    {
                        Order = OrderedSetEnum.StrictlyIncreasing;
                    }
                    else if (AreRangesWeaklyIncreasing())
                    {
                        Order = OrderedSetEnum.WeaklyIncreasing;
                    }
                    else
                    {
                        Order = OrderedSetEnum.NonMonotonic;
                    }
                }
                else
                {
                    //each individual function is strictly increasing but the domains and/or ranges
                    //are not strictly increasing when taken together.
                    //it is possible that this is non monotonic, or weakly increasing

                    Order = OrderedSetEnum.NonMonotonic;
                }
            }

            else if (IsFunctionsWeaklyIncreasing(functionOrders))
            {
                //need to check that the domains and ranges don't overlap
                if (AreDomainsIncreasing() && AreRangesWeaklyIncreasing())
                {
                    Order = OrderedSetEnum.WeaklyIncreasing;
                }
                else
                {
                    Order = OrderedSetEnum.NonMonotonic;
                }

            }

            else if (IsFunctionsWeaklyDecreasing(functionOrders))
            {
                //need to check that the domains and ranges don't overlap
                if (AreDomainsIncreasing() && AreRangesStrictlyDecreasing())
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

        internal abstract bool AreRangesStrictlyIncreasing();

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



        /// <summary>
        /// Might also be strictly increasing, but it is at least weakly increasing.
        /// </summary>
        /// <returns></returns>
        private bool AreRangesWeaklyIncreasing()
        {
            bool retval = true;
            for (int i = 0; i < Functions.Count - 2; i++)
            {
                IFunction func1 = (IFunction)Functions[i];
                IFunction func2 = (IFunction)Functions[i + 1];

                if (func1.Range.Item2 > func2.Range.Item1)
                {
                    retval = false;
                    break;
                }
            }
            return retval;
        }

        private bool AreRangesStrictlyDecreasing()
        {
            bool retval = true;
            for (int i = 0; i < Functions.Count - 2; i++)
            {
                IFunction func1 = (IFunction)Functions[i];
                IFunction func2 = (IFunction)Functions[i + 1];

                if (func1.Range.Item2 <= func2.Range.Item1)
                {
                    retval = false;
                    break;
                }
            }
            return retval;
        }

        private bool AreRangesWeaklyDecreasing()
        {
            bool retval = true;
            for (int i = 0; i < Functions.Count - 2; i++)
            {
                IFunction func1 = (IFunction)Functions[i];
                IFunction func2 = (IFunction)Functions[i + 1];

                if (func1.Range.Item2 < func2.Range.Item1)
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


        internal void CombineCoordinates()
        {
            List<ICoordinate> allCoords = new List<ICoordinate>();
            foreach (ICoordinatesFunction function in Functions)
            {
                allCoords.AddRange(function.Coordinates);
            }

            List<ICoordinate> sortedList = allCoords.OrderBy(coord => coord.X.Value()).ToList();
            List<ICoordinate> coordinates = new List<ICoordinate>();
            foreach (ICoordinate coord in sortedList)
            {
                coordinates.Add(coord);
            }

            Coordinates = coordinates;
        }



    }
}
