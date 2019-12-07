using Functions;
using Functions.Coordinates;
using Functions.CoordinatesFunctions;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace FunctionsTests.CoordinatesFunctions
{
    [ExcludeFromCodeCoverage]
    public  class CoordinateFunctionsTestData
    {

        #region ConstantData
        #region GoodData
        public static TheoryData<IImmutableList<ICoordinate<double, double>>> GoodData_Constant =>
            new TheoryData<IImmutableList<ICoordinate<double, double>>>
            {
                { ConstantCoordinates(new double[]{0}, new double[]{0})},
                {  ConstantCoordinates(new double[]{0,0}, new double[]{0,0})},
                {  ConstantCoordinates(new double[]{Double.MaxValue,0}, new double[]{0,Double.MinValue})},
                {  ConstantCoordinates(new double[]{Double.MinValue,0}, new double[]{0,Double.MaxValue})},
            };
        #endregion
        #region BadData
        public static TheoryData<IImmutableList<ICoordinate<double, double>>> BadData_Constant_Nan =>
            new TheoryData<IImmutableList<ICoordinate<double, double>>>
            {
                { ConstantCoordinates(new double[]{double.NaN}, new double[]{0})},
            };
        public static TheoryData<IImmutableList<ICoordinate<double, double>>> BadData_Constant_PositiveInfinity =>
            new TheoryData<IImmutableList<ICoordinate<double, double>>>
            {
                { ConstantCoordinates(new double[]{double.PositiveInfinity}, new double[]{0})},
            };
        public static TheoryData<IImmutableList<ICoordinate<double, double>>> BadData_Constant_NegativeInfinity =>
           new TheoryData<IImmutableList<ICoordinate<double, double>>>
           {
                { ConstantCoordinates(new double[]{double.NegativeInfinity}, new double[]{0})},
           };
        public static TheoryData<IImmutableList<ICoordinate<double, double>>> BadData_Constant_RepeatXs =>
           new TheoryData<IImmutableList<ICoordinate<double, double>>>
           {
                { ConstantCoordinates(new double[]{1,1}, new double[]{10,11})},
                { ConstantCoordinates(new double[]{1,1}, new double[]{25,25})}

           };
        #endregion
        #region Functions
        internal List<ICoordinatesFunction<double, double>> GoodFunctions_Constant()
        {
            List<ICoordinatesFunction<double, double>> funcs = new List<ICoordinatesFunction<double, double>>();
            CoordinatesFunctionConstants func1 =
                new CoordinatesFunctionConstants(ConstantCoordinates(new double[] { 0, 1, 2, 3, 4 }, new double[] { 0, 1, 2, 3, 4 }));
            funcs.Add(func1);
            return funcs;
        }

        internal CoordinatesFunctionConstants CreateCoordinatesFunctionConstantsBasic()
        {
            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<double> ys = new List<double>() { 10, 11, 12, 13 };
            return (CoordinatesFunctionConstants)ICoordinatesFunctionsFactory.Factory(xs, ys);
        }

        internal CoordinatesFunctionConstants CreateCoordinatesFunctionConstantsBasic(InterpolationEnum interpolator)
        {
            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<double> ys = new List<double>() { 10, 11, 12, 13 };
            return (CoordinatesFunctionConstants)ICoordinatesFunctionsFactory.Factory(xs, ys, interpolator);
        }

        internal CoordinatesFunctionConstants CreateCoordinatesFunctionConstants(List<double> xs, List<double> ys)
        {
            return (CoordinatesFunctionConstants)ICoordinatesFunctionsFactory.Factory(xs, ys);
        }
        internal CoordinatesFunctionConstants CreateCoordinatesFunctionConstants(List<double> xs, List<double> ys, InterpolationEnum interpolator)
        {
            return (CoordinatesFunctionConstants)ICoordinatesFunctionsFactory.Factory(xs, ys, interpolator);
        }

        #endregion
        private static IImmutableList<ICoordinate<double, double>> ConstantCoordinates(double[] xs, double[] ys)
        {
            if (xs.Length != ys.Length)
            {
                throw new ArgumentException("xValues were a different length than the yValues.");
            }

            List<ICoordinate<double, double>> coords = new List<ICoordinate<double, double>>();
            for (int i = 0; i < xs.Length; i++)
            {
                coords.Add(ICoordinateFactory.Factory(xs[i], ys[i]));
            }

            return ImmutableList.Create<ICoordinate<double, double>>(coords.ToArray());
        }

        #endregion

        #region DistributedData

        #region GoodData
        public static TheoryData<IImmutableList<ICoordinate<double, IDistribution>>> GoodDataDistributed =>
           new TheoryData<IImmutableList<ICoordinate<double, IDistribution>>>
           {
                { DistributedCoordinates(new double[]{0}, new IDistribution[]{new Normal(1,0)})},
                { DistributedCoordinates(new double[]{0, 1}, new IDistribution[]{new Normal(1,0), new Normal(1,1)})},
                { DistributedCoordinates(new double[]{0, 1}, new IDistribution[]{new Triangular(1,2,3), new Triangular(3,4,5)})},
                { DistributedCoordinates(new double[]{0, 1}, new IDistribution[]{new Uniform(1,2), new Uniform(3,4)})},
                { DistributedCoordinates(new double[]{0, 1,2,3},
                   new IDistribution[]
                   {
                       new Normal(1,2),
                       new Triangular(3,4,5),
                       new Uniform(5,6),
                       new Normal(1,2)
                   })
                },


           };

        #endregion
        #region BadData
        public static TheoryData<IImmutableList<ICoordinate<double, IDistribution>>> BadDataDistributed =>
            new TheoryData<IImmutableList<ICoordinate<double, IDistribution>>>
            {
                { DistributedCoordinates(new double[]{3},new IDistribution[]{ new Statistics.Distributions.Normal(0,1) }) },
                //{ ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), new ScalarDistributed(new Normal()), map)) },
                //{ ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarHistogram(), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarHistogram(), map)) },
                //{ ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarHistogram(), map), new UnivariateCoordinate(new ScalarHistogram(), IScalarFactory.Factory(0), map)) },
                //{ ImmutableList.Create<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarDistributed(new Normal()), map)) }
            };
        #endregion

        #region Functions
        internal CoordinatesFunctionVariableYs CreateDistributedCoordinatesFunctionBasic()
        {
            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<IDistribution> ys = new List<IDistribution>() { new Normal(1,0), new Normal(1, 0), new Normal(1, 0), new Normal(1, 0) };
            return (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys);
        }

        internal CoordinatesFunctionVariableYs CreateDistributedCoordinatesFunction(List<double> xs, List<IDistribution> ys)
        {
            return (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys);
        }
        #endregion

        internal static IImmutableList<ICoordinate<double, IDistribution>> DistributedCoordinates(double[] xs, IDistribution[] ys)
        {
            if (xs.Length != ys.Length)
            {
                throw new ArgumentException("xValues were a different length than the yValues.");
            }

            List<ICoordinate<double, IDistribution>> coords = new List<ICoordinate<double, IDistribution>>();
            for (int i = 0; i < xs.Length; i++)
            {
                coords.Add(ICoordinateFactory.Factory(xs[i], ys[i]));
            }

            return ImmutableList.Create<ICoordinate<double, IDistribution>>(coords.ToArray());
        }

        #endregion

        #region OrdinateYs

        internal List<ICoordinatesFunction<double, IOrdinate>> Create_3Constant_StrictMonotonicIncreasing_OrdinateFunctions()
        {
            List<ICoordinatesFunction<double, IOrdinate>> functions = new List<ICoordinatesFunction<double, IOrdinate>>();

            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys1 = new List<double>() { 5, 6, 7, 8 };

            List<double> xs2 = new List<double>() { 4,5,6,7 };
            List<double> ys2 = new List<double>() { 9,10,11,12 };

            List<double> xs3 = new List<double>() { 8,9,10,11 };
            List<double> ys3 = new List<double>() { 13,14,15,16 };
            //create a constant func
            CoordinatesFunctionConstants const1Func = CreateCoordinatesFunctionConstants(xs1, ys1);      
            CoordinatesFunctionConstants const2Func = CreateCoordinatesFunctionConstants(xs2, ys2);
            CoordinatesFunctionConstants const3Func = CreateCoordinatesFunctionConstants(xs3, ys3);

            //convert to iordinate
            CoordinatesFunctionOrdinateYs const1OrdFunc = new CoordinatesFunctionOrdinateYs(const1Func);
            CoordinatesFunctionOrdinateYs const2OrdFunc = new CoordinatesFunctionOrdinateYs(const2Func);
            CoordinatesFunctionOrdinateYs const3OrdFunc = new CoordinatesFunctionOrdinateYs(const3Func);

            //add to list of funcs
            functions.Add(const1OrdFunc);
            functions.Add(const2OrdFunc);
            functions.Add(const3OrdFunc);

            return functions;
        }

        internal List<ICoordinatesFunction<double, IOrdinate>> Create_3Constant_NonMonotonic_OrdinateFunctions()
        {
            List<ICoordinatesFunction<double, IOrdinate>> functions = new List<ICoordinatesFunction<double, IOrdinate>>();

            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys1 = new List<double>() { 5, 6, 7, 8 };

            List<double> xs2 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys2 = new List<double>() { 5, 6, 7, 8 };

            List<double> xs3 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys3 = new List<double>() { 5, 6, 7, 8 };

            //create a constant func
            CoordinatesFunctionConstants const1Func = CreateCoordinatesFunctionConstants(xs1, ys1);
            CoordinatesFunctionConstants const2Func = CreateCoordinatesFunctionConstants(xs2, ys2);
            CoordinatesFunctionConstants const3Func = CreateCoordinatesFunctionConstants(xs3, ys3);

            //convert to iordinate
            CoordinatesFunctionOrdinateYs const1OrdFunc = new CoordinatesFunctionOrdinateYs(const1Func);
            CoordinatesFunctionOrdinateYs const2OrdFunc = new CoordinatesFunctionOrdinateYs(const2Func);
            CoordinatesFunctionOrdinateYs const3OrdFunc = new CoordinatesFunctionOrdinateYs(const3Func);

            //add to list of funcs
            functions.Add(const1OrdFunc);
            functions.Add(const2OrdFunc);
            functions.Add(const3OrdFunc);

            return functions;
        }

        internal List<ICoordinatesFunction<double, IOrdinate>> Create_3Constant_WeakMonotonicIncreasing_OrdinateFunctions()
        {
            List<ICoordinatesFunction<double, IOrdinate>> functions = new List<ICoordinatesFunction<double, IOrdinate>>();

            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys1 = new List<double>() { 5, 6, 7, 8 };

            List<double> xs2 = new List<double>() { 4, 5, 6, 7 };
            List<double> ys2 = new List<double>() { 8, 10, 11, 12 };

            List<double> xs3 = new List<double>() { 8, 9, 10, 11 };
            List<double> ys3 = new List<double>() { 12, 14, 15, 16 };

            //create a constant func
            CoordinatesFunctionConstants const1Func = CreateCoordinatesFunctionConstants(xs1, ys1);
            CoordinatesFunctionConstants const2Func = CreateCoordinatesFunctionConstants(xs2, ys2);
            CoordinatesFunctionConstants const3Func = CreateCoordinatesFunctionConstants(xs3, ys3);

            //convert to iordinate
            CoordinatesFunctionOrdinateYs const1OrdFunc = new CoordinatesFunctionOrdinateYs(const1Func);
            CoordinatesFunctionOrdinateYs const2OrdFunc = new CoordinatesFunctionOrdinateYs(const2Func);
            CoordinatesFunctionOrdinateYs const3OrdFunc = new CoordinatesFunctionOrdinateYs(const3Func);

            //add to list of funcs
            functions.Add(const1OrdFunc);
            functions.Add(const2OrdFunc);
            functions.Add(const3OrdFunc);

            return functions;
        }

        internal List<ICoordinatesFunction<double, IOrdinate>> Create_3Constant_StrictMonotonicDecreasing_OrdinateFunctions()
        {
            List<ICoordinatesFunction<double, IOrdinate>> functions = new List<ICoordinatesFunction<double, IOrdinate>>();

            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys1 = new List<double>() { 20, 19, 18, 17 };

            List<double> xs2 = new List<double>() { 4, 5, 6, 7 };
            List<double> ys2 = new List<double>() { 16, 15, 14, 13 };

            List<double> xs3 = new List<double>() { 8, 9, 10, 11 };
            List<double> ys3 = new List<double>() { 12, 11, 10, 9 };

            //create a constant func
            CoordinatesFunctionConstants const1Func = CreateCoordinatesFunctionConstants(xs1, ys1);
            CoordinatesFunctionConstants const2Func = CreateCoordinatesFunctionConstants(xs2, ys2);
            CoordinatesFunctionConstants const3Func = CreateCoordinatesFunctionConstants(xs3, ys3);

            //convert to iordinate
            CoordinatesFunctionOrdinateYs const1OrdFunc = new CoordinatesFunctionOrdinateYs(const1Func);
            CoordinatesFunctionOrdinateYs const2OrdFunc = new CoordinatesFunctionOrdinateYs(const2Func);
            CoordinatesFunctionOrdinateYs const3OrdFunc = new CoordinatesFunctionOrdinateYs(const3Func);

            //add to list of funcs
            functions.Add(const1OrdFunc);
            functions.Add(const2OrdFunc);
            functions.Add(const3OrdFunc);

            return functions;
        }
        internal List<ICoordinatesFunction<double, IOrdinate>> Create_3Constant_WeakMonotonicDecreasing_OrdinateFunctions()
        {
            List<ICoordinatesFunction<double, IOrdinate>> functions = new List<ICoordinatesFunction<double, IOrdinate>>();

            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys1 = new List<double>() { 20, 19, 18, 17 };

            List<double> xs2 = new List<double>() { 4, 5, 6, 7 };
            List<double> ys2 = new List<double>() { 17, 15, 14, 13 };

            List<double> xs3 = new List<double>() { 8, 9, 10, 11 };
            List<double> ys3 = new List<double>() { 12, 11, 10, 9 };

            //create a constant func
            CoordinatesFunctionConstants const1Func = CreateCoordinatesFunctionConstants(xs1, ys1);
            CoordinatesFunctionConstants const2Func = CreateCoordinatesFunctionConstants(xs2, ys2);
            CoordinatesFunctionConstants const3Func = CreateCoordinatesFunctionConstants(xs3, ys3);

            //convert to iordinate
            CoordinatesFunctionOrdinateYs const1OrdFunc = new CoordinatesFunctionOrdinateYs(const1Func);
            CoordinatesFunctionOrdinateYs const2OrdFunc = new CoordinatesFunctionOrdinateYs(const2Func);
            CoordinatesFunctionOrdinateYs const3OrdFunc = new CoordinatesFunctionOrdinateYs(const3Func);

            //add to list of funcs
            functions.Add(const1OrdFunc);
            functions.Add(const2OrdFunc);
            functions.Add(const3OrdFunc);

            return functions;
        }

        internal List<ICoordinatesFunction<double, IOrdinate>> Create_Constant_Distributed_Constant_NonMonotonic_OrdinateFunctions()
        {
            List<ICoordinatesFunction<double, IOrdinate>> functions = new List<ICoordinatesFunction<double, IOrdinate>>();

            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys1 = new List<double>() { 5, 6, 7, 8 };

            List<double> xs2 = new List<double>() { 4, 5, 6, 7 };
            List<IDistribution> ys2 = new List<IDistribution>() { new Normal(1,0), new Normal(1, 0), new Normal(1, 0), new Normal(1, 0) };

            List<double> xs3 = new List<double>() { 8, 9, 10, 11 };
            List<double> ys3 = new List<double>() { 13, 14, 15, 16 };

            //create a constant func
            CoordinatesFunctionConstants const1Func = CreateCoordinatesFunctionConstants(xs1, ys1);
            CoordinatesFunctionVariableYs const2Func = CreateDistributedCoordinatesFunction(xs2, ys2);
            CoordinatesFunctionConstants const3Func = CreateCoordinatesFunctionConstants(xs3, ys3);

            //convert to iordinate
            CoordinatesFunctionOrdinateYs const1OrdFunc = new CoordinatesFunctionOrdinateYs(const1Func);
            CoordinatesFunctionOrdinateYs const2OrdFunc = new CoordinatesFunctionOrdinateYs(const2Func);
            CoordinatesFunctionOrdinateYs const3OrdFunc = new CoordinatesFunctionOrdinateYs(const3Func);

            //add to list of funcs
            functions.Add(const1OrdFunc);
            functions.Add(const2OrdFunc);
            functions.Add(const3OrdFunc);

            return functions;
        }

        //private IImmutableList<ICoordinate<double, IOrdinate>> ConvertDistributedYsToOrdinates(IImmutableList<ICoordinate<double, IDistribution>> coords)
        //{
        //    IImmutableList<ICoordinate<double, IOrdinate>> retval = ImmutableList.Create<ICoordinate<double, IOrdinate>>();
        //    foreach (ICoordinate<double, IDistribution> coord in coords)
        //    {
        //        retval.Add(new CoordinateOrdinateY(coord.X, new Distribution(coord.Y)));
        //    }
        //    return retval;
        //}

        //private IImmutableList<ICoordinate<double, IOrdinate>> ConvertConstantYsToOrdinates(IImmutableList<ICoordinate<double, double>> coords)
        //{
        //    IImmutableList<ICoordinate<double, IOrdinate>> retval = ImmutableList.Create<ICoordinate<double, IOrdinate>>();
        //    foreach (ICoordinate<double, double> coord in coords)
        //    {
        //        retval.Add(new CoordinateOrdinateY(coord.X, new Constant(coord.Y)));
        //    }
        //    return retval;
        //}
        #endregion

        public static bool AreCoordinatesEqual(IImmutableList<ICoordinate<double, IOrdinate>> lista, IImmutableList<ICoordinate<double, IOrdinate>> listb)
        {
            if (lista.Count != listb.Count)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < lista.Count; i++)//(ICoordinate<double, double> coord in lista)
                {
                    if (lista[i].X != listb[i].X || lista[i].Y.Value() != listb[i].Y.Value())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool AreCoordinatesEqual(IImmutableList<ICoordinate<double, double>> lista, IImmutableList<ICoordinate<double, double>> listb)
        {
            if (lista.Count != listb.Count)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < lista.Count; i++)//(ICoordinate<double, double> coord in lista)
                {
                    if (lista[i].X != listb[i].X || lista[i].Y != listb[i].Y)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
